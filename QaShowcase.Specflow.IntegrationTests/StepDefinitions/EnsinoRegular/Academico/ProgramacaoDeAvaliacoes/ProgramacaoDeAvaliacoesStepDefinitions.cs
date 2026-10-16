using QaShowcase.Specflow.IntegrationTests.PageObjects;
using System;
using System.Security.AccessControl;
using TechTalk.SpecFlow;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.ProgramacaoDeAvaliacoes
{
	[Binding]
	public class ProgramacaoDeAvaliacoesStepDefinitions : BaseStep
	{
		private ProgramacaoDeAvaliacoesPO _programacaoDeAvaliacoes = null!;



		[Given(@"Que desejo cadastrar uma programacao de avaliacoes")]
		public async Task QueDesejoCadastrarUmaProgramacaoDeAvaliacoes()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
 



			//SEÇÃO DO USUÁRIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR PROGRAMACAO";
			await RealizeLogin(nomeOperador, "");

			_programacaoDeAvaliacoes = await POFactory.Crie<ProgramacaoDeAvaliacoesPO>(Context);
			await _programacaoDeAvaliacoes.GotoAsync();
		}

		[Given(@"clico em novo")]
		public async Task ClicoEmNovo() => await _programacaoDeAvaliacoes.ClicoEmNovo();

		[Given(@"seleciono a serie")]
		public async Task SelecionoASerie() => await _programacaoDeAvaliacoes.SelecionoASerie();

		[Given(@"seleciono a turma")]
		public async Task SelecionoATurma() => await _programacaoDeAvaliacoes.SelecionoATurma("A");

		[Given(@"informo a etapa")]
		public async Task InformoAEtapa() => await _programacaoDeAvaliacoes.InformoAEtapa("1");

		[Given(@"informo a tabela de avaliacao")]
		public async Task InformoATabelaDeAvaliacao() => await _programacaoDeAvaliacoes.InformoATabelaDeAvaliacao("TESTE 2");

		[Given(@"informo a disciplina")]
		public async Task InformoADisciplina() => await _programacaoDeAvaliacoes.InformoADisciplina("ALGEBRA");

		[Given(@"clico em carregar")]
		public async Task ClicoEmCarregar() => await _programacaoDeAvaliacoes.ClicoEmCarregar();

		[Given(@"informo o professor")]
		public async Task InformoOProfessor() => await _programacaoDeAvaliacoes.InformoOProfessor();

		[When(@"clico em gravar")]
		public async Task ClicoEmGravar() => await _programacaoDeAvaliacoes.InformoOProfessor();

		[Then(@"a programacao eh realizada com sucesso")]
		public async Task AProgramacaoEhRealizadaComSucesso() => await _programacaoDeAvaliacoes.AProgramacaoEhRealizadaComSucesso();
	}
}