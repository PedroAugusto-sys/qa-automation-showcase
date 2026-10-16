using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects.Components;

internal class GridComponent : ComponentBase
{
	public GridComponent(IPage page, string idGrid) : base(page)
	{
		IdGrid = idGrid;
		Grid = page.Locator(idGrid);
	}

	public string IdGrid { get; }

	public ILocator Grid { get; }

	public async Task<IReadOnlyList<ILocator>> ObtenhaLinhasGrid()
	{
		ILocator rowsLocator = Grid.Locator(".em-grid-body-main > .em-grid-body > .em-grid-row");
		return await rowsLocator.AllAsync();
	}
}
