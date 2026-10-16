import React, { useState, useEffect } from 'react';
import './Settings.css';

function Settings({ projectPath }) {
  const [baseTenant, setBaseTenant] = useState('');
  const [baseUrl, setBaseUrl] = useState('');
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });
  const [filePath, setFilePath] = useState('');

  useEffect(() => {
    if (projectPath) {
      loadContexto();
    }
  }, [projectPath]);

  const loadContexto = async () => {
    if (!window.electronAPI || !projectPath) return;
    
    setLoading(true);
    setMessage({ type: '', text: '' });
    
    try {
      const result = await window.electronAPI.readContextoFile(projectPath);
      if (result.success) {
        setBaseTenant(result.data.baseTenant || '');
        setBaseUrl(result.data.baseUrl || '');
        setFilePath(result.data.filePath || '');
      } else {
        setMessage({ type: 'error', text: result.error || 'Erro ao carregar arquivo Contexto.cs' });
      }
    } catch (error) {
      console.error('Erro ao carregar Contexto:', error);
      setMessage({ type: 'error', text: 'Erro ao carregar configurações do Contexto' });
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    if (!window.electronAPI || !projectPath) return;
    
    if (!baseTenant.trim()) {
      setMessage({ type: 'error', text: 'BaseTenant não pode estar vazio' });
      return;
    }
    
    setSaving(true);
    setMessage({ type: '', text: '' });
    
    try {
      const result = await window.electronAPI.writeContextoFile(projectPath, baseTenant, baseUrl);
      if (result.success) {
        setMessage({ type: 'success', text: 'Configurações salvas com sucesso!' });
        // Limpa a mensagem após 3 segundos
        setTimeout(() => setMessage({ type: '', text: '' }), 3000);
      } else {
        setMessage({ type: 'error', text: result.error || 'Erro ao salvar configurações' });
      }
    } catch (error) {
      console.error('Erro ao salvar Contexto:', error);
      setMessage({ type: 'error', text: 'Erro ao salvar configurações' });
    } finally {
      setSaving(false);
    }
  };

  const handleOpenFile = async () => {
    if (!filePath || !window.electronAPI) return;
    
    try {
      await window.electronAPI.showItemInFolder(filePath);
    } catch (error) {
      console.error('Erro ao abrir arquivo:', error);
    }
  };

  if (!projectPath) {
    return (
      <div className="settings-container">
        <div className="empty-state">
          <h2>Projeto não configurado</h2>
          <p>Selecione um projeto para editar as configurações do Contexto.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="settings-container">
      <div className="settings-header">
        <h2>⚙️ Configurações do Contexto</h2>
        <p className="settings-subtitle">
          Edite as configurações do arquivo Contexto.cs
        </p>
      </div>

      {loading ? (
        <div className="settings-loading">
          <div className="loading-spinner"></div>
          <p>Carregando configurações...</p>
        </div>
      ) : (
        <div className="settings-content">
          {message.text && (
            <div className={`settings-message ${message.type}`}>
              <span className="message-icon">
                {message.type === 'success' ? '✅' : '❌'}
              </span>
              <span>{message.text}</span>
            </div>
          )}

          <div className="settings-form">
            <div className="settings-field">
              <label htmlFor="baseTenant">
                <span className="field-label">BaseTenant</span>
                <span className="field-description">
                  Nome do tenant usado nas automações
                </span>
              </label>
              <input
                id="baseTenant"
                type="text"
                value={baseTenant}
                onChange={(e) => setBaseTenant(e.target.value)}
                placeholder="Ex: ceprimavera"
                className="settings-input"
              />
            </div>

            <div className="settings-field">
              <label htmlFor="baseUrl">
                <span className="field-label">BaseUrl</span>
                <span className="field-description">
                  URL base do servidor (opcional)
                </span>
              </label>
              <input
                id="baseUrl"
                type="text"
                value={baseUrl}
                onChange={(e) => setBaseUrl(e.target.value)}
                placeholder="Ex: http://your-server"
                className="settings-input"
              />
            </div>

            {filePath && (
              <div className="settings-file-info">
                <span className="file-path-label">Arquivo:</span>
                <span className="file-path" title={filePath}>
                  {filePath.split('\\').pop() || filePath.split('/').pop()}
                </span>
                <button
                  onClick={handleOpenFile}
                  className="btn-open-file"
                  title="Abrir local do arquivo"
                >
                  📁 Abrir Local
                </button>
              </div>
            )}

            <div className="settings-actions">
              <button
                onClick={handleSave}
                disabled={saving || !baseTenant.trim()}
                className="btn-save"
              >
                {saving ? '💾 Salvando...' : '💾 Salvar Configurações'}
              </button>
              <button
                onClick={loadContexto}
                disabled={loading}
                className="btn-reload"
              >
                🔄 Recarregar
              </button>
            </div>
          </div>

          <div className="settings-info">
            <h3>ℹ️ Informações</h3>
            <ul>
              <li>As alterações são salvas diretamente no arquivo <code>Contexto.cs</code></li>
              <li>O arquivo está localizado em: <code>RAIZ_DO_PROJETO/QaShowcase.Specflow.IntegrationTests/PageObjects/Contexto.cs</code></li>
              <li>O sistema busca automaticamente a pasta <code>QaShowcase.Specflow.IntegrationTests</code> a partir da raiz do projeto</li>
              <li>A URL completa será: <code>{baseUrl ? `${baseUrl}/${baseTenant}` : `{BaseUrl}/${baseTenant}`}</code></li>
            </ul>
          </div>
        </div>
      )}
    </div>
  );
}

export default Settings;

