using Microsoft.Playwright;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Assist;
using static Microsoft.Playwright.Assertions;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class ProgramacaoDeAvaliacoesPO : POBase
{
	public ProgramacaoDeAvaliacoesPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "ProgramacaoAvaliacaoProfessoresAterrisagem";
	
	public async Task ClicoEmNovo()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Novo" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		//var Page2 = await Page.RunAndWaitForNavigationAsync();
	}
	public async Task SelecionoASerie()
	{
		await Page.GetByText("Série *").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByText("Série *").TypeAsync("Serie");
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task SelecionoATurma(string turma)
	{
		await Page.GetByText("Turma *").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByText("Turma *").TypeAsync(turma);
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task InformoAEtapa(string etapa)
	{
		await Page.GetByText("Etapa *").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByLabel("Etapa *").SelectOptionAsync(new[] { etapa });
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task InformoATabelaDeAvaliacao(string tabelaDescricao)
	{
		await Page.Locator("#TabelaAvaliacaoPesquisar").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByText(tabelaDescricao).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task InformoADisciplina(string disciplinaDescricao)
	{
		await Page.Locator("#DisciplinaPesquisar").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByText(disciplinaDescricao).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task ClicoEmCarregar()
	{
		await Page.GetByRole(AriaRole.Button, new() { Name = " Carregar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task InformoOProfessor()
	{
		await Page.GetByText("N1").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByRole(AriaRole.Button, new() { Name = "" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByText("Sim").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByRole(AriaRole.Button, new() { Name = " Confirmar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task ClicoEmGravar()
	{
		await Page.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task AProgramacaoEhRealizadaComSucesso()
	{
		await Page.GetByText("Programação efetuada com sucesso!").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
	}
}