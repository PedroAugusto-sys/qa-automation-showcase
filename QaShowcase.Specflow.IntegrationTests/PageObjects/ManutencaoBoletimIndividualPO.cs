using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;


namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class ManutencaoBoletimIndividualPO : POBase
{
	public ManutencaoBoletimIndividualPO(IPage page) : base(page)
	{ 
	}

	public override string PageUrl => "LancamentoBoletimIndividual";

	public async Task selecionoOAnoLetivo(string anoLetivo)
	{
		await Page.MainFrame.GetByLabel("Ano letivo *").SelectOptionAsync(new[] { anoLetivo });
	}

	public async Task selecionoOAluno()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "" }).ClickAsync();
		await Page.MainFrame.Locator("#DropDownAtivoFiltroAluno").SelectOptionAsync(new[] { "1" });
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}

	public async Task clicoEmCarregar()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Carregar" }).ClickAsync();
	}

	public async Task realizoOLancamentoDeNotas()
	{
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(1) > td:nth-child(2) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(2) > td:nth-child(2) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(4) > td:nth-child(2) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(5) > td:nth-child(2) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(6) > td:nth-child(2) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(7) > td:nth-child(2) > input").FillAsync("10");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(8) > td:nth-child(2) > input").FillAsync("10");
	}

	public async Task realizoOLancamentoDeFaltas()
	{
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(1) > td:nth-child(4) > input").FillAsync("2");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(2) > td:nth-child(4) > input").FillAsync("2");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(4) > td:nth-child(4) > input").FillAsync("2");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(5) > td:nth-child(4) > input").FillAsync("2");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(6) > td:nth-child(4) > input").FillAsync("2");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(7) > td:nth-child(4) > input").FillAsync("2");
		await Page.MainFrame.Locator("#tabela-boletim > .table > .em-corpo-tabela > tr:nth-child(8) > td:nth-child(4) > input").FillAsync("2");
	}

	public async Task clicoEmGravar()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
	}

	public async Task clicoEmSim()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
	}

	public async Task oBoletimEGravadoComSucesso()
	{
		await Expect(Page.MainFrame.GetByText("Boletim gravado com sucesso!")).ToBeVisibleAsync();
	}

	public async Task clicoEmEmitir()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Emitir" }).ClickAsync();
	}

	public async Task clicoEmEmitirNovamente()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Emitir" }).ClickAsync();
	}

	public async Task oBoletimEEmitidoComSucesso()
	{
		await Expect(Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Fechar" })).ToBeEnabledAsync();
	}

	public async Task clicoEmRecalcular()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Recalcular" }).ClickAsync();
	}

	public async Task oRecalculoDoBoletimERealizado()
	{
		await Expect(Page.MainFrame.GetByText("Operação realizada com sucesso.")).ToBeVisibleAsync();
	}
}
