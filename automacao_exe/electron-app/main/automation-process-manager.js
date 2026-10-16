const { spawn } = require('child_process');
const path = require('path');
const fs = require('fs');
const os = require('os');
const { categorizeExecution, normalizeOutput, createGroupKey, extractScenarios } = require('./log-categorizer');
const { parseFeatureFile, findScenarioInFeature, getScenarioSteps } = require('./feature-parser');

class AutomationProcessManager {
  constructor() {
    this.processes = new Map();
    this.maxParallel = 1; // Reduzido para 1 para economizar RAM (evita múltiplas instâncias do Chrome)
    this.queue = []; // Fila de execuções aguardando
    this.maxExecutionTime = 30 * 60 * 1000; // 30 minutos máximo por execução
    this.timeouts = new Map(); // Timeouts para processos
    this.onQueueComplete = null; // Callback para quando a fila terminar
    this.executionResults = new Map(); // Map<processId, { success: boolean, featureName: string }> - Rastreia resultados das execuções
    this.isCancelling = false; // Flag para evitar criação de novos processos durante cancelamento
    
    // Configurações de otimização de memória
    this.MAX_LOG_ENTRIES = 1000; // Máximo de entradas de log em memória por processo
    this.MAX_OUTPUT_SIZE = 5 * 1024 * 1024; // 5MB máximo para stdout/stderr
    this.MAX_FRONTEND_LOGS = 5000; // Máximo de caracteres de log no frontend por processo
  }

  async startAutomation(projectPath, featureName, featurePath, scenarioName, tags, filter, debugPort, onLog, onComplete) {
    // CRÍTICO: Bloqueia criação de novos processos durante cancelamento
    // Se a flag estiver ativa, reseta automaticamente quando o usuário tenta iniciar uma nova automação
    // Isso permite que o usuário inicie novas execuções após o cancelamento sem precisar resetar manualmente
    if (this.isCancelling) {
      // Verifica se realmente não há processos rodando e fila vazia antes de resetar
      if (this.processes.size === 0 && this.queue.length === 0) {
        console.log(`[startAutomation] Flag de cancelamento ativa, mas não há processos ou fila. Resetando flag e permitindo nova execução.`);
        this.isCancelling = false;
      } else {
        const displayName = scenarioName ? `${featureName} - ${scenarioName}` : featureName;
        console.warn(`[startAutomation] Tentativa de iniciar automação durante cancelamento - bloqueando: ${displayName}`);
        throw new Error('Execuções estão sendo canceladas. Aguarde o cancelamento concluir antes de iniciar novas execuções.');
      }
    }
    
    // Normaliza os caminhos para comparação consistente
    const normalizedFeaturePath = featurePath ? path.normalize(featurePath) : null;
    const normalizedScenarioName = scenarioName ? scenarioName.trim() : null;
    
    // Verifica se já existe uma execução para esta feature/cenário em andamento
    // Verifica tanto em processos rodando quanto na fila
    const existingProcess = Array.from(this.processes.values())
      .find(p => {
        const pFeaturePath = p.featurePath ? path.normalize(p.featurePath) : null;
        const pScenarioName = p.scenarioName ? p.scenarioName.trim() : null;
        const sameFeature = pFeaturePath === normalizedFeaturePath;
        const sameScenario = (pScenarioName || null) === (normalizedScenarioName || null);
        return sameFeature && sameScenario && p.process && !p.process.killed;
      });
    
    if (existingProcess) {
      const displayName = normalizedScenarioName ? `${featureName} - ${normalizedScenarioName}` : featureName;
      throw new Error(`${displayName} já está em execução (processId: ${existingProcess.id})`);
    }
    
    // Verifica também na fila
    const existingInQueue = this.queue.find(item => {
      const itemFeaturePath = item.args.featurePath ? path.normalize(item.args.featurePath) : null;
      const itemScenarioName = item.args.scenarioName ? item.args.scenarioName.trim() : null;
      const sameFeature = itemFeaturePath === normalizedFeaturePath;
      const sameScenario = (itemScenarioName || null) === (normalizedScenarioName || null);
      return sameFeature && sameScenario;
    });
    
    if (existingInQueue) {
      const displayName = normalizedScenarioName ? `${featureName} - ${normalizedScenarioName}` : featureName;
      throw new Error(`${displayName} já está na fila de execução`);
    }
    
    // Gera ID único baseado no caminho do arquivo e cenário para evitar duplicações
    const safeFeaturePath = normalizedFeaturePath ? normalizedFeaturePath.replace(/[^a-zA-Z0-9]/g, '_') : featureName;
    const safeScenarioName = normalizedScenarioName ? normalizedScenarioName.replace(/[^a-zA-Z0-9]/g, '_') : '';
    const processId = normalizedScenarioName 
      ? `${safeFeaturePath}_${safeScenarioName}_${Date.now()}`
      : `${safeFeaturePath}_${Date.now()}`;
    
    // Se já atingiu o limite, adiciona à fila em vez de lançar erro
    if (this.processes.size >= this.maxParallel) {
      console.log(`[startAutomation] Limite de ${this.maxParallel} execuções atingido. Adicionando ${processId} à fila (posição: ${this.queue.length + 1})`);
      return new Promise((resolve, reject) => {
        const queueItem = {
          resolve,
          reject,
          args: { projectPath, featureName, featurePath, scenarioName, tags, filter, debugPort, onLog, onComplete },
          processId
        };
        this.queue.push(queueItem);
        console.log(`[startAutomation] ✅ Item adicionado à fila. Total na fila: ${this.queue.length}, Processos rodando: ${this.processes.size}/${this.maxParallel}`);
      });
    }
    
    // Executa imediatamente
    console.log(`[startAutomation] Slot disponível. Executando imediatamente: ${processId} (Processos rodando: ${this.processes.size}/${this.maxParallel})`);
    return this._executeAutomation(processId, projectPath, featureName, featurePath, scenarioName, tags, filter, debugPort, onLog, onComplete);
    }

  async _executeAutomation(processId, projectPath, featureName, featurePath, scenarioName, tags, filter, debugPort, onLog, onComplete) {

    // Normaliza o caminho do projeto (resolve caminhos relativos e normaliza separadores)
    const normalizedProjectPath = path.resolve(projectPath);
    
    // Verifica se o diretório existe
    if (!fs.existsSync(normalizedProjectPath)) {
      throw new Error(`Diretório do projeto não encontrado: ${normalizedProjectPath}`);
    }

    // Tenta encontrar o arquivo .csproj no diretório
    const projectFiles = fs.readdirSync(normalizedProjectPath).filter(f => f.endsWith('.csproj'));
    const projectAssemblyName = projectFiles.length === 1
      ? path.basename(projectFiles[0], '.csproj')
      : path.basename(normalizedProjectPath);
    let testPath = normalizedProjectPath;
    
    if (projectFiles.length === 0) {
      // Se não encontrou .csproj no diretório raiz, usa o diretório como está
      // (dotnet test pode procurar recursivamente)
      console.warn(`Nenhum arquivo .csproj encontrado em ${normalizedProjectPath}`);
    } else if (projectFiles.length === 1) {
      // Se há exatamente um .csproj, usa ele
      testPath = path.join(normalizedProjectPath, projectFiles[0]);
    }
    // Se há múltiplos .csproj, usa o diretório (dotnet test executará todos)

    // Constrói comando dotnet test
    // Normaliza o caminho para garantir que espaços sejam tratados corretamente
    const normalizedTestPath = path.normalize(testPath);
    let testCommand = ['test', normalizedTestPath];
    
    // ABORDAGEM MELHORADA: Filtra por arquivo de feature específico usando o caminho
    // Prioridade: 1) Filtro customizado, 2) Caminho do arquivo feature, 3) Tags, 4) Sem filtro (erro)
    
    const buildFilterCandidates = (value) => {
      if (!value) return [];
      
      const sanitize = (text) => {
        return String(text)
          .normalize('NFD')
          .replace(/[\u0300-\u036f]/g, '')
          .replace(/[^a-zA-Z0-9]+/g, ' ')
          .trim();
      };
      
      const normalized = sanitize(value);
      if (!normalized) return [];
      
      const tokens = normalized.split(/\s+/);
      if (tokens.length === 0) return [];
      
      const pascalCase = tokens.map(token => token.charAt(0).toUpperCase() + token.slice(1)).join('');
      const camelCase = pascalCase ? pascalCase.charAt(0).toLowerCase() + pascalCase.slice(1) : '';
      const snakeCase = tokens.map(token => token.toLowerCase()).join('_');
      const dotCase = tokens.map(token => token.charAt(0).toUpperCase() + token.slice(1)).join('.');
      const lowerJoined = tokens.join('').toLowerCase();
      const upperJoined = tokens.join('');
      
      return Array.from(new Set([
        pascalCase,
        camelCase,
        snakeCase,
        dotCase,
        lowerJoined,
        upperJoined
      ])).filter(candidate => candidate && candidate.length >= 3);
    };
    
    const filterParts = [];
    const deriveNamespace = (relativePath) => {
      if (!relativePath) return 'Features';
      const segments = relativePath
        .replace(/\\/g, '/')
        .split('/')
        .filter(Boolean);
      if (segments.length === 0) return 'Features';
      
      segments.pop(); // Remove arquivo .feature
      const sanitizedSegments = segments.map(segment => {
        return segment
          .normalize('NFD')
          .replace(/[\u0300-\u036f]/g, '')
          .replace(/[^a-zA-Z0-9]+/g, ' ')
          .trim()
          .split(/\s+/)
          .filter(Boolean)
          .map(token => token.charAt(0).toUpperCase() + token.slice(1))
          .join('');
      }).filter(Boolean);
      
      return ['Features', ...sanitizedSegments].join('.');
    };
    const deriveNamespaceInfo = (relativePath) => {
      const baseNamespace = deriveNamespace(relativePath);
      const namespaceWithoutFeatures = baseNamespace.replace(/^Features\./, '');
      const prefixesSet = new Set();
      const assemblyVariants = new Set();
      const assemblyPrefixes = new Set();

      prefixesSet.add(baseNamespace);
      if (namespaceWithoutFeatures && namespaceWithoutFeatures !== baseNamespace) {
        prefixesSet.add(namespaceWithoutFeatures);
      }

      if (projectAssemblyName) {
        const rawAssembly = projectAssemblyName.trim();
        if (rawAssembly) {
          assemblyVariants.add(rawAssembly);

          const sanitizedAssembly = rawAssembly
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/[^a-zA-Z0-9.]+/g, '.')
            .split('.')
            .filter(Boolean)
            .map(segment => segment.charAt(0).toUpperCase() + segment.slice(1))
            .join('.');

          if (sanitizedAssembly) {
            assemblyVariants.add(sanitizedAssembly);
          }
        }
      }

      assemblyVariants.forEach(assembly => {
        assemblyPrefixes.add(`${assembly}.${baseNamespace}`);
        if (namespaceWithoutFeatures) {
          assemblyPrefixes.add(`${assembly}.${namespaceWithoutFeatures}`);
        }
      });

      assemblyPrefixes.forEach(prefix => prefixesSet.add(prefix));

      return {
        baseNamespace,
        namespaceWithoutFeatures: namespaceWithoutFeatures && namespaceWithoutFeatures !== baseNamespace
          ? namespaceWithoutFeatures
          : null,
        prefixes: Array.from(prefixesSet),
        assemblyPrefixes: Array.from(assemblyPrefixes),
        assemblyVariants: Array.from(assemblyVariants)
      };
    };
      
    // Prioridade 1: Se há um filtro customizado válido, usa ele diretamente
    if (filter) {
      const cleanFilter = filter.trim();
      if (cleanFilter.includes('=') || cleanFilter.includes('~') || cleanFilter.includes('|') || cleanFilter.includes('&')) {
        filterParts.push(cleanFilter);
      }
    }
    
