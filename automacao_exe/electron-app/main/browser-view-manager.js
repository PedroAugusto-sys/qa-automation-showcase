const { BrowserView } = require('electron');
const net = require('net');

class BrowserViewManager {
  constructor() {
    this.views = new Map(); // Map<processId, BrowserView>
    this.debugPorts = new Map(); // Map<processId, port>
    this.mainWindow = null;
  }

  setMainWindow(window) {
    this.mainWindow = window;
  }

  /**
   * Cria um BrowserView para visualizar a automação
   * @param {string} processId - ID do processo de automação
   * @param {number} debugPort - Porta do Chrome DevTools Protocol
   */
  async createBrowserView(processId, debugPort) {
    if (!this.mainWindow) {
      console.error('[BrowserViewManager] Janela principal não definida');
      return false;
    }

    // Remove view anterior se existir
    if (this.views.has(processId)) {
      this.destroyBrowserView(processId);
    }

    try {
      // Aguarda o Chrome estar pronto na porta de debug
      await this.waitForChromeDebugPort(debugPort, 10000);

      // Cria o BrowserView
      const view = new BrowserView({
        webPreferences: {
          nodeIntegration: false,
          contextIsolation: true
        }
      });

      // Define o tamanho e posição do BrowserView
      // Vai ocupar a área dos logs (lado direito, sobrepondo os logs)
      const bounds = this.mainWindow.getBounds();
      const viewWidth = Math.floor(bounds.width * 0.48); // 48% da largura
      const viewHeight = bounds.height - 120; // Altura menos o header e margens
      const viewX = Math.floor(bounds.width * 0.52); // Começa após a metade (lado direito)
      const viewY = 80; // Abaixo do header

      view.setBounds({ x: viewX, y: viewY, width: viewWidth, height: viewHeight });
      
      // Carrega a página HTML que se conecta ao Chrome via CDP
      const htmlPath = require('path').join(__dirname, 'chrome-viewer.html');
      const viewerUrl = `file://${htmlPath}?port=${debugPort}`;
      
      await view.webContents.loadURL(viewerUrl).catch(err => {
        console.error(`[BrowserViewManager] Erro ao carregar página do visualizador:`, err);
        throw err;
      });

      // Armazena a view e a porta
      this.views.set(processId, view);
      this.debugPorts.set(processId, debugPort);

      // Adiciona a view à janela principal
      this.mainWindow.setBrowserView(view);

      // Atualiza a posição quando a janela for redimensionada
      this.mainWindow.on('resize', () => {
        this.updateBrowserViewBounds(processId);
      });

      console.log(`[BrowserViewManager] BrowserView criado para processo ${processId} na porta ${debugPort}`);
      return true;
    } catch (error) {
      console.error(`[BrowserViewManager] Erro ao criar BrowserView:`, error);
      return false;
    }
  }

  /**
   * Aguarda o Chrome estar disponível na porta de debug
   */
  async waitForChromeDebugPort(port, timeout = 10000) {
    return new Promise((resolve, reject) => {
      const startTime = Date.now();
      
      const checkPort = () => {
        const socket = new net.Socket();
        
        socket.setTimeout(1000);
        
        socket.on('connect', () => {
          socket.destroy();
          resolve();
        });
        
        socket.on('timeout', () => {
          socket.destroy();
          if (Date.now() - startTime > timeout) {
            reject(new Error(`Timeout aguardando Chrome na porta ${port}`));
          } else {
            setTimeout(checkPort, 500);
          }
        });
        
        socket.on('error', () => {
          socket.destroy();
          if (Date.now() - startTime > timeout) {
            reject(new Error(`Chrome não disponível na porta ${port}`));
          } else {
            setTimeout(checkPort, 500);
          }
        });
        
        socket.connect(port, 'localhost');
      };
      
      checkPort();
    });
  }

  /**
   * Atualiza os bounds do BrowserView quando a janela é redimensionada
   */
  updateBrowserViewBounds(processId) {
    if (!this.mainWindow || !this.views.has(processId)) {
      return;
    }

    const view = this.views.get(processId);
    const bounds = this.mainWindow.getBounds();
    const viewWidth = Math.floor(bounds.width * 0.48);
    const viewHeight = bounds.height - 120;
    const viewX = Math.floor(bounds.width * 0.52);
    const viewY = 80;

    view.setBounds({ x: viewX, y: viewY, width: viewWidth, height: viewHeight });
  }

  /**
   * Destroi um BrowserView
   */
  destroyBrowserView(processId) {
    if (!this.views.has(processId)) {
      return;
    }

    const view = this.views.get(processId);
    
    // Remove a view da janela
    if (this.mainWindow) {
      this.mainWindow.removeBrowserView(view);
    }
    
    // Destroi a view
    view.webContents.destroy();
    
    // Remove dos Maps
    this.views.delete(processId);
    this.debugPorts.delete(processId);

    console.log(`[BrowserViewManager] BrowserView destruído para processo ${processId}`);
  }

  /**
   * Destroi todos os BrowserViews
   */
  destroyAllBrowserViews() {
    const processIds = Array.from(this.views.keys());
    processIds.forEach(processId => {
      this.destroyBrowserView(processId);
    });
  }

  /**
   * Obtém informações sobre os BrowserViews ativos
   */
  getActiveBrowserViews() {
    return Array.from(this.views.keys()).map(processId => ({
      processId,
      debugPort: this.debugPorts.get(processId)
    }));
  }
}

module.exports = new BrowserViewManager();

