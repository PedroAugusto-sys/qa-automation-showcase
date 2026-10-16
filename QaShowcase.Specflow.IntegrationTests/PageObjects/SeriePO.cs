using Microsoft.Playwright;
using System.Runtime.ConstrainedExecution;
using TechTalk.SpecFlow.Assist;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class SeriePO : POBase
{
	public SeriePO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroSerie";
	private async Task OrdenarRegistroDescendentemente()
	{
		await Page.MainFrame.Locator("css=.em-grid-cell:nth-child(1) > .btn").ClickAsync();
		await Page.WaitForTimeoutAsync(500);
		await Page.MainFrame.Locator("css=.em-grid-cell:nth-child(1) > .btn").ClickAsync();
		await Page.WaitForTimeoutAsync(500);
	}

	private async Task PesquisarSerie(string serie)
	{
		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
		await Page.MainFrame.GetByPlaceholder("Pesquisar").FillAsync("");
		await Page.MainFrame.GetByPlaceholder("Pesquisar").TypeAsync($"{serie}");
	}
	public async Task ClicoEmCadastrarASerie()
	{
		await Page.WaitForTimeoutAsync(1200);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
	}

	public async Task InformoADescricaoSerieDataAtual()
	{
		DateTime dataAtual = DateTime.Now;

		await Page.MainFrame.GetByLabel("Descrição *").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição *").FillAsync("Serie " + dataAtual);
	}

	public async Task InformoACargaHoraria()
	{
		await Page.MainFrame.GetByLabel("CH").ClickAsync();
		await Page.MainFrame.GetByLabel("CH").FillAsync("1000");
		await Page.WaitForTimeoutAsync(800);
	}

	public async Task ClicoEmGravarOCadastroDaSerie()
	{
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
	}

	public async Task OCadastroDaSerieEhRealizadoComSucesso()
	{
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByText("Cadastro realizado com sucesso!").ClickAsync();
	}

	public async Task AdicionoDisciplinasNoCadastroDaSerie()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Adicionar" }).ClickAsync();
		await Page.MainFrame.Locator("css=.even:nth-child(1) > .em-grid-cell > .fal").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}

	public async Task ClicoEmEditarASerie()
	{
		await Page.WaitForTimeoutAsync(500);
		await OrdenarRegistroDescendentemente();
		await Page.MainFrame.Locator("css=.em-grid-row:nth-child(1) .btn-outline-success > .fal").ClickAsync();
	}

	public async Task EditoADescricaoDaSerie()
	{
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByLabel("Descrição *").FillAsync("Editado!");
	}

	public async Task SerieEhEditadaComSucesso()
	{
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
	}

	public async Task PesquisoASerieQueDesejoExcluir()
	{
		await PesquisarSerie("Editado");
	}

	public async Task ClicoEmExcluirASerie()
	{
		await Page.MainFrame.Locator("css=.em-grid-row:nth-child(1) .btn-outline-danger > .fal").ClickAsync();
	}

	public async Task ClicoEmSimParaExcluirASerie()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
	}

	public async Task SerieEhExcluidaComSucesso()
	{
		await Page.MainFrame.GetByRole(AriaRole.Dialog).GetByText("Item excluído com sucesso.").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		// Loop para excluir todas as séries com a mesma descrição
		while (await Page.MainFrame.Locator("css=.em-grid-row:nth-child(1) .btn-outline-danger > .fal").IsVisibleAsync())
		{
			await ClicoEmExcluirASerie();
			await Page.WaitForTimeoutAsync(1000);
			await ClicoEmSimParaExcluirASerie();
			await Page.WaitForTimeoutAsync(1000);
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
			await PesquisoASerieQueDesejoExcluir();
			await Page.WaitForTimeoutAsync(1000);
		}
	}
}