    // Prioridade 2: Construção do filtro baseado na seleção (cenário ou feature)
    if (featurePath && !filter) {
      const fileName = path.basename(featurePath, '.feature');
      const fileNameCandidates = buildFilterCandidates(fileName);
      const scenarioSelected = Boolean(scenarioName && scenarioName.trim());
      const namespaceInfo = deriveNamespaceInfo(featurePath);
      const namespacePrefixes = namespaceInfo.prefixes;
      const assemblyPrefixes = namespaceInfo.assemblyPrefixes;
      const baseNamespace = namespaceInfo.baseNamespace;
      const namespaceWithoutFeatures = namespaceInfo.namespaceWithoutFeatures;
      const featureClassCandidates = Array.from(new Set([
        ...buildFilterCandidates(featureName).map(candidate => candidate.endsWith('Feature') ? candidate : `${candidate}Feature`),
        ...buildFilterCandidates(fileName).map(candidate => candidate.endsWith('Feature') ? candidate : `${candidate}Feature`)
      ])).filter(Boolean);
      const primaryFeatureClass = featureClassCandidates.find(candidate => candidate.endsWith('Feature')) || featureClassCandidates[0] || null;
      const fallbackFeatureClass = (() => {
        if (!primaryFeatureClass) return null;
        const alt = featureClassCandidates.find(candidate => candidate !== primaryFeatureClass);
        return alt || null;
      })();
      
      // Log para debug: mostra informações da feature que está sendo executada
      console.log(`[automation-process-manager] Construindo filtro para feature:`, {
        featureName,
        featurePath,
        fileName,
        primaryFeatureClass,
        baseNamespace,
        scenarioName: scenarioName || 'nenhum'
      });
      
      if (scenarioSelected) {
        const scenarioCandidates = buildFilterCandidates(scenarioName)
          .map(candidate => candidate.replace(/\./g, ''))
          .filter(Boolean);
        const primaryScenarioCandidate = scenarioCandidates[0] || null;
        const secondaryScenarioCandidate = scenarioCandidates.find(candidate => candidate !== primaryScenarioCandidate) || null;
        
        if (!primaryScenarioCandidate || !primaryFeatureClass) {
          console.warn(`[automation-process-manager] Não foi possível gerar filtros específicos para o cenário ${scenarioName}.`);
        } else {
          const filtersOrdered = [];
          const filtersSet = new Set();
          
          const prioritizedFeatureClasses = Array.from(new Set([primaryFeatureClass, fallbackFeatureClass])).filter(Boolean);
          const prioritizedPrefixes = [
            ...assemblyPrefixes,
            ...[baseNamespace, namespaceWithoutFeatures].filter(Boolean)
          ];
          const scenarioNameVariants = new Set();
          
          [primaryScenarioCandidate, secondaryScenarioCandidate]
            .filter(Boolean)
            .forEach(candidate => {
              scenarioNameVariants.add(candidate);
              scenarioNameVariants.add(candidate.charAt(0).toLowerCase() + candidate.slice(1));
              scenarioNameVariants.add(candidate.toLowerCase());
            });
          
          const addFilter = (value) => {
            if (!value) return;
            const normalized = value.replace(/\s+/g, '');
            const exactFilter = `FullyQualifiedName=${normalized}`;
            const containsFilter = `FullyQualifiedName~${normalized}`;
            
            if (!filtersSet.has(exactFilter)) {
              filtersSet.add(exactFilter);
              filtersOrdered.push(exactFilter);
            }
            if (!filtersSet.has(containsFilter)) {
              filtersSet.add(containsFilter);
              filtersOrdered.push(containsFilter);
            }
          };
          
          scenarioNameVariants.forEach(scenarioVariant => {
            prioritizedPrefixes.forEach(prefix => {
              prioritizedFeatureClasses.forEach(featureCandidate => {
                addFilter(`${prefix}.${featureCandidate}.${scenarioVariant}`);
              });
              addFilter(`${prefix}.${scenarioVariant}`);
            });
            
            prioritizedFeatureClasses.forEach(featureCandidate => {
              addFilter(`${featureCandidate}.${scenarioVariant}`);
            });
            
            addFilter(scenarioVariant);
          });
          
          if (filtersOrdered.length === 0) {
            console.warn(`[automation-process-manager] Não foi possível gerar filtros específicos para o cenário ${scenarioName}.`);
          } else {
            const MAX_FILTERS = 20;
            const appliedFilters = filtersOrdered.slice(0, MAX_FILTERS).join('|');
            filterParts.push(appliedFilters);
            const filterPreview = filterParts[filterParts.length - 1];
            console.log(`[automation-process-manager] Filtro específico de cenário aplicado: ${filterPreview.length > 2000 ? filterPreview.substring(0, 2000) + '...' : filterPreview}`);
          }
        }
      } else {
        // Sem cenário específico: usa filtros mais específicos primeiro, depois filtros mais genéricos
        // Prioriza filtros exatos (=) antes de filtros que contenham (~)
        const exactFilters = new Set(); // Filtros exatos (mais específicos)
        const containsFilters = new Set(); // Filtros que contenham (menos específicos)
        const prioritizedFeatureClasses = Array.from(new Set([primaryFeatureClass, fallbackFeatureClass, ...featureClassCandidates])).filter(Boolean);
        const generalPrefixes = new Set([
          ...namespacePrefixes,
          ...assemblyPrefixes
        ]);

        const addExactFilter = (value) => {
          if (!value) return;
          const normalized = value.replace(/\s+/g, '');
          exactFilters.add(`FullyQualifiedName=${normalized}`);
        };

        const addContainsFilter = (value) => {
          if (!value) return;
          const normalized = value.replace(/\s+/g, '');
          // Só adiciona filtro "contém" se for significativamente diferente e não muito genérico
          // Evita adicionar filtros muito genéricos que podem pegar features erradas
          if (normalized.length >= 10) { // Só adiciona se tiver pelo menos 10 caracteres
            containsFilters.add(`FullyQualifiedName~${normalized}`);
          }
        };

        // PRIORIDADE 1: Filtros exatos usando o nome completo da feature class com namespace completo
        // Isso garante que pegamos exatamente a feature correta
        generalPrefixes.forEach(prefix => {
          prioritizedFeatureClasses.forEach(candidate => {
            addExactFilter(`${prefix}.${candidate}`);
          });
        });

        // PRIORIDADE 2: Filtros exatos usando apenas a feature class (sem namespace)
        // Útil se o namespace não estiver disponível
        prioritizedFeatureClasses.forEach(candidate => {
          addExactFilter(candidate);
        });

        // PRIORIDADE 3: Filtros exatos usando o nome do arquivo (sem extensão)
        // O nome do arquivo deve ser único e específico
        fileNameCandidates.forEach(candidate => {
          // Prioriza candidatos que terminam com "Feature" (mais específicos)
          if (candidate.endsWith('Feature') || candidate.length >= 15) {
            addExactFilter(candidate);
          }
        });

        // PRIORIDADE 4: Filtros exatos usando o nome da feature (do Gherkin)
        buildFilterCandidates(featureName).forEach(candidate => {
          // Prioriza candidatos maiores e mais específicos
          if (candidate.length >= 15) {
            addExactFilter(candidate);
          }
        });

        // PRIORIDADE 5: Filtros que contenham (apenas para candidatos muito específicos)
        // Usa apenas para garantir que não perdemos a feature, mas com alta especificidade
        const mostSpecificFeatureClass = primaryFeatureClass || featureClassCandidates[0];
        if (mostSpecificFeatureClass && mostSpecificFeatureClass.length >= 20) {
          // Só adiciona filtro "contém" se o nome da classe for muito específico
          generalPrefixes.forEach(prefix => {
            addContainsFilter(`${prefix}.${mostSpecificFeatureClass}`);
          });
        }

        // PRIORIDADE 6: Tags (se disponíveis e específicas)
        if (tags && Array.isArray(tags) && tags.length > 0) {
          // Usa tags apenas como filtro adicional, não como filtro principal
          // Pois múltiplas features podem ter as mesmas tags
          tags.forEach(tag => {
            const cleanTag = tag.replace(/^@+/, '');
            if (cleanTag.length >= 10) { // Só tags específicas (pelo menos 10 caracteres)
              buildFilterCandidates(cleanTag).forEach(candidate => {
                const normalized = candidate.replace(/\s+/g, '');
                if (normalized.length >= 10) {
                  exactFilters.add(`Category=${normalized}`);
                }
              });
            }
          });
        }

        // Combina filtros: primeiro exatos (mais específicos), depois "contém" (menos específicos)
        const allFilters = [];
        if (exactFilters.size > 0) {
          allFilters.push(Array.from(exactFilters).join('|'));
        }
        // Só adiciona filtros "contém" se não houver muitos filtros exatos
        // Isso evita que filtros genéricos sejam aplicados quando temos filtros específicos
        if (containsFilters.size > 0 && exactFilters.size < 5) {
          allFilters.push(Array.from(containsFilters).join('|'));
        }

        if (allFilters.length > 0) {
          // Usa AND entre grupos de filtros para maior especificidade
          // Dentro de cada grupo, usa OR (já feito acima)
          filterParts.push(allFilters.join('&'));
          
          const filterPreview = filterParts[filterParts.length - 1];
          console.log(`[automation-process-manager] Filtro de feature aplicado para "${featureName}" (${exactFilters.size} exatos, ${containsFilters.size} contém)`);
          console.log(`[automation-process-manager] Filtro completo: ${filterPreview.length > 1000 ? filterPreview.substring(0, 1000) + '...' : filterPreview}`);
          console.log(`[automation-process-manager] Feature classes geradas:`, featureClassCandidates);
          console.log(`[automation-process-manager] Namespaces:`, Array.from(generalPrefixes).slice(0, 5));
        } else {
          console.warn(`[automation-process-manager] ⚠️ Nenhum filtro foi gerado para a feature "${featureName}" (arquivo: ${fileName})`);
        }
      }
    }
    
    // Armazena informações do filtro para uso posterior na mensagem de erro
    const filterInfo = {
      filterParts: filterParts,
      featurePathForFilter: featurePath,
      scenarioNameForFilter: scenarioName,
      combinedFilter: filterParts.length > 0 ? filterParts.join('&') : null
    };
    
    // Aplica o filtro
    if (filterParts.length > 0) {
      filterInfo.combinedFilter = filterParts.join('&');
      testCommand.push('--filter', filterInfo.combinedFilter);
      console.log(`[automation-process-manager] Filtro aplicado: ${filterInfo.combinedFilter}`);
    } else {
      // Se não há filtro, não executa (evita executar todos os testes acidentalmente)
      throw new Error(`Não foi possível criar filtro para executar apenas a feature solicitada. Feature: ${featureName}, Path: ${featurePath || 'não informado'}. Adicione tags no arquivo .feature para permitir filtragem.`);
    }

    // Adiciona o arquivo .runsettings se existir no projeto
    const runsettingsPath = path.join(normalizedProjectPath, '.runsettings');
    if (fs.existsSync(runsettingsPath)) {
      testCommand.push('--settings', runsettingsPath);
      console.log(`[automation-process-manager] Arquivo .runsettings encontrado e será usado: ${runsettingsPath}`);
    }

    testCommand.push('--logger', 'console;verbosity=detailed');
    
    // Adiciona logger para gerar arquivo .trx (Test Results XML) para integração com Visual Studio
    const trxFileName = `test-results-${Date.now()}.trx`;
    const trxFilePath = path.join(normalizedProjectPath, 'TestResults', trxFileName);
    // Garante que o diretório TestResults existe
    const testResultsDir = path.join(normalizedProjectPath, 'TestResults');
    if (!fs.existsSync(testResultsDir)) {
      fs.mkdirSync(testResultsDir, { recursive: true });
    }
    testCommand.push('--logger', `trx;LogFileName=${trxFileName}`);
    
    testCommand.push('--nologo');

    // Log do comando para debug
      const commandPreview = `dotnet ${testCommand.join(' ')}`;
      console.log('Executando comando:', commandPreview.length > 2000 ? `${commandPreview.substring(0, 2000)}...` : commandPreview);
    console.log('Diretório de trabalho:', normalizedProjectPath);

    // Detecta a plataforma para configurar o shell corretamente
    const isWindows = os.platform() === 'win32';
    
    // Configura opções de spawn para facilitar o controle do processo e filhos
    // IMPORTANTE: windowsHide deve ser false para permitir que o Playwright abra janelas GUI
    // quando executado em modo não-headless. Isso é necessário para que os testes funcionem corretamente.
    const spawnOptions = {
      cwd: normalizedProjectPath,
      shell: false,
      stdio: ['ignore', 'pipe', 'pipe'],
      windowsHide: false, // Alterado para false para permitir criação de janelas GUI pelo Playwright
      detached: false, // Mantém o processo associado para controle adequado
      env: {
        ...process.env,
        ...(isWindows && { CHCP: '65001' }),
        ...(debugPort && { CHROME_DEBUG_PORT: debugPort.toString() }),
        // Variáveis de ambiente para garantir que o Playwright funcione corretamente
        PLAYWRIGHT_BROWSERS_PATH: process.env.PLAYWRIGHT_BROWSERS_PATH || '',
        // Garante que o processo tenha acesso ao desktop para criar janelas GUI
        ...(isWindows && { 
          DISPLAY: process.env.DISPLAY || '',
          // No Windows, não há DISPLAY, mas garantimos que outras variáveis estejam presentes
        })
      }
    };

    // CRÍTICO: Cria o processInfo ANTES do spawn para poder adicionar ao Map imediatamente
    // Isso previne condições de corrida onde múltiplos testes tentam executar simultaneamente
    console.log(`[automation-process-manager] 🔍 DEBUG: Parâmetros recebidos em _executeAutomation:`);
    console.log(`[automation-process-manager] 🔍 DEBUG: projectPath: ${projectPath}`);
    console.log(`[automation-process-manager] 🔍 DEBUG: featurePath: ${featurePath}`);
    console.log(`[automation-process-manager] 🔍 DEBUG: featureName: ${featureName}`);
    console.log(`[automation-process-manager] 🔍 DEBUG: scenarioName: ${scenarioName}`);
    
    // Resolve o caminho do arquivo .feature em relação ao projeto
    // normalizedProjectPath já foi declarado acima na função
    let normalizedFeaturePath = null;
    if (featurePath) {
      // Se o caminho já é absoluto, usa diretamente
      if (path.isAbsolute(featurePath)) {
        normalizedFeaturePath = path.normalize(featurePath);
      } else {
        // Se é relativo, tenta primeiro com Features/, depois sem
        // O featurePath pode vir com ou sem a pasta Features
        const pathWithFeatures = path.join(normalizedProjectPath, 'Features', featurePath);
        const pathWithoutFeatures = path.resolve(normalizedProjectPath, featurePath);
        
        // Verifica qual caminho existe
        if (fs.existsSync(pathWithFeatures)) {
          normalizedFeaturePath = pathWithFeatures;
          console.log(`[automation-process-manager] 🔍 DEBUG: Arquivo encontrado com Features/: ${normalizedFeaturePath}`);
        } else if (fs.existsSync(pathWithoutFeatures)) {
          normalizedFeaturePath = pathWithoutFeatures;
          console.log(`[automation-process-manager] 🔍 DEBUG: Arquivo encontrado sem Features/: ${normalizedFeaturePath}`);
        } else {
          // Tenta também se o featurePath já inclui Features
          const pathAlreadyHasFeatures = path.resolve(normalizedProjectPath, featurePath);
          normalizedFeaturePath = pathAlreadyHasFeatures;
          console.log(`[automation-process-manager] 🔍 DEBUG: Tentando caminho: ${normalizedFeaturePath}`);
        }
      }
    }
    
    const normalizedScenarioName = scenarioName ? scenarioName.trim() : null;
    
    console.log(`[automation-process-manager] 🔍 DEBUG: normalizedProjectPath: ${normalizedProjectPath}`);
    console.log(`[automation-process-manager] 🔍 DEBUG: normalizedFeaturePath: ${normalizedFeaturePath}`);
    console.log(`[automation-process-manager] 🔍 DEBUG: normalizedScenarioName: ${normalizedScenarioName}`);
    
