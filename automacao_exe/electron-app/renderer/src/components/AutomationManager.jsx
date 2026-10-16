import React, { useState, useEffect } from 'react';
import automationService from '../services/automation-service';
import './AutomationManager.css';

// Template generators
function generateFeatureTemplate(featureName, description = '') {
  return `Feature: ${featureName}

${description ? description : 'Descrição da funcionalidade'}

@${featureName.replace(/\s+/g, '')}
Scenario: ${featureName} - Cenário de teste
\tGiven que eu acesso o sistema
\tWhen eu realizo a ação
\tThen o resultado esperado é apresentado
`;
}

function generateStepDefinitionTemplate(className, namespace = 'AutomacaoHub.IntegrationTests.StepDefinitions') {
  return `using System.Threading.Tasks;
using TechTalk.SpecFlow;
using ${namespace.replace('StepDefinitions', 'PageObjects')};

namespace ${namespace};

[Binding]
public class ${className} : BaseStep
{
\tprivate readonly ScenarioContext _scenarioContext;

\tpublic ${className}(ScenarioContext scenarioContext)
\t{
\t\t_scenarioContext = scenarioContext;
\t}

\t[Given(@"que eu acesso o sistema")]
\tpublic async Task GivenQueEuAcessoOSistema()
\t{
\t\tawait CrieContexto();
\t\tawait RealizeLoginAdmin();
\t}

\t[When(@"eu realizo a ação")]
\tpublic async Task WhenEuRealizoAAcao()
\t{
\t\t// Implementar ação
\t}

\t[Then(@"o resultado esperado é apresentado")]
\tpublic async Task ThenOResultadoEsperadoEApresentado()
\t{
\t\t// Implementar validação
\t}
}
`;
}

function generatePageObjectTemplate(className, namespace = 'AutomacaoHub.IntegrationTests.PageObjects') {
  return `using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ${namespace};

internal class ${className} : POBase
{
\tpublic ${className}(IWebDriver driver) : base(driver)
\t{
\t}

\tpublic override string PageUrl => "";

\t// Exemplo de elementos
\tprivate IWebElement CampoExemplo => Driver.FindElement(By.Id("campo-exemplo"));
\tprivate IWebElement BotaoExemplo => Driver.FindElement(By.Id("botao-exemplo"));

\tpublic void PreencherCampo(string valor)
\t{
\t\tCampoExemplo.Clear();
\t\tCampoExemplo.SendKeys(valor);
\t}

\tpublic void ClicarBotao()
\t{
\t\tBotaoExemplo.Click();
\t}
}
`;
}

