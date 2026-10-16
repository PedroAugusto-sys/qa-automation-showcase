using System.Threading.Tasks;
using QaShowcase.Specflow.IntegrationTests.PageObjects;
using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.OutroProfissionais;

[Binding]
public class OutrosProfissionaisStepDefinitions : BaseStep
{
	private OutrosProfissionaisPO? _profissionalPage;

	private readonly ScenarioContext _scenarioContext;

	// Injeção do ScenarioContext via construtor
	public OutrosProfissionaisStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext; // Inicialize o ScenarioContext
	}

	[Given("que desejo cadastrar um profissional que nao seja um professor")]
	public async Task GivenQueDesejoCadastrarUmProfissionalQueNaoSejaUmProfessor()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo alterar o nome de um profissional que nao seja um professor")]
	public async Task GivenQueDesejoEditarONomeProfissionalQueNaoSejaUmProfessor()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo alterar a situacao de um profissional")]
	public async Task GivenQueDesejoAlterarSituacaoProfissionalQueNaoSejaUmProfessor()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo excluir um profissional")]
	public async Task GivenQueDesejoExcluirUmProfissional()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo localizar um profissional")]
	public async Task GivenQueDesejoLocalizarUmProfissional()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "OutrosProfissionais")]
	public async Task GivenAcessoOSistemaUtilizandoOOperador(string nomeOperador)
	{
		//this.nomeOperador = nomeOperador;
		await RealizeLogin(nomeOperador, "");

		// Armazena o nomeOperador no ScenarioContext
		_scenarioContext["NomeOperador"] = nomeOperador;
		_profissionalPage = await POFactory.Crie<OutrosProfissionaisPO>(Context);

	}


	[Given("informo o nome '([^']*)'"), Scope(Tag = "OutrosProfissionais")]
	public async Task GivenInformoONome(string nomeProfissional)
	{
		await _profissionalPage!.GotoAsync();
		await _profissionalPage!.ClicoEmCadastrar();
		await _profissionalPage!.InformoONomeDoProfissional(nomeProfissional);
	}

	[Given(@"informo a data de nascimento")]
	public async Task GivenSelecionoADataDeNascimento()
	{
		await _profissionalPage!.SelecionoADataDeNascimento();
	}

	[Given(@"informo CPF do profissional")]
	public async Task GivenInformoCPFDoProfissional()
	{
		await _profissionalPage!.InformoCPFDoProfissional();
	}

	[Given(@"informo o endereco")]
	public async Task GivenInformoOEnderecoDoProfissional()
	{
		await _profissionalPage!.InformoOEnderecoDoProfissional();
	}

	[Given("preencho os atributos adicionais obrigatorios")]
	public async Task GivenPreenchoOsAtributosAdicionaisObrigatorios()
	{
		await _profissionalPage!.InserirAtributoAdicional();
	}

	[Given("clico em '([^']*)'"), Scope(Tag = "OutrosProfissionais")]
	public async Task WhenClicoEmGiven(string opcao)
	{
		await _profissionalPage!.ClicoEmWhen(opcao);
	}

	[Given("informo a naturalidade")]
	public async Task GivenInformoANaturalidade()
	{
		await _profissionalPage!.SelecionoANaturalidade();
	}


	[Given("seleciono o sexo"), Scope(Tag = "OutrosProfissionais")]
	public async Task GivenSelecionoOSexo() => await _profissionalPage!.SelecionoOSexo();


	[Given("seleciono a etnia"), Scope(Tag = "OutrosProfissionais")]
	public async Task GivenSelecionoAEtnia()
	{
		await _profissionalPage!.AlteroAEtnia();
	}


	[Given("seleciono o tipo de profissional '([^']*)'")]
	public async Task GivenSelecionoOTipoDeProfissional(string tipoDeProfissional)
	{
		await _profissionalPage!.PreencherModalDescricao(tipoDeProfissional);
		await _profissionalPage!.ClicoEmGravarAEdicao();

	}

	[Given(@"que desejo editar o cadastro de um profissional no sistema")]
	public async Task GivenQueDesejoEditarOCadastroDeUmProfissionalNoSistema()
	{
		await _profissionalPage!.ClicoEmWhen("Sim");
	}

	[Given("altero a situacao para '([^']*)'")]
	public async Task GivenAlteroASituacaoPara(string situacao)
	{
		await _profissionalPage!.ClicoEmAlterarSituacao(situacao);
	}

	[Given("altero o nome do profissional para '([^']*)'")]
	public async Task GivenAlteroONomeDoProfissionalPara(string alterarNome)
	{
		_scenarioContext["NomeAlterado"] = alterarNome;
		await _profissionalPage!.AlterarNome(alterarNome);
	}

	[When("clico em '([^']*)'"), Scope(Tag = "OutrosProfissionais")]
	public async Task WhenClicoEmWhen(string opcao)
	{
		await _profissionalPage!.ClicoEmWhen(opcao);
	}

	[Then("e apresentada a mensagem '([^']*)'"), Scope(Tag = "OutrosProfissionais")]
	public async Task ThenEApresentadaAMensagem(string mensagemEsperada)
	{
		await Task.Delay(2000);

		if (_scenarioContext.ScenarioInfo.Tags.Contains("EditarNomeOutrosProfissionaisCadastrando"))
		{

			// Passa o contexto para validação do nome
			await _profissionalPage!.EApresentadaAMensagem(mensagemEsperada);
		}


		else if(_scenarioContext.ScenarioInfo.Tags.Contains("AlterarSituacaoOutrosProfissionaisCadastrando"))
		{

			// Passa o contexto para validação do nome
			await _profissionalPage!.EApresentadaAMensagem(mensagemEsperada);


		}
		else
		{
			// Para outros casos que não precisam de validação
			await _profissionalPage!.EApresentadaAMensagem(mensagemEsperada);
		}
	}

	[Given(@"informo a nacionalidade")]
	public async Task GivenAdicionoANacionalidade()
	{
		await _profissionalPage!.AlteroANacionalidade();
	}

	[Given(@"altero a Nacionalidade")]
	public async Task GivenAlteroANacionalidade()
	{
		await _profissionalPage!.AlteroAEtnia();
	}

	[Given("pesquiso pelo profissional '([^']*)'")]
	public async Task GivenPesquisoPeloProfissional(string nomeProfissional)
	{
		if (_scenarioContext.ScenarioInfo.Tags.Contains("ExcluirOutrosProfissionaisComVinculosCadastrando"))
		{
			await _profissionalPage!.GotoAsync();
			await _profissionalPage.CadastroCompletoDeProfissional(nomeProfissional);
			await _profissionalPage.RealizarLancamentosFinanceiros();
			await _profissionalPage.PesquisarProfissional(nomeProfissional);
			await _profissionalPage.alterarFiltroGridMaiorParaMenor();
		}
		else
		{
			await _profissionalPage!.GotoAsync();
			await _profissionalPage.CadastroCompletoDeProfissional(nomeProfissional);
			await _profissionalPage.PesquisarProfissional(nomeProfissional);

			await _profissionalPage.alterarFiltroGridMaiorParaMenor();
			
		}
	}

	[When("pesquiso pelo profissional '([^']*)'")]
	public async Task WhenPesquisoPeloProfissional(string nomeProfissional)
	{
			await _profissionalPage!.GotoAsync();
			await _profissionalPage.PesquisarProfissional(nomeProfissional);
		
	}

	[When("altero o filtro de situacao para '([^']*)'")]
	public async Task WhenAlteroOFiltroDeSituacaoPara(string opcao)
	{
		await _profissionalPage!.GotoAsync();

		await _profissionalPage.AlterarFiltroGrid(opcao);
	}

	[Then("e apresentada somente profissionais '([^']*)'")]
	public async Task ThenEApresentadaSomenteProfissionais(string opcao)
	{

		await Task.Delay(2000);
		await _profissionalPage!.ValidadorProfissionais(opcao);
	}

	[Then("e apresentada somente profissionais que contenham o texto informado na pesquisa")]
	public async Task ThenEApresentadaSomenteProfissionaisQueContenhamOTextoInformadoNaPesquisa()
	{
		await _profissionalPage!.ValidarInformacoesGridPesquisa();
	}

	[Then(@"o profissional e excluido com sucesso")]
	public async Task ThenOProfissionalEExcluidoComSucesso()
	{
		await _profissionalPage!.OProfissionalEExcluidoComSucesso();
	}

}

