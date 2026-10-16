using System.Threading.Tasks;
using TechTalk.SpecFlow;
using AutomacaoHub.IntegrationTests.PageObjects;

namespace AutomacaoHub.IntegrationTests.StepDefinitions;

[Binding]
public class LoginStepDefinitions : BaseStep
{
    private readonly ScenarioContext _scenarioContext;
    private LoginPO? loginPage;

    public LoginStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"que eu acesso o sistema")]
    public async Task GivenQueEuAcessoOSistema()
    {
        await CrieContexto();
        loginPage = new LoginPO(Driver);
        await Task.CompletedTask;
    }

    [When(@"eu realizo o login com '([^']*)'")]
    public async Task WhenEuRealizoOLoginCom(string usuario)
    {
        // Implementar lógica de login
        await Task.CompletedTask;
    }

    [Then(@"o sistema apresenta a tela inicial")]
    public async Task ThenOSistemaApresentaATelaInicial()
    {
        // Implementar validação
        await Task.CompletedTask;
    }
}

