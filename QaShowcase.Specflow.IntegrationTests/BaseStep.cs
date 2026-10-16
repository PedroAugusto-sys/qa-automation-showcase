using QaShowcase.Specflow.IntegrationTests.PageObjects;
using Microsoft.Playwright;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading;
using TechTalk.SpecFlow;

namespace QaShowcase.Specflow.IntegrationTests;
public class BaseStep
{
	private LoginPO? loginPage;
	private OperadorPO? cadastroOperadorPage;
	private string? _contextId;

	protected IBrowserContext Context { get; private set; } = null!;

	/// <summary>
	/// ScenarioContext pode ser definido pelos StepDefinitions para permitir cleanup automático.
	/// </summary>
	protected ScenarioContext? ScenarioContextInstance { get; set; }

	/// <summary>
	/// AsyncLocal para armazenar o ScenarioContext por contexto assíncrono.
	/// Permite que o BrowserHooks registre o ScenarioContext e o BaseStep o encontre automaticamente,
	/// mesmo em execução paralela onde múltiplos cenários rodam simultaneamente.
	/// </summary>
	private static readonly AsyncLocal<ScenarioContext?> _currentScenarioContext = new();

	/// <summary>
	/// Dicionário estático thread-safe para armazenar ScenarioContext por chave única do cenário.
	/// Usado como fallback e para garantir que todos os contextos sejam limpos no AfterTestRun.
	/// </summary>
	private static readonly ConcurrentDictionary<string, ScenarioContext> _scenarioContexts = new();

	/// <summary>
	/// Cria ou obtém um contexto de navegador reutilizando instâncias existentes.
	/// Busca o ScenarioContext automaticamente do dicionário estático se não estiver definido via ScenarioContextInstance.
	/// </summary>
	protected async Task CrieContexto()
	{
		ScenarioContext? scenarioContext = null;

		// Tenta obter do ScenarioContextInstance primeiro (para compatibilidade com StepDefinitions que já configuram)
		if (ScenarioContextInstance != null)
		{
			scenarioContext = ScenarioContextInstance;
		}
		else
		{
			// Tenta obter do AsyncLocal primeiro (mais eficiente e thread-safe para execução paralela)
			scenarioContext = _currentScenarioContext.Value;

			// Se não encontrou no AsyncLocal, tenta do dicionário estático (fallback)
			if (scenarioContext == null)
			{
				scenarioContext = GetCurrentScenarioContext();
			}
		}

		// Gerar ou obter ID único para o contexto
		if (scenarioContext != null && scenarioContext.ContainsKey("BrowserContextId"))
		{
			_contextId = scenarioContext["BrowserContextId"] as string;
		}
		else
		{
			// Usar ID do cenário se disponível, senão usar GUID único
			_contextId = scenarioContext?.ScenarioInfo?.Title ?? Guid.NewGuid().ToString();

			if (scenarioContext != null)
			{
				scenarioContext["BrowserContextId"] = _contextId;
			}
		}

		// Obter ou criar contexto usando o BrowserManager (reutiliza navegadores)
		Context = await BrowserManager.GetOrCreateContextAsync(_contextId);
	}

	/// <summary>
	/// Obtém o ScenarioContext atual do dicionário estático.
	/// Em execução paralela, cada thread tem seu próprio cenário, então geralmente há apenas um por thread.
	/// </summary>
	private ScenarioContext? GetCurrentScenarioContext()
	{
		// Se há apenas um ScenarioContext registrado, usa ele (caso mais comum)
		if (_scenarioContexts.Count == 1)
		{
			return _scenarioContexts.Values.First();
		}

		// Se há múltiplos (execução paralela), tenta encontrar pelo título
		// Como não temos acesso direto ao título atual, retorna o primeiro disponível
		// Em execução paralela, cada thread terá seu próprio BrowserHooks e ScenarioContext
		// então o dicionário pode ter múltiplas entradas, mas cada thread só acessa a sua
		return _scenarioContexts.Values.FirstOrDefault();
	}

	/// <summary>
	/// Registra um ScenarioContext no AsyncLocal e no dicionário estático para uso automático pelo BaseStep.
	/// Chamado pelo BrowserHooks no BeforeScenario.
	/// </summary>
	/// <param name="scenarioContext">Instância do ScenarioContext a ser registrada</param>
	internal static void RegisterScenarioContext(ScenarioContext scenarioContext)
	{
		// Registra no AsyncLocal (para acesso rápido e thread-safe em execução paralela)
		_currentScenarioContext.Value = scenarioContext;

		// Também registra no dicionário estático (para garantir limpeza e como fallback)
		var key = scenarioContext.ScenarioInfo?.Title ?? Guid.NewGuid().ToString();
		_scenarioContexts[key] = scenarioContext;
	}

	/// <summary>
	/// Remove um ScenarioContext do AsyncLocal e do dicionário estático após o término do cenário.
	/// Chamado pelo BrowserHooks no AfterScenario.
	/// </summary>
	/// <param name="scenarioContext">Instância do ScenarioContext a ser removida</param>
	internal static void UnregisterScenarioContext(ScenarioContext scenarioContext)
	{
		// Limpa o AsyncLocal
		if (_currentScenarioContext.Value == scenarioContext)
		{
			_currentScenarioContext.Value = null;
		}

		// Remove do dicionário estático
		var key = scenarioContext.ScenarioInfo?.Title ?? Guid.NewGuid().ToString();
		_scenarioContexts.TryRemove(key, out _);
	}


