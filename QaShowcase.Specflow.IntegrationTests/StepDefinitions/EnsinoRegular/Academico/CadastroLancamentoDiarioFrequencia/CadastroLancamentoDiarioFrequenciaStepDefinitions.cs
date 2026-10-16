using QaShowcase.Specflow.IntegrationTests.PageObjects;
using NUnit.Framework;
using System.Threading.Tasks;


namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.CadastroLancamentoDiarioFrequencia;

[Binding]
public class CadastroLancamentoDiarioFrequenciaStepDefinitions : BaseStep
{
	private CadastroLancamentoDiarioFrequenciaPO _cadastroLancamentoDiarioFrequenciaPO = null;
	private readonly ScenarioContext _scenarioContext;

	public CadastroLancamentoDiarioFrequenciaStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
	}

	//Automação está mocada no ambiente de teste

	#region BLOCO GIVEN

	[Given(@"que preciso realizar o '([^']*)' de frequencia"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenQuePrecisoRealizarOLancamentoDeFrequencia(string opcao)
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que preciso realizar a '([^']*)' do lancamento de frequencia"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenQuePrecisoRealizarADoLancamentoDeFrequencia(string edicao)
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}



	[Given(@"acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenSelecionoOOperador(string nomeOperador)
	{
		await RealizeLogin(nomeOperador, "");
		_scenarioContext["NomeOperador"] = nomeOperador;
		_cadastroLancamentoDiarioFrequenciaPO = await POFactory.Crie<CadastroLancamentoDiarioFrequenciaPO>(Context);
		await _cadastroLancamentoDiarioFrequenciaPO.GotoAsync();
	}

	[Given(@"seleciono o ano letivo : '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenSelecionoOAnoLetivo(string anoLetivo)
	{
		await _cadastroLancamentoDiarioFrequenciaPO.SelecioneAnoOuMes("Ano",anoLetivo);
	}

	[Given(@"seleciono o mes : '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenSelecionoOMes(string mes)
	{
		await _cadastroLancamentoDiarioFrequenciaPO.SelecioneAnoOuMes("Mes", mes);
	}

	[Given(@"seleciono a Serie/Turma : '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenSelecionoASerieTurma(string serieTurma)
	{
		await _cadastroLancamentoDiarioFrequenciaPO.SelecioneSerieTurma(serieTurma);
	}

	[Given(@"clico em '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenClicoEm(string opcao)
	{
		await _cadastroLancamentoDiarioFrequenciaPO.ClicoEm(opcao);
	}

	[Given(@"seleciono o dia especifico : '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenSelecionoODiaEspecifico(string diaEspecifico)
	{
		await _cadastroLancamentoDiarioFrequenciaPO.SelecioneDiaEspecifico(diaEspecifico);
	}

	[Given(@"lanco presenca todos os alunos"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenLancoPresencaTodosOsAlunos()
	{
		await _cadastroLancamentoDiarioFrequenciaPO.LancePresencaParaTodosOsAlunos();
	}

	[Given(@"seleciono a disciplina : '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenSelecionoADisciplina(string disciplina)
	{
		await _cadastroLancamentoDiarioFrequenciaPO.SelecioneDisciplina(disciplina);
	}

	[Given(@"lanco falta para todos os alunos"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenLancoFaltaParaTodosOsAlunos()
	{
		await _cadastroLancamentoDiarioFrequenciaPO.LanceFaltaParaTodosOsAlunos();
	}

	[Given("o sistema apresenta a mensagem : '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task GivenOSistemaApresentaAMensagem(string mensagem)
	{
		await _cadastroLancamentoDiarioFrequenciaPO.ValideMensagem(mensagem);
	}

	#endregion


	#region  BLOCO WHEN

	[When("clico em '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task WhenClicoEM(string botao)
	{
		await _cadastroLancamentoDiarioFrequenciaPO.ClicoEm(botao);

	}
	#endregion


	#region BLOCO THEN
	[Then("o sistema apresenta a mensagem : '([^']*)'"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
	public async Task ThenOSistemaApresentaAMensagem(string mensagem)
	{
		await _cadastroLancamentoDiarioFrequenciaPO.ValideMensagem(mensagem);
	}


	[Then("removo o operador"), Scope(Tag = "@ConfiguracaoBoletim"), Scope(Tag = "@CadastroLancamentoDiarioFrequencia")]
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