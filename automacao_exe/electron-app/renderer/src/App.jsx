import React, { useState, useEffect } from 'react';
import Dashboard from './components/Dashboard';
import AutomationRunner from './components/AutomationRunner';
import AutomationManager from './components/AutomationManager';
import LogViewer from './components/LogViewer';
import Settings from './components/Settings';
import './styles/App.css';

function App() {
  const [currentView, setCurrentView] = useState('dashboard');
  const [projectPath, setProjectPath] = useState('');
  const [config, setConfig] = useState(null);
  const [menuOpen, setMenuOpen] = useState(false);

  useEffect(() => {
    loadConfig();
  }, []);

  const handleViewChange = (view) => {
    setCurrentView(view);
    setMenuOpen(false); // Fecha o menu mobile ao selecionar uma opção
  };

  const loadConfig = async () => {
    if (window.electronAPI) {
      const result = await window.electronAPI.getConfig();
      setConfig(result);
      setProjectPath(result.projectPath || '');
    }
  };

  const handleSelectProject = async () => {
    if (window.electronAPI) {
      const result = await window.electronAPI.selectProjectPath();
      if (result.success && !result.canceled) {
        setProjectPath(result.path);
        await window.electronAPI.saveConfig({ projectPath: result.path });
      }
    }
  };

  return (
    <div className="app">
      <nav className="navbar">
        <div className="navbar-left">
          <div className="navbar-brand">
            <h1>🚀 Hub de Automações</h1>
          </div>
          <div className={`navbar-project ${menuOpen ? 'open' : ''}`}>
            {projectPath ? (
              <div className="project-path-container">
                <span className="project-path" title={projectPath}>
                  📁 {projectPath.split('\\').pop() || projectPath.split('/').pop()}
                </span>
                <button 
                  onClick={handleSelectProject} 
                  className="btn-edit-project"
                  title="Alterar caminho do projeto"
                >
                  ✏️
                </button>
              </div>
            ) : (
              <button onClick={handleSelectProject} className="btn-select-project">
                Selecionar Projeto
              </button>
            )}
          </div>
        </div>
        <button 
          className={`navbar-toggle ${menuOpen ? 'active' : ''}`}
          onClick={() => setMenuOpen(!menuOpen)}
          aria-label="Toggle menu"
        >
          <span></span>
          <span></span>
          <span></span>
        </button>
        <div className={`navbar-menu ${menuOpen ? 'open' : ''}`}>
          <button
            className={currentView === 'dashboard' ? 'active' : ''}
            onClick={() => handleViewChange('dashboard')}
          >
            Dashboard
          </button>
          <button
            className={currentView === 'runner' ? 'active' : ''}
            onClick={() => handleViewChange('runner')}
          >
            Executar
          </button>
          <button
            className={currentView === 'manager' ? 'active' : ''}
            onClick={() => handleViewChange('manager')}
          >
            Gerenciar
          </button>
          <button
            className={currentView === 'logs' ? 'active' : ''}
            onClick={() => handleViewChange('logs')}
          >
            Logs
          </button>
          <button
            className={currentView === 'settings' ? 'active' : ''}
            onClick={() => handleViewChange('settings')}
          >
            ⚙️ Configurações
          </button>
        </div>
        <div className="navbar-spacer"></div>
      </nav>

      <main className="main-content">
        {!projectPath && currentView !== 'manager' ? (
          <div className="empty-state">
            <h2>Projeto não configurado</h2>
            <p>Por favor, selecione o diretório do projeto de automação.</p>
            <button onClick={handleSelectProject} className="btn-primary">
              Selecionar Projeto
            </button>
          </div>
        ) : (
          <>
            {currentView === 'dashboard' && <Dashboard projectPath={projectPath} />}
            {currentView === 'runner' && <AutomationRunner projectPath={projectPath} />}
            {currentView === 'manager' && <AutomationManager projectPath={projectPath} />}
            {currentView === 'logs' && <LogViewer projectPath={projectPath} />}
            {currentView === 'settings' && <Settings projectPath={projectPath} />}
          </>
        )}
      </main>
    </div>
  );
}

export default App;