function AutomationManager({ projectPath }) {
  const [activeTab, setActiveTab] = useState('feature');
  const [formData, setFormData] = useState({
    featureName: '',
    featureDescription: '',
    stepClassName: '',
    stepNamespace: 'AutomacaoHub.IntegrationTests.StepDefinitions',
    poClassName: '',
    poNamespace: 'AutomacaoHub.IntegrationTests.PageObjects'
  });
  const [preview, setPreview] = useState('');
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    updatePreview();
  }, [activeTab, formData]);

  const updatePreview = () => {
    let previewContent = '';
    
    if (activeTab === 'feature') {
      previewContent = generateFeatureTemplate(
        formData.featureName || 'NomeDaFeature',
        formData.featureDescription || ''
      );
    } else if (activeTab === 'step') {
      previewContent = generateStepDefinitionTemplate(
        formData.stepClassName || 'NomeStepDefinition',
        formData.stepNamespace
      );
    } else if (activeTab === 'po') {
      previewContent = generatePageObjectTemplate(
        formData.poClassName || 'NomePageObject',
        formData.poNamespace
      );
    }
    
    setPreview(previewContent);
  };

  const handleCreate = async () => {
    if (!projectPath) {
      setMessage({ type: 'error', text: 'Projeto não configurado' });
      return;
    }

    try {
      if (activeTab === 'feature') {
        if (!formData.featureName) {
          setMessage({ type: 'error', text: 'Nome da feature é obrigatório' });
          return;
        }

        const content = generateFeatureTemplate(
          formData.featureName,
          formData.featureDescription
        );

        const result = await automationService.createFeature({
          projectPath,
          featureName: formData.featureName,
          content
        });

        if (result.success) {
          setMessage({ type: 'success', text: `Feature criada: ${result.path}` });
          setFormData(prev => ({ ...prev, featureName: '', featureDescription: '' }));
        } else {
          setMessage({ type: 'error', text: result.error });
        }
      } else if (activeTab === 'step') {
        if (!formData.stepClassName) {
          setMessage({ type: 'error', text: 'Nome da classe é obrigatório' });
          return;
        }

        const content = generateStepDefinitionTemplate(
          formData.stepClassName,
          formData.stepNamespace
        );

        const result = await automationService.createStepDefinition({
          projectPath,
          className: formData.stepClassName,
          namespace: formData.stepNamespace,
          content
        });

        if (result.success) {
          setMessage({ type: 'success', text: `Step Definition criada: ${result.path}` });
          setFormData(prev => ({ ...prev, stepClassName: '' }));
        } else {
          setMessage({ type: 'error', text: result.error });
        }
      } else if (activeTab === 'po') {
        if (!formData.poClassName) {
          setMessage({ type: 'error', text: 'Nome da classe é obrigatório' });
          return;
        }

        const content = generatePageObjectTemplate(
          formData.poClassName,
          formData.poNamespace
        );

        const result = await automationService.createPageObject({
          projectPath,
          className: formData.poClassName,
          namespace: formData.poNamespace,
          content
        });

        if (result.success) {
          setMessage({ type: 'success', text: `Page Object criado: ${result.path}` });
          setFormData(prev => ({ ...prev, poClassName: '' }));
        } else {
          setMessage({ type: 'error', text: result.error });
        }
      }
    } catch (error) {
      setMessage({ type: 'error', text: error.message });
    }
  };

  return (
    <div className="automation-manager">
      <div className="manager-header">
        <h2>Gerenciar Automações</h2>
        <p>Crie novas features, step definitions e page objects seguindo o padrão do projeto</p>
      </div>

      {message.text && (
        <div className={`message ${message.type}`}>
          {message.text}
          <button onClick={() => setMessage({ type: '', text: '' })}>✕</button>
        </div>
      )}

      <div className="manager-tabs">
        <button
          className={activeTab === 'feature' ? 'active' : ''}
          onClick={() => setActiveTab('feature')}
        >
          📄 Nova Feature
        </button>
        <button
          className={activeTab === 'step' ? 'active' : ''}
          onClick={() => setActiveTab('step')}
        >
          🔧 Step Definition
        </button>
        <button
          className={activeTab === 'po' ? 'active' : ''}
          onClick={() => setActiveTab('po')}
        >
          📦 Page Object
        </button>
      </div>

      <div className="manager-content">
        <div className="manager-form">
          {activeTab === 'feature' && (
            <>
              <div className="form-group">
                <label>Nome da Feature *</label>
                <input
                  type="text"
                  value={formData.featureName}
                  onChange={(e) => setFormData(prev => ({ ...prev, featureName: e.target.value }))}
                  placeholder="Ex: CadastrarUsuario"
                />
              </div>
              <div className="form-group">
                <label>Descrição</label>
                <textarea
                  value={formData.featureDescription}
                  onChange={(e) => setFormData(prev => ({ ...prev, featureDescription: e.target.value }))}
                  placeholder="Descrição da funcionalidade"
                  rows="3"
                />
              </div>
            </>
          )}

          {activeTab === 'step' && (
            <>
              <div className="form-group">
                <label>Nome da Classe *</label>
                <input
                  type="text"
                  value={formData.stepClassName}
                  onChange={(e) => setFormData(prev => ({ ...prev, stepClassName: e.target.value }))}
                  placeholder="Ex: CadastrarUsuarioStepDefinitions"
                />
              </div>
              <div className="form-group">
                <label>Namespace</label>
                <input
                  type="text"
                  value={formData.stepNamespace}
                  onChange={(e) => setFormData(prev => ({ ...prev, stepNamespace: e.target.value }))}
                  placeholder="AutomacaoHub.IntegrationTests.StepDefinitions"
                />
              </div>
            </>
          )}

          {activeTab === 'po' && (
            <>
              <div className="form-group">
                <label>Nome da Classe *</label>
                <input
                  type="text"
                  value={formData.poClassName}
                  onChange={(e) => setFormData(prev => ({ ...prev, poClassName: e.target.value }))}
                  placeholder="Ex: CadastrarUsuarioPO"
                />
              </div>
              <div className="form-group">
                <label>Namespace</label>
                <input
                  type="text"
                  value={formData.poNamespace}
                  onChange={(e) => setFormData(prev => ({ ...prev, poNamespace: e.target.value }))}
                  placeholder="AutomacaoHub.IntegrationTests.PageObjects"
                />
              </div>
            </>
          )}

          <button onClick={handleCreate} className="btn-create" disabled={!projectPath}>
            ✅ Criar {activeTab === 'feature' ? 'Feature' : activeTab === 'step' ? 'Step Definition' : 'Page Object'}
          </button>
        </div>

        <div className="manager-preview">
          <h3>Pré-visualização</h3>
          <pre className="preview-content">{preview}</pre>
        </div>
      </div>
    </div>
  );
}

export default AutomationManager;

