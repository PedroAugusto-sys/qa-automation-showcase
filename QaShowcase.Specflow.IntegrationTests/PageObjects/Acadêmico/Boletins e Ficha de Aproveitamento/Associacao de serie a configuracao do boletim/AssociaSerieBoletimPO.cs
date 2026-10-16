using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects.Acadêmico.BoletinsEFichaDeAproveitamento.AssociacaoDeSerieAConfiguracaoDoBoletim;
internal class AssociacaoBoletimSeriePO : POBase
{
	public AssociacaoBoletimSeriePO(IPage page) : base(page) { }
	public override string PageUrl => "AssociacaoConfiguracaoBoletimSerie";

	#region Validações

	public async Task ValidarMensagem(string Mensagem)
	{
		await Page.WaitForTimeoutAsync(1200);
		var TextoMensagem = await Page.MainFrame.GetByText(Mensagem).IsVisibleAsync();
		Assert.IsTrue(TextoMensagem, "Texto da mensagem não encontrado na tela.");
	}

	#endregion

	#region Ações gerais

	public async Task SelecionarAnoLetivo(string AnoLetivo)
	{
		await Page.MainFrame.GetByLabel("Ano letivo").SelectOptionAsync(new[] { AnoLetivo });
	}

	public async Task SelecionarSerie(string Serie)
	{
		await Page.WaitForTimeoutAsync(800);
		var GridSeries = await Page.MainFrame.Locator("#GridAssociacaoSerieConfiguracaoBoletim > div.em-grid-container").IsVisibleAsync();
		Assert.IsTrue(GridSeries, "Grid de séries não visível!");
		var LocalizaSerieNaGrid = await Page.MainFrame.Locator("#GridAssociacaoSerieConfiguracaoBoletim").GetByText(Serie, new() { Exact = true }).IsVisibleAsync();
		Assert.IsTrue(LocalizaSerieNaGrid, "Série não localizada na grid!");

		await Page.MainFrame.Locator("#GridAssociacaoSerieConfiguracaoBoletim").GetByText(Serie, new() { Exact = true }).ClickAsync();
	}

	public async Task SelecionarConfiguracaoDoBoletim(string ConfiguracaoDoBoletim)
	{
		await Page.WaitForTimeoutAsync(800);
		await Page.MainFrame.GetByLabel("Configurações do boletim").SelectOptionAsync(new[] { ConfiguracaoDoBoletim });
	}

	public async Task SelecionarProgressaoParcial(string ProgressaoParcial)
	{
		await Page.WaitForTimeoutAsync(800);
		await Page.MainFrame.Locator("#UtilizaProgressaoParcial").GetByText(ProgressaoParcial).ClickAsync();
	}

	public async Task ClicarEm(string Botao)
	{
		await Page.WaitForTimeoutAsync(800);
		switch (Botao)
		{
			case "Carregar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Carregar" }).ClickAsync();
				break;
			case "Gravar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
				break;

			default:
				Console.WriteLine($"Botão '{Botao}' não está programado.");
				break;
		}
	}

	#endregion

}
