using QaShowcase.Specflow.IntegrationTests.PageObjects;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.Cidades;

[Binding]
public class CidadesStepDefinitions : BaseStep
{
	private readonly ScenarioContext _scenarioContext;
	public CidadesStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext; // Inicialize o ScenarioContext
	}

	private CidadesPO _cidadesPage = null!;

	// GIVEN

	[Given("que desejo realizar o cadastro de cidades"), Scope(Tag = "@Cidades")]
	public async Task GivenQueDesejoRealizarOCadastroDeCidades()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	// AND

	[Given("seleciono o operador '([^']*)'"), Scope(Tag = "@Cidades")]
	public async Task GivenSelecionoOOperador(string nomeOperador)
	{
		await RealizeLogin(nomeOperador, "");
		_scenarioContext["NomeOperador"] = nomeOperador;
		// Voltar para a página de cadastro do aluno
		_cidadesPage = await POFactory.Crie<CidadesPO>(Context);
		await _cidadesPage.GotoAsync();
	}

	// Informar código IBGE
	[Given("informo o codigo do IBGE '([^']*)'"), Scope(Tag = "@Cidades")]
	public async Task GivenInformoOCodigoDoIBGE(string CodigoIBGE)
	{
		await _cidadesPage.InformoCodigoDoIBGE(CodigoIBGE);
	}

	// Validar se o nome da cidade e UF são apresentados
	[Given("noto que o nome da cidade '([^']*)' e UF '([^']*)' sao apresentados"), Scope(Tag = "@Cidades")]
	public async Task GivenNotoQueONomeDaCidadeEUFSaoApresentados(string NomeCidade, string UF)
	{
		await _cidadesPage.validarNomeDaCidade(NomeCidade, UF);
	}

	[Given("insiro o nome da cidade '([^']*)'"), Scope(Tag = "@Cidades")]
	public async Task GivenInsiroONomeDaCidade(string NomeCidade)
	{
		await _cidadesPage.InserirNomeCidade(NomeCidade);
	}

	[Given("seleciono a UF '([^']*)'"), Scope(Tag = "@Cidades")]
	public async Task GivenSelecionoAUF(string NomeUF)
	{
		await _cidadesPage.SelecionarUF(NomeUF);
	}


	// WHEN

	[When("clico em '([^']*)'"), Scope(Tag = "@Cidades")]
	public async Task WhenClicoEm(string Botao)
	{
		await _cidadesPage.ClicarEm(Botao);
	}

	// THEN



	[Then("e apresentada a mensagem '([^']*)'")]
	[Scope(Tag = "@Cidades")]
	public async Task ThenEApresentadaAMensagem(string CadastroSucesso)
	{
		await _cidadesPage.ValidarConfirmacaoDeCadastro(CadastroSucesso);
	}

	[Then(@"removo o operador"), Scope(Tag = "@Cidades")]
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
