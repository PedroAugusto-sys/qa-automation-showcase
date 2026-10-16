using Microsoft.Playwright;
using NUnit.Framework;
namespace QaShowcase.Specflow.IntegrationTests.PageObjects;

internal class CidadesPO : POBase
{

	public CidadesPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroCidade";

	// Variáveis globais
	public string? CodigoIBGEVariavel = null;
	public string? NomeDaCidade = null;
	public string? NomeDaUF = null;

	// CADASTRO DE CIDADE COMPLETO =================================================

	public async Task CadastrarCidadeComCodIbge(string CodigoIBGE, string NomeCidade, string NomeUF, string NomeBotao)
	{
		await this.GotoAsync();
		await this.InformoCodigoDoIBGE(CodigoIBGE);
		await this.validarNomeDaCidade(NomeCidade, NomeUF);
		await this.ClicarEm(NomeBotao);
		await this.ValidarConfirmacaoDeCadastro("Registro cadastrado com sucesso!");
	}

	public async Task CadastrarCidadeSemCodIbge(string NomeCidade, string NomeUF, string NomeBotao)
	{
		await this.GotoAsync();
		await this.InserirNomeCidade(NomeCidade);
		await this.SelecionarUF(NomeUF);
		await this.ClicarEm(NomeBotao);
		await this.ValidarConfirmacaoDeCadastro("Registro cadastrado com sucesso!");
	}

	// VALIDAÇÕES ===================================================================

	public async Task validarNomeDaCidade(string NomeCidade, string UF)
	{
		NomeDaCidade = NomeCidade;

		await Page.WaitForTimeoutAsync(1000);

		// Valida a grid
		var GridLocator = Page.MainFrame.Locator("#GridCidadeIBGE > div.em-grid-container > div.em-grid-body-main");
		bool GridEstaVisivel = await GridLocator.IsVisibleAsync();
		Assert.IsTrue(GridEstaVisivel, "A grid não está visível na tela.");

		// Valida o código IBGE
		var CodigoIBGELocator = Page.MainFrame.Locator("#GridCidadeIBGE #Codigo_0");
		Assert.IsTrue(await CodigoIBGELocator.IsVisibleAsync(), "O elemento do código IBGE não está visível na tela.");
		string CodigoIBGETexto = await CodigoIBGELocator.InnerTextAsync();
		Assert.AreEqual(CodigoIBGEVariavel, CodigoIBGETexto, "Código IBGE não está visível!");

		// Valida o nome da cidade
		var NomeDaCidadeLocator = Page.MainFrame.Locator("#GridCidadeIBGE #Nome_0").GetByText(NomeCidade, new() { Exact = true });
		Assert.IsTrue(await NomeDaCidadeLocator.IsVisibleAsync(), "O elemento do nome da cidade não está visível na tela.");
		string NomeDaCidadeTexto = await NomeDaCidadeLocator.InnerTextAsync();
		Assert.AreEqual(NomeCidade, NomeDaCidadeTexto, "Nome da cidade não está visível!");

		// Valida a UF
		var UfLocator = Page.MainFrame.Locator("#GridCidadeIBGE #UF_Sigla_0");
		Assert.IsTrue(await UfLocator.IsVisibleAsync(), "O elemento da UF não está visível na tela.");
		string UfTexto = await UfLocator.InnerTextAsync();
		Assert.AreEqual(UF, UfTexto, "UF não está visível!");

		await this.ClicarEm("Selecionar");
	}

	public async Task ValidarConfirmacaoDeCadastro(string mensagemEsperada)
	{
		// Localiza o elemento que contém a mensagem de sucesso
		var mensagemDeValidacaoLocator = Page.MainFrame.Locator("css=.modal-dialog-centered .text-center");

		// Obtém o texto do elemento
		string mensagemApresentada = await mensagemDeValidacaoLocator.InnerTextAsync();

		// Compara o texto apresentado com o texto esperado
		Assert.AreEqual(mensagemEsperada, mensagemApresentada.Trim(), $"Falha ao cadastrar cidade! Mensagem esperada: '{mensagemEsperada}', Mensagem apresentada: '{mensagemApresentada}'");

		await Page.WaitForTimeoutAsync(1000);

		// Clica no botão "Voltar" 
		var botaoVoltarLocator = Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" });
		await botaoVoltarLocator.ClickAsync();
	}

	// ACÕES ========================================================================

	public async Task InserirNomeCidade(string NomeCidade)
	{
		NomeDaCidade = NomeCidade;
		await Page.MainFrame.GetByLabel("Nome da cidade *").FillAsync(NomeCidade);
	}

	public async Task SelecionarUF(string NomeUF)
	{
		NomeDaUF = NomeUF;
		await Page.MainFrame.GetByLabel("UF *").SelectOptionAsync(new[] { NomeUF });
	}

	public async Task ClicarEm(string Botao)
	{
		switch (Botao)
		{
			case "Cadastrar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
				break;
			case "Gravar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
				break;
			case "Voltar para a lista":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
				break;
			case "Selecionar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
				break;
			default:
				Console.WriteLine($"Botão '{Botao}' não está programado.");
				break;
		}
	}

	// Informar código IBGE
	public async Task InformoCodigoDoIBGE(string CodigoIBGE)
	{
		CodigoIBGEVariavel = CodigoIBGE; // Armazena o código IBGE na variável de instância
		await Page.MainFrame.Locator("#CidadeIBGEPesquisar").ClickAsync(); // Clico na lupa (Pesquisar)
		await Page.MainFrame.GetByLabel("Código do IBGE ou nome do município").ClickAsync(); // Clico no campo de texto
		await Page.WaitForTimeoutAsync(400);
		await Page.MainFrame.GetByLabel("Código do IBGE ou nome do município").PressSequentiallyAsync(CodigoIBGE); // Digito o código IBGE
	}

}
