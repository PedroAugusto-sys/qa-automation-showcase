using QaShowcase.Specflow.IntegrationTests.PageObjects;
using TechTalk.SpecFlow;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.RemanejamentoIndividual
{
	[Binding]
	public class RemanejamentoIndividualStepDefinitions : BaseStep
	{

		private RemanejamentoIndividualPO _remanejamentoIndividual = null!;



		[Given(@"que desejo remanejar um aluno")]
		public async Task GivenQueDesejoRemanejarUmAluno()
		{
			//SE��O DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();




			//SE��O DO USU�RIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR REMANEJAMENTO";
			await RealizeLogin(nomeOperador, "");

			_remanejamentoIndividual = await POFactory.Crie<RemanejamentoIndividualPO>(Context);
			await _remanejamentoIndividual.GotoAsync();
		}

		[Given(@"que desejo estornar o remanejamento de um aluno")]
		public async Task GivenQueDesejoEstornarORemanejamentoDeUmAluno()
		{
			//SE��O DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
 



			//SE��O DO USU�RIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR REMANEJAMENTO";
			await RealizeLogin(nomeOperador, "");

			_remanejamentoIndividual = await POFactory.Crie<RemanejamentoIndividualPO>(Context);
			await _remanejamentoIndividual.GotoAsync();
		}

		[Given(@"que desejo ir para a tela de geracao de duplicatas")]
		public async Task GivenQueDesejoIrParaATelaDeGeracaoDeDuplicatas()
		{
			//SE��O DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();




			//SE��O DO USU�RIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR REMANEJAMENTO";
			await RealizeLogin(nomeOperador, "");

			_remanejamentoIndividual = await POFactory.Crie<RemanejamentoIndividualPO>(Context);
			await _remanejamentoIndividual.GotoAsync();
		}

		[Given(@"seleciono o aluno a ser remanejado")]
		public async Task GivenSelecionoOAlunoASerRemanejado() => await _remanejamentoIndividual.SelecionoOAlunoNaTelaDeRemanejamento();

		[Given(@"seleciono a serie/turma para remanejamento")]
		public async Task GivenSelecionoASerieTurmaParaRemanejamento() => await _remanejamentoIndividual.SelecionoASerieTurmaParaRemanejamento();

		[Given(@"clico em carregar remanejamento")]
		public async Task GivenClicoEmCarregarRemanejamento() => await _remanejamentoIndividual.ClicoEmCarregarRemanejamento();

		[Given(@"E configuro as opcoes de remanejamento")]
		public async Task GivenEConfiguroAsOpcoesDeRemanejamento() => await _remanejamentoIndividual.EConfiguroAsOpcoesDeRemanejamento();

		[Given(@"seleciono o aluno a ser estornado o remanejamento")]
		public async Task GivenSelecionoOAlunoASerEstornadoORemanejamento() => await _remanejamentoIndividual.SelecionoOAlunoNaTelaDeRemanejamento();

		[Given(@"clico em estornar o remanejamento")]
		public async Task GivenClicoEmEstornarORemanejamento() => await _remanejamentoIndividual.ClicoEmEstornarORemanejamento();

		[When(@"clico em gravar remanejamento")]
		public async Task WhenClicoEmGravarRemanejamento() => await _remanejamentoIndividual.ClicoEmGravarRemanejamento();

		[When(@"clico em sim"), Scope(Tag = "RemanejamentoIndividual")]
		public async void WhenClicoEmSim() => await _remanejamentoIndividual.ClicoEmSim();

		[When(@"clico no botao financeiro, na tela de remanejamento individual")]
		public async void WhenClicoNoBotaoFinanceiroNaTelaDeRemanejamentoIndividual() => await _remanejamentoIndividual.ClicoNoBotaoFinanceiroNaTelaDeRemanejamentoIndividual();

		[Then(@"o remanejamento eh realizado com sucesso")]
		public async Task ThenORemanejamentoEhRealizadoComSucesso() => await _remanejamentoIndividual.ORemanejamentoEhRealizadoComSucesso();

		[Then(@"o estorno do remanejamento eh realizado com sucesso")]
		public async Task ThenOEstornoDoRemanejamentoEhRealizadoComSucesso() => await _remanejamentoIndividual.OEstornoDoRemanejamentoEhRealizadoComSucesso();

		[Then(@"a tela de geracao de duplicatas eh aberta")]
		public async Task ThenATelaDeGeracaoDeDuplicatasEhAberta() => await _remanejamentoIndividual.ATelaDeGeracaoDeDuplicatasEhAberta();
	}
}