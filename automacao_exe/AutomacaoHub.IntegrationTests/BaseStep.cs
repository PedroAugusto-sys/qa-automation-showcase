using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AutomacaoHub.IntegrationTests.PageObjects;

namespace AutomacaoHub.IntegrationTests;

public class BaseStep : IDisposable
{
    protected IWebDriver Driver { get; private set; }
    protected IJavaScriptExecutor JavaScriptExecutor => (IJavaScriptExecutor)Driver;

    protected async Task CrieContexto()
    {
        await Task.Run(() =>
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless"); // Descomentar para modo headless
            options.AddArgument("--start-maximized");
            
            // Verifica se há uma porta de debug configurada (para visualização na aplicação)
            var debugPort = Environment.GetEnvironmentVariable("CHROME_DEBUG_PORT");
            if (!string.IsNullOrEmpty(debugPort) && int.TryParse(debugPort, out int port))
            {
                // Configura o Chrome para usar remote-debugging-port
                // Isso permite que o Electron se conecte ao Chrome via Chrome DevTools Protocol
                options.AddArgument($"--remote-debugging-port={port}");
                // Desabilita algumas features que podem interferir
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddExcludedArgument("enable-automation");
            }
            
            Driver = new ChromeDriver(options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        });
    }

    protected async Task RealizeLoginAdmin()
    {
        // Implementar login admin conforme necessário
        await Task.CompletedTask;
    }

    protected async Task RealizeLogin(string usuario, string senha)
    {
        // Implementar login com usuário conforme necessário
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}

