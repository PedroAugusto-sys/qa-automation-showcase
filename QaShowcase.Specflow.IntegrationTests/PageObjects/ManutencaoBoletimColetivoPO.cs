using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using Microsoft.Playwright.TestAdapter;
using QaShowcase.Specflow.IntegrationTests;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class ManutencaoBoletimColetivoPO : POBase
{
	public ManutencaoBoletimColetivoPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "LancamentoBoletimColetivo";

	public async Task selecionoASerie()
	{
		await Page.MainFrame.GetByLabel("Série *").SelectOptionAsync(new[] { "15" });
	}

	public async Task selecionoATurma()
	{
		await Page.MainFrame.GetByLabel("Turma *").SelectOptionAsync(new[] { "310" });
	}

	public async Task selecionoOAno()
	{
		await Page.MainFrame.GetByLabel("Ano").SelectOptionAsync(new[] { "2024" });
	}

	public async Task selecionoADisciplina()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "" }).ClickAsync();
		await Page.MainFrame.GetByText("ARTE", new() { Exact = true }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}

	public async Task clicoEmCarregar()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Carregar" }).ClickAsync();
	}

	public async Task realizoOLancamentoDeNotas()
	{
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(1) > td:nth-child(4) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(2) > td:nth-child(4) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(3) > td:nth-child(4) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(4) > td:nth-child(4) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(5) > td:nth-child(4) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(6) > td:nth-child(4) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(7) > td:nth-child(4) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(8) > td:nth-child(4) > input").FillAsync("10");
	}

	public async Task realizoOlancamentoDeFaltas()
	{
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(1) > td:nth-child(6) > input").FillAsync("3");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(2) > td:nth-child(6) > input").FillAsync("3");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(3) > td:nth-child(6) > input").FillAsync("3");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(4) > td:nth-child(6) > input").FillAsync("3");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(5) > td:nth-child(6) > input").FillAsync("3");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(6) > td:nth-child(6) > input").FillAsync("3");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(7) > td:nth-child(6) > input").FillAsync("3");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(8) > td:nth-child(6) > input").FillAsync("3");
	}

	public async Task clicoEmGravar()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
	}

	public async Task clicoEmSim()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
	}

	public async Task clicoEmRecalcular()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Recalcular" }).ClickAsync();
	}

	public async Task clicoEmEmitir()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Emitir" }).ClickAsync();
	}

	public async Task clicoEmBoletimIndividual()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Boletim individual" }).ClickAsync();
	}

	public async Task selecionoUmAluno()
	{
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(2) > td:nth-child(3)").ClickAsync();
	}

	public async Task oBoletimEGravadoComSucesso()
	{
		await Page.MainFrame.WaitForTimeoutAsync(5000);
		await Expect(Page.MainFrame.GetByText("Boletim gravado com sucesso!")).ToBeVisibleAsync();
	}

	public async Task oRecalculoDoBoletimERealizado()
	{
		await Page.MainFrame.WaitForTimeoutAsync(5000);
		await Expect(Page.MainFrame.GetByText("Operação realizada com sucesso!")).ToBeVisibleAsync();
	}

	public async Task aTelaDeEmissaoDoBoletimEscolarECarregada()
	{
		await Page.MainFrame.WaitForTimeoutAsync(5000);
		await Expect(Page.MainFrame.GetByRole(AriaRole.Heading, new() { Name = "Emissão do Boletim Escolar" })).ToBeVisibleAsync();
	}

	public async Task eApresentadoUmAlerta()
	{
		await Expect(Page.MainFrame.GetByText("É necessário selecionar um aluno")).ToBeVisibleAsync();
	}

	public async Task aTelaDeManutencaoDeBoletimIndividualECarregada()
	{
		var page2 = await Page.RunAndWaitForPopupAsync(async () =>
		{
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Boletim individual" }).ClickAsync();
		});
		await Expect(page2.GetByText("Manutenção de Boletim - individual", new() { Exact = true })).ToBeVisibleAsync();
	}
}
