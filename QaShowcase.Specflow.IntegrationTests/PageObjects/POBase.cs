using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal abstract class POBase {
	public POBase(IPage page)
	{
		Page = page;
	}

	public abstract string PageUrl { get; }

	public IPage Page { get; }

	public async Task GotoAsync()
	{
		const int maxRetries = 3;
		const int baseDelayMs = 1000;
		var url = $"{Contexto.Url}/{PageUrl}";
		
		for (int attempt = 1; attempt <= maxRetries; attempt++)
		{
			try
			{
				await Page.GotoAsync(url, new PageGotoOptions
				{
					WaitUntil = WaitUntilState.Load,
					Timeout = 60000 
				});
				return; 
			}
			catch (PlaywrightException ex) when (attempt < maxRetries && IsRetryableError(ex))
			{
				var delayMs = baseDelayMs * attempt; // Backoff exponencial: 1s, 2s, 3s
				Console.WriteLine($"[WARN] Erro de navegação na tentativa {attempt}/{maxRetries}: {ex.Message}");
				Console.WriteLine($"[INFO] Tentando novamente em {delayMs}ms...");
				await Task.Delay(delayMs);
			}
		}
		
		// Se chegou aqui, todas as tentativas falharam
		// Fazer uma última tentativa sem catch para propagar o erro original
		await Page.GotoAsync(url, new PageGotoOptions
		{
			WaitUntil = WaitUntilState.Load,
			Timeout = 60000
		});
	}

	/// <summary>
	/// Verifica se o erro é do tipo que pode ser recuperado com retry.
	/// </summary>
	private static bool IsRetryableError(PlaywrightException ex)
	{
		var errorMessage = ex.Message.ToLowerInvariant();
		return errorMessage.Contains("err_internet_disconnected") ||
		       errorMessage.Contains("net::err_") ||
		       errorMessage.Contains("navigation timeout") ||
		       errorMessage.Contains("timeout") ||
		       errorMessage.Contains("network") ||
		       errorMessage.Contains("connection");
	}
}
