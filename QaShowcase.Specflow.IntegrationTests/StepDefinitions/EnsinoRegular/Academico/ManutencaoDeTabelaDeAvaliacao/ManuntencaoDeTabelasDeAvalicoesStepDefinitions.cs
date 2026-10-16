using QaShowcase.Specflow.IntegrationTests.PageObjects;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.ManuntencaoDeTabelasDeAvaliacoes
{
	[Binding]
	public class ManuntencaoDeTabelasDeAvaliacoesStepDefinitions : BaseStep
	{
		private ManutencaoDeTabelasDeAvaliacoesPO _manuntencaoDeTabelasDeAvaliacoes = null!;
		private readonly ScenarioContext _scenarioContext;

		// Injeção do ScenarioContext via construtor
		public ManuntencaoDeTabelasDeAvaliacoesStepDefinitions(ScenarioContext scenarioContext)
		{
			_scenarioContext = scenarioContext; // Inicialize o ScenarioContext
		}

		// BLOCOS DADO

		[Given(@"que desejo cadastrar uma tabela de avaliacao"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenQueDesejoCadastrarUmResponsavel()
		{
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		[Given(@"que desejo editar uma tabela de avaliacao cadastrando"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenQueDesejoEditarUmaTabelaDeAvaliacaoCadastrando()
		{
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		[Given(@"que desejo excluir uma tabela de avaliacao"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenQueDesejoExcluirUmaTabelaDeAvaliacao()
		{
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		// BLOCOS E

		[Given(@"acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenAcessoOSistemaUtilizandoOOperador(string nomeOperador)
		{
			await RealizeLogin(nomeOperador, "");
			_scenarioContext["NomeOperador"] = nomeOperador;
			_manuntencaoDeTabelasDeAvaliacoes = await POFactory.Crie<ManutencaoDeTabelasDeAvaliacoesPO>(Context);
			await _manuntencaoDeTabelasDeAvaliacoes.GotoAsync();
		}

		[Given(@"pesquiso a tabela '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task PesquisoATabela(string NomeTabela)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.CadastrarPesquisando(NomeTabela);
		}

		[Given(@"clico em '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task ClicarEm(string botao)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.ClicarEm(botao);
		}

		[Given(@"altero o tipo de tabela de avaliacao para '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenAlteroOTipoDeTabelaDeAvaliacaoPara(string tipo)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.TipoAvalicao(tipo);
		}

		[Given(@"preencho com o nome de '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task InformoONome(string nome)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.InformoONome(nome);
		}

		[Given(@"altero a opcao de tabela ativa para '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenAlteroAOpcaoDeTabelaAtivaPara(string TabelaAtiva)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.OpcaoTabelaAtiva(TabelaAtiva);
		}

		[Given(@"altero a tabela padrao para '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenAlteroATabelaPadraoPara(string TabelaPadrao)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.OpcaoTabelaPadrao(TabelaPadrao);
		}

		[Given(@"altero a opcao calcular avaliacoes analiticas para '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenAlteroAOpcaoCalcularAvaliacoesAnaliticasPara(string CalcAvAn)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.CalcularAvaliacoesAnaliticas(CalcAvAn);
		}

		[Given(@"altero a opcao template portal de professor para '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenAlteroAOpcaoTemplatePortalDeProfessorPara(string Template)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.OpcaoTemplatePortalDeProfessor(Template);
		}

		[Given(@"insiro o valor SAV '([^']*)'"), Scope(Tag = "Avaliacoes")]
		public void GivenInsiroOValorSAV(string ValorSAV)
		{
			_manuntencaoDeTabelasDeAvaliacoes.InserirValorSAV(ValorSAV);
		}

		[Given(@"insiro o tipo de avaliacao '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenInsiroOTipoDeAvaliacao(string Tipo)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.TipoAvalicao(Tipo);
		}

		[Given(@"insiro a descricao resumida '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenInsiroADescricaoResumida(string DescResumida)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.DescricaoResumida(DescResumida);
		}

		[Given(@"insiro a descricao '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenInsiroADescricao(string Descricao)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.Descricao(Descricao);
		}

		[Given(@"insiro o valor maximo '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task GivenInsiroOValorMaximo(string VlrMax)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.ValorMaximo(VlrMax);
		}

		[Given(@"repito a adicao de avaliacoes '([^']*)' mais '([^']*)' vezes"), Scope(Tag = "@Avaliacoes")]
		public async Task AdicionoDuasAvaliacoes(string Tipo, int QtdVezes)
		{
			if (QtdVezes <= 0)
			{
				return;
			}
			await _manuntencaoDeTabelasDeAvaliacoes.AdicionarAvaliacoes(Tipo);
			await AdicionoDuasAvaliacoes(Tipo, QtdVezes - 1);
		}


		[Given(@"altero a opcao formula aceita nulo para '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task SelecionoFormulaAceitaNulo(string Opcao)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.FormulaAceitaNulo(Opcao);
		}

		[Given(@"insiro a formula '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task InsiroAFormula(string Formula)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.InserirFormula(Formula);
		}

		[Given(@"altero o nome para '([^']*)'"), Scope(Tag = "Avaliacoes")]
		public async Task GivenAlteroONomePara(string NomeAlterado)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.AlterarNome(NomeAlterado);
		}

		// BLOCOS QUANDO

		[When("clico em '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task WhenClicoEm(string botao)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.ClicarEm(botao);
		}

		[When(@"e trago a mensagem '([^']*)'"), Scope(Tag = "@Avaliacoes")]
		public async Task WhenETragoAMensagem(string Mensagem)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.ValidarMensagem(Mensagem);
			await _manuntencaoDeTabelasDeAvaliacoes.ClicarEm("Voltar para a lista");
		}

		// BLOCOS ENTÃO

		[Then(@"e trago a mensagem '(.*)'"), Scope(Tag = "Avaliacoes")]
		public async Task OCadastroEhRealizadoComSucesso(string Mensagem)
		{
			await _manuntencaoDeTabelasDeAvaliacoes.ValidarMensagem(Mensagem);
		}

		[Then(@"valido a alteracao do status da tabela filtrando"), Scope(Tag = "Avaliacoes")]
		public async Task ThenValidoAAlteracaoDoStatusDaTabelaFiltrando()
		{
			await _manuntencaoDeTabelasDeAvaliacoes.ValidarAlteracaoDoStatusDaTabela();
		}

		[Then("removo o operador"), Scope(Tag = "@Avaliacoes")]
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

}