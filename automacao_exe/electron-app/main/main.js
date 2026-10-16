const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const { spawn } = require('child_process');
const fs = require('fs');
const automationProcessManager = require('./automation-process-manager');
const ipcHandlers = require('./ipc-handlers');
const browserViewManager = require('./browser-view-manager');

// Define o nome da aplicação para notificações (deve ser feito ANTES de app.whenReady())
// No Windows, isso define o nome que aparece nas notificações
app.setName('Hub de Automação');

// Garante que o nome está correto (útil para debug)
console.log(`[main] Nome da aplicação definido: ${app.getName()}`);

// Define o ícone da aplicação (para a barra de tarefas no Windows)
const iconPath = path.join(__dirname, '../assets/a.png');
if (fs.existsSync(iconPath)) {
  app.dock?.setIcon?.(iconPath); // macOS
}

let mainWindow;

function createWindow() {
  // Caminho do ícone da aplicação
  const iconPath = path.join(__dirname, '../assets/a.png');
  const iconExists = fs.existsSync(iconPath);
  
  // Obtém as dimensões da tela para calcular tamanho inicial responsivo
  const { screen } = require('electron');
  const primaryDisplay = screen.getPrimaryDisplay();
  const { width: screenWidth, height: screenHeight } = primaryDisplay.workAreaSize;
  
  // Calcula tamanho inicial baseado na resolução da tela (80% da tela, mínimo 1024x768)
  const initialWidth = Math.max(1024, Math.min(1600, Math.floor(screenWidth * 0.8)));
  const initialHeight = Math.max(768, Math.min(1200, Math.floor(screenHeight * 0.8)));
  
  // Converte PNG para caminho absoluto para garantir que o ícone funcione
  const absoluteIconPath = iconExists ? path.resolve(iconPath) : undefined;
  
  // Log para debug do ícone
  if (iconExists) {
    console.log(`[main] Ícone encontrado: ${absoluteIconPath}`);
  } else {
    console.warn(`[main] Ícone não encontrado em: ${iconPath}`);
  }
  
  mainWindow = new BrowserWindow({
    width: initialWidth,
    height: initialHeight,
    minWidth: 800,
    minHeight: 600,
    maxWidth: screenWidth,
    maxHeight: screenHeight,
    icon: absoluteIconPath, // Usa caminho absoluto para garantir que funcione
    frame: true, // Mantém a frame para ter controle sobre o título
    resizable: true, // Permite redimensionar
    titleBarStyle: 'hidden', // Esconde a barra de título padrão (macOS)
    titleBarOverlay: process.platform === 'win32' ? {
      color: '#0f172a', // Mesma cor base do header (rgba(15, 23, 42, 0.85) sem transparência)
      symbolColor: '#ffffff',
      height: 40
    } : undefined, // Personaliza a barra de título no Windows
    autoHideMenuBar: true, // Esconde o menu bar automaticamente
    webPreferences: {
      preload: path.join(__dirname, '../preload/preload.js'),
      nodeIntegration: false,
      contextIsolation: true,
      enableRemoteModule: false
    },
  });
  
  // Centraliza a janela na tela
  mainWindow.center();
  
  // Força atualização do ícone após a janela ser criada (para resolver cache no modo dev)
  if (absoluteIconPath && process.platform === 'win32') {
    // No Windows, força atualização do ícone
    setTimeout(() => {
      try {
        mainWindow.setIcon(absoluteIconPath);
        console.log(`[main] Ícone atualizado forçadamente: ${absoluteIconPath}`);
      } catch (err) {
        console.error(`[main] Erro ao atualizar ícone:`, err);
      }
    }, 100);
  }
  
  // Remove o menu bar completamente
  mainWindow.setMenuBarVisibility(false);
  
  // Remove o menu completamente (File, Edit, View, Window, Help)
  mainWindow.setMenu(null);

  // Carrega a interface React
  if (process.env.NODE_ENV === 'development' || process.argv.includes('--dev')) {
    mainWindow.loadURL('http://localhost:3000');
    mainWindow.webContents.openDevTools();
  } else {
    // No build, usa app.getAppPath() que funciona dentro do app.asar
    const htmlPath = app.isPackaged
      ? path.join(app.getAppPath(), 'renderer', 'dist', 'index.html')
      : path.join(__dirname, '../renderer/dist/index.html');
    
    console.log('Carregando HTML de:', htmlPath);
    mainWindow.loadFile(htmlPath).catch(err => {
      console.error('Erro ao carregar arquivo:', err);
      mainWindow.webContents.openDevTools();
    });
    
    // Abre DevTools em caso de erro para debug
    mainWindow.webContents.on('did-fail-load', (event, errorCode, errorDescription, validatedURL) => {
      console.error('Erro ao carregar:', errorCode, errorDescription, validatedURL);
      mainWindow.webContents.openDevTools();
    });
  }

  mainWindow.on('closed', () => {
    browserViewManager.destroyAllBrowserViews();
    mainWindow = null;
  });

  // Configura o BrowserViewManager com a janela principal
  browserViewManager.setMainWindow(mainWindow);
}

app.whenReady().then(() => {
  createWindow();

  // Registra handlers IPC
  ipcHandlers.registerHandlers(ipcMain, mainWindow);

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });
});

app.on('window-all-closed', () => {
  // Encerra processos de automação pendentes
  automationProcessManager.cleanupAll();
  
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('before-quit', () => {
  automationProcessManager.cleanupAll();
  browserViewManager.destroyAllBrowserViews();
});

