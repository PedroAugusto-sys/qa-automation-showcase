using QaShowcase.Specflow.IntegrationTests.PageObjects;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Logins;

[Binding]
public class OperadorStepDefinitions : BaseStep
{
	private OperadorPO _cadastroDeOperadorPage = null!;

	//GIVEN (DADO)

	[Given("que desejo cadastrar um operador")]
	public async Task GivenQueDesejoCadastrarUmOperador()
	{

		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
		_cadastroDeOperadorPage = await POFactory.Crie<OperadorPO>(Context);
		await _cadastroDeOperadorPage.GotoAsync();

	}

	[Given(@"que desejo editar Operador")]
	public async Task QueDesejoEditarOperador()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
		_cadastroDeOperadorPage = await POFactory.Crie<OperadorPO>(Context);
		await _cadastroDeOperadorPage.GotoAsync();

	}

	[Given("que desejo excluir usuario")]
	public async Task GivenQueDesejoExcluirUsuario()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
		_cadastroDeOperadorPage = await POFactory.Crie<OperadorPO>(Context);
		await _cadastroDeOperadorPage.GotoAsync();
	}

	

	[Given("que desejo pesquisar um operador")]
	public async Task GivenQueDesejoPesquisarUmOperador()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
		_cadastroDeOperadorPage = await POFactory.Crie<OperadorPO>(Context);
		await _cadastroDeOperadorPage.GotoAsync();

	}

	//GIVEN (E)

	//'Clico em' estático, ou seja só vai operar aquilo que foi definido no objeto pelo método <.ClicoEmCadastrar();>
	[Given("clico em cadastrar")]
	public async Task GivenClicoEmCadastrar()
	{
		await _cadastroDeOperadorPage.ClicoEmCadastrar();
	}

	//'Clico em' genérico que permite enviar qualquer string para o objeto (portanto tratamento deve ser feito no objeto)
	//Porém se observa que nesse método não esta se fazendo uso da string enviada, ou seja, esta operando de forma estática
	[Given("clico em '([^']*)'"), Scope(Tag = "Logins")]
	public async Task GivenClicoEm(string gravar)
	{
		await _cadastroDeOperadorPage.GravarOperador();
	}

	[Given("preencho o campo nome '([^']*)'")]
	public async Task GivenPreenchoOCampoNome(string nomeOperador)
	{
		await _cadastroDeOperadorPage.PreencherCampoNome(nomeOperador);
	}

	[Given("preencho o campo login '([^']*)'")]
	public async Task GivenPreenchoOCampoLogin(string nomeOperador)
	{
		await _cadastroDeOperadorPage.PreencherCampoLogin(nomeOperador);
	}

	[Given("preencho o campo senha '([^']*)'")]
	public async Task GivenPreenchoOCampoSenha(string senhaOperador)
	{
		await _cadastroDeOperadorPage.PreencherCampoSenha(senhaOperador);
	}

	[Given("habilito o usuario '([^']*)'"), Scope(Tag = "Logins")]
	public async Task GivenHabilitoOUsuario(string ativo)
	{
		await _cadastroDeOperadorPage.HabilitarCampo(ativo);
	}

	[Given("seleciono a empresas visiveis para este operador")]
	public async Task GivenSelecionoAEmpresasVisiveisParaEsteOperador()
	{
		await _cadastroDeOperadorPage.SelecionarEmpresasDisponiveis();
	}

	[Given("seleciono empresa padrao")]
	public async Task GivenSelecionoEmpresaPadrao()
	{
		await _cadastroDeOperadorPage.SelecionarEmpresaPadrao();
	}

	[Given("seleciono as permissoes essenciais")]
	public async Task GivenSelecionoAsPermissoesEssenciais()
	{
		await _cadastroDeOperadorPage.SelecionarPermissoesNecessarias();
	}

	// Pesquisar operador cadastrando antes
	[Given("seleciono o operador")]
	public async Task GivenSelecionoOOperador()
	{
		string? validador = null;
		await _cadastroDeOperadorPage.PesquisarUsuario(validador);
	}

	//Pesquisar operador que foi passado pela string definida na feature
	[Given("pesquiso pelo '([^']*)'")]
	public async Task GivenPesquisoPelo(string nomeOperador)
	{
		await _cadastroDeOperadorPage.PesquisarUsuario(nomeOperador);
	}

	//Pesquisar operador que foi passado pela string definida na feature
	[Given(@"pesquiso pelo operador '([^']*)'")]
	public async Task GivenPesquisoPeloOperador(string nomeOperador)
	{
		await _cadastroDeOperadorPage.PesquisarOperador(nomeOperador);
	}

	//Cadastra o operador e depois pesquisa ele
	[Given(@"pesquiso pelo operador")]
	public async Task GivenPesquisoPeloOperador()
	{
		string? validador = null;
		await _cadastroDeOperadorPage.PesquisarOperador(validador);
	}

	// Clicar para excluir operador
	[Given("clico em Remover")]
	public async Task GivenClicoEmRemover()
	{
		await _cadastroDeOperadorPage.ClicarEmRemover();
	}


	[Given("cadastro um operador")]
	public async Task GivenCadastroUmOperador()
	{
		string? validador = null;
		await _cadastroDeOperadorPage.CadastroOperadorPesquisar(validador);
	}

	//Clicar em Editar
	[Given("clico em editar")]
	public async Task GivenClicoEmEditar()
	{
		await _cadastroDeOperadorPage.ClicoEmEditar();
	}

	[Given("altero a descricao incluindo")]
	public async Task GivenAlteroADescricaoIncluindo()
	{
		// Chama o método do Page Object para informar a nova descrição
		await _cadastroDeOperadorPage.InformoANovaDescricao("ALTERADO");
	}


	//Clico em pesquisar
	[Given("clico no campo pesquisar")]
	public async Task GivenClicoNoCampoPesquisar()
	{
		await _cadastroDeOperadorPage.ClicoNoCampoPesquisar();
	}

	//WHEN (QUANDO)

	//'Clico em' genérico que permite enviar qualquer string para o objeto (portanto tratamento deve ser feito no objeto)

	[When("clico em Gravar")]
	public async Task WhenClicoEmGravar()
	{
		await _cadastroDeOperadorPage.Gravar();
	}


	[When(@"clico em '([^']*)'"), Scope(Tag = "EditarOperadorCadastrando")]
	public async Task ClicoEm(string botao)
	{
		await _cadastroDeOperadorPage.Gravar();
	}

	// Selecionar a opção
	[When("seleciono a opcao '([^']*)'")]
	public async Task WhenSelecionoAOpcao(string escolha)
	{
		await _cadastroDeOperadorPage.ClicarEm(escolha);
	}

	[When("digito '([^']*)'")]
	public async Task WhenDigito(string NomeOperador)
	{
		await _cadastroDeOperadorPage.DigitarNomeOperador(NomeOperador);
	}

	[When("digito operador")]
	public async Task WhenDigitoOperador()
	{
		await _cadastroDeOperadorPage.DigitarNomeOperadorCadastrado();
	}

	//THEN (ENTÃO) 

	//Confirmação de cadastro
	[Then("e apresentada a mensagem '([^']*)'"), Scope(Tag = "Logins")]
	public async Task ThenEApresentadaAMensagem(string p0)
	{

		await _cadastroDeOperadorPage.EApresentadaAMensagem(p0);

	}

	//Confirmação de exclusão
	[Then("e apresentada a mensagem de exclusao '([^']*)'")]
	public async Task ThenEApresentadaAMensagemDeExclusao(string p0)
	{
		await _cadastroDeOperadorPage.EApresentadaAMensagemVoltar();
	}


	[When("clico em '([^']*)'"), Scope(Tag = "Logins")]
	public async Task WhenClicoEm(string gravar)
	{
		await _cadastroDeOperadorPage.clicarEmOpcao(gravar);
	}



	// VALIDAR OPERADOR BUSCADO
	[Given("que desejo validar a situacao de um operador ativo"), Scope(Tag = "@ValidarSituacao")]
	public async Task GivenQueDesejoValidarASituacaoDeUmOperadorAtivo()
	{
		await CrieContexto();
		await RealizeLoginAdmin();

		_cadastroDeOperadorPage = await POFactory.Crie<OperadorPO>(Context); // instancia o objeto
		await _cadastroDeOperadorPage.GotoAsync(); // Instancia a página
	}

	[Given("digito o nome do operador")]
	public async Task WhenDigitoONomeDoOperador()
	{
		string? OperadorBuscado = null;
		await _cadastroDeOperadorPage.PesquisarOperador(OperadorBuscado);
	}

	[When("seleciono o filtro '([^']*)'")]
	public async Task GivenSelecionoOFiltro(string filtroSituacao)
	{
		await _cadastroDeOperadorPage.selecionoFiltroSituacao(filtroSituacao);
	}

	[Then("o operador e apresentado")]
	public async Task ThenOOperadorEApresentado()
	{
		await _cadastroDeOperadorPage.ValidarOperadorApresentado();
	}

}

