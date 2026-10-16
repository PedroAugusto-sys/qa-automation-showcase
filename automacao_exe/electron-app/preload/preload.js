const { contextBridge, ipcRenderer } = require('electron');

// Expõe APIs seguras para o renderer process
contextBridge.exposeInMainWorld('electronAPI', {
  // Features
  scanFeatures: (projectPath) => ipcRenderer.invoke('scan-features', projectPath),
  
  // Execução
  runAutomation: (config) => ipcRenderer.invoke('run-automation', config),
  stopAutomation: (processId) => ipcRenderer.invoke('stop-automation', processId),
  stopAllAutomations: () => ipcRenderer.invoke('stop-all-automations'),
  getRunningAutomations: () => ipcRenderer.invoke('get-running-automations'),
  getQueueStatus: () => ipcRenderer.invoke('get-queue-status'),
  setMaxParallel: (max) => ipcRenderer.invoke('set-max-parallel', max),
  
  // Eventos
  onAutomationLog: (callback) => {
    // Remove listeners anteriores antes de adicionar novo para evitar duplicação
    ipcRenderer.removeAllListeners('automation-log');
    ipcRenderer.on('automation-log', (event, data) => callback(data));
  },
  onBddEvent: (callback) => {
    // Remove listeners anteriores antes de adicionar novo para evitar duplicação
    ipcRenderer.removeAllListeners('bdd-event');
    ipcRenderer.on('bdd-event', (event, data) => callback(data));
  },
  onAutomationComplete: (callback) => {
    // Remove listeners anteriores antes de adicionar novo para evitar duplicação
    ipcRenderer.removeAllListeners('automation-complete');
    ipcRenderer.on('automation-complete', (event, data) => callback(data));
  },
  removeAutomationLogListener: () => {
    ipcRenderer.removeAllListeners('automation-log');
  },
  removeBddEventListener: () => {
    ipcRenderer.removeAllListeners('bdd-event');
  },
  removeAutomationCompleteListener: () => {
    ipcRenderer.removeAllListeners('automation-complete');
  },
  
  // Gerenciamento de automações
  createFeature: (config) => ipcRenderer.invoke('create-feature', config),
  createStepDefinition: (config) => ipcRenderer.invoke('create-step-definition', config),
  createPageObject: (config) => ipcRenderer.invoke('create-page-object', config),
  
  // Configuração
  getConfig: () => ipcRenderer.invoke('get-config'),
  saveConfig: (config) => ipcRenderer.invoke('save-config', config),
  selectProjectPath: () => ipcRenderer.invoke('select-project-path'),
  
  // Exportação
  exportLogs: (config) => ipcRenderer.invoke('export-logs', config),
  showItemInFolder: (filePath) => ipcRenderer.invoke('show-item-in-folder', filePath),
  
  // Configurações do Contexto
  readContextoFile: (projectPath) => ipcRenderer.invoke('read-contexto-file', projectPath),
  writeContextoFile: (projectPath, baseTenant, baseUrl) => ipcRenderer.invoke('write-contexto-file', projectPath, baseTenant, baseUrl)
});

