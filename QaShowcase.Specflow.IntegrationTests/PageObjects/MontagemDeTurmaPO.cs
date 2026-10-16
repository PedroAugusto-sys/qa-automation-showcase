using Microsoft.Playwright;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Assist;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class MontagemDeTurmaPO : POBase
{
	public MontagemDeTurmaPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroTurmaMontada";

	public async Task ClicoEmCadastrar()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task SelecionoASerie(string serie)
	{
		await Page.MainFrame.GetByText("Série *").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByLabel("Série *").TypeAsync(serie);
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task SelecionoATurma(string turma)
	{
		await Page.MainFrame.GetByText("Turma *").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByLabel("Turma *").TypeAsync(turma);
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task InformoOTurno()
	{
		await Page.MainFrame.GetByLabel("Turno *").SelectOptionAsync(new[] { "0" });
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task InformoALotacao(string lotacao)
	{
		await Page.MainFrame.GetByLabel("Lotação *").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByLabel("Lotação *").FillAsync(lotacao);
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task ClicoEmGravar()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task OCadastroEhRealizadoComSucesso(string lotacao)
	{
		await Page.MainFrame.GetByText("Cadastro realizado com sucesso!").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByText(lotacao).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task SelecionoAMontagemDeTurmaQueDesejoEditar(string lotacao)
	{
		await Page.MainFrame.GetByText(lotacao).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.Locator("body").PressAsync("Tab");
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.Locator("body").PressAsync("Enter");
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task InformoANovaLotacao(string lotacao)
	{
		await Page.MainFrame.GetByLabel("Lotação *").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByLabel("Lotação *").FillAsync(lotacao);
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task AEdicaoEhRealizadaComSucesso(string lotacao)
	{
		await Page.MainFrame.GetByText("Cadastro realizado com sucesso!").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByText(lotacao).ClickAsync();
	}
	public async Task ClicoEmExcluir(string lotacao)
	{
		await Page.MainFrame.GetByText(lotacao).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.Locator("body").PressAsync("Tab");
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.Locator("body").PressAsync("Tab");
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.Locator("body").PressAsync("Enter");
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task ClicoEmSim()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task AExclusaoEhRealizadaComSucesso()
	{
		await Page.MainFrame.GetByText("Item excluído com sucesso.").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}
}