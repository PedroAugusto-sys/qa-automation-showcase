using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
internal class ComponentBase
{
	public ComponentBase(IPage page)
	{
		Page = page;
	}

	public IPage Page { get; }
}
