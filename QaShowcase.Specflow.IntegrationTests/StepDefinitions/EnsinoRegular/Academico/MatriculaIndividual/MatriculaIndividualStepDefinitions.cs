using QaShowcase.Specflow.IntegrationTests.PageObjects;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.MatriculaIndividual;

[Binding]
public class MatriculaIndividualStepDefinitions : BaseStep
{
	private MatriculaIndividualPO _matriculaIndividualPage = null!;


	[Given(@"que acesso a tela de matricula")]
	public async Task GivenQueAcessoATelaDeMatricula()
	{
		//SE��O DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();



		//SE��O DO USU�RIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR MATRICULA";
		await RealizeLogin(nomeOperador, "");

		_matriculaIndividualPage = await POFactory.Crie<MatriculaIndividualPO>(Context);
		await _matriculaIndividualPage.GotoAsync();
	}

	[Given(@"seleciono um aluno")]
	public async Task GivenSelecionoUmAluno()
	{
		throw new PendingStepException();
	}

	[Given(@"clico para excluir a matricula")]
	public void GivenClicoParaExcluirAMatricula()
	{
		throw new PendingStepException();
	}

	[When(@"confirmo a exclus�o da matr�cula")]
	public void WhenConfirmoAExclusaoDaMatricula()
	{
		throw new PendingStepException();
	}

	[Then(@"a matricula e excluida com sucesso")]
	public void ThenAMatriculaEExcluidaComSucesso()
	{
		throw new PendingStepException();
	}

	[Then(@"e apresentada a mensagem que a matricula nao pode ser excluida")]
	public void ThenEApresentadaAMensagemQueAMatriculaNaoPodeSerExcluida()
	{
		throw new PendingStepException();
	}

	[Given(@"que desejo matricular um aluno")]
	public void GivenQueDesejoMatricularUmAluno()
	{
		throw new PendingStepException();
	}

	[Given(@"seleciono um aluno que nao possui matricula")]
	public void GivenSelecionoUmAlunoQueNaoPossuiMatricula()
	{
		throw new PendingStepException();
	}

	[Given(@"seleciono uma serie para matricular")]
	public void GivenSelecionoUmaSerieParaMatricular()
	{
		throw new PendingStepException();
	}

	[Given(@"marco o aluno como proprio responsavel")]
	public void GivenMarcoOAlunoComoProprioResponsavel()
	{
		throw new PendingStepException();
	}

	[When(@"clico em gravar matricula")]
	public void WhenClicoEmGravarMatricula()
	{
		throw new PendingStepException();
	}

	[Then(@"a matricula e realizada")]
	public void ThenAMatriculaERealizada()
	{
		throw new PendingStepException();
	}

	[Given(@"que desejo matricular um aluno gerando financeiro")]
	public void GivenQueDesejoMatricularUmAlunoGerandoFinanceiro()
	{
		throw new PendingStepException();
	}

	[Given(@"seleciono um aluno que nao possui matricula ativa")]
	public void GivenSelecionoUmAlunoQueNaoPossuiMatriculaAtiva()
	{
		throw new PendingStepException();
	}

	[Given(@"preencho os dados das duplicatas")]
	public void GivenPreenchoOsDadosDasDuplicatas()
	{
		throw new PendingStepException();
	}

	[Given(@"que desejo matricular um aluno com financeiro por plano de pagamento")]
	public void GivenQueDesejoMatricularUmAlunoComFinanceiroProPlanoDePagamento()
	{
		throw new PendingStepException();
	}

	[Given(@"seleciono um aluno que nao possui matricula no ano atual")]
	public void GivenSelecionoUmAlunoQueNaoPossuiMatriculaNoAnoAtual()
	{
		throw new PendingStepException();
	}

	[Given(@"seleciono um plano de pagamento")]
	public void GivenSelecionoUmPlanoDePagamento()
	{
		throw new PendingStepException();
	}

	[Given(@"que desejo cancelar a matricula de um aluno")]
	public void GivenQueDesejoCancelarAMatriculaDeUmAluno()
	{
		throw new PendingStepException();
	}

	[Given(@"seleciono um aluno com a matr�cula ativa")]
	public void GivenSelecionoUmAlunoComAMatriculaAtiva()
	{
		throw new PendingStepException();
	}

	[Given(@"clico para cancelar a matricula")]
	public void GivenClicoParaCancelarAMatricula()
	{
		throw new PendingStepException();
	}

	[Given(@"confirmo as consequencias")]
	public void GivenConfirmoAsConsequencias()
	{
		throw new PendingStepException();
	}

	[Given(@"insiro uma data de cancelamento")]
	public void GivenInsiroUmaDataDeCancelamento()
	{
		throw new PendingStepException();
	}

	[Given(@"confirmo o cancelamento")]
	public void GivenConfirmoOCancelamento()
	{
		throw new PendingStepException();
	}

	[Given(@"marco sim para cancelar")]
	public void GivenMarcoSimParaCancelar()
	{
		throw new PendingStepException();
	}

	[When(@"clico em voltar")]
	public void WhenClicoEmVoltar()
	{
		throw new PendingStepException();
	}

	[Then(@"noto que a matricula esta cancelada")]
	public void ThenNotoQueAMatriculaEstaCancelada()
	{
		throw new PendingStepException();
	}

}