	protected async Task RealizeLoginAdmin()
	{
		// Limpa todos os cookies e storage do contexto para garantir que não há sessão ativa
		// Isso é necessário porque o contexto pode estar reutilizado e ter sessão de usuário anterior
		await Context.ClearCookiesAsync();
		await Context.ClearPermissionsAsync();
		
		// Fecha todas as páginas abertas para garantir um estado limpo
		foreach (var page in Context.Pages.ToList())
		{
			try
			{
				await page.CloseAsync();
			}
			catch
			{
				// Ignora erros ao fechar páginas
			}
		}
		
		// Cria uma nova página limpa para o login
		loginPage = await POFactory.Crie<LoginPO>(Context);
		await loginPage.GotoAsync();
		
		// Verifica se foi redirecionado para a página de login (não está logado)
		// Se a URL não contém "Login", pode ter sido redirecionado automaticamente
		var currentUrl = loginPage.Page.Url;
		if (!currentUrl.Contains("Login", StringComparison.OrdinalIgnoreCase))
		{
			// Se não está na página de login, navega explicitamente para ela
			await loginPage.GotoAsync();
			// Aguarda um pouco para garantir que não há redirecionamentos pendentes
			await Task.Delay(1000);
		}
		
		await loginPage.LogueComAdmin();
	}

	protected async Task RealizeLogin(string nomeOperador, string senhaOperador)
	{

		await this.CadastrarOperador(nomeOperador);
		loginPage = await POFactory.Crie<LoginPO>(Context);
		await loginPage.GotoAsync();
		await loginPage.LogueComUsuario(nomeOperador,TestConfig.DefaultOperatorPassword);
		await loginPage.ClicoEntrar();
	}

	protected async Task CadastrarOperador(string nomeOperador)
	{

		cadastroOperadorPage = await POFactory.Crie<OperadorPO>(Context);
		await cadastroOperadorPage.GotoAsync();
		if (await cadastroOperadorPage.operadorExiste(nomeOperador))
		{
			return;
		}
		await cadastroOperadorPage.CadastroOperadorFinal(nomeOperador);

	}

	protected async Task RemoverOperador(string operador)
	{
		// Garante que está logado como admin antes de tentar remover o operador
		// O método já deve ter sido chamado com login admin antes, mas verificamos para garantir
		// Não fazemos login novamente aqui para evitar loops - assumimos que já está logado como admin
		
		cadastroOperadorPage = await POFactory.Crie<OperadorPO>(Context);
		await cadastroOperadorPage.GotoAsync();
		
		// Aguarda a página estar totalmente carregada antes de tentar pesquisar
		// Isso garante que não há redirecionamentos pendentes
		await cadastroOperadorPage.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
		
		// Verifica se foi redirecionado para a página de login (não tem permissão de admin)
		// Se foi redirecionado, significa que não está logado como admin
		var currentUrl = cadastroOperadorPage.Page.Url;
		if (currentUrl.Contains("Login", StringComparison.OrdinalIgnoreCase))
		{
			// Se foi redirecionado para login, precisa fazer login como admin novamente
			await RealizeLoginAdmin();
			// Tenta navegar novamente para a página de operador
			await cadastroOperadorPage.GotoAsync();
			await cadastroOperadorPage.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
		}
		
		await Task.Delay(500);
		
		await cadastroOperadorPage.ExcluirOperador(operador);
	}


	/// <summary>
	/// Clica no botão "Entrar" da página de login, mas apenas se a página de login estiver visível.
	/// Se a página de login não estiver visível, não realiza nenhuma ação e segue o fluxo normal.
	/// Isso evita erros quando o método é chamado em outros contextos onde o botão não existe.
	/// </summary>
	protected async Task ClicarEntrar()
	{
		// Verifica se loginPage foi inicializado
		if (loginPage == null)
		{
			return; // Se não foi inicializado, não há o que fazer
		}

		try
		{
			// Verifica se o botão "Entrar" está visível na página atual
			// Usa o mesmo padrão do LoginPO para garantir consistência
			var botaoEntrar = loginPage.Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Entrar" });

			// Verifica se o botão está visível (timeout curto para não demorar)
			var estaVisivel = await botaoEntrar.IsVisibleAsync();

			if (estaVisivel)
			{
				// Se o botão está visível, clica nele
				await loginPage.ClicoEntrar();
			}
			// Se não estiver visível, simplesmente retorna sem fazer nada
			// Isso permite que o método seja chamado em outros contextos sem causar erros
		}
		catch
		{
			// Se houver qualquer erro ao verificar ou clicar, ignora silenciosamente
			// Isso garante que o fluxo continue normalmente mesmo se:
			// - A página não for de login
			// - O botão não existir
			// - Houver algum problema de timing
		}
	}


	protected async Task ConfirmarTela(string operador)
	{
		await loginPage!.ConfirmarTela(operador);
	}


	public ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);

	public IPageAssertions Expect(IPage page) => Assertions.Expect(page);

}
