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
  const pageObjectsNamespace = namespace.replace('StepDefinitions', 'PageObjects');
  return `using System.Threading.Tasks;
using TechTalk.SpecFlow;
using ${pageObjectsNamespace};

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

\tpublic async Task PreencherCampo(string valor)
\t{
\t\tawait Task.Run(() => {
\t\t\tCampoExemplo.Clear();
\t\t\tCampoExemplo.SendKeys(valor);
\t\t});
\t}

\tpublic async Task ClicarBotao()
\t{
\t\tawait Task.Run(() => {
\t\t\tBotaoExemplo.Click();
\t\t});
\t}
}
`;
}

function generateBaseStepTemplate(namespace = 'AutomacaoHub.IntegrationTests') {
  return `using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ${namespace}.PageObjects;

namespace ${namespace};

public class BaseStep
{
\tprotected IWebDriver Driver { get; private set; }
\tprotected IJavaScriptExecutor JavaScriptExecutor => (IJavaScriptExecutor)Driver;

\tprotected async Task CrieContexto()
\t{
\t\tawait Task.Run(() => {
\t\t\tvar options = new ChromeOptions();
\t\t\t// options.AddArgument("--headless"); // Descomentar para modo headless
\t\t\tDriver = new ChromeDriver(options);
\t\t\tDriver.Manage().Window.Maximize();
\t\t\tDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
\t\t});
\t}

\tprotected async Task RealizeLoginAdmin()
\t{
\t\t// Implementar login admin
\t\tawait Task.CompletedTask;
\t}

\tprotected async Task RealizeLogin(string usuario, string senha)
\t{
\t\t// Implementar login com usuário
\t\tawait Task.CompletedTask;
\t}

\tpublic void Dispose()
\t{
\t\tDriver?.Quit();
\t\tDriver?.Dispose();
\t}
}
`;
}

function generatePOBaseTemplate(namespace = 'AutomacaoHub.IntegrationTests.PageObjects') {
  return `using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ${namespace};

internal abstract class POBase
{
\tprotected IWebDriver Driver { get; }

\tpublic POBase(IWebDriver driver)
\t{
\t\tDriver = driver;
\t}

\tpublic abstract string PageUrl { get; }

\tpublic void Goto()
\t{
\t\tDriver.Navigate().GoToUrl($"{Contexto.Url}/{PageUrl}");
\t}

\tprotected WebDriverWait Wait(int seconds = 10)
\t{
\t\treturn new WebDriverWait(Driver, TimeSpan.FromSeconds(seconds));
\t}
}
`;
}

function generateContextoTemplate(namespace = 'AutomacaoHub.IntegrationTests.PageObjects') {
  return `namespace ${namespace};

internal static class Contexto
{
\tpublic static string BaseTenant { get; set; } = Environment.GetEnvironmentVariable("QA_TENANT") 
\t\t?? throw new InvalidOperationException("QA_TENANT environment variable is not set");
\tpublic static string BaseUrl { get; set; } = Environment.GetEnvironmentVariable("QA_BASE_URL") 
\t\t?? throw new InvalidOperationException("QA_BASE_URL environment variable is not set");
\tpublic static string Url => $"{BaseUrl}/{BaseTenant}";
}
`;
}

module.exports = {
  generateFeatureTemplate,
  generateStepDefinitionTemplate,
  generatePageObjectTemplate,
  generateBaseStepTemplate,
  generatePOBaseTemplate,
  generateContextoTemplate
};

