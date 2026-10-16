using Microsoft.Playwright;
using QaShowcase.Specflow.IntegrationTests.PageObjects;
using NUnit.Framework;
using QaShowcase.Specflow.IntegrationTests.PageObjects.Acadêmico.BoletinsEFichaDeAproveitamento.AssociacaoDeSerieAConfiguracaoDoBoletim;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.BoletinsEFichaDeAproveitamento.AssociacaoDeSerieAConfiguracaoDoBoletim;

[Binding]
public class AssociaBoletimSerieStepDefinitions : BaseStep
{
	private AssociacaoBoletimSeriePO _associacaoConfiguracaoBoletimSerie = null!;
	private readonly ScenarioContext _scenarioContext;

	public AssociaBoletimSerieStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
	}

	#region GIVEN

	[Given("que desejo cadastrar uma nova associacao de configuracao com uma serie"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task GivenQueDesejoCadastrarUmaNovaAssociacaoDeConfiguracaoComUmaSerie()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	#endregion

	#region AND

	[Given(@"acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task AcessoOSistemaUtilizandoOperador(string nomeOperador)
	{
		await RealizeLogin(nomeOperador, "");
		_scenarioContext["NomeOperador"] = nomeOperador;
		_associacaoConfiguracaoBoletimSerie = await POFactory.Crie<AssociacaoBoletimSeriePO>(Context);
		await _associacaoConfiguracaoBoletimSerie.GotoAsync();
	}

	[Given(@"clico em '([^']*)'"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task GivenClicoEm(string Botao)
	{
		await _associacaoConfiguracaoBoletimSerie.ClicarEm(Botao);
	}

	[Given("seleciono o ano letivo '([^']*)'"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task GivenSelecionoOAnoLetivo(string AnoLetivo)
	{
		await _associacaoConfiguracaoBoletimSerie.SelecionarAnoLetivo(AnoLetivo);
	}

	[Given("seleciono a serie '([^']*)'"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task GivenSelecionoASerie(string Serie)
	{
		await _associacaoConfiguracaoBoletimSerie.SelecionarSerie(Serie);
	}

	[Given("seleciono a configuracao do boletim '([^']*)'"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task GivenSelecionoAConfiguracaoDoBoletim(string ConfiguracaoDoBoletim)
	{
		await _associacaoConfiguracaoBoletimSerie.SelecionarConfiguracaoDoBoletim(ConfiguracaoDoBoletim);
	}

	[Given("seleciono a progressao parcial como '([^']*)'"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task GivenSelecionoAProgressaoParcialComo(string ProgressaoParcial)
	{
		await _associacaoConfiguracaoBoletimSerie.SelecionarProgressaoParcial(ProgressaoParcial);
	}


	#endregion

	#region WHEN

	[When(@"clico em '([^']*)'"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task WhenClicoEm(string Botao)
	{
		await _associacaoConfiguracaoBoletimSerie.ClicarEm(Botao);
	}

	#endregion

	#region THEN

	[Then(@"e apresentada a mensagem '([^']*)'"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task ThenEApresentadoAMensagem(string Mensagem)
	{
		await _associacaoConfiguracaoBoletimSerie.ValidarMensagem(Mensagem);
	}

	[Then(@"removo o operador"), Scope(Tag = "@AssociacaoDeSerieAConfiguracaoDoBoletim")]
	public async Task ThenRemovoOOperador()
	{
		string nomeOperador = _scenarioContext["NomeOperador"] as string;
		Assert.NotNull(nomeOperador, "O nome do operador não foi encontrado no contexto.");
		await CrieContexto();
		await RealizeLoginAdmin();
		await RemoverOperador(nomeOperador);
	}

	#endregion
}
