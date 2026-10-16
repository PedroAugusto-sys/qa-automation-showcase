using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.Json;
using System.Globalization;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects.Financeiro.Planos_de_Pagamentos;

internal class PlanosDePagamentosPO : POBase
{
	public PlanosDePagamentosPO(IPage page) : base(page)
	{
	}

	// Classe para deserialização
	public class SimulacaoParcela
	{
		public int NumeroParcela { get; set; }
		public decimal ValorParcela { get; set; }
	}

	public override string PageUrl => "CadastroPlanoPagamento";

	#region Variáveis Globais
	public string DescricaoPlanoPagamento;
	public string situacao;
	#endregion

	#region Variáveis Locais
	private readonly string QtdeParcelas = "10";
	private readonly string VlrParcela = "1000";
	private string? AnoDoPlano;
	#endregion

	#region Cadastros

	public async Task CadastrarPlanoDePagamentoCCBancariaInterno(string PlanoDePagamento)
	{
		await this.ClicarEm("Cadastrar");
		await this.InformarDescricao(PlanoDePagamento);
		if (situacao.Contains("Inativo", StringComparison.OrdinalIgnoreCase))
		{
			await this.ClicarEmAtivo("Não");
		}
		else
		{
			await this.ClicarEmAtivo("Sim");
		}
		await this.ClicarEm("Adicionar");
		await this.SelecionarFinalidade("Mensalidade");
		await this.InserirValorMensal(VlrParcela);
		await this.InserirParcelas(QtdeParcelas);
		await this.SelecionarDataPrimeiraParcela("Data Atual");
		await this.InserirQtdeDiasAMais("1");
		await this.SelecionarOutrosVencimentos("Fixo, todo dia", "1");
		await this.SelecionarPagamento("Manter o vencimento");
		await this.SelecionarContaContabilConf("Não");
		await this.SelecionarTipoContaContabil("Bancária");
		await this.SelecionarInformacaoBancaria("1 - BRADESCO PRINCIPAL - T9");
		await this.SelecionarContaContabil("MENSALIDADES - UNID - T9 [1 - BRADESCO PRINCIPAL - T9]");
		await this.ClicarEm("Confirmar");
		await this.SelecionarSerie("1º ANO [Ensino Fundamental]");
		await this.SelecionarTurma("Todos");
		await this.ClicarEm("Associar");
		await this.ValidarSimulacaoDuplicatasJson(QtdeParcelas, VlrParcela);
		await this.ClicarEm("Gravar");
		await this.ClicarEm("Sim");
		await this.ValidarMensagem("Cadastro realizado com sucesso!");
	}

	public async Task CadastrarPlanoDePagamentoCCTesourariaInterno(string PlanoDePagamento)
	{
		await this.ClicarEm("Cadastrar");
		await this.InformarDescricao(PlanoDePagamento);
		if (situacao.Contains("Ativo", StringComparison.OrdinalIgnoreCase))
		{
			await this.ClicarEmAtivo("Sim");
		}
		else
		{
			await this.ClicarEmAtivo("Não");
		}
		await this.ClicarEm("Adicionar");
		await this.SelecionarFinalidade("MENSALIDADE");
		await this.InserirValorMensal(VlrParcela);
		await this.InserirParcelas(QtdeParcelas);
		await this.SelecionarDataPrimeiraParcela("Data Atual");
		await this.InserirQtdeDiasAMais("10");
		await this.SelecionarOutrosVencimentos("Fixo, todo dia", "1");
		await this.SelecionarPagamento("Manter o vencimento");
		await this.SelecionarContaContabilConf("Não");
		await this.SelecionarTipoContaContabil("Tesouraria");
		await this.SelecionarContaContabil("MENSALIDADES - UNID - T9");
		await this.ClicarEm("Confirmar");
		await this.SelecionarSerie("1º ANO [Ensino Fundamental]");
		await this.SelecionarTurma("Todos");
		await this.ClicarEm("Associar");
		await this.ValidarSimulacaoDuplicatasJson(QtdeParcelas, VlrParcela);
		await this.ClicarEm("Gravar");
		await this.ClicarEm("Sim");
		await this.ValidarMensagem("Cadastro realizado com sucesso!");
	}

	#endregion

	#region Pesquisas

