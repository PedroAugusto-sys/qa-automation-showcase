using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AutomacaoHub.IntegrationTests.PageObjects;

internal abstract class POBase
{
    protected IWebDriver Driver { get; }

    public POBase(IWebDriver driver)
    {
        Driver = driver;
    }

    public abstract string PageUrl { get; }

    public void Goto()
    {
        Driver.Navigate().GoToUrl($"{Contexto.Url}/{PageUrl}");
    }

    protected WebDriverWait Wait(int seconds = 10)
    {
        return new WebDriverWait(Driver, TimeSpan.FromSeconds(seconds));
    }
}

