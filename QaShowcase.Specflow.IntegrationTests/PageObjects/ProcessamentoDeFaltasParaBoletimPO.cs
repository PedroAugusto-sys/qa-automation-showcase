using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class ProcessamentoDeFaltasParaBoletimPO : POBase
{
	public ProcessamentoDeFaltasParaBoletimPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "ProcessamentoFaltasBoletim";

	public async Task ClicoEmProcessarAsFaltasParaOBoletim()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Processar" }).ClickAsync();
	}

	public async Task OSistemaApresentaAMensagem()
	{
		await Page.MainFrame.GetByText("Processamento concluído com sucesso!").ClickAsync();
	}

	public async Task SelecionoASerieParaRealizarOProcessamentoDeFaltas()
	{
		await Page.MainFrame.GetByLabel("Filtrar por").SelectOptionAsync(new[] { "serie" });
		await Page.MainFrame.GetByLabel("Série").SelectOptionAsync(new[] { "15" });
	}
}