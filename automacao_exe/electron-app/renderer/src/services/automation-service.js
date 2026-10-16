class AutomationService {
  constructor() {
    if (!window.electronAPI) {
      console.error('Electron API não disponível');
      return;
    }

    // Configura listeners de eventos
    this.setupEventListeners();
  }

  setupEventListeners() {
    // Remove listeners antigos se existirem
    window.electronAPI.removeAutomationLogListener();
    window.electronAPI.removeBddEventListener();
    window.electronAPI.removeAutomationCompleteListener();
  }

  async scanFeatures(projectPath) {
    try {
      const result = await window.electronAPI.scanFeatures(projectPath);
      return result;
    } catch (error) {
      console.error('Erro ao escanear features:', error);
      return { success: false, error: error.message };
    }
  }

  async runAutomation(config) {
    try {
      const result = await window.electronAPI.runAutomation(config);
      return result;
    } catch (error) {
      console.error('Erro ao executar automação:', error);
      return { success: false, error: error.message };
    }
  }

  async stopAutomation(processId) {
    try {
      const result = await window.electronAPI.stopAutomation(processId);
      return result;
    } catch (error) {
      console.error('Erro ao parar automação:', error);
      return { success: false, error: error.message };
    }
  }

  async stopAllAutomations() {
    try {
      const result = await window.electronAPI.stopAllAutomations();
      return result;
    } catch (error) {
      console.error('Erro ao parar todas as automações:', error);
      return { success: false, error: error.message };
    }
  }

  async getRunningAutomations() {
    try {
      const result = await window.electronAPI.getRunningAutomations();
      return result;
    } catch (error) {
      console.error('Erro ao obter automações em execução:', error);
      return { success: false, error: error.message };
    }
  }

  async getQueueStatus() {
    try {
      const result = await window.electronAPI.getQueueStatus();
      return result;
    } catch (error) {
      console.error('Erro ao obter status da fila:', error);
      return { success: false, error: error.message };
    }
  }

  async setMaxParallel(max) {
    try {
      const result = await window.electronAPI.setMaxParallel(max);
      return result;
    } catch (error) {
      console.error('Erro ao configurar limite de paralelismo:', error);
      return { success: false, error: error.message };
    }
  }

  async createFeature(config) {
    try {
      const result = await window.electronAPI.createFeature(config);
      return result;
    } catch (error) {
      console.error('Erro ao criar feature:', error);
      return { success: false, error: error.message };
    }
  }

  async createStepDefinition(config) {
    try {
      const result = await window.electronAPI.createStepDefinition(config);
      return result;
    } catch (error) {
      console.error('Erro ao criar step definition:', error);
      return { success: false, error: error.message };
    }
  }

  async createPageObject(config) {
    try {
      const result = await window.electronAPI.createPageObject(config);
      return result;
    } catch (error) {
      console.error('Erro ao criar page object:', error);
      return { success: false, error: error.message };
    }
  }

  async exportLogs(config) {
    try {
      const result = await window.electronAPI.exportLogs(config);
      return result;
    } catch (error) {
      console.error('Erro ao exportar logs:', error);
      return { success: false, error: error.message };
    }
  }

  onAutomationLog(callback) {
    if (window.electronAPI) {
      window.electronAPI.onAutomationLog(callback);
    }
  }

  onBddEvent(callback) {
    console.log(`[AutomationService] Configurando listener para eventos BDD`);
    if (window.electronAPI) {
      window.electronAPI.onBddEvent((data) => {
        console.log(`[AutomationService] Evento BDD recebido no service:`, data);
        callback(data);
      });
    } else {
      console.error(`[AutomationService] ❌ electronAPI não disponível para eventos BDD`);
    }
  }

  onAutomationComplete(callback) {
    if (window.electronAPI) {
      window.electronAPI.onAutomationComplete(callback);
    }
  }

  removeAutomationLogListener() {
    if (window.electronAPI && window.electronAPI.removeAutomationLogListener) {
      window.electronAPI.removeAutomationLogListener();
    }
  }

  removeBddEventListener() {
    if (window.electronAPI && window.electronAPI.removeBddEventListener) {
      window.electronAPI.removeBddEventListener();
    }
  }

  removeAutomationCompleteListener() {
    if (window.electronAPI && window.electronAPI.removeAutomationCompleteListener) {
      window.electronAPI.removeAutomationCompleteListener();
    }
  }

  async getConfig() {
    try {
      const result = await window.electronAPI.getConfig();
      return result;
    } catch (error) {
      console.error('Erro ao obter configuração:', error);
      return null;
    }
  }

  async saveConfig(config) {
    try {
      const result = await window.electronAPI.saveConfig(config);
      return result;
    } catch (error) {
      console.error('Erro ao salvar configuração:', error);
      return { success: false, error: error.message };
    }
  }
}

export default new AutomationService();