	public async Task PesquisarPlanoDePagamentosCadastrando(string PlanoDePagamento)
	{
		await this.CadastrarPlanoDePagamentoCCBancariaInterno(PlanoDePagamento);
		await this.PesquisarPlanoDePagamento(DescricaoPlanoPagamento);
	}

	public async Task PesquisarPlanoDePagamento(string PlanoDePagamento)
	{
		await Page.WaitForTimeoutAsync(800);
		DescricaoPlanoPagamento ??= PlanoDePagamento;
		await Page.MainFrame.GetByPlaceholder("Pesquisar").PressSequentiallyAsync(DescricaoPlanoPagamento);
		await Page.WaitForTimeoutAsync(800);
	}

	public async Task PesquisarPlanoDePagamentoAtivo(string PlanoDePagamento)
	{
		situacao = "Ativo";
		await this.PesquisarPlanoDePagamentosCadastrando(PlanoDePagamento);
	}

	public async Task PesquisarPlanoDePagamentoInativo(string PlanoDePagamento)
	{
		situacao = "Inativo";
		await this.PesquisarPlanoDePagamentosCadastrando(PlanoDePagamento);
		await Page.MainFrame.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new[] { "2" });
		await Page.WaitForTimeoutAsync(500);
	}

	public async Task PesquisarPlanoDePagamentoAnoDiferente(string PlanoDePagamento)
	{
		AnoDoPlano = "2024";
		situacao = "Ativo";
		await this.PesquisarPlanoDePagamentosCadastrando(PlanoDePagamento);
		await Page.MainFrame.GetByLabel("Ano referência").SelectOptionAsync(new[] { AnoDoPlano });
	}

	public async Task PesquisarPlanoDePagamentoClonado(string PlanoDePagamento)
	{
		situacao = "Ativo";
		await this.PesquisarPlanoDePagamentosCadastrando(PlanoDePagamento);
		await Page.WaitForTimeoutAsync(500);
		await this.ClicarEm("Clonar");
		await this.ClicarEm("Gravar");
		await this.ClicarEm("Sim");
		await this.ValidarMensagem("Cadastro realizado com sucesso!");
		await this.PesquisarPlanoDePagamento(DescricaoPlanoPagamento);
	}

	#endregion

	#region Validações

	public async Task ValidarMensagem(string mensagem)
	{
		await Page.WaitForTimeoutAsync(2000);
		var TextoMensagem = await Page.MainFrame.GetByText(mensagem).IsVisibleAsync();
		if (mensagem == "Cadastro realizado com sucesso!")
		{
			await this.ClicarEm("Voltar para a lista");
		}
		Assert.IsTrue(TextoMensagem, "Texto da mensagem não encontrado na tela.");
	}

	public async Task ValidarSimulacaoDuplicatasJson(string qtdEsperada, string valorEsperado)
	{
		var input = await Page.MainFrame.Locator("#SimulacoesParcelas_Datasource").InputValueAsync();
		Assert.IsNotNull(input, "Input de simulação de parcelas não encontrado.");

		var parcelas = JsonSerializer.Deserialize<List<SimulacaoParcela>>(input);
		Assert.IsNotNull(parcelas, "Não foi possível deserializar as parcelas.");

		var parcelasValidas = parcelas.Where(p => p.NumeroParcela > 0).ToList();
		Assert.AreEqual(int.Parse(qtdEsperada), parcelasValidas.Count, "Quantidade de duplicatas não confere.");

		var valorEsperadoDecimal = decimal.Parse(valorEsperado, NumberStyles.Any, new CultureInfo("pt-BR"));
		foreach (var parcela in parcelasValidas)
		{
			Assert.AreEqual(valorEsperadoDecimal, parcela.ValorParcela, $"Valor da parcela {parcela.NumeroParcela} não confere.");
		}
	}

	public async Task ValidarPlanoPagamentoPesquisado()
	{
		await Page.WaitForTimeoutAsync(1000);
		var GridPlanosDePagamentos = Page.MainFrame.
			Locator("#GridAterrissagem > div.em-grid-container > div.em-grid-body-main");
		Assert.IsTrue(await GridPlanosDePagamentos.IsVisibleAsync(),
			"Grid de planos de pagamento não foi exibida.");

		if (!string.IsNullOrEmpty(AnoDoPlano) && AnoDoPlano.Contains("2024", StringComparison.OrdinalIgnoreCase))
		{
			var linha = Page.MainFrame.Locator($"text={DescricaoPlanoPagamento}");
			Assert.IsFalse(await linha.IsVisibleAsync(),
				$"Plano de pagamento '{DescricaoPlanoPagamento}' foi exibido na grid.");
		}
		else
		{
			var linha = Page.MainFrame.Locator($"text={DescricaoPlanoPagamento}");
			Assert.IsTrue(await linha.IsVisibleAsync(),
				$"Plano de pagamento '{DescricaoPlanoPagamento}' não foi exibido na grid.");
		}
	}

	public async Task ValidarPlanoPagamentoPesquisadoComStatusAlterado(string status)
	{
		await Page.WaitForTimeoutAsync(2000);
		await Page.MainFrame.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new[] { status });
		await this.ValidarPlanoPagamentoPesquisado();
	}

	public async Task VerificarValidacaoCampo(string campo)
	{
		var locator = Page.MainFrame.Locator("#form0 > div.card.mb-5.h-100 > div > div:nth-child(14) > div.col-xl-7 > label > span.validationMessage.ms-auto");
		Assert.IsTrue(await locator.IsVisibleAsync(), $"Validação para campo '{campo}' não foi exibida.");
	}

	public async Task ValidarTelaAterrissagem()
	{
		var gridAterrissagem = Page.MainFrame.Locator("#formulario_aterrissagem");
		Assert.IsTrue(await gridAterrissagem.IsVisibleAsync(), "Tela de aterrissagem não foi exibida.");
	}

	#endregion

	#region Ações Gerais

	public async Task InformarDescricao(string descricao)
	{
		string dataHora = DateTime.Now.ToString("dd|HH:mm:ss");
		DescricaoPlanoPagamento = $"{descricao} {dataHora}";
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Descrição *" }).FillAsync(DescricaoPlanoPagamento);
	}

	public async Task AlterarAnoDeReferencia(string Ano)
	{
		await Page.WaitForTimeoutAsync(500);
		await Page.MainFrame.GetByLabel("Ano referência").SelectOptionAsync(new[] { Ano });
	}

	public async Task SelecionarFinalidade(string finalidade)
	{
		await Page.WaitForTimeoutAsync(1000);
		finalidade = finalidade.ToUpper();
		await Page.MainFrame.Locator("#FinalidadeDuplicataPesquisar").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Finalidade" }).PressSequentiallyAsync(finalidade);
		await Page.WaitForTimeoutAsync(1000);

		var GridFinalidades = Page.MainFrame.Locator("#GridFinalidadeDuplicata > div.em-grid-container");
		Assert.IsTrue(await GridFinalidades.IsVisibleAsync(), "Grid de finalidades nao foi exibida.");

		var finalidadeSelecionada = await GridFinalidades.GetByText(finalidade, new() { Exact = true }).IsVisibleAsync();
   		Assert.IsTrue(finalidadeSelecionada, "Finalidade nao foi selecionada.");

		await Page.MainFrame.GetByText(finalidade, new() { Exact = true }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}

	public async Task AlterarFinalidade(string finalidade)
	{
		await this.SelecionarFinalidade(finalidade);
	}

	public async Task InserirValorMensal(string valor)
	{
		var campo = Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Valor mensal" });
		await campo.FillAsync(""); // Limpa o campo antes de inserir o valor
		await campo.TypeAsync(valor); // Insere o valor desejado
	}

	public async Task InserirParcelas(string parcelas)
	{
		var campo = Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Parcelas" });
		await campo.FillAsync(""); // Limpa o campo antes de inserir o valor
		await campo.TypeAsync(parcelas); // Insere o valor desejado
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task SelecionarDataPrimeiraParcela(string data)
	{
		await Page.MainFrame.GetByLabel("Data 1ª parcela").SelectOptionAsync(new[] { data });
	}

	public async Task InserirQtdeDiasAMais(string dias)
	{
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Qtde. dias a mais " }).FillAsync(dias);
	}

	public async Task SelecionarOutrosVencimentos(string tipo, string dia)
	{
		await Page.MainFrame.GetByLabel("Outros vencimentos").SelectOptionAsync(new[] { tipo });
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Dia", Exact = true }).FillAsync(dia);
	}

	public async Task SelecionarPagamento(string pagamento)
	{
		await Page.WaitForTimeoutAsync(500);
		await Page.MainFrame.GetByLabel("Pagamento").SelectOptionAsync(new[] { pagamento });
	}

	public async Task SelecionarContaContabilConf(string opcao)
	{
		await Page.WaitForTimeoutAsync(500);
		await Page.MainFrame.Locator("#ObterContaContabilAPartirConfiguracoesSistema").GetByText(opcao).ClickAsync();
	}

	public async Task SelecionarTipoContaContabil(string tipo)
	{
		await Page.WaitForTimeoutAsync(500);
		await Page.MainFrame.GetByLabel("Tipo de conta contábil").SelectOptionAsync(new[] { tipo });
	}

	public async Task SelecionarInformacaoBancaria(string info)
	{
		await Page.WaitForTimeoutAsync(500);
		await Page.MainFrame.Locator("#TipoConta_FiltroTipoContaInformacaoBancariaContaContabilPesquisar").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Informação bancária" }).FillAsync(info);
		await Page.MainFrame.GetByText(info).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task SelecionarContaContabil(string conta)
	{
		await Page.WaitForTimeoutAsync(500);
		await Page.MainFrame.Locator("#TipoConta_FiltroTipoContaContaContabilPesquisar").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Conta contábil" }).FillAsync(conta);
		await Page.MainFrame.GetByText(conta).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task SelecionarSerie(string serie)
	{
		await Page.MainFrame.GetByLabel("Série").SelectOptionAsync(new[] { serie });
		await Page.WaitForTimeoutAsync(500);
	}

	public async Task SelecionarTurma(string turma)
	{
		await Page.MainFrame.GetByLabel("Turma").SelectOptionAsync(new[] { turma });
		await Page.WaitForTimeoutAsync(500);
	}

	public async Task ClicarEm(string Botao)
	{
		switch (Botao)
		{
			case "Cadastrar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
				break;
			case "Gravar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
				break;
			case "Voltar para a lista":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
				await Page.WaitForTimeoutAsync(1000);
				break;
			case "Sim":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Sim" }).ClickAsync();
				break;
			case "Não":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Não" }).ClickAsync();
				break;
			case "Cancelar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cancelar" }).ClickAsync();
				break;
			case "Adicionar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Adicionar" }).ClickAsync();
				await Page.WaitForTimeoutAsync(1000);
				break;
			case "Confirmar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Confirmar" }).ClickAsync();
				await Page.WaitForTimeoutAsync(1000);
				break;
			case "Associar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Associar" }).ClickAsync();
				break;
			case "Clonar":
				await Page.MainFrame.Locator("//*[@id=\"grid-body\"]/div[1]/div[4]/button[1]").ClickAsync();
				DescricaoPlanoPagamento += " (CLONADO)";
				break;
			case "Editar":
				await Page.MainFrame.Locator("//*[@id=\"grid-body\"]/div[1]/div[4]/button[2]").ClickAsync();
				break;
			case "Remover":
				await Page.MainFrame.Locator("//*[@id=\"grid-body\"]/div[1]/div[4]/button[3]").ClickAsync();
				break;

			default:
				Console.WriteLine($"Botão '{Botao}' não está programado.");
				break;
		}
	}

	public async Task ClicarEmAtivo(string Botao)
	{
		switch (Botao)
		{
			case "Sim":
				await Page.MainFrame.Locator("#EstaAtivo").GetByText("Sim").ClickAsync();
				break;
			case "Não":
				await Page.MainFrame.Locator("#EstaAtivo").GetByText("Não").ClickAsync();
				break;
			default:
				Console.WriteLine($"Opção '{Botao}' não está programada.");
				break;
		}
	}

	public async Task BotoesEspeciaisDoXPath(string Botao)
	{
		switch (Botao)
		{
			case "Editar":
				await Page.MainFrame.Locator("//*[@id=\"grid-body\"]/div/div[4]/button[1]").ClickAsync();
				break;

			default:
				Console.WriteLine($"Opção '{Botao}' não está programada.");
				break;
		}
	}
	#endregion

}
