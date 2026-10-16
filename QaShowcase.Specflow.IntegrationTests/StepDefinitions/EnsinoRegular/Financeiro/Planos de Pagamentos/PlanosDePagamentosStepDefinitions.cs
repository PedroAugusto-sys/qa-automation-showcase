using NUnit.Framework;
using QaShowcase.Specflow.IntegrationTests.PageObjects;
using QaShowcase.Specflow.IntegrationTests.PageObjects.Financeiro.Planos_de_Pagamentos;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Financeiro.Planos_de_Pagamentos;

[Binding]
public class PlanosDePagamentosStepDefinitions : BaseStep
{
	private PlanosDePagamentosPO _planosDePagamentosPage = null!;
	private readonly ScenarioContext _scenarioContext;

	public PlanosDePagamentosStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
	}

	#region GIVEN

	[Given("que desejo cadastrar planos de pagamentos"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenQueDesejoCadastrarPlanosDePagamentos()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo pesquisar planos de pagamentos"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenQueDesejoPesquisarPlanosDePagamentos()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo clonar planos de pagamentos"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenQueDesejoClonarPlanosDePagamentos()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo editar planos de pagamentos"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenQueDesejoEditarPlanosDePagamentos()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo excluir planos de pagamentos"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenQueDesejoExcluirPlanosDePagamentos()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	#endregion

	#region AND

	[Given(@"seleciono o operador '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoOOperador(string nomeOperador)
	{
		await RealizeLogin(nomeOperador, "");
		_scenarioContext["NomeOperador"] = nomeOperador;
		_planosDePagamentosPage = await POFactory.Crie<PlanosDePagamentosPO>(Context);
		await _planosDePagamentosPage.GotoAsync();
	}

	[Given(@"e apresentada a mensagem '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenEApresentadaAMensagem(string mensagem)
	{
		await _planosDePagamentosPage.ValidarMensagem(mensagem);
	}

	[Given(@"clico em '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task ClicoEm(string Botao)
	{
		await _planosDePagamentosPage.ClicarEm(Botao);
	}

	[Given("clico em '([^']*)' no plano de pagamentos")]
	public async Task GivenClicoEmNoPlanoDePagamentos(string Botao)
	{
		await _planosDePagamentosPage.ClicarEm(Botao);
	}

	[Given("na grid Finalidade clico em '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenNaGridFinalidadeClicoEm(string Botao)
	{
		await _planosDePagamentosPage.BotoesEspeciaisDoXPath(Botao);
	}

	[Given("pesquiso pelo plano de pagamentos '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenPesquisoPeloPlanoDePagamentos(string PlanoDePagamento)
	{
		await _planosDePagamentosPage.PesquisarPlanoDePagamentoAtivo(PlanoDePagamento);
	}

	[Given("pesquiso pelo plano de pagamentos ativo '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenPesquisoPeloPlanoDePagamentosAtivo(string PlanoDePagamento)
	{
		await _planosDePagamentosPage.PesquisarPlanoDePagamentoAtivo(PlanoDePagamento);
	}

	[Given("pesquiso pelo plano de pagamentos inativo '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenPesquisoPeloPlanoDePagamentosInativo(string PlanoDePagamento)
	{
		await _planosDePagamentosPage.PesquisarPlanoDePagamentoInativo(PlanoDePagamento);
	}

	[Given(@"informo a descricao '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task InformoADescricao(string descricao)
	{
		await _planosDePagamentosPage.InformarDescricao(descricao);
	}

	[Given(@"altero a descricao para '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task InformoOCodigo(string descricao)
	{
		await _planosDePagamentosPage.InformarDescricao(descricao);
	}

	[Given("altero a Finalidade para '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenAlteroAFinalidadePara(string finalidade)
	{
		await _planosDePagamentosPage.SelecionarFinalidade(finalidade);
	}

	[Given(@"clico em Ativo '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task ClicoEmAtivo(string Botao)
	{
		await _planosDePagamentosPage.ClicarEmAtivo(Botao);
	}

	[Given("altero o campo Ativo para '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenAlteroOCampoAtivoPara(string Botao)
	{
		await _planosDePagamentosPage.ClicarEmAtivo(Botao);
	}


	[Given(@"seleciono a Finalidade '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoAFinalidade(string finalidade)
	{
		await _planosDePagamentosPage.SelecionarFinalidade(finalidade);
	}

	[Given(@"insiro o valor mensal '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task InsiroOValorMensal(string valorMensal)
	{
		await _planosDePagamentosPage.InserirValorMensal(valorMensal);
	}

	[Given(@"insiro as Parcelas '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task InsiroAsParcelas(string parcelas)
	{
		await _planosDePagamentosPage.InserirParcelas(parcelas);
	}

	[Given(@"seleciono a Data Primeira parcela '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoADataPrimeiraParcela(string data)
	{
		await _planosDePagamentosPage.SelecionarDataPrimeiraParcela(data);
	}

	[Given(@"insiro a Qtde dias a mais '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task InsiroAQtdeDiasAMais(string dias)
	{
		await _planosDePagamentosPage.InserirQtdeDiasAMais(dias);
	}

	[Given(@"seleciono Outros vencimentos '([^']*)' Dia '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoOutrosVencimentos(string tipo, string dia)
	{
		await _planosDePagamentosPage.SelecionarOutrosVencimentos(tipo, dia);
	}

	[Given(@"seleciono o Pagamento '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoOPagamento(string pagamento)
	{
		await _planosDePagamentosPage.SelecionarPagamento(pagamento);
	}

	[Given(@"seleciono Conta contábil de conf '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoContaContabilDeConf(string valor)
	{
		await _planosDePagamentosPage.SelecionarContaContabilConf(valor);
	}

	[Given(@"seleciono o Tipo de conta contabil '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoOTipoDeContaContabil(string tipoConta)
	{
		await _planosDePagamentosPage.SelecionarTipoContaContabil(tipoConta);
	}

	[Given(@"seleciono a Informacao bancaria '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoAInformacaoBancaria(string infoBancaria)
	{
		await _planosDePagamentosPage.SelecionarInformacaoBancaria(infoBancaria);
	}

	[Given(@"seleciono a Conta contabil '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoAContaContabil(string conta)
	{
		await _planosDePagamentosPage.SelecionarContaContabil(conta);
	}

	[Given(@"seleciono a Serie '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoASerie(string serie)
	{
		await _planosDePagamentosPage.SelecionarSerie(serie);
	}

	[Given(@"seleciono a Turma '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task SelecionoATurma(string turma)
	{
		await _planosDePagamentosPage.SelecionarTurma(turma);
	}

	[Given(@"valido a simulacao das '([^']*)' duplicatas de valor '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task ValidoASimulacaoDasDuplicatas(string qtd, string valor)
	{
		await _planosDePagamentosPage.ValidarSimulacaoDuplicatasJson(qtd, valor);
	}

	[Given(@"verifico a validacao de campo '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task VerificoAValidacaoDeCampo(string campo)
	{
		await _planosDePagamentosPage.VerificarValidacaoCampo(campo);
	}

	[Given("altero o ano de referencia para '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task GivenAlteroOAnoDeReferenciaPara(string Ano)
	{
		await _planosDePagamentosPage.AlterarAnoDeReferencia(Ano);
	}

	#endregion

	#region WHEN

	[When("pesquiso pelo plano de pagamentos ativo '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task WhenPesquisoPeloPlanoDePagamentosAtivo(string PlanoDePagamento)
	{
		await _planosDePagamentosPage.PesquisarPlanoDePagamentoAtivo(PlanoDePagamento);
	}

	[When("pesquiso pelo plano de pagamentos inativo '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task WhenPesquisoPeloPlanoDePagamentosInativo(string PlanoDePagamento)
	{
		await _planosDePagamentosPage.PesquisarPlanoDePagamentoInativo(PlanoDePagamento);
	}

	[When("pesquiso pelo plano de pagamentos de ano diferente '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task WhenPesquisoPeloPlanoDePagamentosDeAnoDiferente(string PlanoDePagamento)
	{
		await _planosDePagamentosPage.PesquisarPlanoDePagamentoAnoDiferente(PlanoDePagamento);
	}

	[When("pesquiso pelo plano de pagamentos clonado '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task WhenPesquisoPeloPlanoDePagamentosClonado(string PlanoDePagamento)
	{
		await _planosDePagamentosPage.PesquisarPlanoDePagamentoClonado(PlanoDePagamento);
	}

	[When(@"e apresentada a mensagem '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task WhenEApresentadaAMensagem(string mensagem)
	{
		await _planosDePagamentosPage.ValidarMensagem(mensagem);
	}

	[When(@"clico em '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task WhenClicoEm(string botao)
	{
		await _planosDePagamentosPage.ClicarEm(botao);
	}

	[When("pesquiso pelo plano de pagamentos '([^']*)' com status alterado"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task WhenPesquisoPeloPlanoDePagamentosComStatusAlterado(string PlanoDePagamento)
	{
		await _planosDePagamentosPage.PesquisarPlanoDePagamento(PlanoDePagamento);
	}

	#endregion

	#region THEN

	[Then(@"clico em '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task ThenClicoEm(string botao)
	{
		await _planosDePagamentosPage.ClicarEm(botao);
	}

	[Then(@"e apresentada a mensagem '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task ThenEApresentadaAMensagem(string mensagem)
	{
		await _planosDePagamentosPage.ValidarMensagem(mensagem);
	}

	[Then(@"e apresentada a tela de aterrissagem"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task ThenEApresentadaATelaDeAterrissagem()
	{
		await _planosDePagamentosPage.ValidarTelaAterrissagem();
	}

	[Then("e apresentado o plano de pagamentos pesquisado"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task ThenEApresentadoOPlanoDePagamentosPesquisado()
	{
		await _planosDePagamentosPage.ValidarPlanoPagamentoPesquisado();
	}

	[Then("nao e apresentado o plano de pagamentos pesquisado"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task ThenNaoEApresentadoOPlanoDePagamentosPesquisado()
	{
		await _planosDePagamentosPage.ValidarPlanoPagamentoPesquisado();
	}

	[Then("e apresentado o plano de pagamento com status '([^']*)'"), Scope(Tag = "@PlanosDePagamentos")]
	public async Task ThenEApresentadoOPlanoDePagamentoComStatus(string status)
	{
		await _planosDePagamentosPage.ValidarPlanoPagamentoPesquisadoComStatusAlterado(status);
	}

	[Then(@"removo o operador"), Scope(Tag = "@PlanosDePagamentos")]
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

	#endregion

}
