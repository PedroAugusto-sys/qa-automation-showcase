const { dialog, shell, Notification } = require('electron');
const path = require('path');
const fs = require('fs');
const automationProcessManager = require('./automation-process-manager');
const featureScanner = require('../automation-runner/feature-scanner');
const logParser = require('../automation-runner/log-parser');
const browserViewManager = require('./browser-view-manager');

function registerHandlers(ipcMain, mainWindow) {
  // Configura callback para quando a fila terminar
  automationProcessManager.setQueueCompleteCallback((stats) => {
    // Verifica se as notificações estão disponíveis
    if (Notification.isSupported()) {
      const { successCount = 0, failureCount = 0, totalCount = 0 } = stats || {};
      
      // Determina o título e ícone baseado nos resultados
      let title = '✅ Fila de Execuções Finalizada';
      let body = '';
      
      if (failureCount === 0 && successCount > 0) {
        // Todos sucessos
        title = '✅ Fila de Execuções Finalizada';
        body = `Todos os ${successCount} cenário${successCount > 1 ? 's' : ''} foram executados com sucesso!`;
      } else if (successCount === 0 && failureCount > 0) {
        // Todas falhas
        title = '❌ Fila de Execuções Finalizada';
        body = `Todos os ${failureCount} cenário${failureCount > 1 ? 's' : ''} falharam.`;
      } else if (successCount > 0 && failureCount > 0) {
        // Misturado
        title = '⚠️ Fila de Execuções Finalizada';
        body = `${successCount} sucesso${successCount > 1 ? 's' : ''} e ${failureCount} falha${failureCount > 1 ? 's' : ''} de ${totalCount} total.`;
      } else {
        // Sem resultados (não deveria acontecer, mas trata o caso)
        title = 'ℹ️ Fila de Execuções Finalizada';
        body = 'A fila foi finalizada.';
      }
      
      // Importa app para garantir que o nome da aplicação está correto
      const { app } = require('electron');
      
      // Garante que o nome da aplicação está definido antes de criar a notificação
      if (app.getName() !== 'Hub de Automação') {
        app.setName('Hub de Automação');
        console.log(`[ipc-handlers] Nome da aplicação redefinido para: ${app.getName()}`);
      }
      
      const notification = new Notification({
        title: title,
        body: body,
        icon: path.join(__dirname, '../assets/a.png'), 
        urgency: failureCount > 0 ? 'critical' : 'normal',
        timeoutType: 'default'
      });
      
      notification.on('click', () => {
        // Foca a janela principal quando a notificação é clicada
        if (mainWindow) {
          if (mainWindow.isMinimized()) mainWindow.restore();
          mainWindow.focus();
        }
      });
      
      notification.show();
      console.log(`[ipc-handlers] Notificação de conclusão da fila enviada: ${successCount} sucesso(s), ${failureCount} falha(s) de ${totalCount} total`);
    } else {
      console.warn('[ipc-handlers] Notificações não são suportadas nesta plataforma');
    }
  });
  // Escanear features disponíveis
  ipcMain.handle('scan-features', async (event, projectPath) => {
    try {
      console.log(`[scan-features] Recebido caminho do projeto: ${projectPath}`);
      const normalizedPath = path.resolve(projectPath);
      console.log(`[scan-features] Caminho normalizado: ${normalizedPath}`);
      
      // Verifica se o caminho existe
      if (!fs.existsSync(normalizedPath)) {
        console.error(`[scan-features] Caminho não existe: ${normalizedPath}`);
        return { success: false, error: `Diretório não encontrado: ${normalizedPath}` };
      }

      // Verifica se é um diretório
      const stats = fs.statSync(normalizedPath);
      if (!stats.isDirectory()) {
        console.error(`[scan-features] Caminho não é um diretório: ${normalizedPath}`);
        return { success: false, error: `Caminho não é um diretório: ${normalizedPath}` };
      }

      const features = await featureScanner.scanFeatures(normalizedPath);
      console.log(`[scan-features] Total de features encontradas: ${features.length}`);
      return { success: true, data: features };
    } catch (error) {
      console.error(`[scan-features] Erro:`, error);
      return { success: false, error: error.message };
    }
  });

  // Executar automação
  ipcMain.handle('run-automation', async (event, config) => {
    const { projectPath, featureName, featurePath, scenarioName, tags, filter } = config;
    
    try {
      const processId = await automationProcessManager.startAutomation(
        projectPath,
        featureName,
        featurePath,
        scenarioName,
        tags,
        filter,
        null, // debugPort sempre null (funcionalidade removida)
        (data) => {
          const dataString = data.toString();
          
          // Divide em linhas para processamento (sempre necessário para filtrar logs)
          const lines = dataString.split('\n');
          
          // Verifica se é um evento BDD antes de enviar como log normal
          // Melhorado: verifica se a string inteira é um JSON de evento BDD (pode vir sem quebras de linha)
          let hasBddEvent = false;
          let parsedEvent = null;
          
          // Tenta parsear a string inteira primeiro (caso o evento venha completo)
          const trimmedData = dataString.trim();
          if (trimmedData && trimmedData.startsWith('{') && trimmedData.includes('"type"') && trimmedData.includes('bdd-')) {
            try {
              parsedEvent = JSON.parse(trimmedData);
              if (parsedEvent && parsedEvent.type && parsedEvent.type.startsWith('bdd-')) {
                hasBddEvent = true;
                console.log(`[ipc-handlers] 📡 Evento BDD detectado (string completa) para processId: ${processId}`);
              }
            } catch (e) {
              // Não é JSON válido como string completa, tenta linha por linha
            }
          }
          
          // Se não parseou como string completa, tenta linha por linha
          if (!hasBddEvent) {
            for (const line of lines) {
              const trimmed = line.trim();
              // Remove caracteres de controle (como \r) antes de tentar parsear
              const cleanLine = trimmed.replace(/\r/g, '');
              
              if (cleanLine && cleanLine.startsWith('{') && cleanLine.includes('"type"') && cleanLine.includes('bdd-')) {
                console.log(`[ipc-handlers] 🔍 DEBUG: Linha parece ser evento BDD:`, cleanLine.substring(0, 200));
                try {
                  parsedEvent = JSON.parse(cleanLine);
                  if (parsedEvent && parsedEvent.type && parsedEvent.type.startsWith('bdd-')) {
                    hasBddEvent = true;
                    console.log(`[ipc-handlers] 📡 Evento BDD detectado (linha por linha) para processId: ${processId}`);
                    break; // Encontrou o evento, para de procurar
                  }
                } catch (e) {
                  console.error(`[ipc-handlers] ❌ Erro ao parsear evento BDD:`, e.message);
                  console.error(`[ipc-handlers] ❌ Linha problemática:`, cleanLine.substring(0, 200));
                }
              }
            }
          }
          
          // Se encontrou um evento BDD, envia pelo canal dedicado
          if (hasBddEvent && parsedEvent) {
            const eventProcessId = parsedEvent.processId || processId;
            console.log(`[ipc-handlers] 📡 Enviando evento BDD ${parsedEvent.type} para processId: ${eventProcessId}`);
            console.log(`[ipc-handlers] 📡 Evento completo:`, JSON.stringify(parsedEvent, null, 2));
            mainWindow.webContents.send('bdd-event', {
              processId: eventProcessId,
              event: parsedEvent
            });
            console.log(`[ipc-handlers] ✅ Evento BDD enviado com sucesso via canal 'bdd-event'`);
          }
          
          // Remove eventos BDD do log normal antes de enviar
          if (hasBddEvent) {
            const filteredLines = lines.filter(line => {
              const trimmed = line.trim();
              if (!trimmed) return true;
              if (trimmed.startsWith('{') && trimmed.includes('"type"') && trimmed.includes('bdd-')) {
                try {
                  JSON.parse(trimmed);
                  return false; // Remove esta linha
                } catch {
                  return true; // Mantém se não for JSON válido
                }
              }
              return true;
            });
            
            const remainingLog = filteredLines.join('\n');
            if (remainingLog.trim()) {
              // Envia logs em tempo real para o renderer (sem eventos BDD)
              mainWindow.webContents.send('automation-log', {
                processId,
                data: remainingLog
              });
            }
          } else {
            // Envia logs em tempo real para o renderer
            mainWindow.webContents.send('automation-log', {
              processId,
              data: dataString
            });
          }
        },
        async (result) => {
          // Salva logs automaticamente em pasta categorizada
          try {
            const { app } = require('electron');
            const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
            const safeFeatureName = (result.featureName || 'unknown').replace(/[<>:"/\\|?*]/g, '_');
            const filename = `logs-${safeFeatureName}-${timestamp}.txt`;
            
            // Usa exatamente os streams de saída capturados, sem gerar resumos adicionais
            const logChunks = [];
            if (Array.isArray(result.logs) && result.logs.length > 0) {
              result.logs.forEach(entry => {
                if (typeof entry === 'string') {
                  logChunks.push(entry);
                } else if (entry && typeof entry === 'object' && 'message' in entry) {
                  logChunks.push(String(entry.message));
                } else {
                  logChunks.push(JSON.stringify(entry));
                }
              });
            }
            if (result.stdout) {
              logChunks.push('===== STDOUT =====');
              logChunks.push(result.stdout);
            }
            if (result.stderr) {
              logChunks.push('===== STDERR =====');
              logChunks.push(result.stderr);
            }
            
            const logContent = logChunks.join('\n');
            
            // Determina pasta de destino baseada na categoria
            const appPath = app.isPackaged 
              ? path.dirname(process.execPath)
              : path.join(__dirname, '../..');
            const logsDir = path.join(appPath, 'logs');
            
            let categoryDir = logsDir;
            if (result.category) {
              categoryDir = path.join(logsDir, result.category);
              
              // Se houver groupKey, cria subpasta para agrupar logs semelhantes
              if (result.groupKey) {
                categoryDir = path.join(categoryDir, result.groupKey);
              }
            }
            
            // Cria diretórios se não existirem
            if (!fs.existsSync(categoryDir)) {
              fs.mkdirSync(categoryDir, { recursive: true });
            }
            
            const filePath = path.join(categoryDir, filename);
            const normalizedFilePath = path.normalize(filePath);
            
            // Salva o arquivo
            fs.writeFileSync(normalizedFilePath, logContent, 'utf8');
            console.log(`[run-automation] Logs salvos automaticamente em: ${normalizedFilePath}`);
          } catch (saveError) {
            console.error('[run-automation] Erro ao salvar logs automaticamente:', saveError);
            // Não interrompe o fluxo se houver erro ao salvar
          }
          
          // Envia resultado final
          mainWindow.webContents.send('automation-complete', {
            processId,
            result
          });
        }
      );
      
      return { success: true, processId };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  // Parar automação
  ipcMain.handle('stop-automation', async (event, processId) => {
    try {
      await automationProcessManager.stopAutomation(processId);
      return { success: true };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  // Parar todas as automações (execuções e fila)
  ipcMain.handle('stop-all-automations', async () => {
    try {
      console.log('[stop-all-automations] Iniciando cancelamento de todas as automações...');
      
      // PRIMEIRO: Captura informações dos processos ANTES de removê-los
      const runningStatus = automationProcessManager.getRunningAutomations();
      const runningProcesses = runningStatus.running || [];
      const queuedProcesses = runningStatus.queued || [];
      
      console.log(`[stop-all-automations] Encontrados ${runningProcesses.length} processo(s) em execução e ${queuedProcesses.length} item(ns) na fila`);
      
      // SEGUNDO: Para todas as automações (processos e fila)
      console.log('[stop-all-automations] Parando todas as automações...');
      await automationProcessManager.stopAllAutomations();
      
      // TERCEIRO: Notifica o frontend que todas as automações foram canceladas
      // Envia eventos de conclusão para cada processo que estava rodando
      runningProcesses.forEach(auto => {
        try {
          mainWindow.webContents.send('automation-complete', {
            processId: auto.id,
            result: {
              processId: auto.id,
              featureName: auto.featureName,
              exitCode: -1,
              success: false,
              error: 'Execução cancelada pelo usuário',
              duration: auto.duration || 0,
              startTime: auto.startTime,
              endTime: new Date(),
              cancelled: true
            }
          });
        } catch (error) {
          console.error(`[stop-all-automations] Erro ao notificar cancelamento do processo ${auto.id}:`, error);
        }
      });
      
      // Notifica também os itens da fila que foram cancelados
      queuedProcesses.forEach(auto => {
        try {
          mainWindow.webContents.send('automation-complete', {
            processId: auto.id,
            result: {
              processId: auto.id,
              featureName: auto.featureName,
              exitCode: -1,
              success: false,
              error: 'Execução cancelada pelo usuário (removida da fila)',
              duration: 0,
              startTime: new Date(),
              endTime: new Date(),
              cancelled: true
            }
          });
        } catch (error) {
          console.error(`[stop-all-automations] Erro ao notificar cancelamento do item da fila ${auto.id}:`, error);
        }
      });
      
      console.log('[stop-all-automations] ✅ Todas as automações foram canceladas com sucesso');
      return { success: true };
    } catch (error) {
      console.error('[stop-all-automations] Erro ao cancelar automações:', error);
      return { success: false, error: error.message };
    }
  });

  // Listar automações em execução
  ipcMain.handle('get-running-automations', async () => {
    try {
      const status = automationProcessManager.getRunningAutomations();
      return { success: true, data: status };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  // Obter status da fila
  ipcMain.handle('get-queue-status', async () => {
    try {
      const status = automationProcessManager.getQueueStatus();
      return { success: true, data: status };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  // Configurar limite de execuções paralelas
  ipcMain.handle('set-max-parallel', async (event, max) => {
    try {
      automationProcessManager.setMaxParallel(max);
      return { success: true };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  // Selecionar diretório do projeto
  ipcMain.handle('select-project-path', async () => {
    try {
      const result = await dialog.showOpenDialog(mainWindow, {
        properties: ['openDirectory'],
        title: 'Selecione o diretório do projeto de automação'
      });
      
      if (!result.canceled && result.filePaths.length > 0) {
        return { success: true, path: result.filePaths[0] };
      }
      
      return { success: false, canceled: true };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  // Criar nova feature
  ipcMain.handle('create-feature', async (event, config) => {
    const { projectPath, featureName, content } = config;
    try {
      const featurePath = path.join(projectPath, 'Features', `${featureName}.feature`);
      const featureDir = path.dirname(featurePath);
      
      // Cria diretório se não existir
      if (!fs.existsSync(featureDir)) {
        fs.mkdirSync(featureDir, { recursive: true });
      }
      
      fs.writeFileSync(featurePath, content, 'utf8');
      return { success: true, path: featurePath };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  // Criar scaffold de Step Definition
  ipcMain.handle('create-step-definition', async (event, config) => {
    const { projectPath, className, namespace, content } = config;
    try {
      const stepPath = path.join(projectPath, 'StepDefinitions', `${className}.cs`);
      const stepDir = path.dirname(stepPath);
      
      if (!fs.existsSync(stepDir)) {
        fs.mkdirSync(stepDir, { recursive: true });
      }
      
      fs.writeFileSync(stepPath, content, 'utf8');
      return { success: true, path: stepPath };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  // Criar scaffold de Page Object
  ipcMain.handle('create-page-object', async (event, config) => {
    const { projectPath, className, namespace, content } = config;
    try {
      const poPath = path.join(projectPath, 'PageObjects', `${className}.cs`);
      const poDir = path.dirname(poPath);
      
      if (!fs.existsSync(poDir)) {
        fs.mkdirSync(poDir, { recursive: true });
      }
      
      fs.writeFileSync(poPath, content, 'utf8');
      return { success: true, path: poPath };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  // Exportar logs
  ipcMain.handle('export-logs', async (event, config) => {
    const { app } = require('electron');
    const { logs, format, filename, category, groupKey, featureName, executionInfo } = config;
    try {
      // No build, usa o diretório do executável; em dev, usa o diretório raiz
      const appPath = app.isPackaged 
        ? path.dirname(process.execPath)
        : path.join(__dirname, '../..');
      const logsDir = path.join(appPath, 'logs');
      
      // Determina a pasta de destino baseada na categoria
      // Estrutura: logs/categoria/grupo (se groupKey existir)
      let categoryDir = logsDir;
      if (category && category !== 'sem_categoria') {
        // Normaliza o nome da categoria para usar como nome de pasta
        const sanitizedCategory = category
          .replace(/[^a-zA-Z0-9_-]/g, '_')
          .toLowerCase();
        
        categoryDir = path.join(logsDir, sanitizedCategory);
        
        // Se houver groupKey, cria subpasta para agrupar logs semelhantes
        if (groupKey) {
          categoryDir = path.join(categoryDir, groupKey);
        }
      } else {
        // Se não há categoria, coloca em uma pasta "outros"
        categoryDir = path.join(logsDir, 'outros');
      }
      
      // Cria diretórios se não existirem
      if (!fs.existsSync(categoryDir)) {
        fs.mkdirSync(categoryDir, { recursive: true });
      }
      
      // Garante que o filename sempre começa com o nome da feature/task
      let finalFilename = filename;
      if (featureName && featureName !== 'unknown') {
        const sanitizedFeatureName = featureName
          .replace(/[^a-zA-Z0-9_-]/g, '_')
          .substring(0, 50);
        
        const ext = path.extname(filename) || (format === 'json' ? '.json' : '.txt');
        const nameWithoutExt = path.basename(filename, ext);
        
        // Se o nome do arquivo não começa com o nome da feature, adiciona no início
        if (!nameWithoutExt.startsWith(sanitizedFeatureName)) {
          finalFilename = `${sanitizedFeatureName}-${nameWithoutExt}${ext}`;
        } else {
          // Se já começa, usa o nome original
          finalFilename = filename;
        }
      }
      
      const filePath = path.join(categoryDir, finalFilename);
      
      // Normaliza o caminho para garantir formato correto
      const normalizedFilePath = path.normalize(filePath);
      
      console.log('[export-logs] Exportando para:', normalizedFilePath);
      console.log('[export-logs] Categoria:', category || 'sem categoria');
      console.log('[export-logs] Grupo:', groupKey || 'sem grupo');
      console.log('[export-logs] Feature:', featureName || 'sem nome');
      
      // Garante que logs seja um array
      let logsArray = logs;
      if (!Array.isArray(logs)) {
        logsArray = typeof logs === 'string' ? [logs] : [];
      }
      
      // Funções auxiliares para formatação
      const formatDate = (date) => {
        if (!date) return 'N/A';
        try {
          return new Date(date).toLocaleString('pt-BR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
          });
        } catch {
          return String(date);
        }
      };
      
      const formatDuration = (ms) => {
        if (!ms && ms !== 0) return 'N/A';
        const seconds = Math.floor(ms / 1000);
        const minutes = Math.floor(seconds / 60);
        const hours = Math.floor(minutes / 60);
        if (hours > 0) {
          return `${hours}h ${minutes % 60}m ${seconds % 60}s`;
        } else if (minutes > 0) {
          return `${minutes}m ${seconds % 60}s`;
        } else {
          return `${seconds}s`;
        }
      };
      
      // Converte logs para string
      const logsContent = logsArray.map(log => {
        if (typeof log === 'string') return log;
        if (log && typeof log === 'object' && log.message) return log.message;
        return String(log);
      }).join('\n');
      
      if (format === 'json') {
        // Para JSON, cria um objeto estruturado com informações de execução e logs
        const exportData = {
          executionInfo: executionInfo || null,
          logs: logsArray.map(log => {
            if (typeof log === 'string') return log;
            if (log && typeof log === 'object' && log.message) return log.message;
            return String(log);
          })
        };
        fs.writeFileSync(normalizedFilePath, JSON.stringify(exportData, null, 2), 'utf8');
      } else {
        // Para TXT, cria header com informações de execução PRIMEIRO, depois os logs
        let fileContent = '';
        
        // Adiciona informações de execução no início do arquivo
        if (executionInfo) {
          fileContent += `${'='.repeat(80)}\n`;
          fileContent += `INFORMAÇÕES DE EXECUÇÃO\n`;
          fileContent += `${'='.repeat(80)}\n`;
          fileContent += `📄 Feature: ${executionInfo.featureName || 'N/A'}\n`;
          if (executionInfo.featurePath) {
            fileContent += `📁 Arquivo: ${executionInfo.featurePath}\n`;
          }
          if (executionInfo.scenarioName) {
            fileContent += `🎯 Cenário: ${executionInfo.scenarioName}\n`;
          }
          fileContent += `✅ Status: ${executionInfo.success ? 'SUCESSO' : 'FALHA'}\n`;
          fileContent += `⏱️  Duração: ${formatDuration(executionInfo.duration)}\n`;
          fileContent += `🕐 Início: ${formatDate(executionInfo.startTime)}\n`;
          fileContent += `🕐 Fim: ${formatDate(executionInfo.endTime)}\n`;
          fileContent += `🔢 Exit Code: ${executionInfo.exitCode !== undefined ? executionInfo.exitCode : 'N/A'}\n`;
          if (executionInfo.category) {
            fileContent += `🏷️  Categoria: ${executionInfo.category}\n`;
          }
          if (executionInfo.categoryDescription) {
            fileContent += `\n${executionInfo.categoryDescription}\n`;
          }
          
          // Adiciona informações detalhadas de cada cenário se disponíveis
          if (executionInfo.scenarios && Array.isArray(executionInfo.scenarios) && executionInfo.scenarios.length > 0) {
            fileContent += `\n${'='.repeat(80)}\n`;
            fileContent += `CENÁRIOS EXECUTADOS (${executionInfo.scenarios.length})\n`;
            fileContent += `${'='.repeat(80)}\n\n`;
            
            executionInfo.scenarios.forEach((scenario, index) => {
              const statusIcon = scenario.status === 'passed' ? '✅' : scenario.status === 'failed' ? '❌' : '⏸️';
              const statusText = scenario.status === 'passed' ? 'APROVADO' : scenario.status === 'failed' ? 'FALHOU' : 'NÃO EXECUTADO';
              const duration = scenario.duration ? ` (${scenario.duration}ms)` : '';
              
              fileContent += `${statusIcon} CENÁRIO ${index + 1}: ${scenario.name}${duration}\n`;
              fileContent += `Status: ${statusText}\n`;
              
              if (scenario.featureName) {
                fileContent += `Feature: ${scenario.featureName}\n`;
              }
              
              // BDD Steps
              if (scenario.bddSteps && scenario.bddSteps.length > 0) {
                fileContent += `\n📝 Passos BDD Executados:\n`;
                scenario.bddSteps.forEach((step, stepIndex) => {
                  const stepStatusIcon = step.result.includes('error') ? '❌' : 
                                        step.result.includes('skipped') ? '⏭️' : '✅';
                  fileContent += `  ${stepStatusIcon} ${step.fullLine}\n`;
                });
              }
              
              // Mensagem de Erro
              if (scenario.errorMessage) {
                fileContent += `\n💥 Mensagem de Erro:\n`;
                fileContent += `${scenario.errorMessage}\n`;
              }
              
              // Stack Trace
              if (scenario.stackTrace) {
                fileContent += `\n📍 Rastreamento de Pilha:\n`;
                fileContent += `${scenario.stackTrace}\n`;
              }
              
              // Output Padrão
              if (scenario.standardOutput && scenario.standardOutput.trim()) {
                fileContent += `\n📋 Saída Padrão:\n`;
                fileContent += `${scenario.standardOutput}\n`;
              }
              
              fileContent += `\n${'-'.repeat(80)}\n\n`;
            });
          }
          
          fileContent += `\n${'='.repeat(80)}\n`;
          fileContent += `LOGS DA EXECUÇÃO\n`;
          fileContent += `${'='.repeat(80)}\n\n`;
        }
        
        // Adiciona os logs do console após as informações de execução
        fileContent += logsContent;
        
        fs.writeFileSync(normalizedFilePath, fileContent, 'utf8');
      }
      
      console.log('[export-logs] Arquivo exportado com sucesso:', normalizedFilePath);
      
      // Retorna o caminho normalizado e absoluto
      return { success: true, path: path.resolve(normalizedFilePath) };
    } catch (error) {
      console.error('[export-logs] Erro:', error);
      return { success: false, error: error.message };
    }
  });

  // Obter configuração
  ipcMain.handle('get-config', async () => {
    const Store = require('electron-store');
    const store = new Store();
    return {
      projectPath: store.get('projectPath', ''),
      maxParallelExecutions: store.get('maxParallelExecutions', 3),
      defaultTimeout: store.get('defaultTimeout', 300000)
    };
  });

  // Salvar configuração
  ipcMain.handle('save-config', async (event, config) => {
    const Store = require('electron-store');
    const store = new Store();
    
    if (config.projectPath) store.set('projectPath', config.projectPath);
    if (config.maxParallelExecutions) store.set('maxParallelExecutions', config.maxParallelExecutions);
    if (config.defaultTimeout) store.set('defaultTimeout', config.defaultTimeout);
    
    return { success: true };
  });

  // Ler arquivo Contexto.cs
  ipcMain.handle('read-contexto-file', async (event, projectPath) => {
    try {
      const normalizedProjectPath = path.resolve(projectPath);
      
      // Tenta encontrar o arquivo Contexto.cs
      // Estratégia: busca em múltiplos locais possíveis
      let normalizedPath = null;
      
      // 1. Tenta: projectPath/QaShowcase.Specflow.IntegrationTests/PageObjects/Contexto.cs (raiz do projeto)
      let contextoPath = path.join(normalizedProjectPath, 'QaShowcase.Specflow.IntegrationTests', 'PageObjects', 'Contexto.cs');
      if (fs.existsSync(contextoPath)) {
        normalizedPath = path.resolve(contextoPath);
        console.log(`[read-contexto-file] Arquivo encontrado em: ${normalizedPath}`);
      }
      
      // 2. Se não encontrou, tenta: projectPath/PageObjects/Contexto.cs (se projectPath já aponta para QaShowcase.Specflow.IntegrationTests)
      if (!normalizedPath) {
        contextoPath = path.join(normalizedProjectPath, 'PageObjects', 'Contexto.cs');
        if (fs.existsSync(contextoPath)) {
          normalizedPath = path.resolve(contextoPath);
          console.log(`[read-contexto-file] Arquivo encontrado em: ${normalizedPath}`);
        }
      }
      
      // 3. Se ainda não encontrou, busca a pasta QaShowcase.Specflow.IntegrationTests recursivamente
      if (!normalizedPath) {
        console.log(`[read-contexto-file] Arquivo não encontrado nos caminhos diretos, buscando recursivamente...`);
        
        function findQaShowcaseDirectory(dir) {
          try {
            const entries = fs.readdirSync(dir, { withFileTypes: true });
            for (const entry of entries) {
              if (entry.isDirectory() && entry.name === 'QaShowcase.Specflow.IntegrationTests') {
                return path.join(dir, entry.name);
              }
              if (entry.isDirectory() && !entry.name.startsWith('.') && entry.name !== 'node_modules') {
                const found = findQaShowcaseDirectory(path.join(dir, entry.name));
                if (found) return found;
              }
            }
          } catch (err) {
            // Ignora erros de permissão
          }
          return null;
        }
        
        const qashowcaseDir = findQaShowcaseDirectory(normalizedProjectPath);
        if (qashowcaseDir) {
          contextoPath = path.join(qashowcaseDir, 'PageObjects', 'Contexto.cs');
          if (fs.existsSync(contextoPath)) {
            normalizedPath = path.resolve(contextoPath);
            console.log(`[read-contexto-file] Pasta QaShowcase.Specflow.IntegrationTests encontrada em: ${qashowcaseDir}`);
          }
        }
      }
      
      if (!fs.existsSync(normalizedPath)) {
        return { success: false, error: `Arquivo não encontrado: ${normalizedPath}\n\nProcurei em:\n- ${path.join(normalizedProjectPath, 'QaShowcase.Specflow.IntegrationTests', 'PageObjects', 'Contexto.cs')}\n\nCertifique-se de que o projectPath aponta para a raiz do projeto.` };
      }
      
      const content = fs.readFileSync(normalizedPath, 'utf8');
      
      // Extrai BaseTenant e BaseUrl do arquivo
      const baseTenantMatch = content.match(/BaseTenant\s*\{\s*get;\s*set;\s*\}\s*=\s*"([^"]+)"/);
      const baseUrlMatch = content.match(/BaseUrl\s*\{\s*get;\s*set;\s*\}\s*=\s*"([^"]+)"/);
      
      return {
        success: true,
        data: {
          baseTenant: baseTenantMatch ? baseTenantMatch[1] : '',
          baseUrl: baseUrlMatch ? baseUrlMatch[1] : '',
          filePath: normalizedPath,
          content: content
        }
      };
    } catch (error) {
      console.error('[read-contexto-file] Erro:', error);
      return { success: false, error: error.message };
    }
  });

  // Escrever arquivo Contexto.cs
  ipcMain.handle('write-contexto-file', async (event, projectPath, baseTenant, baseUrl) => {
    try {
      const normalizedProjectPath = path.resolve(projectPath);
      
      // Tenta encontrar o arquivo Contexto.cs (mesma lógica do read)
      let normalizedPath = null;
      
      // 1. Tenta: projectPath/QaShowcase.Specflow.IntegrationTests/PageObjects/Contexto.cs (raiz do projeto)
      let contextoPath = path.join(normalizedProjectPath, 'QaShowcase.Specflow.IntegrationTests', 'PageObjects', 'Contexto.cs');
      if (fs.existsSync(contextoPath)) {
        normalizedPath = path.resolve(contextoPath);
      }
      
      // 2. Se não encontrou, tenta: projectPath/PageObjects/Contexto.cs (se projectPath já aponta para QaShowcase.Specflow.IntegrationTests)
      if (!normalizedPath) {
        contextoPath = path.join(normalizedProjectPath, 'PageObjects', 'Contexto.cs');
        if (fs.existsSync(contextoPath)) {
          normalizedPath = path.resolve(contextoPath);
        }
      }
      
      // 3. Se ainda não encontrou, busca a pasta QaShowcase.Specflow.IntegrationTests recursivamente
      if (!normalizedPath) {
        function findQaShowcaseDirectory(dir) {
          try {
            const entries = fs.readdirSync(dir, { withFileTypes: true });
            for (const entry of entries) {
              if (entry.isDirectory() && entry.name === 'QaShowcase.Specflow.IntegrationTests') {
                return path.join(dir, entry.name);
              }
              if (entry.isDirectory() && !entry.name.startsWith('.') && entry.name !== 'node_modules') {
                const found = findQaShowcaseDirectory(path.join(dir, entry.name));
                if (found) return found;
              }
            }
          } catch (err) {
            // Ignora erros de permissão
          }
          return null;
        }
        
        const qashowcaseDir = findQaShowcaseDirectory(normalizedProjectPath);
        if (qashowcaseDir) {
          contextoPath = path.join(qashowcaseDir, 'PageObjects', 'Contexto.cs');
          if (fs.existsSync(contextoPath)) {
            normalizedPath = path.resolve(contextoPath);
          }
        }
      }
      
      if (!fs.existsSync(normalizedPath)) {
        return { success: false, error: `Arquivo não encontrado: ${normalizedPath}\n\nProcurei em:\n- ${path.join(normalizedProjectPath, 'QaShowcase.Specflow.IntegrationTests', 'PageObjects', 'Contexto.cs')}\n\nCertifique-se de que o projectPath aponta para a raiz do projeto.` };
      }
      
      let content = fs.readFileSync(normalizedPath, 'utf8');
      
      // Substitui BaseTenant
      if (baseTenant !== undefined) {
        content = content.replace(
          /(BaseTenant\s*\{\s*get;\s*set;\s*\}\s*=\s*)"[^"]*"/,
          `$1"${baseTenant}"`
        );
      }
      
      // Substitui BaseUrl
      if (baseUrl !== undefined) {
        content = content.replace(
          /(BaseUrl\s*\{\s*get;\s*set;\s*\}\s*=\s*)"[^"]*"/,
          `$1"${baseUrl}"`
        );
      }
      
      // Salva o arquivo
      fs.writeFileSync(normalizedPath, content, 'utf8');
      
      console.log(`[write-contexto-file] Arquivo atualizado: ${normalizedPath}`);
      return { success: true, filePath: normalizedPath };
    } catch (error) {
      console.error('[write-contexto-file] Erro:', error);
      return { success: false, error: error.message };
    }
  });

  // Abrir local do arquivo no explorador
  ipcMain.handle('show-item-in-folder', async (event, filePath) => {
    try {
      console.log('[show-item-in-folder] Caminho recebido:', filePath);
      
      if (!filePath) {
        console.error('[show-item-in-folder] Caminho não fornecido');
        return { success: false, error: 'Caminho não fornecido' };
      }
      
      // Normaliza o caminho e resolve para caminho absoluto
      const normalizedPath = path.resolve(filePath);
      console.log('[show-item-in-folder] Caminho normalizado:', normalizedPath);
      
      // Verifica se o arquivo existe
      if (fs.existsSync(normalizedPath)) {
        const stats = fs.statSync(normalizedPath);
        if (stats.isFile()) {
          // Se é um arquivo, mostra o arquivo selecionado no explorador
          console.log('[show-item-in-folder] Abrindo arquivo no explorador:', normalizedPath);
          shell.showItemInFolder(normalizedPath);
          return { success: true };
        } else if (stats.isDirectory()) {
          // Se é um diretório, abre o diretório
          console.log('[show-item-in-folder] Abrindo diretório:', normalizedPath);
          shell.openPath(normalizedPath);
          return { success: true };
        }
      }
      
      // Se o arquivo não existe, tenta abrir o diretório pai
      const dirPath = path.dirname(normalizedPath);
      console.log('[show-item-in-folder] Arquivo não encontrado. Tentando abrir diretório pai:', dirPath);
      
      if (fs.existsSync(dirPath)) {
        // Tenta mostrar o diretório (abre o explorador na pasta)
        try {
          shell.openPath(dirPath);
          console.log('[show-item-in-folder] Diretório aberto:', dirPath);
          return { success: true };
        } catch (openError) {
          console.error('[show-item-in-folder] Erro ao abrir diretório:', openError);
          // Fallback: tenta usar showItemInFolder no diretório
          if (fs.existsSync(dirPath)) {
            shell.showItemInFolder(dirPath);
            return { success: true };
          }
        }
      }
      
      console.error('[show-item-in-folder] Diretório não encontrado:', dirPath);
      return { success: false, error: `Diretório não encontrado: ${dirPath}` };
    } catch (error) {
      console.error('[show-item-in-folder] Erro:', error);
      return { success: false, error: error.message };
    }
  });

  // Handlers para gerenciar BrowserView
  ipcMain.handle('create-browser-view', async (event, processId, debugPort) => {
    try {
      const success = await browserViewManager.createBrowserView(processId, debugPort);
      return { success };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  ipcMain.handle('destroy-browser-view', async (event, processId) => {
    try {
      browserViewManager.destroyBrowserView(processId);
      return { success: true };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });

  ipcMain.handle('get-active-browser-views', async () => {
    try {
      const views = browserViewManager.getActiveBrowserViews();
      return { success: true, data: views };
    } catch (error) {
      return { success: false, error: error.message };
    }
  });
}

module.exports = { registerHandlers };

