using QaShowcase.Specflow.IntegrationTests.PageObjects;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.FichaAluno;

[Binding]
public class FichaAlunoStepDefinitions : BaseStep
{
	private FichaAlunoPO _fichaAlunoPage = null!;
	private readonly ScenarioContext _scenarioContext;

	public FichaAlunoStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext; // Inicialize o ScenarioContext
	}

	// DADO

	[Given(@"que preciso cadastrar um novo aluno"), Scope(Tag = "@FichaAluno")]
	public async Task GivenQuePrecisoCadastrarUmNovoAluno()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo excluir um aluno no sistema"), Scope(Tag = "@FichaAluno")]

	public async Task GivenQueDesejoExcluirUmAlunoNoSistema()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que preciso adiconar uma anexo a uma ficha do aluno"), Scope(Tag = "@FichaAluno")]
	public async Task QuePrecisoAdiconarUmaAnexoAUmaFichaDoAluno()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que preciso pesquisar um aluno"), Scope(Tag = "@FichaAluno")]
	public async Task QuePrecisoPesquisarUmAluno()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo editar o CEP de um aluno no sistema"), Scope(Tag = "@FichaAluno")]
	public async Task QueDesejoEditarOCEPDeUmAlunoNoSistema()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo editar um aluno"), Scope(Tag = "@FichaAluno")]
	public async Task QueDesejoEditarUmAluno()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que preciso excluir um aluno"), Scope(Tag = "@FichaAluno")]
	public async Task QuePrecisoExcluirUmAluno()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	// E

	[Given(@"acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task AcessoOSistemaUtilizandoOperador(string nomeOperador)
	{
		await RealizeLogin(nomeOperador, "");
		_scenarioContext["NomeOperador"] = nomeOperador;
		// Voltar para a página de cadastro do aluno
		_fichaAlunoPage = await POFactory.Crie<FichaAlunoPO>(Context);
		await _fichaAlunoPage.GotoAsync();
	}

	[Given(@"clico em '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenClicoEmCadastrarAsync(string botao)
	{
		await _fichaAlunoPage.ClicarEm(botao);
	}

	[Given("digito em '([^']*)' o nome do aluno '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenDigitoEmONomeDoAluno(string NomeCampoTexto, string NomeAluno)
	{
		await _fichaAlunoPage.PesquisarAlunoPorNome(NomeCampoTexto, NomeAluno);
	}

	[Given("pesquiso pelo aluno '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenPesquisoPeloAluno(string NomeAluno)
	{
		await _fichaAlunoPage.PesquisarAlunoPorID(NomeAluno);
	}

	[Given("altero o nome do aluno para '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenAlteroONomeDoAlunoPara(string NomeAluno)
	{
		await _fichaAlunoPage.AlterarONomeDoAluno(NomeAluno);
	}

	[Given(@"preecho o Nome civil '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenPreechoONomeCivil(string NomeCivil)
	{
		await _fichaAlunoPage.PreechoONomeCivil(NomeCivil);
	}

	[Given(@"seleciono o sexo '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenSelecionoOSexo(string Sexo) => await _fichaAlunoPage.SelecionoOSexo(Sexo);

	[Given(@"insiro o nascimento '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenInsiroONascimento(string Nascimento) => await _fichaAlunoPage.InsiroONascimento(Nascimento);

	[Given(@"seleciono a naturalidade '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenSelecionoANaturalidade(string Naturalidade)
	{
		await _fichaAlunoPage.SelecionoANaturalidade(Naturalidade);
	}

	[Given(@"preencho o CEP '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenPreenchoOCEP(string CEP) => await _fichaAlunoPage.PreenchoOCEP(CEP);

	[Given(@"preencho o CPF '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenPreenchoOCPF(string CPF) => await _fichaAlunoPage.InserirNovoCPF(CPF);

	[Given(@"preencho os atributos adicionais obrigatorios"), Scope(Tag = "@FichaAluno")]
	public async Task GivenPreenchoOsAtributosAdicionaisObrigatorios()
	{
		await _fichaAlunoPage!.InserirAtributoAdicional();
	}

	[Given("clico em anexo na ficha do aluno '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenClicoEmAnexoNaFichaDoAluno(string NomeAluno)
	{
		await _fichaAlunoPage.PesquisarAlunoPorID(NomeAluno);
		await _fichaAlunoPage.ClicarEmAnexo(NomeAluno);
	}

	[Given("insiro o titulo '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenInsiroOTitulo(string TituloAnexo)
	{
		await _fichaAlunoPage.InserirTituloAnexo(TituloAnexo);
	}

	[Given("seleciono um anexo"), Scope(Tag = "@FichaAluno")]
	public async Task GivenSelecionoUmAnexo()
	{
		await _fichaAlunoPage.SelecionarAnexo();
	}

	[Given("pesquiso por responsavel o aluno '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenPesquisoPorResponsaveloAluno(string NomeAluno)
	{
		await _fichaAlunoPage.CadastrarAlunoComResponsavel(NomeAluno);
	}

	[Given("removo os dados existente para o '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task GivenRemovoOsDadosExistenteParaO(string RemoverCEP)
	{
		await _fichaAlunoPage.RemoverCEP(RemoverCEP);
	}

	// QUANDO

	[When("pesquiso o ID do aluno '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task WhenPesquisoOIDDoAluno(string NomeAluno)
	{
		await _fichaAlunoPage.PesquisarAlunoPorID(NomeAluno);
	}

	[When("pesquiso pelo aluno alterado"), Scope(Tag = "@FichaAluno")]
	public async Task WhenPesquisoPeloAlunoAlterado()
	{
		await _fichaAlunoPage.PesquisarAlunoAlterado();
	}

	[When(@"clico em '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task WhenClicoEmCadastrarAsync(string botao)
	{
		await _fichaAlunoPage.ClicarEm(botao);
	}

	[When("clico em '([^']*)' para confirmar a operacao"), Scope(Tag = "@FichaAluno")]
	public async Task WhenClicoEmParaConfirmarAOperacao(string opcao)
	{
		await _fichaAlunoPage.ClicarEm(opcao);
	}

	[When("pressiono a tecla '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task WhenPressionoATecla(string Enter)
	{
		await _fichaAlunoPage.ClicarEm(Enter);
	}

	// ENTÃO

	[Then(@"e apresentada a mensagem '([^']*)'"), Scope(Tag = "@FichaAluno")]
	public async Task ThenEApresentadaAMensagem(string mensagem)
	{
		await _fichaAlunoPage.ValidarMensagem(mensagem);
	}

	[Then("o aluno e apresentado na grid"), Scope(Tag = "@FichaAluno")]
	public async Task ThenOAlunoEApresentadoNaGrid()
	{
		await _fichaAlunoPage.ValidarAlunoNaGrid();
	}

	[Then("e apresentada a tela de aterrissagem"), Scope(Tag = "@FichaAluno")]
	public async Task ThenEApresentadaATelaDeAterrizagem()
	{
		await _fichaAlunoPage.ValidarTelaDeAterrissagem();
	}

	[Then("os campos obrigatorios pendentes do CEP sao destacadados"), Scope(Tag = "@FichaAluno")]
	public async Task ThenOsCamposObrigatoriosPendentesDoSaoDestacadados()
	{
		await _fichaAlunoPage.ValidarCamposDestacados();
	}

	[Then("o anexo e apresentado na grid de anexos"), Scope(Tag = "@FichaAluno")]
	public async Task ThenOAnexoEApresentadoNaGridDeAnexos()
	{
		await _fichaAlunoPage.ValidarAnexoInserido();
	}

	[Then("os campos obrigatorios pendentes do '([^']*)' sao destacadados"), Scope(Tag = "@FichaAluno")]
	public async Task ThenOsCamposObrigatoriosPendentesDoSaoDestacadados(string CEP)
	{
		await _fichaAlunoPage.ValidarCamposDestacados(CEP);
	}

	[Then("e apresentado somente alunos que contenham em seu nome o nome digitado"), Scope(Tag = "@FichaAluno")]
	public async Task ThenEApresentadoSomenteAlunosAQueContenhamEmSeuNomeONomeDigitado()
	{
		await _fichaAlunoPage.ValidarAlunosFiltrados();
	}

	[Then("e apresentado somente alunos que contenham nome do responsavel digitado, vinculado a sua ficha do aluno"), Scope(Tag = "@FichaAluno")]
	public async Task ThenEApresentadoSomenteAlunosQueContenhamNomeDoResponsavelDigitadoVinculadoASuaFichaDoAluno()
	{
		await _fichaAlunoPage.ValidarAlunoNaGrid();
	}

	[Then(@"removo o operador"), Scope(Tag = "@FichaAluno")]
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
