import React, { useState, useEffect, useRef, useMemo } from 'react';
import automationService from '../services/automation-service';
import ConfirmModal from './ConfirmModal';
import './LogViewer.css';

const cleanupScenarioPatterns = [
  /^errors?\(\)$/i,
  /^cleanup\(\)$/i,
  /^teardown\(\)$/i,
  /^setup\(\)$/i,
  /^before(?:scenario|feature|testrun)\(\)$/i,
  /^after(?:scenario|feature|testrun)\(\)$/i,
  /^initialize\(\)$/i,
  /^finalize\(\)$/i,
  /^dispose\(\)$/i,
  /cleanup$/i,
  /teardown$/i,
  /hook$/i
];

const hasScenarioDetails = (scenario = {}) => {
  return Boolean(
    (scenario.errorMessage && scenario.errorMessage.trim()) ||
    (scenario.stackTrace && scenario.stackTrace.trim()) ||
    (scenario.standardOutput && scenario.standardOutput.trim()) ||
    (scenario.bddSteps && scenario.bddSteps.length > 0)
  );
};

const isScenarioUseful = (scenario = {}) => {
  if (!scenario || !scenario.name) return false;
  const trimmedName = scenario.name.trim();
  if (!trimmedName) return false;
  if (cleanupScenarioPatterns.some(pattern => pattern.test(trimmedName))) return false;

  const normalizedStatus = (scenario.status || '').toLowerCase();
  if (['passed', 'failed'].includes(normalizedStatus)) {
    return true;
  }

  return hasScenarioDetails(scenario);
};

const summarizeScenarioStatuses = (scenarios = []) => {
  return scenarios.reduce(
    (acc, scenario) => {
      const status = (scenario.status || 'unknown').toLowerCase();
      if (status === 'passed') acc.passed += 1;
      else if (status === 'failed') acc.failed += 1;
      else acc.skipped += 1;
      return acc;
    },
    { passed: 0, failed: 0, skipped: 0 }
  );
};

