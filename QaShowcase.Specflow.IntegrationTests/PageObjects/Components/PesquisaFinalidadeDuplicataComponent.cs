using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
internal class PesquisaFinalidadeDuplicataComponent : ComponentBase
{
	public PesquisaFinalidadeDuplicataComponent(IPage page) : base(page)
	{
	}

	public ILocator FinalidadeDuplicataPesquisar => Page.Locator("#FinalidadeDuplicataPesquisar");
	public ILocator FinalidadeDuplicataPesquisa => Page.Locator("#FinalidadeDuplicataPesquisa");
	public ILocator FinalidadeDuplicataSelecionar => Page.Locator("#FinalidadeDuplicataSelecionar");
	public ILocator GridFinalidadeDuplicata => Page.Locator("#GridFinalidadeDuplicata");

	internal async Task SelecioneFinalidade(string nome)
	{
		await FinalidadeDuplicataPesquisar.ClickAsync();
		await FinalidadeDuplicataPesquisa.FillAsync(nome);
		await GridFinalidadeDuplicata.GetByText(nome, new() { Exact = true }).ClickAsync();
		await FinalidadeDuplicataSelecionar.ClickAsync();
	}
}
