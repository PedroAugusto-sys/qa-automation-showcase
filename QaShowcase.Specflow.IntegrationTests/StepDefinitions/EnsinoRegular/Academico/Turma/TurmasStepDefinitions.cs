using QaShowcase.Specflow.IntegrationTests.PageObjects;
using System;
using System.Security.AccessControl;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.Turma
{
	[Binding]
	public class TurmasStepDefinitions : BaseStep
	{
		private TurmasPO _turma = null!;
		//private LoginPO _loginPage = null!;
		//private readonly OperadorPO _cadastroDeOperadorPage = null!;
		private string nomeOperador = null!;
		private string turma = null!;


		[Given(@"que desejo cadastrar uma turma")]
		public async Task GivenQueDesejoCadastrarUmaTurma()
		{

			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();

		}

		[Given(@"acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "Turmas")]
		public async Task GivenAcessoOSistemaUtilizandoOOperador(string nomeOperador)
		{

			//SEÇÃO DO USUÁRIO
			this.nomeOperador = nomeOperador;
			await RealizeLogin(nomeOperador, "");

			_turma = await POFactory.Crie<TurmasPO>(Context);
			await _turma.GotoAsync();
		}



		[Given(@"clico para gerar um codigo")]
		public async Task GivenClicoEmGerarCodigo() => await _turma.ClicoEmObterOCodigoDaTurma();

		[Given(@"informo a descricao '([^']*)'")]
		public async Task GivenInformoADescricaoDaTurmaQueDesejoCadastrar(string descricao) => await _turma.InformoADescricaoDaTurmaQueDesejoCadastrar(descricao);

		[When(@"clico em '([^']*)'"), Scope(Tag = "CadastrarTurmas"), Scope(Tag = "EditarTurmasCadastrando"), Scope(Tag = "ExcluirTurmaSemUsoCadastrando"), Scope(Tag = "ExcluirTurmaEmUsoCadastrando")]
		public async Task WhenClicoEm(string botao)
		{
			await _turma.ClicoEm(botao);

		}


		[Then(@"e apresentada a mensagem '([^']*)'"), Scope(Tag = "CadastrarTurmas"), Scope(Tag = "EditarTurmasCadastrando"), Scope(Tag = "ExcluirTurmaEmUsoCadastrando"), Scope(Tag = "ExcluirTurmaSemUsoCadastrando")]
		public async Task ThenEApresentadaMensagem(string mensagem)
		{
			await _turma.EntaoApresentaMensagem(mensagem);
		}


		[Given(@"que desejo editar um turma")]
		public async Task GivenQueDesejoEditarUmaTurma()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();


		}

		[Given(@"pesquiso pela turma '([^']*)'")]
		public async Task GivenPesquisoPelaTurma(string turma) => await _turma.PesquisarTurma(turma);


		[Given(@"clico no botao '([^']*)'")]
		public async Task GivenClicoNoBotao(string botao)
		{
			await _turma.ClicoNoBotao(botao);

		}

		[Given(@"altero a descricao para '([^']*)'")]
		public async Task GivenInformoANovaDescricaoDaTurma(string novaDescricao) => await _turma.InformoANovaDescricaoDaTurma(novaDescricao);


		[Given(@"que desejo excluir uma turma")]
		public async Task GivenQueDesejoExcluirUmaTurma()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
		}


		[Given(@"que desejo pesquisar uma turma")]
		public async Task GivenQueDesejoPesquisarUmaTurma()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		[When(@"pesquiso pela turma '([^']*)'")]
		public async Task WhenPesquisoPelaTurma(string turma)
		{
			this.turma = turma;
			await _turma.PesquisarTurma(turma);
		}

		[Then(@"e apresentada somente as turmas que contenham no nome o termo informado")]
		public async Task ThenEApresentadaSomenteAsTurmasQueContenhamNoNomeOTermoInformado()
		{
			await _turma.PesquisarTurma(this.turma);
		}



		[Then("removo o operador"), Scope(Tag = "CadastrarTurmas"), Scope(Tag = "EditarTurmasCadastrando"), Scope(Tag = "ExcluirTurmaEmUsoCadastrando"), Scope(Tag = "ExcluirTurmaSemUsoCadastrando"), Scope(Tag = "PesquisarTurmasCadastrando")]
		public async Task ThenRemovoOOperador()
		{
			await CrieContexto();
			await RealizeLoginAdmin();
			await RemoverOperador(nomeOperador);
		}

	}
}