function LogViewer({ projectPath }) {
  const [executionHistory, setExecutionHistory] = useState([]);
  const [selectedExecution, setSelectedExecution] = useState(null);
  const [logs, setLogs] = useState([]);
  const [filter, setFilter] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('all');
  const [exportFormat, setExportFormat] = useState('txt');
  const [showConfirmModal, setShowConfirmModal] = useState(false);
  const [showExportModal, setShowExportModal] = useState(false);
  const [exportResult, setExportResult] = useState(null);
  const logEndRef = useRef(null);
  
  // Filtros adicionais
  const [nameFilter, setNameFilter] = useState(''); // Filtro por nome da feature
  const [selectedExecutions, setSelectedExecutions] = useState(new Set()); // Execuções selecionadas para exportação em lote
  const [expandedFeatures, setExpandedFeatures] = useState(new Set()); // Set de featurePath/featureName para controlar expansão

  // Função para carregar e limpar histórico de duplicatas
  const loadAndCleanHistory = () => {
    const savedHistory = localStorage.getItem('executionHistory');
    if (savedHistory) {
      try {
        const history = JSON.parse(savedHistory);
        
        // Remove duplicatas baseado no processId (mantém apenas o mais recente)
        // Usa um Map para garantir que apenas o último registro de cada processId seja mantido
        const processIdMap = new Map();
        
        // Itera do mais antigo para o mais recente, sobrescrevendo duplicatas
        history.forEach(record => {
          if (record.processId) {
            // Garante que logs seja sempre um array ao carregar do localStorage
            if (record.logs) {
              record.logs = Array.isArray(record.logs) 
                ? record.logs 
                : (typeof record.logs === 'string' ? [record.logs] : []);
            } else {
              record.logs = [];
            }
            processIdMap.set(record.processId, record);
          }
        });
        
        // Converte Map para array e ordena por data (mais recente primeiro)
        const uniqueHistory = Array.from(processIdMap.values())
          .sort((a, b) => new Date(b.startTime || 0) - new Date(a.startTime || 0));
        
        // Se encontrou duplicatas, salva o histórico limpo
        if (uniqueHistory.length !== history.length) {
          console.log(`[LogViewer] Removidas ${history.length - uniqueHistory.length} duplicata(s) do histórico`);
          localStorage.setItem('executionHistory', JSON.stringify(uniqueHistory));
        }
        
        setExecutionHistory(uniqueHistory);
      } catch (error) {
        console.error('[LogViewer] Erro ao carregar histórico:', error);
        setExecutionHistory([]);
      }
    }
  };

  useEffect(() => {
    // Carregar histórico inicial
    loadAndCleanHistory();
    
    // Escuta eventos de conclusão de automação para atualizar em tempo real
    const handleAutomationComplete = (data) => {
      const { processId, result } = data;
      
      // Atualiza o histórico imediatamente quando uma execução é concluída
      // Garante que logs seja sempre um array
      const resultLogs = result.logs || [];
      const logsArray = Array.isArray(resultLogs) 
        ? resultLogs 
        : (typeof resultLogs === 'string' ? [resultLogs] : []);
      
      const executionRecord = {
        processId,
        featureName: result.featureName,
        featurePath: result.featurePath || null, // Salva o caminho do arquivo
        scenarioName: result.scenarioName || null, // Salva o nome do cenário
        success: result.success,
        duration: result.duration,
        startTime: result.startTime,
        endTime: result.endTime || new Date(),
        logs: logsArray,
        exitCode: result.exitCode,
        category: result.category || 'erro_desconhecido',
        categoryDescription: result.categoryDescription || 'Categoria não definida',
        groupKey: result.groupKey || null,
        patternMismatchReason: result.patternMismatchReason || null,
        patternMismatchDetails: result.patternMismatchDetails || null,
        scenarios: result.scenarios || [] // Salva cenários individuais
      };
      
      // Carrega histórico existente
      const savedHistory = localStorage.getItem('executionHistory');
      const history = savedHistory ? JSON.parse(savedHistory) : [];
      
      // Normaliza logs de registros existentes para garantir que sejam arrays
      const normalizedHistory = history.map(record => {
        if (record.logs) {
          record.logs = Array.isArray(record.logs) 
            ? record.logs 
            : (typeof record.logs === 'string' ? [record.logs] : []);
        } else {
          record.logs = [];
        }
        return record;
      });
      
      // Remove qualquer registro duplicado com o mesmo processId
      const filteredHistory = normalizedHistory.filter(record => record.processId !== processId);
      
      // Adiciona o novo registro no início (mais recente primeiro)
      filteredHistory.unshift(executionRecord);
      
      // Limita a 100 registros mais recentes para evitar acúmulo excessivo
      const limitedHistory = filteredHistory.slice(0, 100);
      
      // Salva no localStorage
      localStorage.setItem('executionHistory', JSON.stringify(limitedHistory));
      
      // Atualiza o estado
      setExecutionHistory(limitedHistory);
      
      // Se a execução selecionada é a que acabou de ser concluída, atualiza os logs
      if (selectedExecution && selectedExecution.processId === processId) {
        // Garante que logs seja sempre um array
        const executionLogs = executionRecord.logs || [];
        const logsArray = Array.isArray(executionLogs) 
          ? executionLogs 
          : (typeof executionLogs === 'string' ? [executionLogs] : []);
        setLogs(logsArray);
        setSelectedExecution(executionRecord);
      }
      
      console.log(`[LogViewer] Histórico atualizado em tempo real: ${executionRecord.featureName} (${executionRecord.success ? 'Sucesso' : 'Falha'})`);
    };
    
    // Registra o listener
    automationService.onAutomationComplete(handleAutomationComplete);
    
    // Cleanup: remove listener quando o componente é desmontado
    return () => {
      automationService.removeAutomationCompleteListener();
    };
  }, [selectedExecution]);

  useEffect(() => {
    if (selectedExecution) {
      // Garante que logs seja sempre um array
      const executionLogs = selectedExecution.logs || [];
      const logsArray = Array.isArray(executionLogs) 
        ? executionLogs 
        : (typeof executionLogs === 'string' ? [executionLogs] : []);
      setLogs(logsArray);
    } else {
      setLogs([]);
    }
  }, [selectedExecution]);

  useEffect(() => {
    if (logEndRef.current) {
      logEndRef.current.scrollIntoView({ behavior: 'smooth' });
    }
  }, [logs]);

  const handleExportSingle = async () => {
    if (!selectedExecution) {
      return;
    }

    const exec = selectedExecution;
    const sanitizedFeatureName = (exec.featureName || 'unknown')
      .replace(/[^a-zA-Z0-9_-]/g, '_')
      .substring(0, 50);
    
    const timestamp = new Date(exec.endTime || exec.startTime || new Date())
      .toISOString()
      .replace(/[:.]/g, '-')
      .substring(0, 19);
    
    const filename = `${sanitizedFeatureName}-${timestamp}.${exportFormat}`;

    // Garante que logs seja sempre um array
    const executionLogs = exec.logs || [];
    const logsArray = Array.isArray(executionLogs) 
      ? executionLogs 
      : (typeof executionLogs === 'string' ? [executionLogs] : []);

    try {
      const result = await automationService.exportLogs({
        logs: logsArray,
        format: exportFormat,
        filename,
        category: exec.category || 'sem_categoria',
        groupKey: exec.groupKey || null,
        featureName: exec.featureName || 'unknown',
        executionInfo: {
          featureName: exec.featureName || 'unknown',
          featurePath: exec.featurePath || null,
          scenarioName: exec.scenarioName || null,
          success: exec.success,
          duration: exec.duration,
          startTime: exec.startTime,
          endTime: exec.endTime,
          exitCode: exec.exitCode,
          category: exec.category || null,
          categoryDescription: exec.categoryDescription || null,
          processId: exec.processId,
          scenarios: exec.scenarios || [] // Inclui cenários individuais na exportação
        }
      });

      setExportResult({
        success: result.success,
        message: result.success ? 'Arquivo exportado com sucesso!' : `Erro ao exportar: ${result.error || 'Erro desconhecido'}`,
        path: result.path || null
      });
      setShowExportModal(true);
    } catch (error) {
      setExportResult({
        success: false,
        message: `Erro ao exportar logs: ${error.message || 'Erro desconhecido'}`,
        path: null
      });
      setShowExportModal(true);
    }
  };

  const handleExportMultiple = async () => {
    if (selectedExecutions.size === 0) {
      return;
    }

    const executionsToExport = Array.from(selectedExecutions)
      .map(processId => executionHistory.find(exec => exec.processId === processId))
      .filter(Boolean);

    if (executionsToExport.length === 0) {
      return;
    }

    const results = [];
    for (const exec of executionsToExport) {
      const sanitizedFeatureName = (exec.featureName || 'unknown')
        .replace(/[^a-zA-Z0-9_-]/g, '_')
        .substring(0, 50);
      
      const timestamp = new Date(exec.endTime || exec.startTime || new Date())
        .toISOString()
        .replace(/[:.]/g, '-')
        .substring(0, 19);
      
      const filename = `${sanitizedFeatureName}-${timestamp}.${exportFormat}`;

      // Garante que logs seja sempre um array
      const executionLogs = exec.logs || [];
      const logsArray = Array.isArray(executionLogs) 
        ? executionLogs 
        : (typeof executionLogs === 'string' ? [executionLogs] : []);

      const result = await automationService.exportLogs({
        logs: logsArray,
        format: exportFormat,
        filename,
        category: exec.category || 'sem_categoria',
        groupKey: exec.groupKey || null,
        featureName: exec.featureName || 'unknown',
        executionInfo: {
          featureName: exec.featureName || 'unknown',
          featurePath: exec.featurePath || null,
          scenarioName: exec.scenarioName || null,
          success: exec.success,
          duration: exec.duration,
          startTime: exec.startTime,
          endTime: exec.endTime,
          exitCode: exec.exitCode,
          category: exec.category || null,
          categoryDescription: exec.categoryDescription || null,
          processId: exec.processId,
          scenarios: exec.scenarios || [] // Inclui cenários individuais na exportação
        }
      });

      results.push({ execution: exec, result });
    }

    const successCount = results.filter(r => r.result.success).length;
    const failureCount = results.filter(r => !r.result.success).length;

    setExportResult({
      success: failureCount === 0,
      message: `${successCount} arquivo(s) exportado(s) com sucesso${failureCount > 0 ? `, ${failureCount} falha(s)` : ''}`,
      path: results.length > 0 ? results[0].result.path : null
    });
    setShowExportModal(true);
    
    // Limpa seleção após exportação
    setSelectedExecutions(new Set());
  };

  const handleOpenFileLocation = async () => {
    if (exportResult && exportResult.success && exportResult.path) {
      try {
        console.log('[LogViewer] Abrindo local do arquivo:', exportResult.path);
        
        if (window.electronAPI && window.electronAPI.showItemInFolder) {
          const result = await window.electronAPI.showItemInFolder(exportResult.path);
          if (result && !result.success) {
            console.error('[LogViewer] Erro ao abrir local:', result.error);
            alert(`Erro ao abrir local do arquivo: ${result.error || 'Erro desconhecido'}`);
          } else {
            console.log('[LogViewer] Arquivo aberto com sucesso');
          }
        } else {
          console.error('[LogViewer] API showItemInFolder não disponível');
          alert('Funcionalidade de abrir local não está disponível');
        }
      } catch (error) {
        console.error('[LogViewer] Erro ao abrir local do arquivo:', error);
        alert(`Erro ao abrir local do arquivo: ${error.message || 'Erro desconhecido'}`);
      }
    }
    // Fecha o modal após tentar abrir
    setShowExportModal(false);
    setExportResult(null);
  };

  const handleCloseExportModal = () => {
    setShowExportModal(false);
    setExportResult(null);
  };

  const handleClearHistory = () => {
    if (executionHistory.length === 0) return;
    setShowConfirmModal(true);
  };

  const confirmClearHistory = () => {
    setExecutionHistory([]);
    setSelectedExecution(null);
    setLogs([]);
    localStorage.removeItem('executionHistory');
    setShowConfirmModal(false);
  };

  const cancelClearHistory = () => {
    setShowConfirmModal(false);
  };

  const executionStats = useMemo(() => {
    const total = executionHistory.length;
    const success = executionHistory.filter(exec => exec.success).length;
    const failed = executionHistory.filter(exec => !exec.success).length;
    const lastExecution = executionHistory[0] || null;

    return {
      total,
      success,
      failed,
      successRate: total > 0 ? Math.round((success / total) * 100) : 0,
      lastExecution
    };
  }, [executionHistory]);

  const statusQuickFilters = [
    { id: 'all', label: 'Todas', icon: '📊' },
    { id: 'success', label: 'Sucesso', icon: '✅' },
    { id: 'failed', label: 'Falha', icon: '❌' }
  ];

  // Filtra execuções por categoria e nome
  const filteredHistory = executionHistory.filter(exec => {
    // Filtro por categoria
    let categoryMatch = false;
    if (categoryFilter === 'all') {
      categoryMatch = true;
    } else if (categoryFilter === 'success' && exec.success) {
      categoryMatch = true;
    } else if (categoryFilter === 'failed' && !exec.success) {
      categoryMatch = true;
    } else if (categoryFilter === exec.category) {
      categoryMatch = true;
    }
    
    if (!categoryMatch) return false;
    
    // Filtro por nome da feature
    if (nameFilter) {
      const featureName = exec.featureName || '';
      if (!featureName.toLowerCase().includes(nameFilter.toLowerCase())) {
        return false;
      }
    }
    
    return true;
  });

  // Agrupa execuções por feature
  const groupedByFeature = useMemo(() => {
    const groups = new Map();
    
    filteredHistory.forEach(exec => {
      const featureKey = exec.featurePath || exec.featureName || 'unknown';
      
      if (!groups.has(featureKey)) {
        groups.set(featureKey, {
          featureName: exec.featureName || 'Desconhecido',
          featurePath: exec.featurePath || null,
          executions: [],
          totalExecutions: 0,
          successCount: 0,
          failedCount: 0,
          allScenarios: [] // Agrega todos os cenários de todas as execuções
        });
      }
      
      const group = groups.get(featureKey);
      group.executions.push(exec);
      group.totalExecutions++;
      if (exec.success) {
        group.successCount++;
      } else {
        group.failedCount++;
      }
      
      // Agrega cenários se disponíveis
      if (exec.scenarios && Array.isArray(exec.scenarios) && exec.scenarios.length > 0) {
        exec.scenarios
          .filter(isScenarioUseful)
          .forEach(scenario => {
          // Verifica se o cenário já existe (por nome) para evitar duplicatas
          const existingScenario = group.allScenarios.find(s => s.name === scenario.name);
          if (!existingScenario) {
            group.allScenarios.push({
              ...scenario,
              executions: [exec] // Lista de execuções que contêm este cenário
            });
          } else {
            // Se já existe, adiciona a execução à lista
            if (!existingScenario.executions) {
              existingScenario.executions = [];
            }
            existingScenario.executions.push(exec);
          }
        });
      }
    });
    
    // Ordena execuções dentro de cada grupo por data (mais recente primeiro)
    groups.forEach(group => {
      group.executions.sort((a, b) => new Date(b.startTime || 0) - new Date(a.startTime || 0));
    });
    
    return Array.from(groups.values()).sort((a, b) => {
      // Ordena por nome da feature
      return a.featureName.localeCompare(b.featureName);
    });
  }, [filteredHistory]);

  // Função para alternar expansão de feature
  const handleToggleFeatureExpand = (featureKey) => {
    setExpandedFeatures(prev => {
      const newSet = new Set(prev);
      if (newSet.has(featureKey)) {
        newSet.delete(featureKey);
      } else {
        newSet.add(featureKey);
      }
      return newSet;
    });
  };

  const executionScenarios = useMemo(() => {
    if (!selectedExecution || !Array.isArray(selectedExecution.scenarios)) {
      return { useful: [], hiddenCount: 0, summary: { passed: 0, failed: 0, skipped: 0 } };
    }

    const useful = selectedExecution.scenarios.filter(isScenarioUseful);
    const hiddenCount = selectedExecution.scenarios.length - useful.length;

    return {
      useful,
      hiddenCount,
      summary: summarizeScenarioStatuses(useful)
    };
  }, [selectedExecution]);

  // Garante que logs seja sempre um array
  const logsArray = Array.isArray(logs) ? logs : (typeof logs === 'string' ? [logs] : []);
  
  const filteredLogs = logsArray.filter(log => {
    if (!filter) return true;
    const logText = typeof log === 'string' ? log : log.message || '';
    return logText.toLowerCase().includes(filter.toLowerCase());
  });

  // Obtém lista única de categorias do histórico
  const categories = Array.from(new Set(executionHistory.map(exec => exec.category).filter(Boolean)));

  const dotnetMarkers = [
    '<App>',
    '[App]',
    'dotnet',
    '.NET',
    'Microsoft.',
    'System.',
    'Unhandled exception'
  ];

  const getLogType = (logLine) => {
    const line = typeof logLine === 'string' ? logLine : logLine.message || '';
    if (dotnetMarkers.some(marker => line.toLowerCase().includes(marker.toLowerCase()))) {
      return 'dotnet';
    }
    if (line.match(/error|exception|failed/i)) return 'error';
    if (line.match(/warning|warn/i)) return 'warning';
    if (line.match(/passed|success|ok/i)) return 'success';
    if (line.match(/given|when|then|and/i)) return 'step';
    return 'info';
  };

  const logTypeLabels = {
    error: 'Erro',
    warning: 'Alerta',
    success: 'Sucesso',
    step: 'BDD',
    dotnet: '.NET',
    info: 'Info'
  };

  return (
    <>
      <ConfirmModal
        isOpen={showConfirmModal}
        onClose={cancelClearHistory}
        onConfirm={confirmClearHistory}
        title="⚠️ Confirmar Exclusão"
        message="Tem certeza que deseja limpar todo o histórico de execuções? Esta ação não pode ser desfeita e todos os logs serão permanentemente removidos."
        confirmText="🗑️ Sim, Limpar"
        cancelText="Cancelar"
        type="danger"
      />
      <ConfirmModal
        isOpen={showExportModal}
        onClose={handleCloseExportModal}
        onConfirm={exportResult?.success && exportResult?.path ? handleOpenFileLocation : handleCloseExportModal}
        title={exportResult?.success ? "✅ Exportação Concluída" : "❌ Erro na Exportação"}
        message={exportResult?.success 
          ? exportResult?.message 
            ? `${exportResult.message}\n\n${exportResult.path ? `Primeiro arquivo: ${exportResult.path}\n\nDeseja abrir o local do arquivo no explorador?` : ''}`
            : `Logs exportados com sucesso!\n\nArquivo: ${exportResult.path}\n\nDeseja abrir o local do arquivo no explorador?`
          : `Erro ao exportar logs:\n\n${exportResult?.error || 'Erro desconhecido'}`
        }
        confirmText={exportResult?.success && exportResult?.path ? "📂 Abrir Local" : "OK"}
        cancelText={exportResult?.success && exportResult?.path ? "Fechar" : ""}
        type={exportResult?.success ? "success" : "danger"}
        showCancel={exportResult?.success && exportResult?.path}
      />
      <div className="log-viewer">
        <div className="log-viewer-header">
        <h2>Visualizador de Logs</h2>
        <div className="log-actions">
          <select
            value={exportFormat}
            onChange={(e) => setExportFormat(e.target.value)}
            className="export-select"
            title="Formato de exportação"
          >
            <option value="txt">TXT</option>
            <option value="json">JSON</option>
          </select>
          <button
            onClick={handleClearHistory}
            disabled={executionHistory.length === 0}
            className="btn-clear"
            title="Limpar histórico de execuções"
          >
            🗑️ Limpar
          </button>
        </div>
      </div>

      <div className="log-summary-grid">
        <div className="summary-card total">
          <div className="summary-label">Execuções</div>
          <div className="summary-value">{executionStats.total}</div>
          <span className="summary-pill">📊 Total</span>
        </div>
        <div className="summary-card success">
          <div className="summary-label">Com Sucesso</div>
          <div className="summary-value">{executionStats.success}</div>
          <span className="summary-pill">✅ {executionStats.successRate}%</span>
        </div>
        <div className="summary-card failed">
          <div className="summary-label">Com Falha</div>
          <div className="summary-value">{executionStats.failed}</div>
          <span className="summary-pill">❌ {executionStats.total > 0 ? 100 - executionStats.successRate : 0}%</span>
        </div>
        <div className="summary-card highlight">
          <div className="summary-label">Última Execução</div>
          {executionStats.lastExecution ? (
            <>
              <div className="summary-value last">
                {executionStats.lastExecution.success ? '✅ Sucesso' : '❌ Falha'}
              </div>
              <div className="summary-extra">
                {new Date(executionStats.lastExecution.startTime).toLocaleString('pt-BR')}
              </div>
            </>
          ) : (
            <div className="summary-value last">—</div>
          )}
        </div>
      </div>

      <div className="log-viewer-content">
        <div className="execution-list">
          <h3>Histórico de Execuções</h3>
          {executionHistory.length === 0 ? (
            <div className="empty-state">
              <p>Nenhuma execução registrada.</p>
              <p>Execute automações na aba "Executar" para ver logs aqui.</p>
            </div>
          ) : (
            <>
              <div className="log-filters-section">
                <div className="status-quick-filters">
                  {statusQuickFilters.map((option) => (
                    <button
                      key={option.id}
                      className={`status-chip ${categoryFilter === option.id ? 'active' : ''}`}
                      onClick={() => setCategoryFilter(option.id)}
                    >
                      <span className="chip-icon">{option.icon}</span>
                      {option.label}
                    </button>
                  ))}
                </div>
                <div className="category-filter">
                  <label htmlFor="category-filter">Filtrar por categoria:</label>
                  <select
                    id="category-filter"
                    value={categoryFilter}
                    onChange={(e) => setCategoryFilter(e.target.value)}
                    className="category-select"
                  >
                    <option value="all">Todas</option>
                    <option value="success">✅ Sucesso</option>
                    <option value="failed">❌ Falhas</option>
                    {categories.filter(cat => cat !== 'sucesso').map(cat => (
                      <option key={cat} value={cat}>
                        {getCategoryLabel(cat)}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="name-filter">
                  <label htmlFor="name-filter">🔍 Filtrar por nome:</label>
                  <div style={{ position: 'relative', width: '100%' }}>
                    <input
                      id="name-filter"
                      type="text"
                      placeholder="Buscar por nome da feature..."
                      value={nameFilter}
                      onChange={(e) => setNameFilter(e.target.value)}
                      className="name-filter-input"
                    />
                    {nameFilter && (
                      <button
                        onClick={() => setNameFilter('')}
                        className="filter-clear-btn"
                        title="Limpar filtro"
                      >
                        ✕
                      </button>
                    )}
                  </div>
                </div>
                {selectedExecutions.size > 0 && (
                  <div className="selection-actions">
                    <span className="selected-count">{selectedExecutions.size} selecionada(s)</span>
                    <button
                      onClick={handleExportMultiple}
                      className="btn-export-multiple"
                      title="Exportar selecionadas"
                    >
                      📥 Exportar Selecionadas
                    </button>
                    <button
                      onClick={() => setSelectedExecutions(new Set())}
                      className="btn-clear-selection"
                      title="Limpar seleção"
                    >
                      ✕ Limpar Seleção
                    </button>
                  </div>
                )}
              </div>
              <div className="execution-items">
                {groupedByFeature.length === 0 ? (
                  <div className="empty-state">
                    <p>Nenhuma execução encontrada com o filtro selecionado.</p>
                  </div>
                ) : (
                  groupedByFeature.map((featureGroup) => {
                    const featureKey = featureGroup.featurePath || featureGroup.featureName;
                    const isExpanded = expandedFeatures.has(featureKey);
                    const relevantScenarios = (featureGroup.allScenarios || []).filter(isScenarioUseful);
                    const hasScenarios = relevantScenarios.length > 0;
                    const scenarioSummary = summarizeScenarioStatuses(relevantScenarios);
                    
                    return (
                      <div key={featureKey} className="feature-group-container">
                        <div
                          className={`feature-group-header ${isExpanded ? 'expanded' : ''}`}
                          onClick={(e) => {
                            // Não expande se clicou no checkbox ou botão de expandir
                            if (e.target.closest('.execution-checkbox') || e.target.closest('.expand-feature-btn')) {
                              return;
                            }
                            handleToggleFeatureExpand(featureKey);
                          }}
                        >
                          <button
                            className="expand-feature-btn"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleToggleFeatureExpand(featureKey);
                            }}
                            title={isExpanded ? 'Colapsar feature' : 'Expandir feature'}
                          >
                            {isExpanded ? '▼' : '▶'}
                          </button>
                          <div className="feature-group-info">
                            <div className="feature-group-name">{featureGroup.featureName}</div>
                            {featureGroup.featurePath && (
                              <div className="feature-group-path">📄 {featureGroup.featurePath}</div>
                            )}
                            <div className="feature-group-stats">
                              <span className="stat-item">
                                <span className="stat-label">Total:</span>
                                <span className="stat-value">{featureGroup.totalExecutions}</span>
                              </span>
                              <span className="stat-item success">
                                <span className="stat-label">✅ Sucesso:</span>
                                <span className="stat-value">{featureGroup.successCount}</span>
                              </span>
                              <span className="stat-item failed">
                                <span className="stat-label">❌ Falhas:</span>
                                <span className="stat-value">{featureGroup.failedCount}</span>
                              </span>
                              {hasScenarios && (
                                <span className="stat-item">
                                  <span className="stat-label">📝 Cenários:</span>
                                  <span className="stat-value">{relevantScenarios.length}</span>
                                </span>
                              )}
                            </div>
                          </div>
                        </div>
                        
                        {isExpanded && (
                          <div className="feature-executions-list">
                            {/* Mostra cenários agregados se disponíveis */}
                            {hasScenarios && (
                              <div className="feature-scenarios-section">
                                <div className="scenarios-section-header">
                                  <h4>📊 Cenários Relevantes ({relevantScenarios.length})</h4>
                                  <div className="scenario-summary-chips compact">
                                    <span className="scenario-summary-chip passed">
                                      ✅ {scenarioSummary.passed}
                                    </span>
                                    <span className="scenario-summary-chip failed">
                                      ❌ {scenarioSummary.failed}
                                    </span>
                                    <span className="scenario-summary-chip skipped">
                                      ⏸️ {scenarioSummary.skipped}
                                    </span>
                                  </div>
                                </div>
                                <div className="scenarios-list-compact">
                                  {relevantScenarios.map((scenario, index) => {
                                    const statusIcon = scenario.status === 'passed' ? '✅' : scenario.status === 'failed' ? '❌' : '⏸️';
                                    const statusText = scenario.status === 'passed' ? 'APROVADO' : scenario.status === 'failed' ? 'FALHOU' : 'NÃO EXECUTADO';
                                    const executionsCount = scenario.executions ? scenario.executions.length : 0;
                                    
                                    return (
                                      <div key={index} className={`scenario-item-compact scenario-${scenario.status}`}>
                                        <span className="scenario-status-icon">{statusIcon}</span>
                                        <span className="scenario-name-compact">{scenario.name}</span>
                                        <span className={`scenario-status-badge-compact scenario-status-${scenario.status}`}>
                                          {statusText}
                                        </span>
                                        {scenario.duration && (
                                          <span className="scenario-duration-compact">({scenario.duration}ms)</span>
                                        )}
                                        {executionsCount > 1 && (
                                          <span className="scenario-executions-count" title={`Aparece em ${executionsCount} execuções`}>
                                            🔢 {executionsCount}
                                          </span>
                                        )}
                                      </div>
                                    );
                                  })}
                                </div>
                              </div>
                            )}
                            
                            {/* Lista de execuções */}
                            <div className="executions-section">
                              <div className="executions-section-header">
                                <h4>📋 Execuções ({featureGroup.executions.length})</h4>
                              </div>
                              <div className="execution-items-list">
                                {featureGroup.executions.map((exec) => {
                                  const isSelected = selectedExecution?.processId === exec.processId;
                                  const isMultiSelected = selectedExecutions.has(exec.processId);
                                  
                                  return (
                                    <div
                                      key={exec.processId || `${exec.featureName}-${exec.startTime}`}
                                      className={`execution-item ${isSelected ? 'selected' : ''} ${isMultiSelected ? 'multi-selected' : ''} ${exec.success ? 'success' : 'failed'}`}
                                      onClick={(e) => {
                                        // Se Ctrl/Cmd está pressionado, permite seleção múltipla
                                        if (e.ctrlKey || e.metaKey) {
                                          e.stopPropagation();
                                          setSelectedExecutions(prev => {
                                            const newSet = new Set(prev);
                                            if (newSet.has(exec.processId)) {
                                              newSet.delete(exec.processId);
                                            } else {
                                              newSet.add(exec.processId);
                                            }
                                            return newSet;
                                          });
                                        } else {
                                          setSelectedExecution(exec);
                                          setSelectedExecutions(new Set()); // Limpa seleção múltipla
                                        }
                                      }}
                                      title={exec.categoryDescription || ''}
                                    >
                                      <div className="execution-header">
                                        <input
                                          type="checkbox"
                                          checked={isMultiSelected}
                                          onChange={(e) => {
                                            e.stopPropagation();
                                            setSelectedExecutions(prev => {
                                              const newSet = new Set(prev);
                                              if (newSet.has(exec.processId)) {
                                                newSet.delete(exec.processId);
                                              } else {
                                                newSet.add(exec.processId);
                                              }
                                              return newSet;
                                            });
                                          }}
                                          onClick={(e) => e.stopPropagation()}
                                          className="execution-checkbox"
                                        />
                                        <span className="execution-status">
                                          {exec.success ? '✅' : '❌'}
                                        </span>
                                        <span className="execution-name">
                                          {exec.scenarioName ? `${exec.featureName} - ${exec.scenarioName}` : exec.featureName}
                                        </span>
                                        {exec.category && (
                                          <span className="execution-category" title={exec.categoryDescription}>
                                            {getCategoryIcon(exec.category)}
                                          </span>
                                        )}
                                      </div>
                                      <div className="execution-meta">
                                        <span>{new Date(exec.startTime).toLocaleString('pt-BR')}</span>
                                        <span>{formatDuration(exec.duration)}</span>
                                        {exec.category && (
                                          <span className="category-badge">{getCategoryLabel(exec.category)}</span>
                                        )}
                                      </div>
                                    </div>
                                  );
                                })}
                              </div>
                            </div>
                          </div>
                        )}
                      </div>
                    );
                  })
                )}
              </div>
            </>
          )}
        </div>

        <div className="log-content-panel">
          {selectedExecution ? (
            <>
              <div className="log-header">
                <div>
                  <h3>Logs: {selectedExecution.featureName}</h3>
                  {selectedExecution.featurePath && (
                    <p className="log-file-path">📄 Arquivo: {selectedExecution.featurePath}</p>
                  )}
                </div>
                <div className="log-header-actions">
                  <select
                    value={exportFormat}
                    onChange={(e) => setExportFormat(e.target.value)}
                    className="export-select"
                    title="Formato de exportação"
                  >
                    <option value="txt">TXT</option>
                    <option value="json">JSON</option>
                  </select>
                  <button
                    onClick={handleExportSingle}
                    className="btn-export"
                    title="Exportar logs desta execução"
                  >
                    📥 Exportar Logs
                  </button>
                  <input
                    type="text"
                    placeholder="🔍 Filtrar logs..."
                    value={filter}
                    onChange={(e) => setFilter(e.target.value)}
                    className="log-filter"
                  />
                </div>
              </div>
              
              {/* Container principal com scroll para navegar entre scenarios-view e log-display */}
              <div className="log-content-scroll-container">
                {/* Exibe cenários individuais se disponíveis */}
                {executionScenarios.useful.length > 0 ? (
                  <div className="scenarios-view-container">
                    <div className="scenarios-view">
                      <div className="scenarios-view-header">
                        <h4>📊 Cenários Relevantes ({executionScenarios.useful.length})</h4>
                        <div className="scenario-summary-chips">
                          <span className="scenario-summary-chip passed">
                            ✅ {executionScenarios.summary.passed} aprovados
                          </span>
                          <span className="scenario-summary-chip failed">
                            ❌ {executionScenarios.summary.failed} falhos
                          </span>
                          <span className="scenario-summary-chip skipped">
                            ⏸️ {executionScenarios.summary.skipped} sem execução
                          </span>
                        </div>
                      </div>
                      {executionScenarios.hiddenCount > 0 && (
                        <div className="scenario-hidden-note">
                          Ocultamos {executionScenarios.hiddenCount} item(ns) técnico(s) (ex.: Errors(), Cleanup()) para manter o foco apenas em cenários que ajudam na triagem.
                        </div>
                      )}
                      <div className="scenarios-list">
                        {executionScenarios.useful.map((scenario, index) => (
                          <ScenarioCard 
                            key={index} 
                            scenario={scenario} 
                            index={index}
                            filter={filter}
                          />
                        ))}
                      </div>
                    </div>
                  </div>
                ) : (
                  <div className="scenario-hidden-note no-data">
                    Nenhum cenário relevante foi identificado para esta execução. Isso geralmente ocorre quando o .NET não listou os cenários (ex.: execução abortada antes da fase de testes).
                  </div>
                )}
                
                <div className="log-display">
                  <h4>📋 Log Completo da Execução</h4>
                  <div className="log-legend">
                    {Object.entries(logTypeLabels).map(([type, label]) => (
                      <span key={type} className={`legend-chip legend-${type}`}>
                        <span className="legend-dot" />
                        {label}
                      </span>
                    ))}
                  </div>
                  {filteredLogs.length === 0 ? (
                    <div className="empty-logs">Nenhum log encontrado</div>
                  ) : (
                    <pre className="log-text">
                      {filteredLogs.map((log, index) => {
                        const logLine = typeof log === 'string' ? log : log.message || '';
                        const type = getLogType(log);
                        return (
                          <div key={index} className={`log-line log-${type}`}>
                            <span className="log-line-badge">{logTypeLabels[type] || 'Info'}</span>
                            <span>{logLine}</span>
                          </div>
                        );
                      })}
                      <div ref={logEndRef} />
                    </pre>
                  )}
                </div>
              </div>
            </>
          ) : (
            <div className="empty-selection">
              <p>Selecione uma execução para visualizar os logs</p>
            </div>
          )}
        </div>
      </div>
    </div>
    </>
  );
}

function formatDuration(ms) {
  const seconds = Math.floor(ms / 1000);
  const minutes = Math.floor(seconds / 60);
  const hours = Math.floor(minutes / 60);
  
  if (hours > 0) return `${hours}h ${minutes % 60}m ${seconds % 60}s`;
  if (minutes > 0) return `${minutes}m ${seconds % 60}s`;
  return `${seconds}s`;
}

function getCategoryLabel(category) {
  const labels = {
    // Sucesso
    'sucesso': '✅ Sucesso',
    
    // Arquivos e testes não encontrados/não executados
    'arquivo_nao_encontrado': '📄 Arquivo Não Encontrado',
    'testes_nao_executados': '⏸️ Testes Não Executados',
    'problema_execucao': '⚠️ Problema na Execução',
    
    // Erros do sistema
    'erro_sistema': '🚨 Erro do Sistema',
    'sem_testes_disponiveis': '🚨 Sem Testes Disponíveis',
    'falha_descoberta_testes': '🚨 Falha ao Descobrir Testes',
    'host_indisponivel': '🚨 Host Indisponível',
    
    // Falhas na execução dos testes
    'teste_falhou': '❌ Teste Falhou',
    'timeout': '⏱️ Timeout',
    'assertion_falhou': '❌ Assertion Falhou',
    'elemento_nao_encontrado': '🔍 Elemento Não Encontrado',
    'erro_conexao': '🌐 Erro de Conexão',
    'erro_null_reference': '💥 Erro Null Reference',
    'erro_chrome': '🌐 Erro Chrome/ChromeDriver',
    
    // Categorias antigas (mantidas para compatibilidade)
    'padrao_errado': '📄 Problema com Filtro',
    'padrao_errado_arquivo_nao_encontrado': '📄 Arquivo Não Encontrado',
    'padrao_errado_teste_nao_executado': '⏸️ Teste Não Executado',
    'erro_critico': '🚨 Erro Crítico',
    'erro_critico_sem_testes': '🚨 Sem Testes',
    'erro_critico_descoberta_falhou': '🚨 Descoberta Falhou',
    'erro_critico_host_indisponivel': '🚨 Host Indisponível',
    'erro_execucao': '❌ Erro na Execução',
    'erro_execucao_timeout': '⏱️ Timeout',
    'erro_execucao_assertion': '❌ Assertion Falhou',
    'erro_execucao_elemento_nao_encontrado': '🔍 Elemento Não Encontrado',
    'erro_execucao_conexao': '🌐 Erro de Conexão',
    'erro_execucao_null_reference': '💥 Null Reference',
    'erro_execucao_chrome': '🌐 Erro Chrome',
    
    // Desconhecido
    'erro_desconhecido': '❓ Erro Desconhecido'
  };
  return labels[category] || category;
}

function getCategoryIcon(category) {
  const icons = {
    // Sucesso
    'sucesso': '✅',
    
    // Arquivos e testes não encontrados/não executados
    'arquivo_nao_encontrado': '📄',
    'testes_nao_executados': '⏸️',
    'problema_execucao': '⚠️',
    
    // Erros do sistema
    'erro_sistema': '🚨',
    'sem_testes_disponiveis': '🚨',
    'falha_descoberta_testes': '🚨',
    'host_indisponivel': '🚨',
    
    // Falhas na execução dos testes
    'teste_falhou': '❌',
    'timeout': '⏱️',
    'assertion_falhou': '❌',
    'elemento_nao_encontrado': '🔍',
    'erro_conexao': '🌐',
    'erro_null_reference': '💥',
    'erro_chrome': '🌐',
    
    // Categorias antigas (mantidas para compatibilidade)
    'padrao_errado': '📄',
    'padrao_errado_arquivo_nao_encontrado': '📄',
    'padrao_errado_teste_nao_executado': '⏸️',
    'erro_critico': '🚨',
    'erro_critico_sem_testes': '🚨',
    'erro_critico_descoberta_falhou': '🚨',
    'erro_critico_host_indisponivel': '🚨',
    'erro_execucao': '❌',
    'erro_execucao_timeout': '⏱️',
    'erro_execucao_assertion': '❌',
    'erro_execucao_elemento_nao_encontrado': '🔍',
    'erro_execucao_conexao': '🌐',
    'erro_execucao_null_reference': '💥',
    'erro_execucao_chrome': '🌐',
    
    // Desconhecido
    'erro_desconhecido': '❓'
  };
  return icons[category] || '❓';
}

// Componente para exibir um cenário individual
function ScenarioCard({ scenario, index, filter }) {
  const [expanded, setExpanded] = React.useState(scenario.status === 'failed');
  const statusIcon = scenario.status === 'passed' ? '✅' : scenario.status === 'failed' ? '❌' : '⏸️';
  const statusText = scenario.status === 'passed' ? 'APROVADO' : scenario.status === 'failed' ? 'FALHOU' : 'NÃO EXECUTADO';
  
  // Verifica se o cenário corresponde ao filtro
  const matchesFilter = !filter || 
    scenario.name.toLowerCase().includes(filter.toLowerCase()) ||
    (scenario.errorMessage && scenario.errorMessage.toLowerCase().includes(filter.toLowerCase())) ||
    (scenario.stackTrace && scenario.stackTrace.toLowerCase().includes(filter.toLowerCase())) ||
    (scenario.bddSteps && scenario.bddSteps.some(step => step.fullLine.toLowerCase().includes(filter.toLowerCase())));
  
  if (!matchesFilter && filter) {
    return null;
  }
  
  return (
    <div className={`scenario-card scenario-${scenario.status}`}>
      <div 
        className="scenario-header" 
        onClick={() => setExpanded(!expanded)}
        style={{ cursor: 'pointer' }}
      >
        <span className="scenario-status-icon">{statusIcon}</span>
        <span className="scenario-name">{scenario.name}</span>
        <span className={`scenario-status-badge scenario-status-${scenario.status}`}>
          {statusText}
        </span>
        {scenario.duration && (
          <span className="scenario-duration">({scenario.duration}ms)</span>
        )}
        <span className="scenario-toggle">{expanded ? '▼' : '▶'}</span>
      </div>
      
      {expanded && (
        <div className="scenario-details">
          {/* BDD Steps */}
          {scenario.bddSteps && scenario.bddSteps.length > 0 && (
            <div className="scenario-section">
              <h5>📝 Passos BDD Executados ({scenario.bddSteps.length}):</h5>
              {scenario.failedStep && (
                <div className="failed-step-indicator">
                  🎯 Step que falhou: <strong>{scenario.failedStep.translated || scenario.failedStep.fullLine || `${scenario.failedStep.keyword} ${scenario.failedStep.text}`}</strong>
                </div>
              )}
              <div className="bdd-steps">
                {scenario.bddSteps.map((step, stepIndex) => {
                  const stepStatusIcon = step.status === 'passed' ? '✅' : 
                                        step.status === 'failed' ? '❌' : 
                                        step.status === 'skipped' ? '⏭️' : '⚪';
                  const isFailedStep = step.status === 'failed' || (scenario.failedStep && scenario.failedStep === step);
                  
                  return (
                    <div 
                      key={stepIndex} 
                      className={`bdd-step bdd-step-${step.status || (step.result.includes('error') ? 'error' : step.result.includes('skipped') ? 'skipped' : 'success')} ${isFailedStep ? 'failed-step-highlight' : ''}`}
                    >
                      <span className="bdd-step-icon">{stepStatusIcon}</span>
                      <span className="bdd-step-text">{step.translated || step.fullLine || `${step.keyword} ${step.text}`}</span>
                      {step.result && (
                        <span className={`bdd-step-result result-${step.status || 'unknown'}`}>
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
            </div>
          )}
          
          {/* Informações de erro detalhadas */}
          {scenario.errorDetails && (
            <div className="scenario-section">
              <h5>🔍 Tipo de Erro:</h5>
              <div className="error-type-info">
                <div className="error-type-badge">{scenario.errorDetails.type || 'unknown'}</div>
                {scenario.errorDetails.suggestion && (
                  <div className="error-suggestion">
                    💡 <strong>Sugestão:</strong> {scenario.errorDetails.suggestion}
                  </div>
                )}
                {scenario.errorDetails.type === 'timeout' && scenario.errorDetails.locator && (
                  <div className="error-timeout-details">
                    <div><strong>Elemento:</strong> {scenario.errorDetails.locator}</div>
                    {scenario.errorDetails.expectedText && (
                      <div><strong>Texto esperado:</strong> "{scenario.errorDetails.expectedText}"</div>
                    )}
                    <div><strong>Timeout:</strong> {scenario.errorDetails.timeoutValue}{scenario.errorDetails.timeoutUnit}</div>
                  </div>
                )}
              </div>
            </div>
          )}
          
          {/* Mensagem de Erro */}
          {scenario.errorMessage && (
            <div className="scenario-section">
              <h5>💥 Mensagem de Erro:</h5>
              <pre className="error-message">{scenario.errorMessage}</pre>
            </div>
          )}
          
          {/* Stack Trace (colapsável) */}
          {scenario.stackTrace && (
            <details className="scenario-section stack-trace-details">
              <summary className="stack-trace-summary">📍 Rastreamento de Pilha (clique para expandir)</summary>
              <pre className="stack-trace">{scenario.stackTrace}</pre>
            </details>
          )}
          
          {/* Output Padrão (se houver mais informações) */}
          {scenario.standardOutput && scenario.standardOutput.trim() && (
            <div className="scenario-section">
              <h5>📋 Saída Padrão:</h5>
              <pre className="standard-output">{scenario.standardOutput}</pre>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

export default LogViewer;