    // Cria um objeto temporário para o processo (será atualizado após o spawn)
    const processInfo = {
      id: processId,
      featureName,
      featurePath: normalizedFeaturePath,
      scenarioName: normalizedScenarioName,
      process: null, // Será atualizado após o spawn
      startTime: new Date(),
      logs: [],
      completed: false,
      childPids: new Set(),
      chromePids: new Set(),
      chromeDriverPids: new Set(),
      filterInfo: filterInfo
    };
    
    // ADICIONA AO MAP ANTES DO SPAWN para evitar condições de corrida
    // Isso garante que outros testes que chegarem depois verão este processo como "em execução"
    this.processes.set(processId, processInfo);
    console.log(`[startAutomation] Processo ${processId} adicionado ao Map ANTES do spawn. Execuções ativas: ${this.processes.size}/${this.maxParallel}, Fila: ${this.queue.length}`);
    
    const dotnetProcess = spawn('dotnet', testCommand, spawnOptions);
    
    // Atualiza o processInfo com o processo real
    processInfo.process = dotnetProcess;
    
    // Lê arquivo .feature e extrai steps BDD para rastreamento em tempo real
    let featureSteps = [];
    let currentStepIndex = -1;
    
    console.log(`[automation-process-manager] 🔍 DEBUG: Tentando ler arquivo .feature`);
    console.log(`[automation-process-manager] 🔍 DEBUG: normalizedFeaturePath: ${normalizedFeaturePath}`);
    console.log(`[automation-process-manager] 🔍 DEBUG: normalizedFeaturePath existe? ${normalizedFeaturePath ? fs.existsSync(normalizedFeaturePath) : 'null'}`);
    
    if (normalizedFeaturePath && fs.existsSync(normalizedFeaturePath)) {
      try {
        console.log(`[automation-process-manager] 🔍 DEBUG: Lendo e parseando arquivo .feature: ${normalizedFeaturePath}`);
        const parsedFeature = parseFeatureFile(normalizedFeaturePath);
        console.log(`[automation-process-manager] 🔍 DEBUG: Feature parseada:`, parsedFeature ? `Sim, ${parsedFeature.scenarios?.length || 0} cenários` : 'Não');
        
        if (parsedFeature && parsedFeature.scenarios.length > 0) {
          console.log(`[automation-process-manager] 🔍 DEBUG: Cenários encontrados:`, parsedFeature.scenarios.map(s => s.name));
          // Se há um cenário específico, usa os steps desse cenário
          if (normalizedScenarioName) {
            const scenario = parsedFeature.scenarios.find(s => 
              s.name.toLowerCase() === normalizedScenarioName.toLowerCase() ||
              s.name.toLowerCase().includes(normalizedScenarioName.toLowerCase()) ||
              normalizedScenarioName.toLowerCase().includes(s.name.toLowerCase())
            );
            if (scenario) {
              featureSteps = scenario.steps.map(step => ({
                ...step,
                status: 'pending' // pending, running, completed, failed, skipped
              }));
            } else {
              // Se não encontrou o cenário específico, usa o primeiro cenário
              featureSteps = parsedFeature.scenarios[0].steps.map(step => ({
                ...step,
                status: 'pending'
              }));
            }
          } else {
            // Se não há cenário específico, usa o primeiro cenário
            featureSteps = parsedFeature.scenarios[0].steps.map(step => ({
              ...step,
              status: 'pending'
            }));
          }
          
          // Envia steps iniciais para o frontend via IPC
          console.log(`[automation-process-manager] 🔍 DEBUG: Verificando envio de steps BDD`);
          console.log(`[automation-process-manager] 🔍 DEBUG: featureSteps.length=${featureSteps.length}, onLog=${!!onLog}`);
          
          if (featureSteps.length > 0 && onLog) {
            const stepsEvent = JSON.stringify({
              type: 'bdd-steps-initialized',
              processId: processId,
              steps: featureSteps.map(step => ({
                keyword: step.keyword,
                text: step.text,
                fullLine: step.fullLine,
                status: step.status
              }))
            }) + '\n';
            console.log(`[automation-process-manager] ✅ Enviando ${featureSteps.length} steps BDD para frontend (processId: ${processId})`);
            console.log(`[automation-process-manager] Evento JSON completo:`, stepsEvent);
            console.log(`[automation-process-manager] Primeiros 500 chars:`, stepsEvent.substring(0, 500));
            console.log(`[automation-process-manager] 🔍 DEBUG: Chamando onLog com Buffer`);
            onLog(Buffer.from(stepsEvent, 'utf8'));
            console.log(`[automation-process-manager] ✅ Buffer enviado via onLog`);
          } else {
            console.log(`[automation-process-manager] ⚠️ Não enviando steps BDD: featureSteps.length=${featureSteps.length}, onLog=${!!onLog}`);
            if (featureSteps.length === 0) {
              console.log(`[automation-process-manager] ⚠️ Nenhum step encontrado no arquivo .feature`);
            }
            if (!onLog) {
              console.log(`[automation-process-manager] ⚠️ Callback onLog não está disponível`);
            }
          }
        } else {
          console.log(`[automation-process-manager] ⚠️ Feature parseada mas sem cenários ou feature inválida`);
        }
      } catch (error) {
        console.error(`[automation-process-manager] ❌ Erro ao ler arquivo .feature: ${error.message}`);
        console.error(`[automation-process-manager] ❌ Stack trace:`, error.stack);
      }
    } else {
      console.log(`[automation-process-manager] ⚠️ Arquivo .feature não encontrado ou caminho inválido`);
      console.log(`[automation-process-manager] ⚠️ normalizedFeaturePath: ${normalizedFeaturePath}`);
      console.log(`[automation-process-manager] ⚠️ featurePath recebido: ${featurePath}`);
    }
    
    // Armazena steps no processInfo para rastreamento
    processInfo.featureSteps = featureSteps;
    processInfo.currentStepIndex = currentStepIndex;
    
    // No Windows, quando detached é true, desassocia o processo para que ele possa abrir janelas GUI
    // Mantém o processo associado para evitar múltiplos prompts no Windows

    let stdout = '';
    let stderr = '';
    let stdoutSize = 0;
    let stderrSize = 0;
    let outputTruncated = false; // Flag para indicar se o output foi truncado

    // Função helper para limitar tamanho dos logs em memória
    const limitLogs = () => {
      if (processInfo.logs.length > this.MAX_LOG_ENTRIES) {
        // Remove os logs mais antigos, mantendo apenas os últimos MAX_LOG_ENTRIES
        const removeCount = processInfo.logs.length - this.MAX_LOG_ENTRIES;
        processInfo.logs.splice(0, removeCount);
      }
    };
    
