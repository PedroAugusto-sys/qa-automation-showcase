using TechTalk.SpecFlow;

namespace QaShowcase.Specflow.IntegrationTests;

/// <summary>
/// Hooks do SpecFlow para gerenciar o ciclo de vida dos navegadores.
/// Garante que os contextos sejam fechados após cada cenário e todos os navegadores após a execução completa.
/// </summary>
[Binding]
public class BrowserHooks
{
	private readonly ScenarioContext _scenarioContext;

	public BrowserHooks(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
	}

	/// <summary>
	/// Registra o ScenarioContext no BaseStep antes de cada cenário para uso automático.
	/// </summary>
	[BeforeScenario]
	public void BeforeScenario()
	{
		// Registra o ScenarioContext no dicionário estático do BaseStep
		// Isso permite que o BaseStep.CrieContexto() encontre automaticamente o ScenarioContext
		// mesmo quando os StepDefinitions não definem ScenarioContextInstance
		BaseStep.RegisterScenarioContext(_scenarioContext);
	}

	/// <summary>
	/// Fecha o contexto do navegador após cada cenário para liberar memória.
	/// </summary>
	[AfterScenario]
	public async Task AfterScenario()
	{
		// Fechar contexto se foi armazenado no ScenarioContext
		if (_scenarioContext.ContainsKey("BrowserContextId"))
		{
			var contextId = _scenarioContext["BrowserContextId"] as string;
			if (!string.IsNullOrEmpty(contextId))
			{
				await BrowserManager.CloseContextAsync(contextId);
				_scenarioContext.Remove("BrowserContextId");
			}
		}

		// Remove o ScenarioContext do dicionário estático após o término do cenário
		BaseStep.UnregisterScenarioContext(_scenarioContext);
	}

	/// <summary>
	/// Fecha todos os navegadores e contextos após a execução completa de todos os testes.
	/// </summary>
	[AfterTestRun]
	public static async Task AfterTestRun()
	{
		await BrowserManager.CloseAllAsync();
	}
}

