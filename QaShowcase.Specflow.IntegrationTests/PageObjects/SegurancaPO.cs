using System;
using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class SegurancaPO : POBase
{
	public SegurancaPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "ConfiguracoesSistemaGeralSeguranca";

	public async Task selecionarUtilizarSenhaPadrao(string opcaoSelecionada)
	{
		// Sempre garante que estamos na página correta navegando explicitamente
		await GotoAsync();
		
		// Aguarda a página estar totalmente carregada
		await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
		await Page.WaitForTimeoutAsync(3000);
		
		// Verifica se realmente está na página de segurança
		var currentUrl = Page.Url;
		if (!currentUrl.Contains("Seguranca", StringComparison.OrdinalIgnoreCase))
		{
			// Se não estiver, tenta navegar novamente
			await GotoAsync();
			await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
			await Page.WaitForTimeoutAsync(3000);
		}
		
		// Aguarda o label estar visível antes de tentar interagir
		var labelLocator = Page.MainFrame.GetByLabel("Utilizar senha padrão no cadastro de operadores");
		await labelLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 30000 });
		
		if (opcaoSelecionada == "Sim")
		{
			// Aguarda o texto "Sim" estar visível e clica
			var simLocator = labelLocator.GetByText("Sim");
			await simLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 30000 });
			await simLocator.ClickAsync();
			
			// Aguarda um pouco para o campo de senha aparecer
			await Page.WaitForTimeoutAsync(1000);
			
			// Aguarda o botão de expandir estar visível e clica
			var botaoExpandir = Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "" }).First;
			await botaoExpandir.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
			await botaoExpandir.ClickAsync();
			
			// Aguarda o campo de senha estar visível e preenche
			var senhaLocator = Page.MainFrame.GetByLabel("Senha padrão", new() { Exact = true });
			await senhaLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
			await senhaLocator.ClickAsync();
			await senhaLocator.FillAsync(TestConfig.DefaultOperatorPassword);
		}
		else if(opcaoSelecionada == "Nao")
		{
			// Aguarda o texto "Não" estar visível e clica
			var naoLocator = labelLocator.GetByText("Não");
			await naoLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 30000 });
			await naoLocator.ClickAsync();
		}
	}

	public async Task selecionarObrigarMudarSenhaPrimeiroAcesso(string opcaoSelecionada)
	{
		// Sempre garante que estamos na página correta navegando explicitamente
		await GotoAsync();
		
		// Aguarda a página estar totalmente carregada
		await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
		await Page.WaitForTimeoutAsync(3000);
		
		// Verifica se realmente está na página de segurança
		var currentUrl = Page.Url;
		if (!currentUrl.Contains("Seguranca", StringComparison.OrdinalIgnoreCase))
		{
			// Se não estiver, tenta navegar novamente
			await GotoAsync();
			await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
			await Page.WaitForTimeoutAsync(3000);
		}
		
		// Aguarda o label estar visível antes de tentar interagir
		var labelLocator = Page.MainFrame.GetByLabel("Obrigar mudança de senha no primeiro login");
		await labelLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 30000 });
		
		if (opcaoSelecionada == "Sim")
		{
			// Aguarda o texto "Sim" estar visível e clica
			var simLocator = labelLocator.GetByText("Sim");
			await simLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 30000 });
			await simLocator.ClickAsync();
		}
		else if (opcaoSelecionada == "Nao")
		{
			// Aguarda o texto "Não" estar visível e clica
			var naoLocator = labelLocator.GetByText("Não");
			await naoLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 30000 });
			await naoLocator.ClickAsync();
		}
	}

	public async Task clicarEmGravar()
	{
		await Page.WaitForTimeoutAsync(2000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
	}


	public async Task confirmacaoSucesso()
	{
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByText("Cadastro realizado com sucesso!").ClickAsync();
	}

}
