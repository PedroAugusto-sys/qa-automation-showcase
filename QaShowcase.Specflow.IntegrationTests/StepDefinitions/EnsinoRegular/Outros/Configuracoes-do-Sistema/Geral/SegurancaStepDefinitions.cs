using QaShowcase.Specflow.IntegrationTests.PageObjects;
using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Outros.Configuracoes_do_Sistema.Geral;

[Binding]
public class SegurancaStepDefinitions : BaseStep
{
	private SegurancaPO _segurancaPage = null!;


	[Given(@"que desejo alterar a configuracao da senha padrao")]
	public async Task GivenQueDesejoAlterarAConfiguracaoDaSenhaPadrao()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();

		// Aguarda a página inicial carregar completamente após o login
		var loginPage = await POFactory.Crie<LoginPO>(Context);
		await loginPage.Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
		await loginPage.Page.WaitForTimeoutAsync(3000);

		// Navega para a página de segurança
		_segurancaPage = await POFactory.Crie<SegurancaPO>(Context);
		await _segurancaPage.GotoAsync();
		
		// Aguarda a página de segurança carregar completamente
		await _segurancaPage.Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
		await _segurancaPage.Page.WaitForTimeoutAsync(2000);
	}

	[Given(@"que desejo alterar a configuracao do primeiro login")]
	public async Task GivenQueDesejoAlterarAConfiguracaoDoPrimeiroLogin()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();


		// Aguarda a página inicial carregar completamente após o login
		var loginPage = await POFactory.Crie<LoginPO>(Context);
		await loginPage.Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
		await loginPage.Page.WaitForTimeoutAsync(3000);

		// Navega para a página de segurança
		_segurancaPage = await POFactory.Crie<SegurancaPO>(Context);
		await _segurancaPage.GotoAsync();
		
		// Aguarda a página de segurança carregar completamente
		await _segurancaPage.Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
		await _segurancaPage.Page.WaitForTimeoutAsync(2000);
	}


	[Given(@"seleciono a opcao '([^']*)'"), Scope(Tag = "AlterarOpcaoDeSenhaPadrao")]
	public async Task GivenSelecionoAOpcaoSenhaPadrao(string opcaoSelecionada)
	{
		await _segurancaPage.selecionarUtilizarSenhaPadrao(opcaoSelecionada);
	}

	[Given(@"seleciono a opcao '([^']*)'"), Scope(Tag = "AlterarOpcaoDePrimeiroLogin")]
	public async Task GivenSelecionoAOpcaoPrimeiroLogin(string opcaoSelecionada)
	{
		await _segurancaPage.selecionarObrigarMudarSenhaPrimeiroAcesso(opcaoSelecionada);
	}

	[When(@"clico em Gravar"), Scope(Tag = "Seguranca")]
	public async Task WhenClicoEmGravar()
	{
		await _segurancaPage.clicarEmGravar();
	}

	[Then(@"apresenta a mensagem de confirmacao")]
	public async Task ThenApresentaAMensagemDeConfirmacao()
	{
		await _segurancaPage.confirmacaoSucesso();
	}




}
