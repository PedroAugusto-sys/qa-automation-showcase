import React, { useState, useEffect, useRef } from 'react';
import automationService from '../services/automation-service';
import ConfirmModal from './ConfirmModal';
import './AutomationRunner.css';

function AutomationRunner({ projectPath }) {
  const [features, setFeatures] = useState([]);
  const [selectedFeatures, setSelectedFeatures] = useState([]);
  const [selectedScenarios, setSelectedScenarios] = useState(new Map()); // Map<featurePath, Set<scenarioName>>
  const [expandedFeatures, setExpandedFeatures] = useState(new Set()); // Set<featurePath>
  const [runningAutomations, setRunningAutomations] = useState(new Map());
  const [logs, setLogs] = useState(new Map());
  const [filter, setFilter] = useState('');
  const [selectedTags, setSelectedTags] = useState([]);
  const [tagsExpanded, setTagsExpanded] = useState(true);
  const [hoveredTagFeaturePath, setHoveredTagFeaturePath] = useState(null);
  const [viewingScenario, setViewingScenario] = useState(null); // { feature, scenario }
  const [isRunningSelected, setIsRunningSelected] = useState(false);
  const [executeInQueue, setExecuteInQueue] = useState(true); // Switch para executar em fila
  const [failedItems, setFailedItems] = useState(new Set()); // Set de processIds que falharam
  const [fadingOutItems, setFadingOutItems] = useState(new Set()); // Set de processIds em fadeout
  const [showRamWarningModal, setShowRamWarningModal] = useState(false);
  const [showStopAllModal, setShowStopAllModal] = useState(false);
  const [pendingExecutions, setPendingExecutions] = useState(null); // Armazena execuções pendentes quando modal é exibido
  const [isStoppingAll, setIsStoppingAll] = useState(false); // Flag para mostrar animação de cancelamento
  const [isCancellingAll, setIsCancellingAll] = useState(false); // Flag para cancelar execuções pendentes no frontend
  const [recentAutomations, setRecentAutomations] = useState([]); // Histórico visual de execuções concluídas
  const [currentExecutionResult, setCurrentExecutionResult] = useState(null); // Resultado da execução atual
  const [bddSteps, setBddSteps] = useState(new Map()); // Map<processId, Array<step>> - Steps BDD por processo
  const timeoutRefs = useRef(new Map()); // Map de processId -> timeout IDs para limpeza
  const abortControllerRef = useRef(null); // AbortController para cancelar execuções pendentes
  const processedProcessIdsRef = useRef(new Set()); // Rastreia processIds já processados

  useEffect(() => {
    if (projectPath) {
      loadFeatures();
      loadRunningAutomations();
      
      // Atualiza a lista de automações em execução periodicamente
      // Intervalo reduzido para atualização mais frequente da fila
      const intervalId = setInterval(() => {
        loadRunningAutomations();
      }, 1000); // Atualiza a cada 1 segundo para melhor responsividade da fila
      
      // Limpeza periódica de logs antigos para economizar memória (limpeza será feita no handleAutomationComplete)
      
      return () => {
        clearInterval(intervalId);
      };
    }
  }, [projectPath]);

  useEffect(() => {
    // Remove listeners antigos antes de adicionar novos para evitar duplicação
    automationService.removeAutomationLogListener();
    automationService.removeBddEventListener();
    automationService.removeAutomationCompleteListener();

    // Usa um Set para rastrear os últimos logs e evitar duplicação
    const recentLogs = new Map(); // Map<processId, Set<lastFewLogs>>
    
    // Usa um Set compartilhado para rastrear quais processIds já foram processados
    const processedProcessIds = processedProcessIdsRef.current;
    processedProcessIds.clear();

    // Configurações de otimização de memória no frontend
    const MAX_LOG_CHARACTERS_PER_PROCESS = 50000; // Máximo de 50KB de logs por processo no frontend
    const MAX_LOG_ENTRIES_FRONTEND = 2000; // Máximo de entradas de log por processo

    // Handler para logs de execução
    const handleAutomationLog = (data) => {
      let { processId, data: logData } = data;
      
      // Verifica se é um evento de steps BDD (JSON)
      // Os eventos podem vir em uma linha completa ou em chunks
      const logString = typeof logData === 'string' ? logData : logData.toString();
      
      // Tenta encontrar eventos JSON na string (pode ter múltiplas linhas)
      const lines = logString.split('\n');
      
      // Debug: log todas as linhas que contêm "bdd" ou "type" para rastrear eventos
      const hasBddKeyword = logString.toLowerCase().includes('bdd');
      const hasTypeKeyword = logString.includes('"type"') || logString.includes('type');
      const hasBddType = logString.includes('bdd-');
      
      if (hasBddKeyword || hasTypeKeyword || hasBddType) {
        console.log(`[BDDSteps] 🔍 Log recebido contém indicadores BDD para processId: ${processId}`);
        console.log(`[BDDSteps] 🔍 hasBddKeyword: ${hasBddKeyword}, hasTypeKeyword: ${hasTypeKeyword}, hasBddType: ${hasBddType}`);
        console.log(`[BDDSteps] 🔍 Conteúdo completo (primeiros 1000 chars):`, logString.substring(0, 1000));
        console.log(`[BDDSteps] 🔍 Todas as linhas:`, lines.filter(l => l.trim()).map(l => l.substring(0, 200)));
      }
      let hasBddEvent = false;
      
      for (const line of lines) {
        const trimmedLine = line.trim();
        if (!trimmedLine) continue;
        
        // Verifica se a linha parece ser um JSON de evento BDD
        // Melhorado: verifica múltiplas condições para detectar eventos BDD
        const startsWithBrace = trimmedLine.startsWith('{');
        const hasTypeProperty = trimmedLine.includes('"type"') || trimmedLine.includes('"type":');
        const hasBddType = trimmedLine.includes('bdd-');
        const looksLikeBddEvent = startsWithBrace && hasTypeProperty && hasBddType;
        
        // Também tenta detectar se é JSON válido mesmo sem começar com {
        let isJsonLike = false;
        if (!looksLikeBddEvent && (hasTypeProperty || hasBddType)) {
          try {
            // Tenta parsear para ver se é JSON válido
            const testParse = JSON.parse(trimmedLine);
            isJsonLike = testParse && typeof testParse === 'object' && testParse.type;
          } catch {
            // Não é JSON válido
          }
        }
        
        if (looksLikeBddEvent || (isJsonLike && hasBddType)) {
          console.log(`[BDDSteps] 🎯 Tentando parsear linha como evento BDD:`, trimmedLine.substring(0, 300));
          console.log(`[BDDSteps] 🎯 Linha completa:`, trimmedLine);
          try {
            const event = JSON.parse(trimmedLine);
            if (event.type && event.type.startsWith('bdd-')) {
              hasBddEvent = true;
              console.log(`[BDDSteps] ✅ Evento BDD recebido e parseado: ${event.type} para processId: ${processId}`);
              console.log(`[BDDSteps] ✅ Detalhes do evento:`, JSON.stringify(event, null, 2));
              
              // É um evento de steps BDD, processa separadamente
              // Usa o processId do evento se disponível, caso contrário usa o do log
              const eventProcessId = event.processId || processId;
              console.log(`[BDDSteps] 🔄 Processando evento ${event.type} para processId: ${eventProcessId} (recebido: ${processId})`);
              
              setBddSteps(prev => {
                const newSteps = new Map(prev);
                const currentSteps = newSteps.get(eventProcessId) || [];
                
                if (event.type === 'bdd-steps-initialized') {
                  // Inicializa steps
                  const steps = (event.steps || []).map(step => ({
                    ...step,
                    status: step.status || 'pending'
                  }));
                  newSteps.set(eventProcessId, steps);
                  console.log(`[BDDSteps] ✅ Inicializados ${steps.length} steps para processId: ${eventProcessId}`);
                  console.log(`[BDDSteps] ✅ Steps detalhados:`, steps.map(s => `${s.keyword} ${s.text}`));
                } else if (event.type === 'bdd-step-progress') {
                  // Atualiza step atual
                  const updatedSteps = [...currentSteps];
                  if (event.stepIndex >= 0 && event.stepIndex < updatedSteps.length) {
                    // Marca o step anterior como completed se existir
                    for (let i = 0; i < event.stepIndex; i++) {
                      if (updatedSteps[i].status === 'running' || updatedSteps[i].status === 'pending') {
                        updatedSteps[i] = { ...updatedSteps[i], status: 'completed' };
                      }
                    }
                    // Atualiza o step atual para running
                    updatedSteps[event.stepIndex] = { ...updatedSteps[event.stepIndex], ...event.step, status: 'running' };
                    newSteps.set(eventProcessId, updatedSteps);
                    console.log(`[BDDSteps] 🔄 Step ${event.stepIndex} em execução: ${event.step?.fullLine || event.step?.text}`);
                  } else {
                    console.warn(`[BDDSteps] ⚠️ Step index ${event.stepIndex} fora do range (0-${updatedSteps.length - 1}) para processId: ${eventProcessId}`);
                    console.warn(`[BDDSteps] ⚠️ Steps disponíveis: ${updatedSteps.length}, tentando usar processId do evento`);
                  }
                } else if (event.type === 'bdd-step-completed') {
                  // Marca step como concluído
                  const updatedSteps = [...currentSteps];
                  if (event.stepIndex >= 0 && event.stepIndex < updatedSteps.length) {
                    updatedSteps[event.stepIndex] = { ...updatedSteps[event.stepIndex], status: 'completed' };
                    newSteps.set(eventProcessId, updatedSteps);
                    console.log(`[BDDSteps] ✅ Step ${event.stepIndex} concluído`);
                  }
                } else if (event.type === 'bdd-step-failed') {
                  // Marca step como falhou e steps subsequentes como skipped
                  const updatedSteps = [...currentSteps];
                  if (event.stepIndex >= 0 && event.stepIndex < updatedSteps.length) {
                    updatedSteps[event.stepIndex] = { ...updatedSteps[event.stepIndex], status: 'failed' };
                    // Marca steps subsequentes como skipped
                    for (let i = event.stepIndex + 1; i < updatedSteps.length; i++) {
                      if (updatedSteps[i].status === 'pending' || updatedSteps[i].status === 'running') {
                        updatedSteps[i] = { ...updatedSteps[i], status: 'skipped' };
                      }
                    }
                    newSteps.set(eventProcessId, updatedSteps);
                    console.log(`[BDDSteps] ❌ Step ${event.stepIndex} falhou`);
                  }
                }
                
                return newSteps;
              });
            }
          } catch (e) {
            // Não é JSON válido, continua
            // Log apenas se realmente parece ser um JSON
            if (trimmedLine.startsWith('{') && trimmedLine.includes('"type"')) {
              console.debug('[BDDSteps] Erro ao parsear evento JSON:', e.message, 'Linha:', trimmedLine.substring(0, 100));
            }
          }
        }
      }
      
      // Se encontrou evento BDD, remove a linha do log normal
      if (hasBddEvent) {
        // Remove linhas que são eventos BDD do log normal
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
        
        // Se sobrou algo após filtrar, adiciona ao log normal
        if (filteredLines.length > 0 && filteredLines.some(l => l.trim())) {
          const remainingLog = filteredLines.join('\n');
          if (remainingLog.trim()) {
            // Continua processamento normal com o log filtrado
            logData = remainingLog;
          } else {
            // Se não sobrou nada, não adiciona ao log
            return;
          }
        } else {
          // Se não sobrou nada, não adiciona ao log
          return;
        }
      }
      
      setLogs(prev => {
        const newLogs = new Map(prev);
        const currentLogs = newLogs.get(processId) || [];
        
        // Verifica duplicação de forma mais robusta
        // Compara os últimos 3 logs para evitar duplicatas mesmo se vierem em chunks diferentes
        const lastFewLogs = currentLogs.slice(-3).join('');
        const currentLogKey = `${processId}:${lastFewLogs}:${logData}`;
        
        if (!recentLogs.has(processId)) {
          recentLogs.set(processId, new Set());
        }
        
        const processRecentLogs = recentLogs.get(processId);
        
        // Se já viu este log recentemente, ignora
        if (processRecentLogs.has(currentLogKey)) {
          return prev; // Retorna o estado anterior sem modificar
        }
        
        // Adiciona à lista de logs recentes (mantém apenas os últimos 10)
        processRecentLogs.add(currentLogKey);
        if (processRecentLogs.size > 10) {
          const firstKey = processRecentLogs.values().next().value;
          processRecentLogs.delete(firstKey);
        }
        
        // Limita o tamanho dos logs em memória
        let updatedLogs = [...currentLogs, logData];
        
        // Limita número de entradas
        if (updatedLogs.length > MAX_LOG_ENTRIES_FRONTEND) {
          // Remove os logs mais antigos, mantendo apenas os últimos
          const removeCount = updatedLogs.length - MAX_LOG_ENTRIES_FRONTEND;
          updatedLogs = updatedLogs.slice(removeCount);
        }
        
        // Limita tamanho total em caracteres
        const totalChars = updatedLogs.join('').length;
        if (totalChars > MAX_LOG_CHARACTERS_PER_PROCESS) {
          // Remove logs antigos até ficar abaixo do limite
          let truncatedLogs = [...updatedLogs];
          while (truncatedLogs.join('').length > MAX_LOG_CHARACTERS_PER_PROCESS && truncatedLogs.length > 1) {
            truncatedLogs.shift(); // Remove o mais antigo
          }
          // Adiciona aviso de truncamento se necessário
          if (truncatedLogs.length < updatedLogs.length) {
            truncatedLogs.unshift('\n[AVISO: Logs antigos foram removidos para economizar memória]\n');
          }
          updatedLogs = truncatedLogs;
        }
        
        newLogs.set(processId, updatedLogs);
        return newLogs;
      });
    };

    // Handler dedicado para eventos BDD (canal separado, mais confiável)
    const handleBddEvent = (data) => {
      console.log(`[BDDSteps] 📡 RECEBIDO evento BDD no frontend!`, data);
      const { processId, event } = data;
      
      if (!event || !event.type) {
        console.error(`[BDDSteps] ❌ Evento BDD inválido recebido:`, data);
        return;
      }
      
      console.log(`[BDDSteps] 📡 Evento BDD recebido via canal dedicado: ${event.type} para processId: ${processId}`);
      console.log(`[BDDSteps] 📡 Detalhes do evento:`, JSON.stringify(event, null, 2));
      
      setBddSteps(prev => {
        const newSteps = new Map(prev);
        const currentSteps = newSteps.get(processId) || [];
        
        if (event.type === 'bdd-steps-initialized') {
          // Inicializa steps
          const steps = (event.steps || []).map(step => ({
            ...step,
            status: step.status || 'pending'
          }));
          newSteps.set(processId, steps);
          console.log(`[BDDSteps] ✅ Inicializados ${steps.length} steps para processId: ${processId}`);
          console.log(`[BDDSteps] ✅ Steps detalhados:`, steps.map(s => `${s.keyword} ${s.text}`));
        } else if (event.type === 'bdd-step-progress') {
          // Atualiza step atual - apenas marca o step atual como running
          const updatedSteps = [...currentSteps];
          if (event.stepIndex >= 0 && event.stepIndex < updatedSteps.length) {
            // Primeiro, marca qualquer step anterior que esteja "running" como "completed"
            // Isso garante que apenas um step esteja "running" por vez
            for (let i = 0; i < updatedSteps.length; i++) {
              if (i < event.stepIndex && updatedSteps[i].status === 'running') {
                // Step anterior que estava running agora deve ser completed
                updatedSteps[i] = { ...updatedSteps[i], status: 'completed' };
                console.log(`[BDDSteps] 🔄 Step anterior ${i} marcado como completed (novo step ${event.stepIndex} iniciou)`);
              } else if (i === event.stepIndex) {
                // Atualiza o step atual para running
                updatedSteps[i] = { ...updatedSteps[i], ...event.step, status: 'running' };
                console.log(`[BDDSteps] 🔄 Step ${event.stepIndex} em execução: ${event.step?.fullLine || event.step?.text}`);
              } else if (i > event.stepIndex && updatedSteps[i].status === 'running') {
                // Se algum step futuro está running (não deveria acontecer), marca como pending
                updatedSteps[i] = { ...updatedSteps[i], status: 'pending' };
              }
            }
            newSteps.set(processId, updatedSteps);
          } else {
            console.warn(`[BDDSteps] ⚠️ Step index ${event.stepIndex} fora do range (0-${updatedSteps.length - 1}) para processId: ${processId}`);
            console.warn(`[BDDSteps] ⚠️ Steps disponíveis: ${updatedSteps.length}`);
          }
        } else if (event.type === 'bdd-step-completed') {
          // Marca step como concluído - apenas o step específico
          const updatedSteps = [...currentSteps];
          if (event.stepIndex >= 0 && event.stepIndex < updatedSteps.length) {
            // Se o step estava running, marca como completed
            if (updatedSteps[event.stepIndex].status === 'running') {
              updatedSteps[event.stepIndex] = { ...updatedSteps[event.stepIndex], status: 'completed' };
              newSteps.set(processId, updatedSteps);
              console.log(`[BDDSteps] ✅ Step ${event.stepIndex} concluído: ${updatedSteps[event.stepIndex].fullLine || updatedSteps[event.stepIndex].text}`);
            } else {
              console.log(`[BDDSteps] ℹ️ Step ${event.stepIndex} já estava ${updatedSteps[event.stepIndex].status}, mantendo status`);
            }
          }
        } else if (event.type === 'bdd-step-failed') {
          // Marca step como falhou e steps subsequentes como skipped
          const updatedSteps = [...currentSteps];
          if (event.stepIndex >= 0 && event.stepIndex < updatedSteps.length) {
            updatedSteps[event.stepIndex] = { ...updatedSteps[event.stepIndex], status: 'failed' };
            // Marca steps subsequentes como skipped
            for (let i = event.stepIndex + 1; i < updatedSteps.length; i++) {
              if (updatedSteps[i].status === 'pending' || updatedSteps[i].status === 'running') {
                updatedSteps[i] = { ...updatedSteps[i], status: 'skipped' };
              }
            }
            newSteps.set(processId, updatedSteps);
            console.log(`[BDDSteps] ❌ Step ${event.stepIndex} falhou`);
          }
        }
        
        return newSteps;
      });
    };

    // Handler para conclusão de execução
    const handleAutomationComplete = async (data) => {
      const { processId, result } = data;
      
      // Verifica se este processId já foi processado para evitar duplicação
      if (processedProcessIds.has(processId)) {
        console.log(`[handleAutomationComplete] ProcessId ${processId} já foi processado, ignorando duplicata`);
        return;
      }
      
      // Marca como processado
      processedProcessIds.add(processId);
      
      // Limpa processIds antigos após 1 minuto para evitar acúmulo de memória
      setTimeout(() => {
        processedProcessIds.delete(processId);
      }, 60000);
      
      // Primeiro, recarrega a lista de automações para garantir sincronização com o backend
      // Isso atualiza automaticamente a fila quando um item termina e o próximo começa
      await loadRunningAutomations();
      
      // Após recarregar, verifica se o item falhou para mostrar feedback visual
      // (mesmo que já tenha sido removido da fila, podemos mostrar feedback se necessário)
      if (!result.success) {
        // Para itens que falharam, podemos mostrar um breve feedback
        // Mas não mantemos na lista se já foi removido do backend
      }
      
      // Limpa os logs deste processo após um delay para economizar memória
      setTimeout(() => {
        setLogs(prevLogs => {
          const newLogs = new Map(prevLogs);
          newLogs.delete(processId);
          return newLogs;
        });
      }, 10000); // Remove logs após 10 segundos
      
      // Adiciona resultado final aos logs
      setLogs(prev => {
        const newLogs = new Map(prev);
        const currentLogs = newLogs.get(processId) || [];
        
        // Determina a mensagem de status baseada no resultado
        let statusMessage = '';
        let failureDetails = '';
        
        if (result.success) {
          statusMessage = '✅ Sucesso';
        } else {
          if (result.patternMismatchReason) {
            // Usa o motivo simplificado como título
            statusMessage = `❌ ${result.patternMismatchReason}`;
            // Usa os detalhes completos se disponíveis (já incluem explicações e sugestões)
            if (result.patternMismatchDetails) {
              failureDetails = `\n\n${result.patternMismatchDetails}`;
            } else {
              // Fallback caso não tenha detalhes
              failureDetails = `\n\n${result.patternMismatchReason}`;
              if (result.filterUsed) {
                failureDetails += `\nFiltro usado: "${result.filterUsed}"`;
              }
              if (result.filesMatched !== null) {
                failureDetails += `\nArquivos encontrados: ${result.filesMatched}`;
              }
              if (result.testsDiscovered !== null) {
                failureDetails += `\nTestes descobertos: ${result.testsDiscovered}`;
              }
              if (result.testsRun !== null) {
                failureDetails += `\nTestes executados: ${result.testsRun}`;
              }
            }
          } else if (result.exitCode !== 0) {
            statusMessage = `❌ Falha na Execução (Exit Code: ${result.exitCode})`;
            // Se houver categoria e descrição, adiciona
            if (result.categoryDescription) {
              failureDetails = `\n\n${result.categoryDescription}`;
            }
          } else if (result.hasCriticalErrors) {
            statusMessage = '❌ Erro Crítico do Sistema';
            // Se houver categoria e descrição, adiciona
            if (result.categoryDescription) {
              failureDetails = `\n\n${result.categoryDescription}`;
            }
          } else if (!result.hasTestExecution) {
            statusMessage = '❌ Nenhum Teste Executado';
            // Se houver categoria e descrição, adiciona
            if (result.categoryDescription) {
              failureDetails = `\n\n${result.categoryDescription}`;
            }
          } else {
            statusMessage = '❌ Falha';
            // Se houver categoria e descrição, adiciona
            if (result.categoryDescription) {
              failureDetails = `\n\n${result.categoryDescription}`;
            }
          }
        }
        
        // Adiciona informações do arquivo executado no log
        const fileInfo = result.featurePath 
          ? `\n📄 Arquivo executado: ${result.featurePath}`
          : result.featureName 
            ? `\n📄 Feature: ${result.featureName}`
            : '';
        
        // Se houver cenários individuais, mostra cada um separadamente
        let scenariosInfo = '';
        if (result.scenarios && Array.isArray(result.scenarios) && result.scenarios.length > 0) {
          const passedScenarios = result.scenarios.filter(s => s.status === 'passed');
          const failedScenarios = result.scenarios.filter(s => s.status === 'failed');
          
          scenariosInfo = `\n\n📊 RESULTADO DOS CENÁRIOS:\n${'='.repeat(60)}\n`;
          
          if (passedScenarios.length > 0) {
            scenariosInfo += `\n✅ CENÁRIOS APROVADOS (${passedScenarios.length}):\n`;
            passedScenarios.forEach((scenario, index) => {
              scenariosInfo += `  ${index + 1}. ${scenario.name}\n`;
            });
          }
          
          if (failedScenarios.length > 0) {
            scenariosInfo += `\n❌ CENÁRIOS FALHARAM (${failedScenarios.length}):\n`;
            failedScenarios.forEach((scenario, index) => {
              scenariosInfo += `  ${index + 1}. ${scenario.name}\n`;
            });
          } else if (result.success && passedScenarios.length > 0) {
            scenariosInfo += `\n✅ Todos os ${result.scenarios.length} cenário(s) foram aprovados!\n`;
          }
        } else if (result.scenarioName) {
          // Fallback: se não há cenários individuais mas há nome de cenário, mostra
          scenariosInfo = `\n🎯 Cenário executado: ${result.scenarioName}\n`;
        }
        
        const resultLog = `\n${'='.repeat(60)}\n=== Execução Finalizada ===${fileInfo}${scenariosInfo}\nStatus: ${statusMessage}${failureDetails}\nDuração: ${formatDuration(result.duration)}\nExit Code: ${result.exitCode}\n${'='.repeat(60)}\n`;
        
        // Limita o tamanho dos logs antes de adicionar o resultado
        let finalLogs = [...currentLogs];
        const MAX_RESULT_LOG_CHARS = 50000; // Limite para logs finais
        
        // Remove logs antigos se necessário para manter o limite
        const currentTotalChars = finalLogs.join('').length;
        if (currentTotalChars + resultLog.length > MAX_RESULT_LOG_CHARS) {
          // Mantém apenas os últimos logs que cabem no limite
          let truncatedLogs = [];
          let totalChars = resultLog.length;
          for (let i = finalLogs.length - 1; i >= 0; i--) {
            const logChars = finalLogs[i].length;
            if (totalChars + logChars <= MAX_RESULT_LOG_CHARS) {
              truncatedLogs.unshift(finalLogs[i]);
              totalChars += logChars;
            } else {
              break;
            }
          }
          if (truncatedLogs.length < finalLogs.length) {
            truncatedLogs.unshift('\n[AVISO: Logs antigos foram removidos para economizar memória]\n');
          }
          finalLogs = truncatedLogs;
        }
        
        finalLogs.push(resultLog);
        newLogs.set(processId, finalLogs);
        
        // Normaliza logs para histórico (converte para array de strings)
        const normalizedLogsForHistory = Array.isArray(finalLogs) 
          ? finalLogs 
          : (typeof finalLogs === 'string' ? [finalLogs] : []);
        
        // Salva no histórico do localStorage (evita duplicatas)
        // OTIMIZAÇÃO: Mantém apenas os últimos registros para economizar memória
        const executionRecord = {
          processId,
          featureName: result.featureName,
          featurePath: result.featurePath || null, // Salva o caminho do arquivo
          scenarioName: result.scenarioName || null, // Salva o nome do cenário
          success: result.success,
          duration: result.duration,
          startTime: result.startTime,
          endTime: result.endTime || new Date(),
          logs: normalizedLogsForHistory,
          exitCode: result.exitCode,
          category: result.category,
          categoryDescription: result.categoryDescription,
          groupKey: result.groupKey,
          // Salva cenários individuais com informações detalhadas
          scenarios: result.scenarios || [],
          // Adiciona resumo para não perder informações importantes
          summary: {
            status: statusMessage,
            failureReason: failureDetails,
            category: result.category,
            categoryDescription: result.categoryDescription
          }
        };
        
        // Carrega histórico existente
        const savedHistory = localStorage.getItem('executionHistory');
        let history = savedHistory ? JSON.parse(savedHistory) : [];
        
        // Remove qualquer registro duplicado com o mesmo processId (garante que não há duplicatas)
        const processIdMap = new Map();
        
        // Primeiro, adiciona todos os registros existentes ao Map (último vence)
        history.forEach(record => {
          if (record.processId) {
            processIdMap.set(record.processId, record);
          }
        });
        
        // Adiciona o novo registro (sobrescreve se já existir)
        processIdMap.set(processId, executionRecord);
        
        // Converte Map para array e ordena por data (mais recente primeiro)
        const filteredHistory = Array.from(processIdMap.values())
          .sort((a, b) => new Date(b.startTime || 0) - new Date(a.startTime || 0));
        
        // Mantém apenas os últimos 50 registros (reduzido de 100 para economizar memória)
        const limitedHistory = filteredHistory.slice(0, 50);
        
        // Salva apenas se houve mudanças (evita escrita desnecessária)
        if (limitedHistory.length !== history.length || 
            !history.some(r => r.processId === processId && 
              r.endTime === executionRecord.endTime)) {
          try {
            localStorage.setItem('executionHistory', JSON.stringify(limitedHistory));
          } catch (e) {
            // Se o localStorage estiver cheio, remove os registros mais antigos
            console.warn('localStorage cheio, removendo registros antigos...');
            const reducedHistory = limitedHistory.slice(0, 25);
            try {
              localStorage.setItem('executionHistory', JSON.stringify(reducedHistory));
            } catch (e2) {
              console.error('Erro ao salvar histórico:', e2);
            }
          }
        }
        
        setRecentAutomations(prev => {
          const filtered = prev.filter(item => item.processId !== processId);
          const executionKey = `${result.featurePath || result.featureName || 'unknown'}::${result.scenarioName || 'feature'}`;
          const occurrences = filtered.filter(item => item.executionKey === executionKey && Math.abs(new Date(item.startTime) - new Date(result.startTime)) < 5 * 60 * 1000).length;
          filtered.unshift({
            processId,
            featureName: result.featureName,
            featurePath: result.featurePath || null,
            scenarioName: result.scenarioName || null,
            success: result.success,
            exitCode: result.exitCode,
            startTime: result.startTime,
            endTime: result.endTime || new Date(),
            duration: result.duration,
            category: result.category,
            categoryDescription: result.categoryDescription,
            executionKey,
            repeatCount: occurrences + 1
          });
          return filtered.slice(0, 20);
        });
        
        // Armazena o resultado da execução atual para exibição de steps BDD
        setCurrentExecutionResult({
          processId,
          featureName: result.featureName,
          featurePath: result.featurePath,
          scenarioName: result.scenarioName,
          success: result.success,
          scenarios: result.scenarios || [],
          category: result.category,
          categoryDescription: result.categoryDescription
        });
        
        return newLogs;
      });
    };

    // Registra os novos listeners
    automationService.onAutomationLog(handleAutomationLog);
    automationService.onBddEvent(handleBddEvent);
    automationService.onAutomationComplete(handleAutomationComplete);

    // Cleanup: remove listeners quando o componente é desmontado ou o effect é re-executado
    return () => {
      automationService.removeAutomationLogListener();
      automationService.removeBddEventListener();
      automationService.removeAutomationCompleteListener();
      // Limpa todos os timeouts pendentes
      timeoutRefs.current.forEach((timeouts) => {
        timeouts.forEach(timeout => clearTimeout(timeout));
      });
      timeoutRefs.current.clear();
    };
  }, [projectPath]);

  // Auto-scroll removido - o usuário pode rolar manualmente

  const loadFeatures = async () => {
    const result = await automationService.scanFeatures(projectPath);
    if (result.success) {
      setFeatures(result.data);
    }
  };

  const loadRunningAutomations = async () => {
    try {
      const result = await automationService.getRunningAutomations();
      if (result.success) {
        const runningMap = new Map();
        // Atualiza estrutura para suportar nova resposta com running e queued
        if (result.data.running) {
          result.data.running.forEach(auto => {
            runningMap.set(auto.id, {
              id: auto.id,
              featureName: auto.featureName,
              featurePath: auto.featurePath,
              scenarioName: auto.scenarioName,
              startTime: new Date(auto.startTime),
              duration: auto.duration
            });
          });
        }
        
        // Adiciona também itens na fila como "queued"
        if (result.data.queued && Array.isArray(result.data.queued)) {
          result.data.queued.forEach(auto => {
            runningMap.set(auto.id, {
              id: auto.id,
              featureName: auto.featureName,
              featurePath: auto.featurePath,
              scenarioName: auto.scenarioName,
              startTime: new Date(),
              duration: 0,
              queued: true,
              queuePosition: auto.queuePosition
            });
          });
        }
        
        // Se não há estrutura running/queued, tenta fallback para array
        if (!result.data.running && Array.isArray(result.data)) {
          result.data.forEach(auto => {
            runningMap.set(auto.id, {
              id: auto.id,
              featureName: auto.featureName,
              featurePath: auto.featurePath,
              scenarioName: auto.scenarioName,
              startTime: new Date(auto.startTime),
              duration: auto.duration
            });
          });
        }
        
        // Atualiza o estado com os dados do backend (isso sobrescreve qualquer estado local)
        setRunningAutomations(runningMap);
      }
    } catch (error) {
      console.error('[loadRunningAutomations] Erro ao carregar automações:', error);
    }
  };

  const handleToggleFeature = (feature) => {
    setSelectedFeatures(prev => {
      const exists = prev.find(f => f.path === feature.path);
      if (exists) {
        // Remove apenas a feature, mas mantém os cenários selecionados
        // Isso permite que os cenários continuem selecionados e executáveis
        return prev.filter(f => f.path !== feature.path);
      } else {
        return [...prev, feature];
      }
    });
  };

  const handleToggleFeatureExpand = (featurePath) => {
    setExpandedFeatures(prev => {
      const newExpanded = new Set(prev);
      if (newExpanded.has(featurePath)) {
        newExpanded.delete(featurePath);
      } else {
        newExpanded.add(featurePath);
      }
      return newExpanded;
    });
  };

  const handleToggleScenario = (featurePath, scenarioName) => {
    setSelectedScenarios(prev => {
      const newScenarios = new Map(prev);
      const existingScenarios = newScenarios.get(featurePath);
      
      // Cria uma nova cópia do Set para garantir que o React detecte a mudança
      const featureScenarios = existingScenarios ? new Set(existingScenarios) : new Set();
      
      if (featureScenarios.has(scenarioName)) {
        // Remove o cenário
        featureScenarios.delete(scenarioName);
        if (featureScenarios.size === 0) {
          // Se não há mais cenários, remove a entrada do Map
          newScenarios.delete(featurePath);
        } else {
          // Atualiza com o novo Set
          newScenarios.set(featurePath, featureScenarios);
        }
      } else {
        // Adiciona o cenário
        featureScenarios.add(scenarioName);
        newScenarios.set(featurePath, featureScenarios);
      }
      
      return newScenarios;
    });
  };

  const handleRunSelected = async () => {
    // CRÍTICO: Verifica se está cancelando ANTES de permitir nova execução
    // Se estiver cancelando, bloqueia completamente novas execuções
    if (isCancellingAll) {
      console.warn('[handleRunSelected] Execuções estão sendo canceladas - bloqueando nova execução');
      return;
    }
    
    // Previne múltiplas execuções simultâneas
    if (isRunningSelected) {
      console.warn('Execução já em andamento, aguarde...');
      return;
    }

    setIsRunningSelected(true);

    try {
      const executions = [];
      const executionKeysSet = new Set(); // Para evitar duplicatas dentro da própria lista
      
      // Cria um Map de features por path para busca rápida
      const featuresMap = new Map(features.map(f => [f.path, f]));
      
      // Processa features selecionadas
      const uniqueFeatures = selectedFeatures.filter((feature, index, self) =>
        index === self.findIndex(f => f.path === feature.path)
      );
      
      for (const feature of uniqueFeatures) {
        const selectedScenariosForFeature = selectedScenarios.get(feature.path);
        
        // Se há cenários específicos selecionados, executa cada um
        if (selectedScenariosForFeature && selectedScenariosForFeature.size > 0) {
          for (const scenarioName of selectedScenariosForFeature) {
            const executionKey = `${feature.path}:${scenarioName}`;
            // Evita duplicatas dentro da própria lista
            if (!executionKeysSet.has(executionKey)) {
              executionKeysSet.add(executionKey);
              executions.push({ feature, scenarioName });
            }
          }
        } else {
          // Se não há cenários selecionados, executa a feature inteira
          const executionKey = feature.path;
          if (!executionKeysSet.has(executionKey)) {
            executionKeysSet.add(executionKey);
            executions.push({ feature, scenarioName: null });
          }
        }
      }
      
      // Processa cenários selecionados de features NÃO selecionadas
      // Isso permite executar cenários individuais sem selecionar a feature pai
      for (const [featurePath, selectedScenariosSet] of selectedScenarios.entries()) {
        // Verifica se a feature não está selecionada (mas tem cenários selecionados)
        const isFeatureSelected = selectedFeatures.some(f => f.path === featurePath);
        if (!isFeatureSelected && selectedScenariosSet && selectedScenariosSet.size > 0) {
          // Busca a feature pelo path
          const feature = featuresMap.get(featurePath);
          if (feature) {
            // Executa cada cenário selecionado
            for (const scenarioName of selectedScenariosSet) {
              const executionKey = `${feature.path}:${scenarioName}`;
              // Evita duplicatas dentro da própria lista
              if (!executionKeysSet.has(executionKey)) {
                executionKeysSet.add(executionKey);
                executions.push({ feature, scenarioName });
              }
            }
          }
        }
      }
      
      // Filtra execuções que já estão rodando ou na fila
      const executionsToRun = executions.filter(({ feature, scenarioName }) => {
        const executionKey = scenarioName ? `${feature.path}:${scenarioName}` : feature.path;
        
        // Verifica se já está rodando
        const isRunning = Array.from(runningAutomations.values())
          .some(a => {
            const runningKey = a.scenarioName ? `${a.featurePath}:${a.scenarioName}` : a.featurePath;
            return runningKey === executionKey;
          });
        
        return !isRunning;
      });
      
      if (executionsToRun.length === 0) {
        console.warn('Nenhuma execução selecionada ou todas já estão em execução');
        return;
      }
      
      // Aviso de consumo de RAM para mais de 20 execuções
      if (executionsToRun.length > 20) {
        // Armazena as execuções pendentes e exibe o modal
        setPendingExecutions(executionsToRun);
        setShowRamWarningModal(true);
        setIsRunningSelected(false);
        return;
      }
      
      // Executa as automações
      await executeAutomations(executionsToRun);
    } finally {
      setIsRunningSelected(false);
    }
  };

  const runAutomation = async (feature, scenarioName = null) => {
    // Verifica se foi cancelado antes de iniciar
    if (isCancellingAll || abortControllerRef.current?.signal.aborted) {
      throw new Error('Execução cancelada pelo usuário');
    }
    
    // Verifica se já está executando para evitar duplicação
    const executionKey = scenarioName ? `${feature.path}:${scenarioName}` : feature.path;
    const isAlreadyRunning = Array.from(runningAutomations.values())
      .some(a => {
        const runningKey = a.scenarioName ? `${a.featurePath}:${a.scenarioName}` : a.featurePath;
        return runningKey === executionKey;
      });
    
    if (isAlreadyRunning) {
      const displayName = scenarioName ? `${feature.name} - ${scenarioName}` : feature.name;
      console.warn(`[runAutomation] ${displayName} já está em execução, ignorando...`);
      return;
    }

    try {
      const result = await automationService.runAutomation({
        projectPath,
        featureName: feature.name,
        featurePath: feature.path,
        scenarioName: scenarioName, // Passa o nome do cenário se especificado
        tags: feature.tags
      });

      // CRÍTICO: Verifica novamente se foi cancelado APÓS a chamada assíncrona retornar
      // Isso previne condição de corrida onde o cancelamento acontece durante a chamada
      if (isCancellingAll || abortControllerRef.current?.signal.aborted) {
        console.log(`[runAutomation] Execução cancelada durante a inicialização - parando processo ${result.processId || 'desconhecido'}`);
        // Se o processo foi iniciado no backend, para ele imediatamente
        if (result.success && result.processId) {
          try {
            await automationService.stopAutomation(result.processId);
          } catch (stopError) {
            console.warn(`[runAutomation] Erro ao parar processo cancelado: ${stopError.message}`);
          }
        }
        throw new Error('Execução cancelada pelo usuário');
      }

      if (result.success) {
        const processId = result.processId;
        
        // Verifica novamente antes de adicionar ao estado (dupla verificação)
        if (isCancellingAll || abortControllerRef.current?.signal.aborted) {
          console.log(`[runAutomation] Execução cancelada antes de adicionar ao estado - parando processo ${processId}`);
          try {
            await automationService.stopAutomation(processId);
          } catch (stopError) {
            console.warn(`[runAutomation] Erro ao parar processo cancelado: ${stopError.message}`);
          }
          throw new Error('Execução cancelada pelo usuário');
        }
        
        setRunningAutomations(prev => {
          const newMap = new Map(prev);
          newMap.set(processId, {
            id: processId,
            featureName: feature.name,
            featurePath: feature.path,
            scenarioName: scenarioName || null, // Armazena o cenário para verificação (null se não especificado)
            startTime: new Date()
          });
          return newMap;
        });
        setLogs(prev => {
          const newLogs = new Map(prev);
          const headerLog = `\n${'='.repeat(60)}\n=== Iniciando Execução ===\nFeature: ${feature.name}${scenarioName ? `\nCenário: ${scenarioName}` : ''}\nProcess ID: ${processId}\n${'='.repeat(60)}\n`;
          newLogs.set(processId, [headerLog]);
          return newLogs;
        });
        console.log(`[runAutomation] Execução iniciada: ${feature.name}${scenarioName ? ` - ${scenarioName}` : ''} (processId: ${processId})`);
      } else {
        // Verifica se é erro de cancelamento - não loga no console
        const errorMessage = result.error || '';
        if (errorMessage.includes('cancelando todas as execuções') || 
            errorMessage.includes('cancelada pelo usuário') ||
            errorMessage.includes('Execução cancelada')) {
          // Silenciosamente ignora o erro de cancelamento
          return;
        }
        console.error(`[runAutomation] Falha ao iniciar execução: ${errorMessage || 'Erro desconhecido'}`);
        throw new Error(errorMessage || 'Falha ao iniciar execução');
      }
    } catch (error) {
      // Verifica se é erro de cancelamento - não loga no console
      const errorMessage = error.message || '';
      if (errorMessage.includes('cancelando todas as execuções') || 
          errorMessage.includes('cancelada pelo usuário') ||
          errorMessage.includes('Execução cancelada') ||
          errorMessage.includes('cancelada')) {
        // Silenciosamente ignora o erro de cancelamento
        return;
      }
      console.error(`[runAutomation] Erro ao executar automação:`, error);
      throw error; // Re-lança para o chamador tratar apenas se não for erro de cancelamento
    }
  };

  const stopAutomation = async (processId) => {
    await automationService.stopAutomation(processId);
    setRunningAutomations(prev => {
      const newMap = new Map(prev);
      newMap.delete(processId);
      return newMap;
    });
  };

  const executeAutomations = async (executionsToRun) => {
    // CRÍTICO: Verifica se está cancelando ANTES de iniciar qualquer execução
    if (isCancellingAll) {
      console.warn('[executeAutomations] Execuções estão sendo canceladas - abortando antes de iniciar');
      return;
    }
    
    console.log(`[handleRunSelected] Iniciando ${executionsToRun.length} execução(ões) - Modo: ${executeInQueue ? 'Fila' : 'Paralelo'}`);
    
    // Cria um novo AbortController para esta execução
    abortControllerRef.current = new AbortController();
    const signal = abortControllerRef.current.signal;
    
    // Configura o modo de execução
    if (executeInQueue) {
      // Modo fila: executa sequencialmente (maxParallel = 1)
      await automationService.setMaxParallel(1);
      
      // CRÍTICO: Adiciona TODOS os testes à fila de uma vez, sem await
      // Isso garante que a fila seja populada completamente antes de iniciar qualquer execução
      // O backend processará a fila automaticamente de forma sequencial
      console.log(`[executeAutomations] Adicionando ${executionsToRun.length} execução(ões) à fila...`);
      
      const queuePromises = [];
      
      for (const { feature, scenarioName } of executionsToRun) {
        // Verifica se foi cancelado antes de adicionar cada execução
        if (signal.aborted || isCancellingAll) {
          console.log(`[executeAutomations] Execução cancelada - parando adição à fila`);
          break;
        }
        
        try {
          // Chama runAutomation SEM await - isso adiciona à fila se necessário
          // O primeiro teste executará imediatamente se houver slot disponível
          // Os demais serão adicionados à fila e processados sequencialmente
          const promise = runAutomation(feature, scenarioName);
          queuePromises.push(promise);
          
          // Pequena pausa entre chamadas para evitar condições de corrida
          // Mas não aguarda a execução completa
          await new Promise(resolve => setTimeout(resolve, 100));
        } catch (error) {
          // Verifica se é erro de cancelamento - não loga no console
          const errorMessage = error.message || '';
          if (errorMessage.includes('cancelando todas as execuções') || 
              errorMessage.includes('cancelada pelo usuário') ||
              errorMessage.includes('Execução cancelada') ||
              errorMessage.includes('cancelada')) {
            // Silenciosamente ignora o erro de cancelamento
            break; // Para o loop quando cancelado
          }
          console.error(`[executeAutomations] Erro ao adicionar ${feature.name}${scenarioName ? ` - ${scenarioName}` : ''} à fila:`, error);
          // Continua com a próxima execução mesmo se uma falhar (apenas se não foi cancelado)
          if (signal.aborted || isCancellingAll) {
            break;
          }
        }
      }
      
      // Aguarda todas as execuções serem iniciadas (adicionadas à fila ou iniciadas)
      // Mas não aguarda a conclusão de cada uma - isso será feito pelo backend sequencialmente
      console.log(`[executeAutomations] ${queuePromises.length} execução(ões) iniciadas. A fila será processada automaticamente.`);
      
      // Não aguarda todas as promessas aqui - deixe o backend processar a fila
      // Apenas inicia todas de uma vez para que a fila seja populada
      Promise.all(queuePromises).catch(error => {
        // Apenas loga erros não relacionados a cancelamento
        if (!error.message || (!error.message.includes('cancelada') && !error.message.includes('cancelando'))) {
          console.error('[executeAutomations] Erro ao processar fila:', error);
        }
      });
    } else {
      // Modo paralelo: executa tudo de uma vez (maxParallel = número de execuções, limitado a 10)
      const maxParallel = Math.min(executionsToRun.length, 10);
      await automationService.setMaxParallel(maxParallel);
      
      // Executa todas em paralelo
      const promises = executionsToRun.map(({ feature, scenarioName }) => {
        // Verifica se foi cancelado antes de iniciar
        if (signal.aborted || isCancellingAll) {
          return Promise.resolve(null);
        }
        
        return runAutomation(feature, scenarioName).catch(error => {
          // Verifica se é erro de cancelamento - não loga no console
          const errorMessage = error.message || '';
          if (errorMessage.includes('cancelando todas as execuções') || 
              errorMessage.includes('cancelada pelo usuário') ||
              errorMessage.includes('Execução cancelada') ||
              errorMessage.includes('cancelada')) {
            // Silenciosamente ignora o erro de cancelamento
            return null;
          }
          console.error(`[handleRunSelected] Erro ao executar ${feature.name}${scenarioName ? ` - ${scenarioName}` : ''}:`, error);
          return null; // Retorna null em caso de erro para não quebrar Promise.all
        });
      });
      
      await Promise.all(promises);
    }
  };

  const handleRamWarningConfirm = async () => {
    setShowRamWarningModal(false);
    if (pendingExecutions) {
      // CRÍTICO: Verifica se está cancelando ANTES de permitir nova execução
      // Se estiver cancelando, bloqueia completamente novas execuções
      if (isCancellingAll) {
        console.warn('[handleRamWarningConfirm] Execuções estão sendo canceladas - bloqueando nova execução');
        setPendingExecutions(null);
        return;
      }
      
      const executions = pendingExecutions;
      setPendingExecutions(null);
      setIsRunningSelected(true);
      try {
        await executeAutomations(executions);
      } finally {
        setIsRunningSelected(false);
      }
    }
  };

  const handleRamWarningCancel = () => {
    setShowRamWarningModal(false);
    setPendingExecutions(null);
    setIsRunningSelected(false);
  };

  const stopAllAutomations = async () => {
    setShowStopAllModal(true);
  };

  const handleStopAllConfirm = async () => {
    setShowStopAllModal(false);
    
    // Mostra animação de cancelamento
    setIsStoppingAll(true);
    
    // Define flag de cancelamento no frontend para parar execuções pendentes
    setIsCancellingAll(true);
    
    // Cancela todas as execuções pendentes usando AbortController
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }
    abortControllerRef.current = new AbortController();
    
    // Limpa todos os timeouts pendentes primeiro
    timeoutRefs.current.forEach((timeouts) => {
      timeouts.forEach(timeout => clearTimeout(timeout));
    });
    timeoutRefs.current.clear();
    
    try {
      // PRIMEIRO: Para todas as automações no backend
      await automationService.stopAllAutomations();
      
      // SEGUNDO: Limpa o estado após parar (para feedback visual)
      setRunningAutomations(new Map());
      // Limpa logs para liberar memória imediatamente
      setLogs(new Map());
      setFailedItems(new Set());
      setFadingOutItems(new Set());
      
      // Força limpeza do histórico de processIds processados
      processedProcessIdsRef.current.clear();
      
      // Recarrega a lista após um delay maior para garantir que o backend processou completamente
      setTimeout(() => {
        loadRunningAutomations();
        // Remove a animação após 1 segundo
        setTimeout(() => {
          setIsStoppingAll(false);
          // Aguarda mais um pouco antes de resetar a flag para garantir que tudo foi finalizado
          setTimeout(() => {
            // Reseta a flag de cancelamento após o cancelamento ser concluído
            // Isso permite que o usuário execute novas automações
            setIsCancellingAll(false);
            console.log('[handleStopAllConfirm] Cancelamento concluído - flag de cancelamento resetada');
          }, 500);
        }, 1000);
      }, 1500);
    } catch (error) {
      console.error('Erro ao parar todas as automações:', error);
      // Mesmo com erro, limpa o estado e recarrega para garantir sincronização
      setRunningAutomations(new Map());
      setLogs(new Map());
      setFailedItems(new Set());
      setFadingOutItems(new Set());
      setTimeout(() => {
        loadRunningAutomations();
        // Remove a animação após 1 segundo
        setTimeout(() => {
          setIsStoppingAll(false);
          // Aguarda mais um pouco antes de resetar a flag para garantir que tudo foi finalizado
          setTimeout(() => {
            // Reseta a flag de cancelamento mesmo em caso de erro
            // Isso permite que o usuário execute novas automações
            setIsCancellingAll(false);
            console.log('[handleStopAllConfirm] Cancelamento concluído (com erro) - flag de cancelamento resetada');
          }, 500);
        }, 1000);
      }, 1500);
    }
  };

  const handleStopAllCancel = () => {
    setShowStopAllModal(false);
  };

  const formatDuration = (ms) => {
    const seconds = Math.floor(ms / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    
    if (hours > 0) return `${hours}h ${minutes % 60}m ${seconds % 60}s`;
    if (minutes > 0) return `${minutes}m ${seconds % 60}s`;
    return `${seconds}s`;
  };

  const formatDateTime = (date) => {
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

  // Extrai todas as tags únicas das features
  const allTags = React.useMemo(() => {
    const tagsSet = new Set();
    features.forEach(f => {
      f.tags?.forEach(tag => tagsSet.add(tag));
    });
    return Array.from(tagsSet).sort();
  }, [features]);

  // Filtra features por nome, cenários e tags
  const filteredFeatures = React.useMemo(() => {
    if (!filter && selectedTags.length === 0) {
      return features;
    }

    const filterLower = filter ? filter.toLowerCase() : '';
    
    return features.filter(f => {
      // Filtro por nome da feature
      const nameMatch = !filter || f.name.toLowerCase().includes(filterLower);
      
      // Filtro por cenários - verifica se algum cenário corresponde ao filtro
      let scenarioMatch = false;
      if (filter && f.scenarios && f.scenarios.length > 0) {
        scenarioMatch = f.scenarios.some(scenario => 
          scenario.name.toLowerCase().includes(filterLower)
        );
      }
      
      // Filtro por tags
      const tagMatch = selectedTags.length === 0 || 
        selectedTags.some(tag => f.tags?.includes(tag));
      
      // Inclui a feature se: (nome OU cenário) corresponde ao filtro E tags correspondem
      return (nameMatch || scenarioMatch) && tagMatch;
    });
  }, [features, filter, selectedTags]);

  // Detecta quais cenários correspondem ao filtro e expande automaticamente as features
  React.useEffect(() => {
    if (!filter) {
      return;
    }

    const filterLower = filter.toLowerCase();
    const featuresToExpand = new Set();

    filteredFeatures.forEach(feature => {
      if (feature.scenarios && feature.scenarios.length > 0) {
        // Verifica se algum cenário corresponde ao filtro
        const hasMatchingScenario = feature.scenarios.some(scenario =>
          scenario.name.toLowerCase().includes(filterLower)
        );
        
        // Se a feature não corresponde ao filtro mas tem cenários que correspondem
        const featureNameMatch = feature.name.toLowerCase().includes(filterLower);
        if (hasMatchingScenario && !featureNameMatch) {
          featuresToExpand.add(feature.path);
        } else if (hasMatchingScenario) {
          // Também expande se a feature corresponde E tem cenários correspondentes
          featuresToExpand.add(feature.path);
        }
      }
    });

    // Expande automaticamente as features com cenários filtrados
    if (featuresToExpand.size > 0) {
      setExpandedFeatures(prev => {
        const newExpanded = new Set(prev);
        featuresToExpand.forEach(path => newExpanded.add(path));
        return newExpanded;
      });
    }
  }, [filter, filteredFeatures]);

  const handleTagToggle = (tag) => {
    setSelectedTags(prev => {
      if (prev.includes(tag)) {
        return prev.filter(t => t !== tag);
      } else {
        return [...prev, tag];
      }
    });
  };

  const clearFilters = () => {
    setFilter('');
    setSelectedTags([]);
  };

  // Prioriza automações em execução sobre as da fila para exibição dos logs
  const runningItems = Array.from(runningAutomations.values()).filter(a => !a.queued);
  const selectedProcessId = runningItems.length > 0 
    ? runningItems[0].id 
    : (Array.from(runningAutomations.keys())[0] || null);
  const currentLogs = selectedProcessId ? logs.get(selectedProcessId) || [] : [];

  // Calcula o total de execuções (features + cenários individuais)
  const calculateExecutionCount = () => {
    let count = 0;
    const details = [];
    const featuresMap = new Map(features.map(f => [f.path, f]));
    
    // Conta features selecionadas
    selectedFeatures.forEach(feature => {
      const scenarios = selectedScenarios.get(feature.path);
      if (scenarios && scenarios.size > 0) {
        count += scenarios.size;
        details.push(`${feature.name}: ${scenarios.size} cenário${scenarios.size > 1 ? 's' : ''}`);
      } else {
        count += 1;
        details.push(feature.name);
      }
    });
    
    // Conta cenários selecionados de features NÃO selecionadas
    for (const [featurePath, selectedScenariosSet] of selectedScenarios.entries()) {
      const isFeatureSelected = selectedFeatures.some(f => f.path === featurePath);
      if (!isFeatureSelected && selectedScenariosSet && selectedScenariosSet.size > 0) {
        const feature = featuresMap.get(featurePath);
        if (feature) {
          count += selectedScenariosSet.size;
          details.push(`${feature.name}: ${selectedScenariosSet.size} cenário${selectedScenariosSet.size > 1 ? 's' : ''}`);
        }
      }
    }
    
    return { count, details };
  };

  const executionInfo = calculateExecutionCount();
  const hasExecutions = executionInfo.count > 0;

  return (
    <div className="automation-runner">
      {isStoppingAll && (
        <div className="stopping-all-overlay">
          <div className="stopping-all-message">
            <div className="stopping-all-spinner">⏹️</div>
            <span>Parando todos os cenários...</span>
          </div>
        </div>
      )}
      <div className="runner-header">
        <h2>Executar Automações</h2>
        <div className="runner-actions">
          {executionInfo.count > 20 && (
            <div className="ram-warning-banner">
              ⚠️ {executionInfo.count} execuções selecionadas - Alto consumo de RAM esperado
            </div>
          )}
          <div className="queue-switch-container">
            <label className="queue-switch-label">
              <input
                type="checkbox"
                checked={executeInQueue}
                onChange={(e) => setExecuteInQueue(e.target.checked)}
                className="queue-switch-input"
              />
              <span className="queue-switch-slider"></span>
              <span className="queue-switch-text">Executar em fila</span>
            </label>
          </div>
          <button
            onClick={handleRunSelected}
            disabled={!hasExecutions || isRunningSelected}
            className="btn-run"
            title={
              !hasExecutions
                ? 'Selecione pelo menos uma feature ou cenário para executar'
                : `Executar ${executionInfo.count === 1 ? executionInfo.details[0] : `${executionInfo.count} execuções`} ${executeInQueue ? 'em fila' : 'em paralelo'}`
            }
          >
            ▶️ Executar ({executionInfo.count || 'Selecionadas'})
          </button>
        </div>
      </div>

      <div className="runner-content">
        <div className="features-panel">
          <div className="filter-section">
            <div className="filter-input-wrapper">
              <span className="filter-icon">🔍</span>
              <input
                type="text"
                placeholder="Buscar por nome da feature ou cenário..."
                value={filter}
                onChange={(e) => setFilter(e.target.value)}
                className="filter-input"
              />
              {(filter || selectedTags.length > 0) && (
                <button 
                  onClick={clearFilters}
                  className="filter-clear-btn"
                  title="Limpar filtros"
                >
                  ✕
                </button>
              )}
            </div>
            
            {allTags.length > 0 && (
              <div className="tags-filter-section">
                <div className="tags-filter-label">
                  <span>Filtrar por tags ({allTags.length}):</span>
                  {selectedTags.length > 0 && (
                    <span className="tags-count">({selectedTags.length} selecionada{selectedTags.length > 1 ? 's' : ''})</span>
                  )}
                  <button
                    onClick={() => setTagsExpanded(!tagsExpanded)}
                    className="tags-toggle-btn"
                    title={tagsExpanded ? 'Colapsar tags' : 'Expandir tags'}
                  >
                    {tagsExpanded ? '▼' : '▶'}
                  </button>
                </div>
                {tagsExpanded && (
                  <div className="tags-filter-list-container">
                    <div className="tags-filter-list">
                      {allTags.map(tag => {
                        const isSelected = selectedTags.includes(tag);
                        const tagCount = features.filter(f => f.tags?.includes(tag)).length;
                        return (
                          <button
                            key={tag}
                            onClick={() => handleTagToggle(tag)}
                            className={`tag-filter-btn ${isSelected ? 'selected' : ''}`}
                            title={`${tagCount} feature${tagCount > 1 ? 's' : ''}`}
                          >
                            {tag}
                            <span className="tag-count-badge">{tagCount}</span>
                          </button>
                        );
                      })}
                    </div>
                  </div>
                )}
              </div>
            )}
            
            <div className="filter-results-info">
              <div className="results-info-left">
              {filteredFeatures.length === 0 ? (
                <span className="no-results">Nenhuma feature ou cenário encontrado</span>
              ) : (
                  <>
                <span className="results-count">
                  {filteredFeatures.length} de {features.length} feature{features.length > 1 ? 's' : ''}
                </span>
                    {filter && (() => {
                      // Conta quantos cenários correspondem ao filtro
                      const filterLower = filter.toLowerCase();
                      const matchingScenariosCount = filteredFeatures.reduce((count, f) => {
                        if (f.scenarios && f.scenarios.length > 0) {
                          const matching = f.scenarios.filter(s => 
                            s.name.toLowerCase().includes(filterLower)
                          ).length;
                          return count + matching;
                        }
                        return count;
                      }, 0);
                      return matchingScenariosCount > 0 ? (
                        <span className="selected-count">
                          • {matchingScenariosCount} cenário{matchingScenariosCount > 1 ? 's' : ''} encontrado{matchingScenariosCount > 1 ? 's' : ''}
                        </span>
                      ) : null;
                    })()}
                    {selectedFeatures.length > 0 && (
                      <span className="selected-count">
                        • {selectedFeatures.length} selecionada{selectedFeatures.length > 1 ? 's' : ''}
                      </span>
                    )}
                  </>
                )}
              </div>
              {filteredFeatures.length > 0 && (
                <div className="selection-actions">
                  <button
                    onClick={() => {
                      const allFiltered = filteredFeatures.filter(f => 
                        !Array.from(runningAutomations.values())
                          .some(a => a.featurePath === f.path && !a.scenarioName)
                      );
                      setSelectedFeatures(allFiltered);
                      // Auto-expande features selecionadas
                      setExpandedFeatures(new Set(allFiltered.map(f => f.path)));
                    }}
                    className="btn-select-all"
                    title="Selecionar todas as features visíveis"
                  >
                    ✓ Selecionar Todas
                  </button>
                  <button
                    onClick={() => {
                      setSelectedFeatures([]);
                      setSelectedScenarios(new Map());
                    }}
                    className="btn-deselect-all"
                    title="Desmarcar todas as seleções"
                  >
                    ✕ Desmarcar Todas
                  </button>
                </div>
              )}
            </div>
          </div>
          
          <div className="features-list">
            {filteredFeatures.map(feature => {
              const isSelected = selectedFeatures.find(f => f.path === feature.path);
              const isExpanded = expandedFeatures.has(feature.path);
              const selectedScenariosForFeature = selectedScenarios.get(feature.path) || new Set();
              const hasScenarios = feature.scenarios && feature.scenarios.length > 0;
              const isRunning = Array.from(runningAutomations.values())
                .some(a => a.featurePath === feature.path && !a.scenarioName);
              
              return (
                <div key={feature.path} className="feature-item-container">
                <div
                  className={`feature-item ${isSelected ? 'selected' : ''} ${isRunning ? 'running' : ''}`}
                    onClick={(e) => {
                      // Não seleciona se clicou no botão de expandir
                      if (e.target.closest('.expand-scenarios-btn')) {
                        return;
                      }
                      if (!isRunning) {
                        handleToggleFeature(feature);
                      }
                    }}
                >
                  <input
                    type="checkbox"
                    checked={!!isSelected}
                      onChange={(e) => {
                        e.stopPropagation();
                        if (!isRunning) {
                          handleToggleFeature(feature);
                        }
                      }}
                    disabled={isRunning}
                      onClick={(e) => e.stopPropagation()}
                  />
                  <div className="feature-item-info">
                      <div className="feature-item-header">
                        <div className="feature-item-name-wrapper">
                    <div className="feature-item-name">{feature.name}</div>
                          {hasScenarios && (
                            <span className="scenarios-count-badge">
                              {feature.scenarios.length} cenário{feature.scenarios.length > 1 ? 's' : ''}
                            </span>
                          )}
                        </div>
                        {hasScenarios && (
                          <button
                            className="expand-scenarios-btn"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleToggleFeatureExpand(feature.path);
                            }}
                            title={isExpanded ? 'Colapsar cenários' : 'Expandir cenários'}
                          >
                            {isExpanded ? '▼' : '▶'}
                          </button>
                        )}
                      </div>
                      <div className="feature-item-meta">
                    <div className="feature-item-tags">
                          {feature.tags.slice(0, 3).map(tag => (
                            <span key={tag} className="tag-small" title={tag}>{tag}</span>
                      ))}
                          {feature.tags.length > 3 && (
                            <div 
                              className="tag-more-wrapper"
                              onMouseEnter={() => setHoveredTagFeaturePath(feature.path)}
                              onMouseLeave={() => setHoveredTagFeaturePath(null)}
                            >
                              <span 
                                className="tag-more-indicator" 
                                title={`Clique ou passe o mouse para ver ${feature.tags.length - 3} tag(s) oculta(s)`}
                              >
                                +{feature.tags.length - 3}
                              </span>
                              {hoveredTagFeaturePath === feature.path && (
                                <div 
                                  className="tags-tooltip"
                                  onMouseEnter={() => setHoveredTagFeaturePath(feature.path)}
                                  onMouseLeave={() => setHoveredTagFeaturePath(null)}
                                >
                                  <div className="tags-tooltip-content">
                                    <div className="tags-tooltip-title">Todas as Tags ({feature.tags.length}):</div>
                                    <div className="tags-tooltip-list">
                                      {feature.tags.map(tag => (
                                        <span key={tag} className="tag-tooltip-item" title={tag}>{tag}</span>
                                      ))}
                                    </div>
                                  </div>
                                </div>
                              )}
                            </div>
                          )}
                        </div>
                        {selectedScenariosForFeature.size > 0 && (
                          <span className="selected-scenarios-indicator">
                            {selectedScenariosForFeature.size} cenário{selectedScenariosForFeature.size > 1 ? 's' : ''} selecionado{selectedScenariosForFeature.size > 1 ? 's' : ''}
                          </span>
                        )}
                    </div>
                  </div>
                  {isRunning && <span className="running-indicator">⏳</span>}
                  </div>
                  
                  {/* Lista de cenários expandida */}
                  {isExpanded && hasScenarios && (
                    <div className="scenarios-list">
                      {feature.scenarios.length === 0 ? (
                        <div className="scenario-item-empty">
                          Nenhum cenário encontrado nesta feature
                        </div>
                      ) : (
                        feature.scenarios.map((scenario, index) => {
                          const isScenarioSelected = selectedScenariosForFeature.has(scenario.name);
                          const isScenarioRunning = Array.from(runningAutomations.values())
                            .some(a => a.featurePath === feature.path && a.scenarioName === scenario.name);
                          
                          // Verifica se o cenário corresponde ao filtro
                          const filterLower = filter ? filter.toLowerCase() : '';
                          const matchesFilter = filter && scenario.name.toLowerCase().includes(filterLower);
                          
                          return (
                            <div
                              key={`${feature.path}-${index}`}
                              className={`scenario-item ${isScenarioSelected ? 'selected' : ''} ${isScenarioRunning ? 'running' : ''} ${matchesFilter ? 'filter-match' : ''}`}
                              onClick={(e) => {
                                e.stopPropagation();
                                if (!isScenarioRunning) {
                                  handleToggleScenario(feature.path, scenario.name);
                                }
                              }}
                            >
                              <input
                                type="checkbox"
                                checked={isScenarioSelected}
                                onChange={(e) => {
                                  e.stopPropagation();
                                  if (!isScenarioRunning) {
                                    handleToggleScenario(feature.path, scenario.name);
                                  }
                                }}
                                disabled={isScenarioRunning}
                                onClick={(e) => e.stopPropagation()}
                              />
                              <div className="scenario-item-info">
                                <div className="scenario-item-name">
                                  {scenario.type === 'outline' ? '📋' : '📝'} {scenario.name}
                                </div>
                                {scenario.steps && scenario.steps.length > 0 && (
                                  <div className="scenario-steps-count">
                                    {scenario.steps.length} passo{scenario.steps.length > 1 ? 's' : ''}
                                  </div>
                                )}
                              </div>
                              <button
                                className="scenario-view-btn"
                                onClick={(e) => {
                                  e.stopPropagation();
                                  setViewingScenario({ feature, scenario });
                                }}
                                title="Visualizar detalhes do cenário"
                              >
                                👁️
                              </button>
                              {isScenarioRunning && <span className="running-indicator">⏳</span>}
                            </div>
                          );
                        })
                      )}
                      {isSelected && hasScenarios && (
                        <div className="scenarios-actions">
                          <button
                            className="btn-select-all-scenarios"
                            onClick={(e) => {
                              e.stopPropagation();
                              const allScenarios = feature.scenarios.map(s => s.name);
                              setSelectedScenarios(prev => {
                                const newScenarios = new Map(prev);
                                // Cria um novo Set para garantir que o React detecte a mudança
                                newScenarios.set(feature.path, new Set(allScenarios));
                                return newScenarios;
                              });
                            }}
                            title="Selecionar todos os cenários desta feature"
                          >
                            ✓ Selecionar Todos
                          </button>
                          <button
                            className="btn-deselect-all-scenarios"
                            onClick={(e) => {
                              e.stopPropagation();
                              setSelectedScenarios(prev => {
                                const newScenarios = new Map(prev);
                                // Remove a entrada do Map
                                newScenarios.delete(feature.path);
                                return newScenarios;
                              });
                            }}
                            title="Desmarcar todos os cenários desta feature"
                          >
                            ✕ Desmarcar Todos
                          </button>
                        </div>
                      )}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>

        <div className="logs-panel">
          <div className="logs-header">
            <div className="logs-header-title-section">
              <h3>Logs de Execução</h3>
              {/* Exibe status da execução atual */}
              {(() => {
                if (currentExecutionResult) {
                  const statusIcon = currentExecutionResult.success ? '✅' : '❌';
                  const statusText = currentExecutionResult.success ? 'Sucesso' : 'Falha';
                  const statusClass = currentExecutionResult.success ? 'execution-status-success' : 'execution-status-failed';
                  
                  return (
                    <div className={`execution-status-badge ${statusClass}`}>
                      <span className="execution-status-icon">{statusIcon}</span>
                      <span className="execution-status-text">{statusText}</span>
                      {currentExecutionResult.scenarios && currentExecutionResult.scenarios.length > 0 && (
                        <span className="execution-status-count">
                          ({currentExecutionResult.scenarios.filter(s => s.status === 'passed').length}/{currentExecutionResult.scenarios.length})
                        </span>
                      )}
                    </div>
                  );
                }
                return null;
              })()}
            </div>
            <div className="logs-header-actions">
              {runningAutomations.size > 0 && (
                <button
                  onClick={stopAllAutomations}
                  className="btn-stop-all"
                  title="Cancelar todas as execuções e itens da fila"
                >
                  ⏹️ Cancelar Tudo
                </button>
              )}
              {selectedProcessId && (
                <button
                  onClick={() => stopAutomation(selectedProcessId)}
                  className="btn-stop"
                  title="Parar execução selecionada"
                >
                  ⏹️ Parar
                </button>
              )}
            </div>
          </div>
          
          {runningAutomations.size > 0 && (
            <div className="execution-status-container">
              {/* Execuções em andamento */}
              {Array.from(runningAutomations.values()).filter(a => !a.queued).length > 0 && (
                <div className="running-section">
                  <div className="section-header">
                    <h4>🔄 Em Execução ({Array.from(runningAutomations.values()).filter(a => !a.queued).length})</h4>
                  </div>
                  <div className="running-list">
                    {Array.from(runningAutomations.values())
                      .filter(a => !a.queued)
                      .map(auto => (
                        <div key={auto.id} className="running-item">
                          <div className="running-item-info">
                            <span className="running-item-name">
                              {auto.scenarioName ? `${auto.featureName} - ${auto.scenarioName}` : auto.featureName}
                            </span>
                            <span className="running-time">
                              ⏱️ {formatDuration(new Date() - auto.startTime)}
                            </span>
                          </div>
                          <button
                            onClick={() => stopAutomation(auto.id)}
                            className="btn-stop-item"
                            title="Parar esta execução"
                          >
                            ✕
                          </button>
                        </div>
                      ))}
                  </div>
                </div>
              )}
              
              {/* Itens na fila */}
              {Array.from(runningAutomations.values()).filter(a => a.queued).length > 0 && (
                <div className="queue-section">
                  <div className="section-header">
                    <h4>📋 Na Fila ({Array.from(runningAutomations.values()).filter(a => a.queued).length})</h4>
                  </div>
                  <div className="queue-list">
                    {Array.from(runningAutomations.values())
                      .filter(a => a.queued)
                      .sort((a, b) => (a.queuePosition || 0) - (b.queuePosition || 0))
                      .map(auto => {
                        const isFailed = failedItems.has(auto.id);
                        const isFadingOut = fadingOutItems.has(auto.id);
                        return (
                          <div 
                            key={auto.id} 
                            className={`queue-item ${isFailed ? 'failed' : ''} ${isFadingOut ? 'fadeout' : ''}`}
                          >
                            <div className="queue-item-info">
                              <span className="queue-position">#{auto.queuePosition || '?'}</span>
                              <span className="queue-item-name">
                                {auto.scenarioName ? `${auto.featureName} - ${auto.scenarioName}` : auto.featureName}
                              </span>
                              {isFailed && (
                                <span className="failed-indicator">❌ Falhou</span>
                              )}
                            </div>
                            <button
                              onClick={() => stopAutomation(auto.id)}
                              className="btn-remove-queue-item"
                              title="Remover da fila"
                            >
                              ✕
                            </button>
                          </div>
                        );
                      })}
                  </div>
                </div>
              )}
            </div>
          )}

          <div className="logs-content">
            {currentLogs.length === 0 ? (
              <div className="empty-logs">
                Nenhuma execução em andamento. Selecione e execute uma automação.
              </div>
            ) : (
              <>
                {/* Exibe steps BDD em tempo real se disponíveis */}
                {(() => {
                  // Debug: log estado atual
                  console.log(`[BDDStepsProgress] Verificando renderização - bddSteps.size: ${bddSteps.size}, runningAutomations.size: ${runningAutomations.size}, selectedProcessId: ${selectedProcessId}`);
                  
                  // Prioriza o processId selecionado, mas também verifica outros processos em execução
                  let processIdToUse = selectedProcessId;
                  let stepsForProcess = null;
                  
                  // Tenta usar o processId selecionado primeiro
                  if (processIdToUse) {
                    stepsForProcess = bddSteps.get(processIdToUse);
                    if (stepsForProcess && stepsForProcess.length > 0) {
                      console.log(`[BDDStepsProgress] ✅ Steps encontrados para processId selecionado: ${processIdToUse} (${stepsForProcess.length} steps)`);
                    }
                  }
                  
                  // Se não encontrou steps para o processId selecionado, tenta encontrar qualquer processo com steps
                  if (!stepsForProcess || stepsForProcess.length === 0) {
                    // Procura em todos os processos em execução (priorizando os que não estão na fila)
                    const runningItems = Array.from(runningAutomations.values()).filter(a => !a.queued);
                    const allProcessIds = runningItems.length > 0 
                      ? runningItems.map(a => a.id).concat(Array.from(runningAutomations.keys()))
                      : Array.from(runningAutomations.keys());
                    
                    // Remove duplicatas mantendo a ordem
                    const uniqueProcessIds = Array.from(new Set(allProcessIds));
                    
                    console.log(`[BDDStepsProgress] Procurando steps em ${uniqueProcessIds.length} processIds:`, uniqueProcessIds);
                    console.log(`[BDDStepsProgress] ProcessIds com steps disponíveis:`, Array.from(bddSteps.keys()));
                    
                    for (const pid of uniqueProcessIds) {
                      const steps = bddSteps.get(pid);
                      if (steps && steps.length > 0) {
                        processIdToUse = pid;
                        stepsForProcess = steps;
                        console.log(`[BDDStepsProgress] ✅ Encontrados steps para processId: ${pid} (${steps.length} steps)`);
                        break;
                      }
                    }
                    
                    // Se ainda não encontrou, tenta qualquer processId que tenha steps (mesmo que não esteja em execução)
                    if (!stepsForProcess || stepsForProcess.length === 0) {
                      const allStepsProcessIds = Array.from(bddSteps.keys());
                      if (allStepsProcessIds.length > 0) {
                        const firstPidWithSteps = allStepsProcessIds[0];
                        stepsForProcess = bddSteps.get(firstPidWithSteps);
                        processIdToUse = firstPidWithSteps;
                        console.log(`[BDDStepsProgress] ⚠️ Usando processId com steps (mesmo que não esteja em execução): ${firstPidWithSteps} (${stepsForProcess?.length || 0} steps)`);
                      }
                    }
                  }
                  
                  // Se encontrou steps, exibe o componente
                  if (stepsForProcess && stepsForProcess.length > 0 && processIdToUse) {
                    console.log(`[BDDStepsProgress] 🎯 Renderizando ${stepsForProcess.length} steps para processId: ${processIdToUse}`);
                    return (
                      <BDDStepsProgress 
                        processId={processIdToUse}
                        steps={stepsForProcess}
                      />
                    );
                  }
                  
                  // Debug: log quando não encontra steps
                  if (bddSteps.size > 0) {
                    console.log(`[BDDStepsProgress] ⚠️ Há ${bddSteps.size} processo(s) com steps, mas nenhum corresponde aos processos em execução`);
                    console.log(`[BDDStepsProgress] ProcessIds com steps:`, Array.from(bddSteps.keys()));
                    console.log(`[BDDStepsProgress] ProcessIds em execução:`, Array.from(runningAutomations.keys()));
                  } else {
                    console.log(`[BDDStepsProgress] ℹ️ Nenhum step BDD disponível ainda (bddSteps.size: ${bddSteps.size})`);
                  }
                  
                  return null;
                })()}
                
                {/* Exibe steps BDD do resultado final se disponíveis */}
                {(() => {
                  // Usa o resultado da execução atual se disponível
                  if (currentExecutionResult && currentExecutionResult.scenarios && currentExecutionResult.scenarios.length > 0) {
                    // Mostra o primeiro cenário que falhou, ou o primeiro se todos passaram
                    const scenarioToShow = currentExecutionResult.scenarios.find(s => s.status === 'failed') || 
                                          currentExecutionResult.scenarios[0];
                    
                    if (scenarioToShow && scenarioToShow.bddSteps && scenarioToShow.bddSteps.length > 0) {
                      return (
                        <BDDStepsViewer 
                          scenario={scenarioToShow} 
                          featureName={currentExecutionResult.featureName}
                          scenarioName={currentExecutionResult.scenarioName || scenarioToShow.name}
                        />
                      );
                    }
                  }
                  
                  // Fallback: procura no resultado mais recente do localStorage
                  const savedHistory = localStorage.getItem('executionHistory');
                  if (savedHistory) {
                    try {
                      const history = JSON.parse(savedHistory);
                      const latest = history.find(h => 
                        selectedProcessId && h.processId === selectedProcessId
                      ) || history[0];
                      
                      if (latest && latest.scenarios && latest.scenarios.length > 0) {
                        const scenarioToShow = latest.scenarios.find(s => s.status === 'failed') || 
                                              latest.scenarios[0];
                        if (scenarioToShow && scenarioToShow.bddSteps && scenarioToShow.bddSteps.length > 0) {
                          return (
                            <BDDStepsViewer 
                              scenario={scenarioToShow} 
                              featureName={latest.featureName}
                              scenarioName={latest.scenarioName || scenarioToShow.name}
                            />
                          );
                        }
                      }
                    } catch (e) {
                      // Ignora erros de parsing
                    }
                  }
                  
                  return null;
                })()}
                
                <pre className="logs-text">
                  {currentLogs.join('')}
                </pre>
              </>
            )}
          </div>

          {recentAutomations.length > 0 && (
            <div className="recent-automations-panel">
              <div className="recent-automations-header">
                <h4>Execuções Recentes</h4>
                <span>{recentAutomations.length} registro(s)</span>
              </div>
              <div className="recent-automations-list">
                {recentAutomations.map(auto => (
                  <div
                    key={`${auto.processId}-${auto.endTime}`}
                    className={`recent-automation-item ${auto.success ? 'success' : 'failed'}`}
                  >
                    <div className="recent-automation-main">
                      <div className="recent-automation-title">
                        <span className="status-icon">{auto.success ? '✅' : '❌'}</span>
                        <strong>
                          {auto.featureName}
                          {auto.scenarioName ? ` - ${auto.scenarioName}` : ''}
                        </strong>
                        {auto.repeatCount > 1 && (
                          <span className="repeat-badge" title="Quantidade de vezes que esta automação rodou recentemente">
                            x{auto.repeatCount}
                          </span>
                        )}
                      </div>
                      <div className="recent-automation-meta">
                        <span title="Process ID">{auto.processId}</span>
                        <span>{formatDateTime(auto.endTime)}</span>
                        <span>{formatDuration(auto.duration)}</span>
                      </div>
                    </div>
                    {auto.category && (
                      <div className="recent-automation-category">
                        {auto.categoryDescription || auto.category}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Modal de visualização do cenário */}
      {viewingScenario && (
        <div className="scenario-modal-overlay" onClick={() => setViewingScenario(null)}>
          <div className="scenario-modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="scenario-modal-header">
              <h3>Detalhes do Cenário</h3>
              <button
                className="scenario-modal-close"
                onClick={() => setViewingScenario(null)}
                title="Fechar"
              >
                ✕
              </button>
            </div>
            
            <div className="scenario-modal-body">
              <div className="scenario-modal-section">
                <h4>Feature</h4>
                <div className="scenario-modal-info">
                  <div className="scenario-modal-info-item">
                    <span className="scenario-modal-label">Nome:</span>
                    <span className="scenario-modal-value">{viewingScenario.feature.name}</span>
                  </div>
                  {viewingScenario.feature.description && (
                    <div className="scenario-modal-info-item">
                      <span className="scenario-modal-label">Descrição:</span>
                      <span className="scenario-modal-value">{viewingScenario.feature.description}</span>
                    </div>
                  )}
                  <div className="scenario-modal-info-item">
                    <span className="scenario-modal-label">Arquivo:</span>
                    <span className="scenario-modal-value">{viewingScenario.feature.path}</span>
                  </div>
                  {viewingScenario.feature.tags && viewingScenario.feature.tags.length > 0 && (
                    <div className="scenario-modal-info-item">
                      <span className="scenario-modal-label">Tags:</span>
                      <div className="scenario-modal-tags">
                        {viewingScenario.feature.tags.map(tag => (
                          <span key={tag} className="tag-small">{tag}</span>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              </div>

              <div className="scenario-modal-section">
                <h4>Cenário</h4>
                <div className="scenario-modal-info">
                  <div className="scenario-modal-info-item">
                    <span className="scenario-modal-label">Tipo:</span>
                    <span className="scenario-modal-value">
                      {viewingScenario.scenario.type === 'outline' ? '📋 Scenario Outline' : '📝 Scenario'}
                    </span>
                  </div>
                  <div className="scenario-modal-info-item">
                    <span className="scenario-modal-label">Nome:</span>
                    <span className="scenario-modal-value">{viewingScenario.scenario.name}</span>
                  </div>
                </div>
              </div>

              {viewingScenario.scenario.steps && viewingScenario.scenario.steps.length > 0 && (
                <div className="scenario-modal-section">
                  <h4>Passos ({viewingScenario.scenario.steps.length})</h4>
                  <div className="scenario-modal-steps">
                    {viewingScenario.scenario.steps.map((step, index) => {
                      const stepText = typeof step === 'string'
                        ? step
                        : (step?.text ||
                           step?.translated ||
                           step?.fullLine ||
                           [step?.keyword, step?.argument || step?.value || step?.name]
                             .filter(Boolean)
                             .join(' ')
                          );

                      const normalizedText = stepText || '';
                      // Identifica o tipo de passo
                      let stepType = '';
                      let stepIcon = '';
                      if (/^Given\s+/i.test(normalizedText)) {
                        stepType = 'given';
                        stepIcon = '🟢';
                      } else if (/^When\s+/i.test(normalizedText)) {
                        stepType = 'when';
                        stepIcon = '🔵';
                      } else if (/^Then\s+/i.test(normalizedText)) {
                        stepType = 'then';
                        stepIcon = '🟡';
                      } else if (/^And\s+/i.test(normalizedText)) {
                        stepType = 'and';
                        stepIcon = '⚪';
                      } else if (/^But\s+/i.test(normalizedText)) {
                        stepType = 'but';
                        stepIcon = '🔴';
                      }
                      
                      return (
                        <div key={index} className={`scenario-modal-step scenario-modal-step-${stepType}`}>
                          <span className="scenario-modal-step-icon">{stepIcon}</span>
                          <code className="scenario-modal-step-text">{normalizedText || '(passo indisponível)'}</code>
                        </div>
                      );
                    })}
                  </div>
                </div>
              )}

              {(!viewingScenario.scenario.steps || viewingScenario.scenario.steps.length === 0) && (
                <div className="scenario-modal-section">
                  <div className="scenario-modal-empty">
                    Nenhum passo encontrado neste cenário.
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Modal de aviso de consumo de RAM */}
      <ConfirmModal
        isOpen={showRamWarningModal}
        onClose={handleRamWarningCancel}
        onConfirm={handleRamWarningConfirm}
        title="⚠️ ATENÇÃO"
        message={`Você está prestes a executar ${pendingExecutions?.length || 0} automações.\n\nIsso pode consumir uma grande quantidade de RAM.\n\nCada execução pode abrir uma janela do Chrome, o que pode impactar significativamente o desempenho do sistema.\n\nRecomendação: Execute em lotes menores (máximo 20 por vez)\n\nDeseja continuar mesmo assim?`}
        confirmText="Sim, Continuar"
        cancelText="Cancelar"
        type="warning"
        showCancel={true}
      />

      {/* Modal de confirmação para parar todas as automações */}
      <ConfirmModal
        isOpen={showStopAllModal}
        onClose={handleStopAllCancel}
        onConfirm={handleStopAllConfirm}
        title="⚠️ Confirmar Cancelamento"
        message="Tem certeza que deseja cancelar todas as execuções e itens da fila?"
        confirmText="Sim, Cancelar Tudo"
        cancelText="Não"
        type="danger"
        showCancel={true}
      />
    </div>
  );
}

// Componente para visualizar steps BDD em tempo real durante a execução
function BDDStepsProgress({ processId, steps }) {
  const currentStepRef = useRef(null);
  const listRef = useRef(null);
  
  if (!steps || steps.length === 0) {
    return null;
  }
  
  // Conta steps por status para exibir estatísticas
  const runningCount = steps.filter(s => s.status === 'running').length;
  const completedCount = steps.filter(s => s.status === 'completed').length;
  const failedCount = steps.filter(s => s.status === 'failed').length;
  const pendingCount = steps.filter(s => s.status === 'pending').length;
  const skippedCount = steps.filter(s => s.status === 'skipped').length;
  
  // Encontra o índice do step atual em execução
  const currentRunningIndex = steps.findIndex(s => s.status === 'running');
  
  // Scroll automático para o step atual quando ele mudar
  useEffect(() => {
    if (currentRunningIndex >= 0 && currentStepRef.current && listRef.current) {
      // Aguarda um pouco para garantir que o DOM foi atualizado
      setTimeout(() => {
        const stepElement = currentStepRef.current;
        const listElement = listRef.current;
        
        if (stepElement && listElement) {
          const stepRect = stepElement.getBoundingClientRect();
          const listRect = listElement.getBoundingClientRect();
          
          // Calcula a posição relativa do step dentro da lista
          const stepTop = stepElement.offsetTop;
          const stepHeight = stepElement.offsetHeight;
          const listHeight = listElement.clientHeight;
          const listScrollTop = listElement.scrollTop;
          
          // Scroll para centralizar o step atual na viewport
          const targetScroll = stepTop - (listHeight / 2) + (stepHeight / 2);
          
          listElement.scrollTo({
            top: Math.max(0, targetScroll),
            behavior: 'smooth'
          });
        }
      }, 100);
    }
  }, [currentRunningIndex]);
  
  // Função para identificar o tipo de step (GIVEN, WHEN, THEN, AND)
  const getStepType = (stepText) => {
    if (!stepText) return 'unknown';
    const normalized = stepText.trim().toUpperCase();
    if (normalized.startsWith('GIVEN')) return 'given';
    if (normalized.startsWith('WHEN')) return 'when';
    if (normalized.startsWith('THEN')) return 'then';
    if (normalized.startsWith('AND')) return 'and';
    return 'unknown';
  };
  
  // Função para obter o ícone do tipo de step
  const getStepTypeIcon = (stepType) => {
    switch(stepType) {
      case 'given': return '📋';
      case 'when': return '⚡';
      case 'then': return '✅';
      case 'and': return '➕';
      default: return '📝';
    }
  };
  
  return (
    <div className="bdd-steps-progress">
      <div className="bdd-steps-progress-header">
        <h4 className="bdd-steps-progress-title">📝 Passos BDD em Execução</h4>
        <div className="bdd-steps-progress-stats">
          {completedCount > 0 && <span className="stat-completed">✅ {completedCount}</span>}
          {runningCount > 0 && <span className="stat-running">🔄 {runningCount}</span>}
          {failedCount > 0 && <span className="stat-failed">❌ {failedCount}</span>}
          {pendingCount > 0 && <span className="stat-pending">⚪ {pendingCount}</span>}
          {skippedCount > 0 && <span className="stat-skipped">⏭️ {skippedCount}</span>}
        </div>
      </div>
      <div className="bdd-steps-progress-list" ref={listRef}>
        {steps.map((step, index) => {
          let statusIcon = '⚪'; // Pendente
          let stepClass = 'bdd-step-pending';
          let isCurrentStep = false;
          
          if (step.status === 'running') {
            statusIcon = '🔄';
            stepClass = 'bdd-step-running';
            isCurrentStep = true;
          } else if (step.status === 'completed') {
            statusIcon = '✅';
            stepClass = 'bdd-step-completed';
          } else if (step.status === 'failed') {
            statusIcon = '❌';
            stepClass = 'bdd-step-failed';
          } else if (step.status === 'skipped') {
            statusIcon = '⏭️';
            stepClass = 'bdd-step-skipped';
          }
          
          // Identifica o tipo de step
          const stepDisplay = step.fullLine || `${step.keyword} ${step.text}`;
          const stepType = getStepType(stepDisplay);
          const stepTypeIcon = getStepTypeIcon(stepType);
          
          // Destaca o step atual em execução
          const stepNumber = index + 1;
          
          return (
            <div 
              key={index} 
              ref={isCurrentStep ? currentStepRef : null}
              className={`bdd-step-progress-item ${stepClass} ${isCurrentStep ? 'current-step' : ''} step-type-${stepType}`}
              title={step.status === 'running' ? 'Step em execução no momento' : `Step ${step.status}`}
            >
              <span className="bdd-step-progress-number">{stepNumber}.</span>
              <span className="bdd-step-progress-type-icon" title={stepType.toUpperCase()}>{stepTypeIcon}</span>
              <span className="bdd-step-progress-icon">{statusIcon}</span>
              <span className="bdd-step-progress-text">{stepDisplay}</span>
              {isCurrentStep && (
                <span className="bdd-step-progress-indicator">
                  <span className="bdd-step-progress-pulse"></span>
                  Executando agora
                </span>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}

// Componente para visualizar steps BDD
function BDDStepsViewer({ scenario, featureName, scenarioName }) {
  const [expanded, setExpanded] = React.useState(scenario.status === 'failed');
  
  if (!scenario || !scenario.bddSteps || scenario.bddSteps.length === 0) {
    return null;
  }
  
  const statusIcon = scenario.status === 'passed' ? '✅' : scenario.status === 'failed' ? '❌' : '⏸️';
  const statusText = scenario.status === 'passed' ? 'APROVADO' : scenario.status === 'failed' ? 'FALHOU' : 'NÃO EXECUTADO';
  
  return (
    <div className={`bdd-steps-viewer ${scenario.status === 'failed' ? 'failed' : scenario.status === 'passed' ? 'passed' : ''}`}>
      <div className="bdd-steps-header" onClick={() => setExpanded(!expanded)}>
        <span className="bdd-steps-status-icon">{statusIcon}</span>
        <div className="bdd-steps-title">
          <strong>{scenarioName || scenario.name}</strong>
          {featureName && <span className="bdd-steps-feature-name">{featureName}</span>}
        </div>
        <span className={`bdd-steps-status-badge status-${scenario.status}`}>{statusText}</span>
        <span className="bdd-steps-toggle">{expanded ? '▼' : '▶'}</span>
      </div>
      
      {expanded && (
        <div className="bdd-steps-content">
          <div className="bdd-steps-list">
            {scenario.bddSteps.map((step, index) => {
              const stepStatusIcon = step.status === 'passed' ? '✅' : 
                                   step.status === 'failed' ? '❌' : 
                                   step.status === 'skipped' ? '⏭️' : '⚪';
              
              const isFailedStep = step.status === 'failed' || (scenario.failedStep && scenario.failedStep === step);
              
              return (
                <div 
                  key={index} 
                  className={`bdd-step-item step-${step.status} ${isFailedStep ? 'failed-step' : ''}`}
                >
                  <span className="bdd-step-icon">{stepStatusIcon}</span>
                  <span className="bdd-step-text">{step.translated || step.fullLine || `${step.keyword} ${step.text}`}</span>
                  {step.result && (
                    <span className={`bdd-step-result result-${step.status}`}>
                      {step.result}
                    </span>
                  )}
                  
                  {/* Mostra detalhes de erro se o step falhou */}
                  {step.status === 'failed' && step.errorDetails && step.errorDetails.length > 0 && (
                    <div className="bdd-step-error-details">
                      {step.errorDetails.map((detail, detailIndex) => (
                        <div key={detailIndex} className="bdd-step-error-line">
                          {detail}
                        </div>
                      ))}
                    </div>
                  )}
                  
                  {/* Mostra informações de timeout se disponíveis */}
                  {step.errorType === 'timeout' && step.errorDetails && (
                    <div className="bdd-step-timeout-info">
                      <div className="timeout-locator">
                        <strong>Elemento esperado:</strong> {step.errorDetails.locator || 'N/A'}
                      </div>
                      {step.errorDetails.expectedText && (
                        <div className="timeout-text">
                          <strong>Texto esperado:</strong> "{step.errorDetails.expectedText}"
                        </div>
                      )}
                      <div className="timeout-duration">
                        <strong>Timeout:</strong> {step.errorDetails.timeoutValue}{step.errorDetails.timeoutUnit}
                      </div>
                    </div>
                  )}
                </div>
              );
            })}
          </div>
          
          {/* Mostra mensagem de erro se o cenário falhou */}
          {scenario.status === 'failed' && scenario.errorMessage && (
            <div className="bdd-scenario-error">
              <div className="bdd-error-header">💥 Mensagem de Erro:</div>
              <pre className="bdd-error-message">{scenario.errorMessage}</pre>
            </div>
          )}
          
          {/* Mostra stack trace se disponível (colapsável) */}
          {scenario.status === 'failed' && scenario.stackTrace && (
            <details className="bdd-stack-trace-details">
              <summary className="bdd-stack-trace-summary">📍 Rastreamento de Pilha (clique para expandir)</summary>
              <pre className="bdd-stack-trace">{scenario.stackTrace}</pre>
            </details>
          )}
        </div>
      )}
    </div>
  );
}

export default AutomationRunner;

