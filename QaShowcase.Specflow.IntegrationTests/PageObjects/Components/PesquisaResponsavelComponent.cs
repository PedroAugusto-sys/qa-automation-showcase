using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
internal class PesquisaAlunoComponent : ComponentBase
{
	public PesquisaAlunoComponent(IPage page) : base(page)
	{
	}

	public ILocator Botao_Pesquisa_AlunoPesquisar => Page.Locator("#Pesquisa_AlunoPesquisar");
	public ILocator Input_Pesquisa_AlunoPesquisa => Page.Locator("#Pesquisa_AlunoPesquisa");
	public ILocator Botao_Pesquisa_AlunoSelecionar => Page.Locator("#Pesquisa_AlunoSelecionar");

	public async Task SelecioneAluno(string matricula)
	{
		await Botao_Pesquisa_AlunoPesquisar.ClickAsync();
		await Input_Pesquisa_AlunoPesquisa.FillAsync(matricula);
		await Page.GetByText(matricula).ClickAsync();
		await Botao_Pesquisa_AlunoSelecionar.ClickAsync();
	}
}
