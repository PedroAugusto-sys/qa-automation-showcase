using System.Threading.Tasks;
using QaShowcase.Specflow.IntegrationTests;
using NUnit.Framework;

[Binding]
public class LogarOperadorStepDefinitions : BaseStep
{
	private readonly ScenarioContext _scenarioContext;
	private string? nomeOperador = null!;

	// Injeção do ScenarioContext via construtor
	public LogarOperadorStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
		// Definir ScenarioContext no BaseStep para permitir cleanup automático
		ScenarioContextInstance = scenarioContext;
	}

	[Given("que desejo logar como um usuario")]
	public async Task GivenQueDesejoLogarComoUmUsuario()
	{
		// SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("seleciono o operador '([^']*)'")]
	public async Task GivenSelecionoOOperador(string nomeOperador)
	{
		// Armazena o nomeOperador no ScenarioContext
		_scenarioContext["NomeOperador"] = nomeOperador;
		// Realiza o login
		this.nomeOperador = nomeOperador;
		await RealizeLogin(nomeOperador, "");
	}

	
	[Given("informo o operador '([^']*)'"), Scope(Tag = "LogarInfomandoOperador")]
	public async Task GivenInformoOOperador(string nomeOperador)
	{

		_scenarioContext["NomeOperador"] = nomeOperador;

		this.nomeOperador = nomeOperador;
		await RealizeLogin(nomeOperador,"");


	} 

	[When("clico em '([^']*)'"), Scope(Tag = "LogarOperador")]
	public async Task WhenClicoEm(string entrar)
	{

		await ClicarEntrar();
	}

	[Then("o sistema apresenta a tela inicial")]
	public async Task ThenOSistemaApresentaATelaInicial()
	{
		// Recupera o nomeOperador armazenado no ScenarioContext
		string nomeOperador = _scenarioContext["NomeOperador"] as string;

		// Verifica se o nomeOperador foi encontrado
		Assert.NotNull(nomeOperador, "O nome do operador não foi encontrado no contexto.");



		// Chama a função de validação da tela inicial
		await ConfirmarTela(nomeOperador);
	}

	[Then("removo o operador")]
	public async Task ThenRemovoOOperador()
	{
		// Recupera o nomeOperador armazenado no ScenarioContext
		string nomeOperador = _scenarioContext["NomeOperador"] as string;

		// Verifica se o nomeOperador foi encontrado
		Assert.NotNull(nomeOperador, "O nome do operador não foi encontrado no contexto.");

		// Realiza as operações de remoção
		await CrieContexto();
		await RealizeLoginAdmin();
		await RemoverOperador(nomeOperador);
	}
}