    // Função helper para detectar e processar steps BDD nos logs
    const detectAndProcessBDDSteps = (logLine, streamType = 'stdout') => {
      if (featureSteps.length === 0) return;
      
      const trimmedLine = logLine.trim();
      
      // Múltiplos padrões de detecção para diferentes formatos de saída do SpecFlow/Reqnroll
      // Padrão 1: Linha que começa com keyword BDD (formato mais comum)
      let stepMatch = trimmedLine.match(/^(Given|When|Then|And|But|Dado|Quando|Então|E|Mas)\s+(.+?)(?:\s*->\s*(.+))?$/i);
      
      // Padrão 2: Keyword BDD em qualquer posição da linha (com prefixos de log)
      if (!stepMatch) {
        stepMatch = trimmedLine.match(/(?:^|->|\s+)(Given|When|Then|And|But|Dado|Quando|Então|E|Mas)\s+(.+?)(?:\s*->\s*(.+))?$/i);
      }
      
      // Padrão 3: Detecção baseada em padrões comuns do SpecFlow
      if (!stepMatch) {
        stepMatch = trimmedLine.match(/(?:Step\s+binding|Executing\s+step|Running\s+step)[:\s]+(Given|When|Then|And|But|Dado|Quando|Então|E|Mas)\s+(.+?)(?:\s*->\s*(.+))?$/i);
      }
      
      // Padrão 4: Detecção mais flexível - procura keyword seguida de texto
      if (!stepMatch) {
        stepMatch = trimmedLine.match(/(Given|When|Then|And|But|Dado|Quando|Então|E|Mas)[:\-\s]+(.+?)(?:\s*->\s*(.+))?$/i);
      }
      
      if (stepMatch) {
        const keyword = stepMatch[1];
        const text = stepMatch[2] ? stepMatch[2].trim() : '';
        const result = stepMatch[3] || '';
        
        // Normaliza keyword para inglês
        const normalizedKeyword = keyword === 'Dado' || keyword === 'Dada' || keyword === 'Dados' || keyword === 'Dadas' ? 'Given' :
                                 keyword === 'Quando' ? 'When' :
                                 keyword === 'Então' || keyword === 'Entao' ? 'Then' :
                                 keyword === 'E' ? 'And' :
                                 keyword === 'Mas' ? 'But' :
                                 keyword;
        
        if (text) {
          // Função helper para normalizar texto de step (remove parâmetros, espaços extras, etc.)
          const normalizeStepText = (stepText) => {
            return stepText
              .toLowerCase()
              .replace(/\s+/g, ' ')
              .replace(/["']/g, '')
              .replace(/\d+/g, '<num>') // Normaliza números
              .trim();
          };
          
          // Procura o step correspondente nos steps do arquivo .feature
          let stepIndex = -1;
          const normalizedText = normalizeStepText(text);
          
          // Tenta correspondência exata primeiro
          stepIndex = featureSteps.findIndex(step => {
            const stepTextLower = normalizeStepText(step.text);
            return step.keyword.toLowerCase() === normalizedKeyword.toLowerCase() &&
                   stepTextLower === normalizedText;
          });
          
          // Se não encontrou correspondência exata, tenta correspondência parcial
          if (stepIndex < 0) {
            stepIndex = featureSteps.findIndex(step => {
              const stepTextLower = normalizeStepText(step.text);
              return step.keyword.toLowerCase() === normalizedKeyword.toLowerCase() &&
                     (stepTextLower.includes(normalizedText) || 
                      normalizedText.includes(stepTextLower) ||
                      // Correspondência por palavras-chave principais (primeiras 3-4 palavras)
                      stepTextLower.split(' ').slice(0, 4).join(' ') === normalizedText.split(' ').slice(0, 4).join(' '));
            });
          }
          
          // Se ainda não encontrou, usa rastreamento de sequência como fallback
          if (stepIndex < 0 && currentStepIndex >= 0) {
            // Se temos um step atual em execução, tenta o próximo step pendente
            const nextPendingIndex = featureSteps.findIndex((step, idx) => 
              idx > currentStepIndex && 
              step.status === 'pending' && 
              step.keyword.toLowerCase() === normalizedKeyword.toLowerCase()
            );
            if (nextPendingIndex >= 0) {
              stepIndex = nextPendingIndex;
              console.log(`[automation-process-manager] Step detectado por sequência (${streamType}): índice ${stepIndex} (keyword: ${normalizedKeyword})`);
            }
          }
          
          // Se ainda não encontrou e é o primeiro step, tenta o primeiro step pendente com a keyword correta
          if (stepIndex < 0 && currentStepIndex < 0) {
            const firstPendingIndex = featureSteps.findIndex(step => 
              step.status === 'pending' && 
              step.keyword.toLowerCase() === normalizedKeyword.toLowerCase()
            );
            if (firstPendingIndex >= 0) {
              stepIndex = firstPendingIndex;
              console.log(`[automation-process-manager] Primeiro step detectado por sequência (${streamType}): índice ${stepIndex} (keyword: ${normalizedKeyword})`);
            }
          }
          
          if (stepIndex >= 0 && stepIndex !== currentStepIndex) {
            // Marca step anterior como concluído se houver
            if (currentStepIndex >= 0 && currentStepIndex < featureSteps.length) {
              if (featureSteps[currentStepIndex].status === 'running') {
                featureSteps[currentStepIndex].status = 'completed';
                
                // Envia evento de conclusão do step anterior
                if (onLog) {
                  const completedEvent = JSON.stringify({
                    type: 'bdd-step-completed',
                    processId: processId,
                    stepIndex: currentStepIndex,
                    step: {
                      keyword: featureSteps[currentStepIndex].keyword,
                      text: featureSteps[currentStepIndex].text,
                      fullLine: featureSteps[currentStepIndex].fullLine,
                      status: 'completed'
                    }
                  }) + '\n';
                  console.log(`[automation-process-manager] Step ${currentStepIndex} concluído (${streamType}): ${featureSteps[currentStepIndex].fullLine}`);
                  onLog(Buffer.from(completedEvent, 'utf8'));
                }
              }
            }
            
            // Atualiza step atual
            currentStepIndex = stepIndex;
            featureSteps[stepIndex].status = 'running';
            processInfo.currentStepIndex = currentStepIndex;
            
            // Envia evento de progresso para o frontend
            if (onLog) {
              const progressEvent = JSON.stringify({
                type: 'bdd-step-progress',
                processId: processId,
                stepIndex: stepIndex,
                step: {
                  keyword: featureSteps[stepIndex].keyword,
                  text: featureSteps[stepIndex].text,
                  fullLine: featureSteps[stepIndex].fullLine,
                  status: 'running'
                }
              }) + '\n';
              console.log(`[automation-process-manager] Step ${stepIndex} em execução (${streamType}): ${featureSteps[stepIndex].fullLine}`);
              onLog(Buffer.from(progressEvent, 'utf8'));
            }
          } else if (stepIndex >= 0 && stepIndex === currentStepIndex) {
            // Step já está em execução, apenas atualiza status se necessário
            if (featureSteps[stepIndex].status !== 'running') {
              featureSteps[stepIndex].status = 'running';
              processInfo.currentStepIndex = currentStepIndex;
              
              if (onLog) {
                const progressEvent = JSON.stringify({
                  type: 'bdd-step-progress',
                  processId: processId,
                  stepIndex: stepIndex,
                  step: {
                    keyword: featureSteps[stepIndex].keyword,
                    text: featureSteps[stepIndex].text,
                    fullLine: featureSteps[stepIndex].fullLine,
                    status: 'running'
                  }
                }) + '\n';
                onLog(Buffer.from(progressEvent, 'utf8'));
              }
            }
          } else if (stepIndex < 0) {
            // Step não encontrado - log para debug
            console.log(`[automation-process-manager] Step não encontrado (${streamType}): "${normalizedKeyword} ${text}" (processId: ${processId})`);
            console.log(`[automation-process-manager] Steps disponíveis:`, featureSteps.map((s, i) => `${i}: ${s.keyword} ${s.text}`).join(', '));
          }
          
          // Detecta se o step falhou ou foi pulado
          if (result.includes('error') || result.includes('failed') || trimmedLine.toLowerCase().includes('failed') || trimmedLine.toLowerCase().includes('error')) {
            if (currentStepIndex >= 0 && currentStepIndex < featureSteps.length) {
              featureSteps[currentStepIndex].status = 'failed';
              // Marca steps subsequentes como skipped
              for (let i = currentStepIndex + 1; i < featureSteps.length; i++) {
                if (featureSteps[i].status === 'pending' || featureSteps[i].status === 'running') {
                  featureSteps[i].status = 'skipped';
                }
              }
              
              // Envia evento de falha
              if (onLog) {
                const failureEvent = JSON.stringify({
                  type: 'bdd-step-failed',
                  processId: processId,
                  stepIndex: currentStepIndex,
                  step: {
                    keyword: featureSteps[currentStepIndex].keyword,
                    text: featureSteps[currentStepIndex].text,
                    fullLine: featureSteps[currentStepIndex].fullLine,
                    status: 'failed'
                  }
                }) + '\n';
                console.log(`[automation-process-manager] Step ${currentStepIndex} falhou (${streamType}): ${featureSteps[currentStepIndex].fullLine}`);
                onLog(Buffer.from(failureEvent, 'utf8'));
              }
            }
          } else if (result.includes('done') || result.includes('completed') || trimmedLine.toLowerCase().includes('passed') || trimmedLine.toLowerCase().includes('success')) {
            // Step concluído com sucesso
            if (currentStepIndex >= 0 && currentStepIndex < featureSteps.length && featureSteps[currentStepIndex].status === 'running') {
              featureSteps[currentStepIndex].status = 'completed';
              
              // Envia evento de conclusão
              if (onLog) {
                const completedEvent = JSON.stringify({
                  type: 'bdd-step-completed',
                  processId: processId,
                  stepIndex: currentStepIndex,
                  step: {
                    keyword: featureSteps[currentStepIndex].keyword,
                    text: featureSteps[currentStepIndex].text,
                    fullLine: featureSteps[currentStepIndex].fullLine,
                    status: 'completed'
                  }
                }) + '\n';
                console.log(`[automation-process-manager] Step ${currentStepIndex} concluído (${streamType}): ${featureSteps[currentStepIndex].fullLine}`);
                onLog(Buffer.from(completedEvent, 'utf8'));
              }
            }
          }
          
          // Detecção adicional: se detectou um novo step mas não havia step anterior running,
          // significa que o step anterior já terminou implicitamente
          // Isso ajuda a sincronizar quando a detecção de "done" não funciona
          if (stepIndex >= 0 && stepIndex > 0 && currentStepIndex >= 0 && currentStepIndex < stepIndex) {
            // Há um step anterior que deveria ter sido concluído
            for (let i = currentStepIndex; i < stepIndex; i++) {
              if (i < featureSteps.length && featureSteps[i].status === 'running') {
                featureSteps[i].status = 'completed';
                if (onLog) {
                  const completedEvent = JSON.stringify({
                    type: 'bdd-step-completed',
                    processId: processId,
                    stepIndex: i,
                    step: {
                      keyword: featureSteps[i].keyword,
                      text: featureSteps[i].text,
                      fullLine: featureSteps[i].fullLine,
                      status: 'completed'
                    }
                  }) + '\n';
                  console.log(`[automation-process-manager] Step ${i} concluído implicitamente (novo step ${stepIndex} detectado): ${featureSteps[i].fullLine}`);
                  onLog(Buffer.from(completedEvent, 'utf8'));
                }
              }
            }
          }
        }
      }
    };

    // Função helper para limitar tamanho de stdout/stderr
    const limitOutput = (output, currentSize, maxSize) => {
      if (currentSize > maxSize) {
        // Mantém apenas os últimos 2MB (últimas linhas mais recentes)
        const keepSize = 2 * 1024 * 1024;
        const truncated = output.substring(output.length - keepSize);
        // Tenta encontrar a primeira quebra de linha após o truncamento para evitar cortar no meio de uma linha
        const firstNewline = truncated.indexOf('\n');
        if (firstNewline > 0 && firstNewline < 1000) {
          return {
            output: truncated.substring(firstNewline + 1),
            size: keepSize - firstNewline - 1,
            truncated: true
          };
        }
        return {
          output: truncated,
          size: keepSize,
          truncated: true
        };
      }
      return { output, size: currentSize, truncated: false };
    };

    dotnetProcess.stdout.on('data', (data) => {
      const logLine = data.toString();
      const logSize = Buffer.byteLength(logLine, 'utf8');
      
      // Adiciona ao stdout apenas se não excedeu o limite
      if (stdoutSize + logSize <= this.MAX_OUTPUT_SIZE) {
        stdout += logLine;
        stdoutSize += logSize;
      } else {
        // Se excedeu, limita o tamanho
        const result = limitOutput(stdout + logLine, stdoutSize + logSize, this.MAX_OUTPUT_SIZE);
        stdout = result.output;
        stdoutSize = result.size;
        if (result.truncated && !outputTruncated) {
          outputTruncated = true;
          console.warn(`[startAutomation] Output do processo ${processId} foi truncado para economizar memória`);
        }
      }
      
      // Limita o número de logs em memória
      processInfo.logs.push({ type: 'stdout', timestamp: new Date(), message: logLine });
      limitLogs();
      
      // Detecta steps BDD nos logs em tempo real usando função helper
      detectAndProcessBDDSteps(logLine, 'stdout');
      
      // Envia apenas uma quantidade limitada para o frontend para evitar sobrecarga
      // Envia apenas os últimos caracteres se o log estiver muito grande
      if (logSize > this.MAX_FRONTEND_LOGS / 10) {
        // Se uma única linha é muito grande, envia apenas os últimos caracteres
        const truncatedLog = logLine.length > this.MAX_FRONTEND_LOGS / 10 
          ? '...[truncated]...\n' + logLine.substring(logLine.length - this.MAX_FRONTEND_LOGS / 10)
          : logLine;
        onLog(Buffer.from(truncatedLog, 'utf8'));
      } else {
        onLog(data);
      }
    });

    dotnetProcess.stderr.on('data', (data) => {
      const logLine = data.toString();
      const logSize = Buffer.byteLength(logLine, 'utf8');
      
      // Adiciona ao stderr apenas se não excedeu o limite
      if (stderrSize + logSize <= this.MAX_OUTPUT_SIZE) {
        stderr += logLine;
        stderrSize += logSize;
      } else {
        // Se excedeu, limita o tamanho
        const result = limitOutput(stderr + logLine, stderrSize + logSize, this.MAX_OUTPUT_SIZE);
        stderr = result.output;
        stderrSize = result.size;
        if (result.truncated && !outputTruncated) {
          outputTruncated = true;
          console.warn(`[startAutomation] Output do processo ${processId} foi truncado para economizar memória`);
        }
      }
      
      // Limita o número de logs em memória
      processInfo.logs.push({ type: 'stderr', timestamp: new Date(), message: logLine });
      limitLogs();
      
      // Detecta steps BDD nos logs de stderr também (alguns frameworks enviam informações importantes para stderr)
      detectAndProcessBDDSteps(logLine, 'stderr');
      
      // Envia apenas uma quantidade limitada para o frontend
      if (logSize > this.MAX_FRONTEND_LOGS / 10) {
        const truncatedLog = logLine.length > this.MAX_FRONTEND_LOGS / 10 
          ? '...[truncated]...\n' + logLine.substring(logLine.length - this.MAX_FRONTEND_LOGS / 10)
          : logLine;
        onLog(Buffer.from(truncatedLog, 'utf8'));
      } else {
        onLog(data);
      }
    });

    dotnetProcess.on('close', (code) => {
      // Limpa timeout se existir
      if (this.timeouts.has(processId)) {
        clearTimeout(this.timeouts.get(processId));
        this.timeouts.delete(processId);
      }

      // Verifica se o processo ainda está no Map (não foi removido manualmente)
      const currentProcessInfo = this.processes.get(processId);
      if (!currentProcessInfo) {
        console.log(`[startAutomation] Processo ${processId} já foi removido, ignorando evento close`);
        // Não processa a fila se o processo foi removido manualmente (cancelado)
        // A fila será processada quando necessário
        return;
      }
      
      // Verifica se já foi completado para evitar duplicação
      if (currentProcessInfo.completed) {
        console.log(`[startAutomation] Processo ${processId} já foi completado, ignorando evento close duplicado`);
        return;
      }
      
      // Marca como completado ANTES de processar
      currentProcessInfo.completed = true;
      
      // Marca todos os steps que estavam running como completed
      if (currentProcessInfo.featureSteps && currentProcessInfo.currentStepIndex >= 0) {
        const featureSteps = currentProcessInfo.featureSteps;
        const currentStepIndex = currentProcessInfo.currentStepIndex;
        
        // Marca o step atual e qualquer step running como completed
        for (let i = 0; i < featureSteps.length; i++) {
          if (featureSteps[i].status === 'running') {
            featureSteps[i].status = 'completed';
            
            // Envia evento de conclusão para o frontend
            if (onLog) {
              const completedEvent = JSON.stringify({
                type: 'bdd-step-completed',
                processId: processId,
                stepIndex: i,
                step: {
                  keyword: featureSteps[i].keyword,
                  text: featureSteps[i].text,
                  fullLine: featureSteps[i].fullLine,
                  status: 'completed'
                }
              }) + '\n';
              console.log(`[automation-process-manager] Step ${i} concluído (execução finalizada): ${featureSteps[i].fullLine}`);
              onLog(Buffer.from(completedEvent, 'utf8'));
            }
          }
        }
      }
      
      const endTime = new Date();
      const duration = endTime - currentProcessInfo.startTime;
      
      // Verifica se os testes foram realmente executados
      // O dotnet test pode retornar exitCode 0 mesmo quando nenhum teste foi encontrado
      const allOutput = (stdout + stderr).toLowerCase();
      const originalOutput = stdout + stderr;
      
      // Extrai informações do filtro usado (do processInfo armazenado anteriormente)
      const filterInfo = currentProcessInfo.filterInfo || {};
      const usedFilter = filterInfo.combinedFilter || null;
      const requestedFeaturePath = filterInfo.featurePathForFilter || featurePath;
      const requestedScenarioName = filterInfo.scenarioNameForFilter || scenarioName;
      
      const filterDescription = usedFilter 
        ? `Filtro aplicado: "${usedFilter}"`
        : requestedFeaturePath 
          ? `Arquivo solicitado: "${path.basename(requestedFeaturePath)}"${requestedScenarioName ? ` (Cenário: ${requestedScenarioName})` : ''}`
          : 'Nenhum filtro específico aplicado';
      
      // Detecta mensagens sobre arquivos correspondendo ao padrão
      const patternMatch = originalOutput.match(/(\d+)\s+arquivos?\s+de\s+teste\s+no\s+total\s+corresponderam\s+ao\s+padrão\s+especificado/i);
      const filesMatched = patternMatch ? parseInt(patternMatch[1]) : null;
      
      // Extrai informações sobre testes descobertos e executados
      const testsDiscoveredMatch = originalOutput.match(/(\d+)\s+teste?s?\s+descobertos?/i);
      const testsDiscovered = testsDiscoveredMatch ? parseInt(testsDiscoveredMatch[1]) : null;
      
      const testsRunMatch = originalOutput.match(/testes?\s+executados?:?\s*(\d+)/i) || 
                            originalOutput.match(/tests?\s+run:?\s*(\d+)/i) ||
                            originalOutput.match(/total\s+tests?:?\s*(\d+)/i) ||
                            originalOutput.match(/total\s*:\s*(\d+)/i);
      const testsRun = testsRunMatch ? parseInt(testsRunMatch[1]) : null;
      
      // Detecta se testes foram realmente executados (verifica resultados de execução, não apenas descoberta)
      // Verifica múltiplos padrões para garantir detecção correta
      let hasTestExecution = 
        [
          () => allOutput.includes('passed:'),
          () => allOutput.includes('failed:'),
          () => allOutput.includes('skipped:'),
          () => allOutput.includes('test run'),
          () => allOutput.includes('total tests:'),
          () => allOutput.includes('tests run:'),
          () => allOutput.includes('test execution'),
          () => allOutput.includes('sucesso!'),
          () => allOutput.includes('falhas:'),
          () => allOutput.includes('aprovado:'),
          () => allOutput.includes('aprovados:'),
          () => allOutput.includes('reprovado:'),
          () => allOutput.includes('reprovados:'),
          () => allOutput.includes('ignorados:'),
          () => allOutput.includes('aprovado '),
          () => allOutput.includes('reprovado '),
          () => allOutput.includes('-> done:'),
          () => /passed\s+\d+/i.test(originalOutput),
          () => /failed\s+\d+/i.test(originalOutput),
          () => /passed!\s+-/i.test(originalOutput),
          () => /test run successful/i.test(originalOutput),
          () => /test run failed/i.test(originalOutput),
          () => /^\s*Aprovado[^\S\r\n]+\S+/im.test(originalOutput),
          () => /^\s*Reprovado[^\S\r\n]+\S+/im.test(originalOutput),
          () => /Sucesso!\s*-\s*Falhas:/i.test(originalOutput),
          () => /->\s*done:/i.test(originalOutput),
          () => /^Passed\s+[A-Z]/m.test(originalOutput),
          () => /^Failed\s+[A-Z]/m.test(originalOutput)
        ].some(check => {
          try {
            return Boolean(check());
          } catch (err) {
            return false;
          }
        });
      
      if (!hasTestExecution) {
        const logsIndicateExecution = processInfo.logs.some(entry => {
          const message = entry?.message || '';
          return /->\s*done:/i.test(message) ||
                 /aprovado\s+/i.test(message) ||
                 /reprovado\s+/i.test(message) ||
                 /test run/i.test(message);
        });
        if (logsIndicateExecution) {
          hasTestExecution = true;
        }
      }
      
      // Verifica se há erros críticos que indicam que o teste não foi executado
      const hasCriticalErrors = 
        allOutput.includes('no test is available') ||
        allOutput.includes('no tests matched') ||
        allOutput.includes('test discovery failed') ||
        allOutput.includes('no suitable test host');
      
      // Detecta se nenhum arquivo correspondeu ao padrão ou se correspondeu mas não executou testes
      let patternMismatchReason = null;
      let patternMismatchDetails = null;
      
      if (filesMatched !== null && filesMatched === 0) {
        // Nenhum arquivo correspondeu
        patternMismatchReason = `Arquivo de teste não encontrado`;
        patternMismatchDetails = `📋 INFORMAÇÕES DA EXECUÇÃO\n${'='.repeat(60)}\n` +
          `${filterDescription}\n\n` +
          `📊 RESULTADO\n${'-'.repeat(60)}\n` +
          `❌ Nenhum arquivo de teste foi encontrado\n` +
          `🔍 Arquivos procurados: 0\n` +
          `📝 Testes executados: 0\n\n` +
          `❓ O QUE ESTÁ ERRADO?\n${'-'.repeat(60)}\n` +
          `O sistema não conseguiu encontrar o arquivo de teste que você solicitou executar.\n\n` +
          `🔧 POSSÍVEIS CAUSAS E SOLUÇÕES:\n${'-'.repeat(60)}\n` +
          `1️⃣ Nome do arquivo incorreto\n` +
          `   • Verifique se o nome do arquivo está correto\n` +
          `   • O nome deve corresponder exatamente ao arquivo .feature\n` +
          `   • Exemplo: Se o arquivo é "Login.feature", use "Login"\n\n` +
          `2️⃣ Arquivo não foi compilado\n` +
          `   • Compile o projeto antes de executar\n` +
          `   • Execute: dotnet build no projeto de testes\n\n` +
          `3️⃣ Arquivo em projeto diferente\n` +
          `   • Verifique se o arquivo está no projeto correto\n` +
          `   • Confirme o caminho do projeto selecionado\n\n` +
          `4️⃣ Filtro muito específico\n` +
          `   • Se você especificou um cenário, tente executar a feature inteira\n` +
          `   • Use tags (@tag) para filtrar cenários específicos\n\n` +
          `📌 INFORMAÇÕES TÉCNICAS (para desenvolvedores):\n${'-'.repeat(60)}\n` +
          `Filtro aplicado: ${usedFilter || 'N/A'}\n` +
          `O sistema busca testes usando o nome do arquivo .feature.\n` +
          `Cada arquivo .feature gera uma classe de teste com o mesmo nome.\n` +
          `Se o nome não corresponder, o teste não será encontrado.`;
      } else if (filesMatched !== null && filesMatched > 0 && !hasTestExecution) {
        // Arquivos corresponderam mas nenhum teste foi executado
        const hasTestDiscovery = allOutput.includes('discovered') || allOutput.includes('nunit3testexecutor');
        if (hasTestDiscovery && testsDiscovered !== null) {
          patternMismatchReason = `Testes encontrados mas não executados`;
          patternMismatchDetails = `📋 INFORMAÇÕES DA EXECUÇÃO\n${'='.repeat(60)}\n` +
            `${filterDescription}\n\n` +
            `📊 RESULTADO\n${'-'.repeat(60)}\n` +
            `✅ Arquivos encontrados: ${filesMatched}\n` +
            `🔍 Testes descobertos: ${testsDiscovered}\n` +
            `❌ Testes executados: 0\n\n` +
            `❓ O QUE ESTÁ ERRADO?\n${'-'.repeat(60)}\n` +
            `Os testes foram encontrados, mas nenhum foi executado.\n` +
            `Isso geralmente significa que os testes estão bloqueados ou desabilitados.\n\n` +
            `🔧 POSSÍVEIS CAUSAS E SOLUÇÕES:\n${'-'.repeat(60)}\n` +
            `1️⃣ Testes marcados para pular\n` +
            `   • Verifique se os testes estão marcados com [Ignore] ou [Skip]\n` +
            `   • Remova essas marcações para habilitar a execução\n\n` +
            `2️⃣ Filtro de tags muito restritivo\n` +
            `   • Se você usou tags para filtrar, verifique se os testes têm essas tags\n` +
            `   • Adicione as tags necessárias nos cenários do arquivo .feature\n\n` +
            `3️⃣ Configuração do executor\n` +
            `   • Verifique a configuração do projeto de testes\n` +
            `   • Tente executar os testes diretamente pelo Visual Studio ou linha de comando\n\n` +
            `4️⃣ Problema na compilação\n` +
            `   • Recompile o projeto: dotnet build\n` +
            `   • Verifique se não há erros de compilação\n\n` +
            `📌 INFORMAÇÕES TÉCNICAS (para desenvolvedores):\n${'-'.repeat(60)}\n` +
            `Filtro aplicado: ${usedFilter || 'N/A'}\n` +
            `Testes descobertos: ${testsDiscovered}\n` +
            `O executor encontrou os testes, mas não os executou.`;
        } else {
          patternMismatchReason = `Arquivos encontrados mas testes não executados`;
          patternMismatchDetails = `📋 INFORMAÇÕES DA EXECUÇÃO\n${'='.repeat(60)}\n` +
            `${filterDescription}\n\n` +
            `📊 RESULTADO\n${'-'.repeat(60)}\n` +
            `✅ Arquivos encontrados: ${filesMatched}\n` +
            `❌ Testes executados: 0\n\n` +
            `❓ O QUE ESTÁ ERRADO?\n${'-'.repeat(60)}\n` +
            `Arquivos foram encontrados, mas nenhum teste foi executado.\n\n` +
            `🔧 O QUE FAZER:\n${'-'.repeat(60)}\n` +
            `1️⃣ Verifique os logs completos para mais detalhes\n` +
            `2️⃣ Tente executar a feature diretamente pelo Visual Studio\n` +
            `3️⃣ Verifique se há erros de configuração no projeto\n\n` +
            `📌 INFORMAÇÕES TÉCNICAS (para desenvolvedores):\n${'-'.repeat(60)}\n` +
            `Filtro aplicado: ${usedFilter || 'N/A'}\n` +
            `Arquivos corresponderam ao padrão, mas não há evidência de execução.`;
        }
      } else if (filesMatched !== null && filesMatched > 1 && hasTestExecution) {
        // Múltiplos arquivos corresponderam mas testes foram executados - isso é informativo, não um problema
        // NÃO cria patternMismatchReason, apenas patternMismatchDetails para informação
        // Isso permite que seja marcado como sucesso se exitCode for 0
        patternMismatchDetails = `📋 INFORMAÇÕES DA EXECUÇÃO\n${'='.repeat(60)}\n` +
          `${filterDescription}\n\n` +
          `📊 RESULTADO\n${'-'.repeat(60)}\n` +
          `✅ Arquivos encontrados: ${filesMatched}\n` +
          (testsDiscovered !== null ? `🔍 Testes descobertos: ${testsDiscovered}\n` : '') +
          (testsRun !== null ? `✅ Testes executados: ${testsRun}\n` : '') +
          `\n💡 INFORMAÇÃO IMPORTANTE\n${'-'.repeat(60)}\n` +
          `Você solicitou executar um arquivo, mas ${filesMatched > 1 ? 'múltiplos arquivos' : 'todos os cenários deste arquivo'} foram executados.\n` +
          `Isso é normal e esperado! ⚠️\n\n` +
          `📖 COMO FUNCIONA:\n${'-'.repeat(60)}\n` +
          `• Quando você executa um arquivo .feature, TODOS os cenários daquele arquivo são executados\n` +
          `• Se o arquivo tem 5 cenários, todos os 5 serão executados\n` +
          `• Isso acontece porque cada arquivo gera uma classe de teste com todos os seus cenários\n\n` +
          `🎯 COMO EXECUTAR APENAS UM CENÁRIO ESPECÍFICO:\n${'-'.repeat(60)}\n` +
          `1️⃣ Adicione uma tag única no cenário que você quer executar\n` +
          `   Exemplo: @meu_teste_especifico\n\n` +
          `2️⃣ Adicione a tag no arquivo .feature:\n` +
          `   @meu_teste_especifico\n` +
          `   Scenario: Nome do meu cenário\n` +
          `     Given ...\n\n` +
          `3️⃣ O sistema automaticamente usará essa tag para filtrar\n\n` +
          `📌 INFORMAÇÕES TÉCNICAS (para desenvolvedores):\n${'-'.repeat(60)}\n` +
          `Filtro aplicado: ${usedFilter || 'N/A'}\n` +
          `O sistema usa o nome do arquivo para encontrar a classe de teste gerada.\n` +
          `Todos os cenários da mesma classe são executados juntos.\n` +
          `Para filtrar por cenário, use tags (@tag) e Category=tag.`;
        // NÃO define patternMismatchReason aqui - isso permite que seja marcado como sucesso
      } else if (!hasTestExecution && !hasCriticalErrors) {
        // Tenta extrair mais informações do output
        if (allOutput.includes('corresponderam ao padrão')) {
          patternMismatchReason = 'Testes não executados';
          patternMismatchDetails = `📋 INFORMAÇÕES DA EXECUÇÃO\n${'='.repeat(60)}\n` +
            `${filterDescription}\n\n` +
            `📊 RESULTADO\n${'-'.repeat(60)}\n` +
            `❌ Nenhum teste foi executado\n\n` +
            `❓ O QUE ESTÁ ERRADO?\n${'-'.repeat(60)}\n` +
            `Arquivos foram encontrados, mas os testes não foram executados.\n\n` +
            `🔧 O QUE FAZER:\n${'-'.repeat(60)}\n` +
            `1️⃣ Verifique os logs completos abaixo para mais detalhes\n` +
            `2️⃣ Compile o projeto: dotnet build\n` +
            `3️⃣ Tente executar diretamente pelo Visual Studio\n\n` +
            `📌 INFORMAÇÕES TÉCNICAS (para desenvolvedores):\n${'-'.repeat(60)}\n` +
            `Filtro aplicado: ${usedFilter || 'N/A'}\n` +
            `Arquivos corresponderam ao padrão, mas não há evidência de execução.`;
        } else {
          patternMismatchReason = 'Nenhum teste executado';
          patternMismatchDetails = `📋 INFORMAÇÕES DA EXECUÇÃO\n${'='.repeat(60)}\n` +
            `${filterDescription}\n\n` +
            `📊 RESULTADO\n${'-'.repeat(60)}\n` +
            `❌ Nenhum teste foi executado\n\n` +
            `❓ O QUE ESTÁ ERRADO?\n${'-'.repeat(60)}\n` +
            `Não há evidência de que testes foram executados.\n\n` +
            `🔧 O QUE FAZER:\n${'-'.repeat(60)}\n` +
            `1️⃣ Verifique os logs completos abaixo\n` +
            `2️⃣ Confirme se o projeto está compilado corretamente\n` +
            `3️⃣ Verifique se há testes disponíveis no projeto\n\n` +
            `📌 INFORMAÇÕES TÉCNICAS (para desenvolvedores):\n${'-'.repeat(60)}\n` +
            `Filtro aplicado: ${usedFilter || 'N/A'}\n` +
            `Não há evidência de execução de testes.`;
        }
      }
      
      // CRÍTICO: Determina se patternMismatchReason indica um problema real ou apenas informação
      // Se foi apenas informativo (múltiplos arquivos mas testes executados), não conta como problema
      const isRealPatternMismatch = patternMismatchReason && 
        !patternMismatchReason.includes('múltiplos arquivos') && 
        !patternMismatchReason.includes('todos os cenários') &&
        filesMatched !== null && filesMatched > 0 && hasTestExecution === false;
      
      // EXTRAI CENÁRIOS REAIS ANTES DE DETERMINAR O SUCESSO
      // Isso permite determinar o status baseado nos cenários reais executados
      const extractedScenarios = extractScenarios(originalOutput);
      const realScenarios = extractedScenarios || [];
      const passedScenarios = realScenarios.filter(s => s.status === 'passed');
      const failedScenarios = realScenarios.filter(s => s.status === 'failed');
      
      // Verifica se há testes falhados no output (mesmo com exitCode 0, pode haver testes que falharam)
      const hasFailedTests = originalOutput.match(/Failed:\s*(\d+)/i) ||
        originalOutput.match(/Falhas?:\s*(\d+)/i) ||
        originalOutput.match(/Com\s+falha:\s*(\d+)/i) ||
        originalOutput.match(/Falhou?s?:\s*(\d+)/i);
      const failedCount = hasFailedTests ? parseInt(hasFailedTests[1]) : 0;
      const hasPassedTests = originalOutput.match(/Passed:\s*(\d+)/i) ||
        originalOutput.match(/Sucesso:\s*(\d+)/i) ||
        originalOutput.match(/Com\s+sucesso:\s*(\d+)/i) ||
        originalOutput.match(/Aprovado?s?:\s*(\d+)/i);
      const passedCount = hasPassedTests ? parseInt(hasPassedTests[1]) : 0;
      
      // Verifica se há erros reais no output (exceções, stack traces, etc.)
      const hasExceptionOrStackTrace = /Exception|StackTrace|Rastreamento de Pilha|System\.\w+Exception/i.test(originalOutput);
      const hasErrorMessages = /Error:|Erro:|test.*failed|test.*error|NullReferenceException|TimeoutException|Falha na Execução|Com falha|Falhou|Execução falhou/i.test(originalOutput);
      const hasRealErrors = hasExceptionOrStackTrace || hasErrorMessages;
      
      // DETERMINA O SUCESSO BASEADO NOS CENÁRIOS REAIS EXTRAÍDOS E EXIT CODE
      // Prioridade: exit code > cenários reais > contadores do output
      // Exit code diferente de 0 sempre indica falha
      let success = false;
      
      // Se exit code não é 0, sempre marca como falha
      if (code !== 0) {
        success = false;
      } else if (hasTestExecution && !hasCriticalErrors && !isRealPatternMismatch) {
        // Se exit code é 0, verifica os cenários e contadores
        // Se há cenários reais extraídos, usa eles para determinar o sucesso
        if (realScenarios.length > 0) {
          // Se há pelo menos um cenário real que falhou, é falha
          if (failedScenarios.length > 0) {
            success = false;
          } 
          // Se todos os cenários reais passaram e não há erros reais, é sucesso
          else if (passedScenarios.length > 0 && failedScenarios.length === 0 && !hasRealErrors) {
            success = true;
          }
          // Se há cenários mas nenhum passou ou falhou (status desconhecido), usa fallback
          else {
            // Fallback: se exitCode é 0 e não há erros reais, considera sucesso
            success = (code === 0 && !hasRealErrors);
          }
        } 
        // Se não há cenários reais extraídos, usa a lógica de fallback
        else {
          // Fallback: usa contadores do output e exit code
          if (failedCount > 0) {
            success = false;
          } else if (passedCount > 0 && failedCount === 0 && !hasRealErrors) {
            success = true;
          } else {
            // Último fallback: se exitCode é 0 e não há erros reais, considera sucesso
            success = (code === 0 && !hasRealErrors);
          }
        }
      } else {
        // Se não há execução de testes ou há erros críticos, é falha
        success = false;
      }
      
      // Log detalhado para debug
      console.log(`[startAutomation] Análise de sucesso para ${processId}:`, {
        exitCode: code,
        hasTestExecution,
        hasCriticalErrors,
        isRealPatternMismatch,
        patternMismatchReason: patternMismatchReason ? patternMismatchReason.substring(0, 50) : null,
        filesMatched,
        testsRun,
        passedCount,
        failedCount,
        hasRealErrors,
        realScenariosCount: realScenarios.length,
        passedScenariosCount: passedScenarios.length,
        failedScenariosCount: failedScenarios.length,
        success
      });
      
      // Log quando exit code é 1 (sempre falha)
      if (code === 1) {
        console.warn(`[startAutomation] Processo ${processId} retornou exit code 1. Marcando como falha.`);
      }
      
      // Log para debug quando há patternMismatchReason informativo
      if (code === 0 && hasTestExecution && patternMismatchReason && !isRealPatternMismatch) {
        console.log(`[startAutomation] Processo ${processId} teve patternMismatchReason informativo (${patternMismatchReason.substring(0, 50)}...), mas todos os testes passaram. Marcando como sucesso.`);
      }
      
      // Se exitCode é 0 mas não há evidência de execução, marca como falha
      if (code === 0 && !hasTestExecution) {
        console.warn(`[startAutomation] Processo ${processId} retornou exitCode 0 mas nenhum teste foi executado. Motivo: ${patternMismatchReason || 'Desconhecido'}`);
        console.warn(`[startAutomation] Output sample (primeiros 500 chars): ${originalOutput.substring(0, 500)}`);
      }
      
      // Se exitCode é 0 mas há testes falhados, marca como falha
      if (code === 0 && hasTestExecution && failedCount > 0) {
        console.warn(`[startAutomation] Processo ${processId} retornou exitCode 0 mas há ${failedCount} teste(s) falhado(s). Marcando como falha.`);
      }
      
      // Procura por arquivo .trx gerado
      let trxFilePath = null;
      try {
        const testResultsDir = path.join(normalizedProjectPath, 'TestResults');
        if (fs.existsSync(testResultsDir)) {
          const trxFiles = fs.readdirSync(testResultsDir)
            .filter(f => f.endsWith('.trx'))
            .map(f => ({
              name: f,
              path: path.join(testResultsDir, f),
              mtime: fs.statSync(path.join(testResultsDir, f)).mtime
            }))
            .sort((a, b) => b.mtime - a.mtime); // Mais recente primeiro
          
          if (trxFiles.length > 0) {
            // Pega o arquivo .trx mais recente que foi criado durante esta execução
            const executionStartTime = currentProcessInfo.startTime;
            const recentTrx = trxFiles.find(f => f.mtime >= executionStartTime) || trxFiles[0];
            if (recentTrx) {
              trxFilePath = recentTrx.path;
              console.log(`[startAutomation] Arquivo .trx encontrado: ${trxFilePath}`);
            }
          }
        }
      } catch (trxError) {
        console.warn(`[startAutomation] Erro ao procurar arquivo .trx: ${trxError.message}`);
      }
      
      // Cria resultado base
      const baseResult = {
        processId,
        featureName,
        featurePath: currentProcessInfo.featurePath || featurePath, // Inclui o caminho do arquivo
        scenarioName: currentProcessInfo.scenarioName || scenarioName, // Inclui o nome do cenário
        exitCode: code,
        success: success,
        duration,
        stdout,
        stderr,
        logs: currentProcessInfo.logs,
        startTime: currentProcessInfo.startTime,
        endTime,
        hasTestExecution,
        hasCriticalErrors,
        patternMismatchReason,
        patternMismatchDetails,
        filterUsed: usedFilter,
        filesMatched,
        testsDiscovered,
        testsRun,
        trxFilePath: trxFilePath // Caminho do arquivo .trx gerado (se disponível)
      };
      
      // Categoriza a execução
      const categorization = categorizeExecution(baseResult);
      const normalizedOutput = normalizeOutput(originalOutput);
      const groupKey = createGroupKey(normalizedOutput);
      
      // Adiciona informações de categorização ao resultado
      const result = {
        ...baseResult,
        category: categorization.category,
        categoryDescription: categorization.description,
        normalizedOutput: normalizedOutput,
        groupKey: groupKey,
        scenarios: categorization.scenarios || [] // Adiciona cenários individuais ao resultado
      };

      // Adiciona aviso se o output foi truncado
      if (outputTruncated) {
        const truncationWarning = '\n\n[AVISO: Output truncado para economizar memória - apenas os últimos 2MB foram mantidos]\n';
        stdout += truncationWarning;
      }
      
      // Remove do Map ANTES de chamar onComplete e processar fila para evitar re-execuções
      this.processes.delete(processId);
      
      // Limpa logs da memória após processar (mantém apenas os últimos 100 para histórico)
      if (currentProcessInfo.logs.length > 100) {
        currentProcessInfo.logs = currentProcessInfo.logs.slice(-100);
      }
      
      // Limpa referências grandes para liberar memória
      currentProcessInfo.childPids.clear();
      currentProcessInfo.chromePids.clear();
      currentProcessInfo.chromeDriverPids.clear();
      
      // Armazena o resultado da execução para estatísticas
      this.executionResults.set(processId, {
        success: success,
        featureName: featureName,
        scenarioName: scenarioName
      });
      
      console.log(`[startAutomation] Processo ${processId} finalizado (exitCode: ${code}). Execuções ativas: ${this.processes.size}/${this.maxParallel}, Fila: ${this.queue.length}`);
      
      // Limpa processos Chrome órfãos
      this._cleanupChromeProcesses(processId);
      
      // Força garbage collection se disponível (Node.js com --expose-gc)
      if (global.gc && typeof global.gc === 'function') {
        try {
          global.gc();
        } catch (e) {
          // Ignora erros de GC
        }
      }
      
      // Processa próximo da fila ANTES de chamar onComplete
      // Isso garante que a próxima execução inicie imediatamente
      // CRÍTICO: Não processa a fila se estiver cancelando OU se a fila estiver vazia
      if (!this.isCancelling && this.queue.length > 0) {
        this._processNextInQueue();
      } else {
        if (this.isCancelling) {
          console.log(`[startAutomation] Cancelamento em andamento - não processando fila após processo ${processId}`);
        } else if (this.queue.length === 0) {
          console.log(`[startAutomation] Fila vazia - não há nada para processar após processo ${processId}`);
        }
      }
      
      // Chama onComplete por último para notificar o frontend
      onComplete(result);
    });

    dotnetProcess.on('error', (error) => {
      // Limpa timeout se existir
      if (this.timeouts.has(processId)) {
        clearTimeout(this.timeouts.get(processId));
        this.timeouts.delete(processId);
      }

      // Verifica se o processo ainda está no Map e se já foi completado
      const currentProcessInfo = this.processes.get(processId);
      if (!currentProcessInfo) {
        console.log(`[startAutomation] Processo ${processId} já foi removido, ignorando evento error`);
        return;
      }
      
      // Verifica se já foi completado para evitar duplicação
      if (currentProcessInfo.completed) {
        console.log(`[startAutomation] Processo ${processId} já foi completado, ignorando evento error duplicado`);
        return;
      }
      
      // Marca como completado ANTES de processar
      currentProcessInfo.completed = true;

      let errorMessage = error.message;
      
      // Mensagem mais clara se o dotnet não for encontrado
      if (error.code === 'ENOENT') {
        errorMessage = 'dotnet não encontrado. Certifique-se de que o .NET SDK está instalado e no PATH do sistema.';
      }
      
      const result = {
        processId,
        featureName,
        exitCode: -1,
        success: false,
        error: errorMessage,
        logs: currentProcessInfo.logs,
        stderr: errorMessage,
        startTime: currentProcessInfo.startTime,
        endTime: new Date()
      };

      // Remove do Map ANTES de processar fila
      this.processes.delete(processId);
      
      // Limpa logs da memória após processar
      if (currentProcessInfo.logs.length > 100) {
        currentProcessInfo.logs = currentProcessInfo.logs.slice(-100);
      }
      
      // Limpa referências grandes para liberar memória
      currentProcessInfo.childPids.clear();
      currentProcessInfo.chromePids.clear();
      currentProcessInfo.chromeDriverPids.clear();
      
      // Armazena o resultado da execução (falha) para estatísticas
      this.executionResults.set(processId, {
        success: false,
        featureName: featureName,
        scenarioName: scenarioName
      });
      
      console.log(`[startAutomation] Processo ${processId} finalizado com erro. Execuções ativas: ${this.processes.size}/${this.maxParallel}, Fila: ${this.queue.length}`);
      
      // Limpa processos Chrome órfãos
      this._cleanupChromeProcesses(processId);
      
      // Força garbage collection se disponível
      if (global.gc && typeof global.gc === 'function') {
        try {
          global.gc();
        } catch (e) {
          // Ignora erros de GC
        }
      }
      
      // Processa próximo da fila ANTES de chamar onComplete
      // CRÍTICO: Não processa a fila se estiver cancelando OU se a fila estiver vazia
      if (!this.isCancelling && this.queue.length > 0) {
        this._processNextInQueue();
      } else {
        if (this.isCancelling) {
          console.log(`[startAutomation] Cancelamento em andamento - não processando fila após erro no processo ${processId}`);
        } else if (this.queue.length === 0) {
          console.log(`[startAutomation] Fila vazia - não há nada para processar após erro no processo ${processId}`);
        }
      }
      
      // Chama onComplete por último
      onComplete(result);
    });

    // Configura timeout automático para evitar processos presos
    const timeoutId = setTimeout(() => {
      if (this.processes.has(processId)) {
        console.warn(`[startAutomation] Processo ${processId} excedeu tempo limite de ${this.maxExecutionTime / 1000 / 60} minutos, finalizando...`);
        this.stopAutomation(processId).catch(err => {
          console.error(`[startAutomation] Erro ao finalizar processo por timeout: ${err.message}`);
        });
      }
    }, this.maxExecutionTime);
    
    this.timeouts.set(processId, timeoutId);
    // NOTA: O processo já foi adicionado ao Map antes do spawn (linha ~257)
    // Isso garante que outros testes vejam este processo como "em execução" imediatamente
    
    // Inicia rastreamento de processos filhos após um pequeno delay
    // Isso permite que o processo dotnet inicie e crie processos filhos
    setTimeout(async () => {
      if (this.processes.has(processId) && dotnetProcess.pid) {
        await this._trackChildProcesses(processId, dotnetProcess.pid);
        // Rastreia novamente após mais tempo para pegar processos criados depois
        setTimeout(async () => {
          if (this.processes.has(processId) && dotnetProcess.pid) {
            await this._trackChildProcesses(processId, dotnetProcess.pid);
          }
        }, 5000);
      }
    }, 2000);
    
    console.log(`[startAutomation] Processo ${processId} spawnado e iniciado. Execuções ativas: ${this.processes.size}/${this.maxParallel}, Fila: ${this.queue.length}`);
    
    return processId;
  }

  _processNextInQueue() {
    // CRÍTICO: Não processa fila durante cancelamento
    // Verifica múltiplas vezes para evitar condições de corrida
    if (this.isCancelling) {
      console.log(`[processNextInQueue] Cancelamento em andamento, ignorando processamento da fila`);
      return;
    }
    
    // Verifica novamente antes de processar (dupla verificação)
    if (this.isCancelling) {
      console.log(`[processNextInQueue] Cancelamento confirmado na verificação dupla, ignorando processamento da fila`);
      return;
    }
    
    // Processa itens da fila enquanto houver espaço disponível
    while (this.queue.length > 0 && this.processes.size < this.maxParallel) {
      // Verifica novamente antes de cada iteração
      if (this.isCancelling) {
        console.log(`[processNextInQueue] Cancelamento detectado durante processamento da fila, parando...`);
        break;
      }
      
      const next = this.queue.shift();
      console.log(`[processNextInQueue] Processando próximo da fila: ${next.processId} (${this.queue.length} restantes na fila)`);
      
      // Executa de forma assíncrona, mas não bloqueia o loop
      this._executeAutomation(
        next.processId,
        next.args.projectPath,
        next.args.featureName,
        next.args.featurePath,
        next.args.scenarioName,
        next.args.tags,
        next.args.filter,
        next.args.debugPort,
        next.args.onLog,
        next.args.onComplete
      ).then(next.resolve).catch(next.reject);
      
      // Se maxParallel > 1, pode processar múltiplos, mas por enquanto é 1
      // Então quebra após processar um
      break;
    }
    
    // Verifica se a fila terminou (sem processos rodando e sem itens na fila)
    // Mas só se não estiver cancelando
    if (!this.isCancelling) {
      this._checkQueueCompletion();
    }
  }

  _checkQueueCompletion() {
    // Verifica se a fila terminou completamente
    if (this.queue.length === 0 && this.processes.size === 0 && this.onQueueComplete) {
      console.log('[checkQueueCompletion] Fila de execuções finalizada completamente');
      
      // Calcula estatísticas de sucessos e falhas
      const results = Array.from(this.executionResults.values());
      const successCount = results.filter(r => r.success).length;
      const failureCount = results.filter(r => !r.success).length;
      const totalCount = results.length;
      
      console.log(`[checkQueueCompletion] Estatísticas: ${successCount} sucesso(s), ${failureCount} falha(s) de ${totalCount} total`);
      
      // Chama o callback de conclusão da fila com estatísticas
      this.onQueueComplete({
        successCount,
        failureCount,
        totalCount
      });
      
      // Limpa os resultados após notificar
      this.executionResults.clear();
      
      // Limpa o callback para evitar múltiplas chamadas
      this.onQueueComplete = null;
    }
  }

  setQueueCompleteCallback(callback) {
    this.onQueueComplete = callback;
  }

  _cleanupChromeProcesses(processId) {
    // Limpeza ESPECÍFICA apenas dos processos Chrome/ChromeDriver rastreados para esta automação
    const processInfo = this.processes.get(processId);
    if (!processInfo) {
      // Se o processo não existe mais, tenta usar informações armazenadas
      // Mas por segurança, não faz limpeza genérica
      console.log(`[cleanupChromeProcesses] Processo ${processId} não encontrado no Map, pulando limpeza`);
      return;
    }

    const isWindows = os.platform() === 'win32';
    const { exec } = require('child_process');
    
    console.log(`[cleanupChromeProcesses] Limpando processos rastreados para ${processId}...`);
    
    // CRÍTICO: Apenas mata processos que foram rastreados como filhos desta automação
    if (isWindows) {
      // Mata apenas processos Chrome rastreados
      if (processInfo.chromePids && processInfo.chromePids.size > 0) {
        processInfo.chromePids.forEach(pid => {
          exec(`taskkill /F /T /PID ${pid} 2>nul`, (killError) => {
            if (killError && killError.code !== 128 && !killError.message.includes('não foi encontrado')) {
              console.warn(`[cleanupChromeProcesses] Erro ao matar Chrome PID ${pid}: ${killError.message}`);
            } else {
              console.log(`[cleanupChromeProcesses] Processo Chrome PID ${pid} finalizado`);
            }
          });
        });
      }
      
      // Mata apenas processos ChromeDriver rastreados
      if (processInfo.chromeDriverPids && processInfo.chromeDriverPids.size > 0) {
        processInfo.chromeDriverPids.forEach(pid => {
          exec(`taskkill /F /T /PID ${pid} 2>nul`, (killError) => {
            if (killError && killError.code !== 128 && !killError.message.includes('não foi encontrado')) {
              console.warn(`[cleanupChromeProcesses] Erro ao matar ChromeDriver PID ${pid}: ${killError.message}`);
            } else {
              console.log(`[cleanupChromeProcesses] Processo ChromeDriver PID ${pid} finalizado`);
            }
          });
        });
      }
    } else {
      // Unix/Linux/Mac - mata apenas processos rastreados
      if (processInfo.chromePids && processInfo.chromePids.size > 0) {
        processInfo.chromePids.forEach(pid => {
          try {
            process.kill(pid, 'SIGKILL');
            console.log(`[cleanupChromeProcesses] Processo Chrome PID ${pid} finalizado`);
          } catch (error) {
            if (error.code !== 'ESRCH') {
              console.warn(`[cleanupChromeProcesses] Erro ao matar Chrome PID ${pid}: ${error.message}`);
            }
          }
        });
      }
      
      if (processInfo.chromeDriverPids && processInfo.chromeDriverPids.size > 0) {
        processInfo.chromeDriverPids.forEach(pid => {
          try {
            process.kill(pid, 'SIGKILL');
            console.log(`[cleanupChromeProcesses] Processo ChromeDriver PID ${pid} finalizado`);
          } catch (error) {
            if (error.code !== 'ESRCH') {
              console.warn(`[cleanupChromeProcesses] Erro ao matar ChromeDriver PID ${pid}: ${error.message}`);
            }
          }
        });
      }
    }
  }

  /**
   * Método para rastrear processos filhos (Chrome, ChromeDriver) criados por uma automação
   * Este método é chamado periodicamente para atualizar a lista de PIDs rastreados
   * Rastreia recursivamente para pegar processos filhos de processos filhos
   */
  async _trackChildProcesses(processId, parentPid, visitedPids = new Set()) {
    const processInfo = this.processes.get(processId);
    if (!processInfo || !parentPid || visitedPids.has(parentPid)) {
      return;
    }
    
    visitedPids.add(parentPid);

    const isWindows = os.platform() === 'win32';
    const { exec } = require('child_process');
    const { promisify } = require('util');
    const execAsync = promisify(exec);

    try {
      if (isWindows) {
        // Encontra processos filhos do processo pai usando wmic
        const { stdout } = await execAsync(`wmic process where "ParentProcessId=${parentPid}" get ProcessId,Name,CommandLine /format:value 2>nul`);
        
        if (stdout) {
          const lines = stdout.split('\n');
          let currentPid = null;
          let currentName = null;
          let currentCommandLine = null;
          const childPids = [];
          
          lines.forEach(line => {
            if (line.startsWith('ProcessId=')) {
              currentPid = line.replace('ProcessId=', '').trim();
            } else if (line.startsWith('Name=')) {
              currentName = line.replace('Name=', '').trim().toLowerCase();
            } else if (line.startsWith('CommandLine=') && currentPid) {
              currentCommandLine = line.replace('CommandLine=', '').toLowerCase();
              
              // Rastreia apenas processos relacionados à automação
              if (currentName === 'chrome.exe' || currentName === 'chromium.exe') {
                // Verifica se não é do Cursor ou outras aplicações
                if (!currentCommandLine.includes('cursor') && 
                    !currentCommandLine.includes('app-id') &&
                    (currentCommandLine.includes('remote-debugging-port') || 
                     currentCommandLine.includes('--test-type') ||
                     currentCommandLine.includes('--disable-blink-features'))) {
                  processInfo.chromePids.add(currentPid);
                  childPids.push(currentPid);
                }
              } else if (currentName === 'chromedriver.exe') {
                processInfo.chromeDriverPids.add(currentPid);
                childPids.push(currentPid);
              } else if (currentName === 'testhost.exe' || currentName === 'vstest.console.exe' || currentName === 'dotnet.exe') {
                // Rastreia processos dotnet/test relacionados para limpeza recursiva
                childPids.push(currentPid);
              }
              
              // Adiciona aos filhos rastreados
              processInfo.childPids.add(currentPid);
              
              currentPid = null;
              currentName = null;
              currentCommandLine = null;
            }
          });
          
          // Rastreia recursivamente os processos filhos encontrados
          for (const childPid of childPids) {
            await this._trackChildProcesses(processId, childPid, visitedPids);
          }
        }
      } else {
        // Unix/Linux/Mac - usa ps para encontrar processos filhos
        try {
          const { stdout } = await execAsync(`ps -o pid=,ppid=,comm=,args= --ppid ${parentPid} 2>/dev/null`);
          
          if (stdout) {
            const lines = stdout.trim().split('\n');
            const childPids = [];
            
            lines.forEach(line => {
              const parts = line.trim().split(/\s+/);
              if (parts.length >= 4) {
                const pid = parts[0];
                const ppid = parts[1];
                const comm = parts[2].toLowerCase();
                const args = parts.slice(3).join(' ').toLowerCase();
                
                // Rastreia apenas processos relacionados à automação
                if (comm === 'chrome' || comm === 'chromium') {
                  // Verifica se não é do Cursor ou outras aplicações
                  if (!args.includes('cursor') && 
                      !args.includes('app-id') &&
                      (args.includes('remote-debugging-port') || 
                       args.includes('--test-type') ||
                       args.includes('--disable-blink-features'))) {
                    processInfo.chromePids.add(pid);
                    childPids.push(pid);
                  }
                } else if (comm === 'chromedriver') {
                  processInfo.chromeDriverPids.add(pid);
                  childPids.push(pid);
                } else if (comm === 'testhost' || comm === 'vstest' || comm === 'dotnet') {
                  // Rastreia processos dotnet/test relacionados para limpeza recursiva
                  childPids.push(pid);
                }
                
                // Adiciona aos filhos rastreados
                processInfo.childPids.add(pid);
              }
            });
            
            // Rastreia recursivamente os processos filhos encontrados
            for (const childPid of childPids) {
              await this._trackChildProcesses(processId, childPid, visitedPids);
            }
          }
        } catch (error) {
          // Ignora erros - não é crítico
        }
      }
    } catch (error) {
      // Ignora erros silenciosamente - não é crítico se não conseguir rastrear
    }
  }

  /**
   * Método REMOVIDO: _killAllChromeProcesses
   * Este método foi removido por segurança - ele matava processos de outras aplicações
   * Agora usamos apenas _cleanupChromeProcesses que mata apenas processos rastreados
   */

  async stopAllAutomations() {
    console.log(`[stopAllAutomations] Iniciando cancelamento de todas as automações...`);
    
    // Define flag de cancelamento para bloquear criação de novos processos
    this.isCancelling = true;
    
    try {
      // Limpa o callback de conclusão para evitar notificação quando cancelado
      this.onQueueComplete = null;
      
      // Limpa os resultados de execução quando cancela tudo
      this.executionResults.clear();
      
      // PRIMEIRO: Cancela todos os itens da fila ANTES de parar processos
      // Isso evita que novos processos sejam iniciados enquanto estamos cancelando
      const queueItems = [...this.queue];
      this.queue = [];
      console.log(`[stopAllAutomations] Cancelando ${queueItems.length} item(ns) da fila...`);
      queueItems.forEach(item => {
        try {
          item.reject(new Error('Execução cancelada pelo usuário (todos os itens da fila foram cancelados)'));
        } catch (error) {
          console.error(`[stopAllAutomations] Erro ao cancelar item da fila:`, error);
        }
      });
      
      // SEGUNDO: Limpa todos os timeouts
      console.log(`[stopAllAutomations] Limpando ${this.timeouts.size} timeout(s)...`);
      this.timeouts.forEach((timeoutId, processId) => {
        try {
          clearTimeout(timeoutId);
          console.log(`[stopAllAutomations] Timeout do processo ${processId} limpo`);
        } catch (error) {
          console.error(`[stopAllAutomations] Erro ao limpar timeout do processo ${processId}:`, error);
        }
      });
      this.timeouts.clear();
      
      // TERCEIRO: Para todos os processos em execução (sem processar a fila)
      const processIds = Array.from(this.processes.keys());
      console.log(`[stopAllAutomations] Parando ${processIds.length} processo(s) em execução...`);
      
      // Cria um array de promessas para parar todos os processos em paralelo
      const stopPromises = processIds.map(async (pid) => {
        try {
          await this.stopAutomation(pid, false); // false = não processa a fila
        } catch (error) {
          console.error(`[stopAllAutomations] Erro ao parar processo ${pid}:`, error);
          // Mesmo com erro, remove do Map para garantir limpeza
          this.processes.delete(pid);
        }
      });
      
      // Aguarda todos os processos serem parados
      await Promise.all(stopPromises);
      
      // Aguarda um pouco para garantir que os processos foram finalizados
      await new Promise(resolve => setTimeout(resolve, 500));
      
      // QUARTO: Limpa processos Chrome órfãos de todos os processos ANTES de remover do Map
      console.log(`[stopAllAutomations] Limpando processos Chrome órfãos (método individual)...`);
      processIds.forEach(pid => {
        this._cleanupChromeProcesses(pid);
      });
      
      // Aguarda um pouco para processos Chrome serem finalizados
      await new Promise(resolve => setTimeout(resolve, 500));
      
      // QUINTO: Garante que todos os processos foram removidos do Map
      const remainingBeforeCleanup = Array.from(this.processes.keys());
      if (remainingBeforeCleanup.length > 0) {
        console.warn(`[stopAllAutomations] Ainda há ${remainingBeforeCleanup.length} processo(s) no Map, removendo forçadamente...`);
        remainingBeforeCleanup.forEach(pid => {
          const processInfo = this.processes.get(pid);
          if (processInfo && processInfo.process) {
            try {
              // Tenta matar o processo diretamente
              if (!processInfo.process.killed) {
                processInfo.process.kill('SIGKILL');
              }
            } catch (error) {
              console.error(`[stopAllAutomations] Erro ao forçar finalização do processo ${pid}:`, error);
            }
          }
          this.processes.delete(pid);
        });
      }
      
      // SEXTO: Limpeza final de processos Chrome rastreados (removido método agressivo)
      console.log(`[stopAllAutomations] Limpando processos Chrome rastreados...`);
      // Limpeza já foi feita individualmente acima, não precisa de método agressivo
      
      // Aguarda um pouco para garantir que os processos Chrome foram finalizados
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // SÉTIMO: Limpeza final ESPECÍFICA - REMOVIDA por segurança
      // Não matamos mais processos genéricos (testhost.exe, vstest.console.exe, etc)
      // porque eles podem pertencer a outras aplicações
      // A limpeza já foi feita acima através do stopAutomation individual
      // que mata apenas os processos rastreados de cada automação
      
      console.log(`[stopAllAutomations] ✅ Limpeza final concluída (apenas processos rastreados foram limpos)`);
      
      // Verifica se ainda há processos rodando antes de resetar a flag
      const remainingAfterCleanup = Array.from(this.processes.keys());
      if (remainingAfterCleanup.length > 0) {
        console.warn(`[stopAllAutomations] Ainda há ${remainingAfterCleanup.length} processo(s) no Map após limpeza, forçando remoção...`);
        remainingAfterCleanup.forEach(pid => {
          this.processes.delete(pid);
        });
      }
      
      console.log(`[stopAllAutomations] ✅ Cancelamento completo: ${processIds.length} processo(s) e ${queueItems.length} item(ns) da fila cancelados`);
      
      // Garante que a fila está vazia
      if (this.queue.length > 0) {
        console.warn(`[stopAllAutomations] Ainda há ${this.queue.length} item(ns) na fila após cancelamento, limpando...`);
        this.queue.forEach(item => {
          try {
            item.reject(new Error('Execução cancelada pelo usuário'));
          } catch (error) {
            console.error(`[stopAllAutomations] Erro ao cancelar item remanescente:`, error);
          }
        });
        this.queue = [];
      }
      
      // Aguarda mais tempo para garantir que todos os eventos pendentes (close, error) foram processados
      // Eventos assíncronos podem disparar depois que o processo foi morto
      await new Promise(resolve => setTimeout(resolve, 2000));
      
      // Verifica novamente se há processos antes de resetar a flag
      const finalCheck = Array.from(this.processes.keys());
      if (finalCheck.length > 0) {
        console.warn(`[stopAllAutomations] Ainda há ${finalCheck.length} processo(s) após verificação final, removendo...`);
        finalCheck.forEach(pid => {
          this.processes.delete(pid);
        });
      }
      
      // Garante que a fila está realmente vazia antes de resetar
      if (this.queue.length > 0) {
        console.warn(`[stopAllAutomations] Ainda há ${this.queue.length} item(ns) na fila, limpando antes de resetar flag...`);
        this.queue.forEach(item => {
          try {
            item.reject(new Error('Execução cancelada pelo usuário'));
          } catch (error) {
            console.error(`[stopAllAutomations] Erro ao cancelar item remanescente:`, error);
          }
        });
        this.queue = [];
      }
      
      // Aguarda mais um pouco para garantir que eventos assíncronos pendentes foram processados
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Verifica uma última vez se há processos ou itens na fila
      const veryFinalCheck = Array.from(this.processes.keys());
      if (veryFinalCheck.length > 0 || this.queue.length > 0) {
        console.warn(`[stopAllAutomations] Ainda há processos ou itens na fila após espera final. Processos: ${veryFinalCheck.length}, Fila: ${this.queue.length}`);
        // Remove processos remanescentes
        veryFinalCheck.forEach(pid => {
          this.processes.delete(pid);
        });
        // Limpa fila remanescente
        this.queue.forEach(item => {
          try {
            item.reject(new Error('Execução cancelada pelo usuário'));
          } catch (error) {
            // Ignora erros
          }
        });
        this.queue = [];
      }
      
      // NÃO reseta a flag automaticamente - o usuário deve explicitamente iniciar novas execuções
      // Isso evita que eventos assíncronos pendentes processem a fila após o cancelamento
      // A flag será resetada quando o usuário tentar iniciar uma nova automação OU
      // pode ser resetada manualmente se necessário
      console.log(`[stopAllAutomations] ✅ Cancelamento concluído. Flag de cancelamento mantida ativa para evitar reinício automático.`);
      console.log(`[stopAllAutomations] ℹ️  Para permitir novas execuções, a flag será resetada automaticamente quando você iniciar uma nova automação.`);
    } catch (error) {
      console.error(`[stopAllAutomations] Erro durante cancelamento:`, error);
      // Mesmo com erro, tenta limpar o máximo possível
      // Remove todos os processos do Map
      this.processes.clear();
      // Limpa a fila
      this.queue.forEach(item => {
        try {
          item.reject(new Error('Execução cancelada pelo usuário'));
        } catch (e) {
          // Ignora erros
        }
      });
      this.queue = [];
      // Limpa timeouts
      this.timeouts.forEach(timeoutId => clearTimeout(timeoutId));
      this.timeouts.clear();
      
      // NÃO reseta a flag automaticamente mesmo com erro
      // O usuário deve tentar iniciar uma nova automação para resetar
      console.log(`[stopAllAutomations] Erro durante cancelamento, mas limpeza foi realizada. Flag mantida ativa.`);
    }
  }
  
  // Método para resetar a flag de cancelamento (permite novas execuções)
  resetCancellationFlag() {
    if (this.isCancelling) {
      this.isCancelling = false;
      console.log(`[resetCancellationFlag] Flag de cancelamento resetada - execuções permitidas novamente`);
    }
  }

  async stopAutomation(processId, processQueue = true) {
    // Limpa timeout se existir
    if (this.timeouts.has(processId)) {
      clearTimeout(this.timeouts.get(processId));
      this.timeouts.delete(processId);
    }

    // Verifica se está na fila
    const queueIndex = this.queue.findIndex(item => item.processId === processId);
    if (queueIndex !== -1) {
      const queuedItem = this.queue.splice(queueIndex, 1)[0];
      console.log(`[stopAutomation] Processo ${processId} removido da fila`);
      queuedItem.reject(new Error('Execução cancelada pelo usuário'));
      // Remove dos resultados se existir (processo cancelado não deve ser contado)
      this.executionResults.delete(processId);
      // Se processQueue é false, não processa a próxima da fila
      if (processQueue) {
        this._processNextInQueue();
      }
      return;
    }

    const processInfo = this.processes.get(processId);
    if (!processInfo) {
      console.warn(`[stopAutomation] Processo ${processId} não encontrado`);
      return;
    }

    console.log(`[stopAutomation] Parando processo ${processId} (${processInfo.featureName})`);

    try {
      const dotnetProcess = processInfo.process;
      
      // Verifica se o processo ainda está rodando
      if (dotnetProcess.killed) {
        console.log(`[stopAutomation] Processo ${processId} já foi finalizado`);
        this.processes.delete(processId);
        return;
      }

      const isWindows = os.platform() === 'win32';
      
      if (isWindows) {
        // No Windows, precisa matar toda a árvore de processos
        // O dotnet test cria processos filhos (testhost.exe, vstest.console.exe, etc)
        // Usa taskkill para matar toda a árvore de processos
        const { exec } = require('child_process');
        const pid = dotnetProcess.pid;
        
        if (!pid) {
          console.warn(`[stopAutomation] PID não disponível para processo ${processId}`);
          this.processes.delete(processId);
          dotnetProcess.killed = true;
          return;
        }
        
        console.log(`[stopAutomation] Matando árvore de processos do PID ${pid}...`);
        
        // Remove do Map primeiro para evitar re-execuções
        this.processes.delete(processId);
        dotnetProcess.killed = true;
        
        // Mata o processo e todos os filhos usando taskkill de forma síncrona
        // /F = força, /T = árvore (mata filhos), /PID = ID do processo
        return new Promise((resolve) => {
          exec(`taskkill /F /T /PID ${pid}`, (error, stdout, stderr) => {
            if (error) {
              // Se o processo já foi finalizado, não é um erro crítico
              if (error.code === 128 || error.message.includes('não foi encontrado')) {
                console.log(`[stopAutomation] Processo ${pid} já foi finalizado`);
              } else {
                console.error(`[stopAutomation] Erro ao matar árvore de processos: ${error.message}`);
                // Tenta matar apenas o processo pai como fallback
                try {
                  if (!dotnetProcess.killed) {
                    dotnetProcess.kill('SIGKILL');
                  }
                } catch (killError) {
                  console.error(`[stopAutomation] Erro no fallback: ${killError.message}`);
                }
              }
            } else {
              console.log(`[stopAutomation] Árvore de processos ${pid} finalizada`);
            }
            
            // Limpa processos Chrome relacionados
            this._cleanupChromeProcesses(processId);
            
            // Remove dos resultados se existir
            this.executionResults.delete(processId);
            
            // Processa próximo da fila apenas se processQueue for true
            if (processQueue) {
              this._processNextInQueue();
            }
            
            console.log(`[stopAutomation] Processo ${processId} removido do gerenciador`);
            resolve();
          });
        });
      } else {
        // Unix/Linux/Mac - mata o processo e todos os filhos
        const pid = dotnetProcess.pid;
        
        if (!pid) {
          console.warn(`[stopAutomation] PID não disponível para processo ${processId}`);
          this.processes.delete(processId);
          dotnetProcess.killed = true;
          
          // Limpa processos Chrome relacionados (método individual)
          this._cleanupChromeProcesses(processId);
          
          // Remove dos resultados se existir
          this.executionResults.delete(processId);
          
          // Processa próximo da fila apenas se processQueue for true
          if (processQueue) {
            this._processNextInQueue();
          }
          
            // Limpeza já foi feita acima através do _cleanupChromeProcesses
            // Não precisa de método agressivo que mataria processos de outras aplicações
          
          return Promise.resolve();
        }
        
        // Remove do Map primeiro para evitar re-execuções
        this.processes.delete(processId);
        dotnetProcess.killed = true;

        // Tenta encerrar graciosamente primeiro
        // O sinal negativo mata todo o grupo de processos
        try {
          process.kill(-pid, 'SIGTERM');
          console.log(`[stopAutomation] Enviado SIGTERM para grupo de processos ${pid}`);
        } catch (error) {
          if (error.code !== 'ESRCH') {
            console.error(`[stopAutomation] Erro ao enviar SIGTERM: ${error.message}`);
          }
        }
        
        // Aguarda um pouco e então força com SIGKILL
        return new Promise((resolve) => {
          setTimeout(() => {
            try {
              process.kill(-pid, 'SIGKILL');
              console.log(`[stopAutomation] Processo ${pid} forçado a encerrar com SIGKILL`);
            } catch (error) {
              // Se o processo já foi finalizado, não é erro
              if (error.code !== 'ESRCH') {
                console.error(`[stopAutomation] Erro ao forçar encerramento: ${error.message}`);
                // Fallback: tenta matar apenas o processo
                try {
                  if (!dotnetProcess.killed) {
                    dotnetProcess.kill('SIGKILL');
                  }
                } catch (killError) {
                  console.error(`[stopAutomation] Erro no fallback: ${killError.message}`);
                }
              }
            }
            
            // Limpa processos Chrome relacionados (método individual)
            this._cleanupChromeProcesses(processId);
            
            // Remove dos resultados se existir
            this.executionResults.delete(processId);
            
            // Processa próximo da fila apenas se processQueue for true
            if (processQueue) {
              this._processNextInQueue();
            }
            
            console.log(`[stopAutomation] Processo ${processId} removido do gerenciador`);
            resolve();
            
            // Limpeza já foi feita acima através do _cleanupChromeProcesses
            // Não precisa de método agressivo que mataria processos de outras aplicações
          }, 2000);
        });
      }
      
    } catch (error) {
      console.error(`[stopAutomation] Erro ao parar processo ${processId}: ${error.message}`);
      // Remove do Map mesmo em caso de erro para evitar ficar preso
          this.processes.delete(processId);
      
      // Limpa timeout se existir
      if (this.timeouts.has(processId)) {
        clearTimeout(this.timeouts.get(processId));
        this.timeouts.delete(processId);
      }
      
      // Processa próximo da fila apenas se processQueue for true
      if (processQueue) {
        this._processNextInQueue();
      }
      
      throw new Error(`Erro ao parar processo: ${error.message}`);
    }
  }

  getRunningAutomations() {
    const running = Array.from(this.processes.values()).map(info => ({
      id: info.id,
      featureName: info.featureName,
      featurePath: info.featurePath,
      scenarioName: info.scenarioName,
      startTime: info.startTime,
      duration: new Date() - info.startTime
    }));
    
    const queued = this.queue.map(item => ({
      id: item.processId,
      featureName: item.args.featureName,
      featurePath: item.args.featurePath,
      scenarioName: item.args.scenarioName,
      queued: true,
      queuePosition: this.queue.indexOf(item) + 1
    }));
    
    return {
      running,
      queued,
      totalRunning: running.length,
      totalQueued: queued.length,
      maxParallel: this.maxParallel
    };
  }

  cleanupAll() {
    // Limpa todos os timeouts
    this.timeouts.forEach((timeoutId) => {
      clearTimeout(timeoutId);
    });
    this.timeouts.clear();
    
    // Limpa todos os processos
    this.processes.forEach((info, processId) => {
      try {
        info.process.kill('SIGKILL');
        this._cleanupChromeProcesses(processId);
      } catch (error) {
        console.error(`Erro ao limpar processo ${processId}:`, error);
      }
    });
    this.processes.clear();
    
    // Limpa a fila
    this.queue.forEach(item => {
      item.reject(new Error('Sistema desligado'));
    });
    this.queue = [];
    
    console.log('[cleanupAll] Todos os processos e fila foram limpos');
  }

  setMaxParallel(max) {
    if (max < 1) {
      console.warn('[setMaxParallel] maxParallel deve ser pelo menos 1');
      max = 1;
    }
    this.maxParallel = max;
    console.log(`[setMaxParallel] Limite de execuções paralelas ajustado para ${max}`);
    
    // Processa fila se houver espaço
    while (this.queue.length > 0 && this.processes.size < this.maxParallel) {
      this._processNextInQueue();
    }
  }

  getQueueStatus() {
    return {
      queueLength: this.queue.length,
      runningCount: this.processes.size,
      maxParallel: this.maxParallel,
      availableSlots: Math.max(0, this.maxParallel - this.processes.size)
    };
  }
}

module.exports = new AutomationProcessManager();

