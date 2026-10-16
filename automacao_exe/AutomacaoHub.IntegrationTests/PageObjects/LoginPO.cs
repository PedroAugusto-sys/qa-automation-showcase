using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace AutomacaoHub.IntegrationTests.PageObjects;

internal class LoginPO : POBase
{
    public LoginPO(IWebDriver driver) : base(driver)
    {
    }

    public override string PageUrl => "Login";

    private IWebElement CampoOperador => Driver.FindElement(By.Id("Operador"));
    private IWebElement CampoSenha => Driver.FindElement(By.Id("Senha"));
    private IWebElement BotaoEntrar => Driver.FindElement(By.Id("btnEntrar"));

    public void PreencherOperador(string operador)
    {
        CampoOperador.Clear();
        CampoOperador.SendKeys(operador);
    }

    public void PreencherSenha(string senha)
    {
        CampoSenha.Clear();
        CampoSenha.SendKeys(senha);
    }

    public void ClicarEntrar()
    {
        BotaoEntrar.Click();
    }

    public void RealizarLogin(string operador, string senha)
    {
        PreencherOperador(operador);
        PreencherSenha(senha);
        ClicarEntrar();
    }
}

