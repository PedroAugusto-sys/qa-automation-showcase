using Microsoft.Playwright;
using System.Collections.Concurrent;

namespace QaShowcase.Specflow.IntegrationTests;

/// <summary>
/// Gerenciador para criação e gerenciamento de navegadores e contextos.
/// Cria um navegador dedicado por cenário para garantir que cada navegador seja fechado após o cenário,
/// economizando memória RAM durante a execução dos testes.
/// </summary>
public static class BrowserManager
{
	private static IPlaywright? _playwright;
	private static readonly SemaphoreSlim _playwrightLock = new(1, 1);
	private static readonly ConcurrentDictionary<string, IBrowserContext> _contexts = new();
	private static readonly ConcurrentDictionary<string, IBrowser> _browsers = new();

	/// <summary>
	/// Obtém ou cria um contexto de navegador. Cria um navegador dedicado por contexto para garantir
	/// que o navegador possa ser fechado após cada cenário.
	/// </summary>
	/// <param name="contextId">ID único para o contexto (geralmente do ScenarioContext)</param>
	/// <param name="contextOptions">Opções opcionais para criação do contexto</param>
	/// <returns>IBrowserContext reutilizado ou recém-criado</returns>
	public static async Task<IBrowserContext> GetOrCreateContextAsync(string contextId, BrowserNewContextOptions? contextOptions = null)
	{
		// Verificar se o contexto já existe e está válido
		if (_contexts.TryGetValue(contextId, out var existingContext) && existingContext != null)
		{
			// Verificar se o contexto ainda está aberto
			try
			{
				// Tentar acessar uma propriedade para verificar se está válido
				var _ = existingContext.Pages;
				return existingContext;
			}
			catch
			{
				// Contexto foi fechado, remover do dicionário
				_contexts.TryRemove(contextId, out _);
				// Se o navegador associado também existir, tentar removê-lo
				if (_browsers.TryRemove(contextId, out var closedBrowser))
				{
					try
					{
						await closedBrowser.CloseAsync();
					}
					catch
					{
						// Ignorar erros
					}
				}
			}
		}

		// Inicializar Playwright se ainda não foi criado (thread-safe)
		if (_playwright == null)
		{
			await _playwrightLock.WaitAsync();
			try
			{
				if (_playwright == null)
				{
					_playwright = await Playwright.CreateAsync();
				}
			}
			finally
			{
				_playwrightLock.Release();
			}
		}

		// Criar um navegador dedicado para este contexto
		var browser = await _playwright!.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
		{ 
			Headless = false,
			// Argumentos do Chrome para reduzir consumo de memória
			Args = new[]
			{
				"--disable-dev-shm-usage",
				"--disable-gpu",
				"--no-sandbox",
				"--disable-setuid-sandbox",
				"--disable-extensions",
				"--disable-background-networking",
				"--disable-background-timer-throttling",
				"--disable-renderer-backgrounding",
				"--disable-backgrounding-occluded-windows",
				"--disable-ipc-flooding-protection",
				"--memory-pressure-off"
			}
		});

		// Armazenar o navegador associado ao contexto
		_browsers[contextId] = browser;

		// Criar novo contexto
		var newContext = await browser.NewContextAsync(contextOptions ?? new BrowserNewContextOptions());
		_contexts[contextId] = newContext;
		return newContext;
	}

	/// <summary>
	/// Fecha um contexto específico e o navegador associado para economizar memória RAM.
	/// Cada cenário tem seu próprio navegador que é fechado após a execução do cenário.
	/// </summary>
	/// <param name="contextId">ID do contexto a ser fechado</param>
	public static async Task CloseContextAsync(string contextId)
	{
		// Fechar o contexto
		if (_contexts.TryRemove(contextId, out var context))
		{
			try
			{
				await context.CloseAsync();
			}
			catch
			{
				// Ignorar erros ao fechar contexto já fechado
			}
		}

		// Fechar o navegador associado a este contexto
		if (_browsers.TryRemove(contextId, out var browser))
		{
			try
			{
				await browser.CloseAsync();
			}
			catch
			{
				// Ignorar erros ao fechar navegador
			}
		}
	}

	/// <summary>
	/// Fecha todos os contextos e navegadores restantes. Deve ser chamado após todos os testes.
	/// </summary>
	public static async Task CloseAllAsync()
	{
		// Fechar todos os contextos
		var closeContextTasks = _contexts.Values.Select(async context =>
		{
			try
			{
				await context.CloseAsync();
			}
			catch
			{
				// Ignorar erros ao fechar contextos
			}
		});

		// Fechar todos os navegadores
		var closeBrowserTasks = _browsers.Values.Select(async browser =>
		{
			try
			{
				await browser.CloseAsync();
			}
			catch
			{
				// Ignorar erros ao fechar navegadores
			}
		});

		await Task.WhenAll(closeContextTasks.Concat(closeBrowserTasks));
		_contexts.Clear();
		_browsers.Clear();

		// Dispose do Playwright
		await _playwrightLock.WaitAsync();
		try
		{
			if (_playwright != null)
			{
				_playwright.Dispose();
				_playwright = null;
			}
		}
		finally
		{
			_playwrightLock.Release();
		}
	}

	/// <summary>
	/// Obtém o navegador associado a um contexto específico (para casos especiais onde é necessário acesso direto).
	/// </summary>
	/// <param name="contextId">ID do contexto</param>
	/// <returns>IBrowser associado ao contexto ou null se não existir</returns>
	public static IBrowser? GetBrowser(string contextId)
	{
		_browsers.TryGetValue(contextId, out var browser);
		return browser;
	}
}

