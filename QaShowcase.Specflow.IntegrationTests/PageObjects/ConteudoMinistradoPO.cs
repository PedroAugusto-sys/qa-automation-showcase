using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class ConteudoMinistradoPo : POBase
{
	public ConteudoMinistradoPo(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroLancamentoConteudoMinistrado";

	public async Task ConfiguroOsFiltrosDoConteudoMinistrado()
	{
		await Page.MainFrame.GetByLabel("Série *").SelectOptionAsync(new[] { "15" });
		await Page.MainFrame.Locator("#DisciplinaPesquisar").ClickAsync();
		await Page.MainFrame.GetByText("BIOLOGIA").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}
	public async Task ClicoEmCarregarOConteudoMinistrado()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Carregar" }).ClickAsync();
	}

	public async Task SelecionoParaReplicarOConteudoMinistrado()
	{
		await Page.MainFrame.GetByText("Sim").ClickAsync();
	}

	public async Task RealizoOLancamentoDoConteudoMinistradoReplicado()
	{
		await Page.MainFrame.GetByText("06 [segunda-feira] - 12:01 as 12:50").ClickAsync();

		await Page.MainFrame.GetByLabel("Conteúdo").ClickAsync();

		await Page.MainFrame.GetByLabel("Conteúdo").FillAsync("replicado");
	}
	public async Task RealizoOLancamentoDoConteudoMinistrado()
	{
		await Page.MainFrame.GetByText("Não").ClickAsync();
		await Page.MainFrame.GetByLabel("Conteúdo").ClickAsync();
		await Page.MainFrame.GetByLabel("Conteúdo").FillAsync("teste");
	}
	public async Task ClicoEmGravarOConteudoMinistrado()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
	}
	public async Task ATelaDeConteudoMinistradoApresentaAMensagem()
	{
		await Page.MainFrame.GetByText("Conteúdos ministrados gravados com sucesso!").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await ValidoOConteudoMinistradoLancado();
	}
	public async Task OConteudoMinistradoEhReplicadoComSucesso()
	{
		await Page.MainFrame.GetByText("Conteúdos ministrados gravados com sucesso!").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await ValidoOConteudoMinistradoReplicado();
	}
	public async Task EditoOConteudoMinistrado()
	{
		await Page.MainFrame.GetByText("Não").ClickAsync();
		await Page.MainFrame.GetByLabel("Conteúdo").ClickAsync();
		await Page.MainFrame.GetByLabel("Conteúdo").FillAsync("editado");
	}

	public async Task OConteudoMinistradoEhEditado()
	{
		await Page.MainFrame.GetByText("Conteúdos ministrados gravados com sucesso!").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await ValidoOConteudoMinistradoEditado();
	}
	private async Task ValidoOConteudoMinistradoLancado()
	{
		await ClicoEmCarregarOConteudoMinistrado();
		await Page.MainFrame.GetByText("teste").ClickAsync();
	}
	private async Task ValidoOConteudoMinistradoEditado()
	{
		await ClicoEmCarregarOConteudoMinistrado();
		await Page.MainFrame.GetByText("editado").ClickAsync();
	}

	private async Task ValidoOConteudoMinistradoReplicado()
	{
		await Page.MainFrame.GetByLabel("Turma *").SelectOptionAsync(new[] { "352" });
		await Page.MainFrame.Locator("#DisciplinaPesquisar").ClickAsync();
		await Page.MainFrame.GetByText("BIOLOGIA").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
		await ClicoEmCarregarOConteudoMinistrado();
		await Page.MainFrame.GetByText("replicado").ClickAsync();
	}
}