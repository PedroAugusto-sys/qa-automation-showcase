using QaShowcase.Specflow.IntegrationTests.PageObjects;
using System;
using System.Security.AccessControl;
using TechTalk.SpecFlow;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.MontagemDeTurma
{
	[Binding]
	public class MontagemDeTurmaStepDefinitions : BaseStep
	{
		private MontagemDeTurmaPO _montagemDeTurma = null!;
		private readonly string Lotacao = "355";
		private readonly string NovaLotacao = "357";
		private readonly string Serie = "serie";
		private readonly string Turma = "turma";


		[Given(@"Que desejo cadastrar uma montagem de turma")]
		public async Task QueDesejoCadastrarUmaMontagemDeTurma()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();




			//SEÇÃO DO USUÁRIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR MONTAGEM";
			await RealizeLogin(nomeOperador, "");

			_montagemDeTurma = await POFactory.Crie<MontagemDeTurmaPO>(Context);
			await _montagemDeTurma.GotoAsync();
		}

		[Given(@"clico em cadastrar"), Scope(Tag = "MontagemDeTurma")]
		public async Task ClicoEmCadastrar() => await _montagemDeTurma.ClicoEmCadastrar();

		[Given(@"seleciono a serie"), Scope(Tag = "MontagemDeTurma")]
		public async Task SelecionoASerie() => await _montagemDeTurma.SelecionoASerie(Serie);

		[Given(@"seleciono a turma"), Scope(Tag = "MontagemDeTurma")]
		public async Task SelecionoATurma() => await _montagemDeTurma.SelecionoATurma(Turma);

		[Given(@"informo o turno"), Scope(Tag = "MontagemDeTurma")]
		public async Task InformoOTurno() => await _montagemDeTurma.InformoOTurno();

		[Given(@"informo a lotacao"), Scope(Tag = "MontagemDeTurma")]
		public async Task InformoALotacao() => await _montagemDeTurma.InformoALotacao(Lotacao);

		[When(@"clico em gravar"), Scope(Tag = "MontagemDeTurma")]
		public async Task ClicoEmGravar() => await _montagemDeTurma.ClicoEmGravar();

		[Then(@"o cadastro eh realizado com sucesso"), Scope(Tag = "MontagemDeTurma")]
		public async Task OCadastroEhRealizadoComSucesso() => await _montagemDeTurma.OCadastroEhRealizadoComSucesso(Lotacao);


		[Given(@"Que desejo editar uma montagem de turma")]
		public async Task QueDesejoEditarUmaMontagemDeTurma()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
 



			//SEÇÃO DO USUÁRIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR MONTAGEM";
			await RealizeLogin(nomeOperador, "");

			_montagemDeTurma = await POFactory.Crie<MontagemDeTurmaPO>(Context);
			await _montagemDeTurma.GotoAsync();
		}

		[Given(@"seleciono a montagem de turma que desejo editar")]
		public async Task SelecionoAMontagemDeTurmaQueDesejoEditar() => await _montagemDeTurma.SelecionoAMontagemDeTurmaQueDesejoEditar(Lotacao);

		[Given(@"informo a nova lotacao")]
		public async Task InformoANovaLotacao() => await _montagemDeTurma.InformoANovaLotacao(NovaLotacao);

		[Then(@"a edicao eh realizada com sucesso"), Scope(Tag = "MontagemDeTurma")]
		public async Task AEdicaoEhRealizadaComSucesso() => await _montagemDeTurma.AEdicaoEhRealizadaComSucesso(NovaLotacao);


		[Given(@"Que desejo excluir uma montagem de turma")]
		public async Task QueDesejoExcluirUmaMontagemDeTurma()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();




			//SEÇÃO DO USUÁRIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR MONTAGEM";
			await RealizeLogin(nomeOperador, "");

			_montagemDeTurma = await POFactory.Crie<MontagemDeTurmaPO>(Context);
			await _montagemDeTurma.GotoAsync();
		}

		[Given(@"clico em excluir"), Scope(Tag = "MontagemDeTurma")]
		public async Task ClicoEmExcluir() => await _montagemDeTurma.ClicoEmExcluir(NovaLotacao);

		[When(@"clico em sim"), Scope(Tag = "MontagemDeTurma")]
		public async Task ClicoEmSim() => await _montagemDeTurma.ClicoEmSim();

		[Then(@"a exclusao eh realizada com sucesso"), Scope(Tag = "MontagemDeTurma")]
		public async Task AExclusaoEhRealizadaComSucesso() => await _montagemDeTurma.AExclusaoEhRealizadaComSucesso();
	}
}