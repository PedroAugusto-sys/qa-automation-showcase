using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal static class POFactory
{
	public static async Task<T> CrieAsync<T>() where T : POBase
	{
		IPlaywright playwright = await Playwright.CreateAsync();
		IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
		T? page = (T?)Activator.CreateInstance(typeof(T), new[] { browser });
		return page!;
	}
	public static async Task<T> Crie<T>(IBrowserContext context) where T : POBase
	{
		// Sempre criar uma nova página para evitar conflitos de estado e navegação.
		// A reutilização do navegador/contexto (via BrowserManager) já proporciona
		// a maior economia de memória RAM. Reutilizar páginas pode causar problemas
		// como ERR_ABORTED quando múltiplos testes rodam em paralelo.
		IPage page = await context.NewPageAsync().ConfigureAwait(false);
		
		return ((T?)Activator.CreateInstance(typeof(T), new[] { page }))!;
	}
}
