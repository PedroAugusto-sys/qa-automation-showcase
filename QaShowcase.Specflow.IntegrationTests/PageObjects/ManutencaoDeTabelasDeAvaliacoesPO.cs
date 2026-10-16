using Microsoft.Playwright;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Assist;
using static Microsoft.Playwright.Assertions;
using NUnit.Framework;
using Bogus.Bson;
using System.Runtime.CompilerServices;
using System.Linq;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class ManutencaoDeTabelasDeAvaliacoesPO : POBase
{
	public ManutencaoDeTabelasDeAvaliacoesPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroTabelaAvaliacao";

	public string NomeUnico = null!;
	public string NomeAlteradoUnico = null!;

	// Constantes para os rótulos dos campos
	private const string BotaoCadastrar = " Cadastrar";
	private const string BotaoAvaliacao = " Avaliação";
	private const string BotaoConfirmar = " Confirmar";
	private const string BotaoEditarFormula = "Editar fórmula";
	private const string BotaoGravar = " Gravar";
	private const string BotaoVoltarParaALista = " Voltar para a lista";
	private const string BotaoSim = " Sim";

	public async Task ClicarEm(string botao)
	{
		switch (botao)
		{
			case "Cadastrar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoCadastrar }).ClickAsync();
				break;
			case "Avaliação":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoAvaliacao }).ClickAsync();
				break;
			case "Confirmar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoConfirmar }).ClickAsync();
				break;
			case "Editar fórmula":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoEditarFormula }).ClickAsync();
				break;
			case "Gravar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoGravar }).ClickAsync();
				break;
			case "Voltar para a lista":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoVoltarParaALista }).ClickAsync();
				break;
			case "Editar":
				await Page.MainFrame.GetByText(NomeUnico).ClickAsync();
				break;
			case "Remover":
				var linha = Page.MainFrame.Locator("#grid-body .em-grid-row").Filter(new() { HasTextString = NomeUnico });
				var excluirButton = linha.Locator("button[data-role='em-grid-remover']");
				await excluirButton.ClickAsync();
				break;
			case "Sim":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoSim }).ClickAsync();
				break;
			default:
				Console.WriteLine($"Botão '{botao}' não está programado.");
				break;
		}
	}

	public async Task TipoAvalicao(string tipo)
	{
		switch (tipo)
		{
			case "Normal":
				await Page.MainFrame.GetByLabel("Tipo tabela de avaliação").SelectOptionAsync(new[] { "N" });
				break;
			default:
				Console.WriteLine($"Tipo de avaliação '{tipo}' não está programado.");
				break;
		}
	}

	public async Task InformoONome(string Nome)
	{
		string dataHora = DateTime.Now.ToString("dd|HH:mm:ss");
		string descricao = $"{Nome} {dataHora}";
		this.NomeUnico = descricao;
		await Page.MainFrame.GetByLabel("Nome *").FillAsync(this.NomeUnico);
	}

	public async Task OpcaoTabelaAtiva(string TabelaAtiva)
	{
		await Page.MainFrame.Locator("#EstaAtivo").GetByText(TabelaAtiva).ClickAsync();
	}

	public async Task OpcaoTabelaPadrao(string TabelaPadrao)
	{
		await Page.MainFrame.Locator("#EhTabelaPadrao").GetByText(TabelaPadrao).ClickAsync();
	}

	public async Task CalcularAvaliacoesAnaliticas(string CalcAvAn)
	{
		await Page.MainFrame.GetByLabel("No lançamento de avaliações analíticas, calcular a média mesmo faltando notas.").GetByText(CalcAvAn).ClickAsync();
	}

	public async Task OpcaoTemplatePortalDeProfessor(string Template)
	{
		await Page.MainFrame.Locator("#CarregarTemplatesPortalProfessor").GetByText(Template).ClickAsync();
	}

	public async Task InserirValorSAV(string VlrSAV)
	{
		await Page.MainFrame.GetByLabel("Valor SAV").FillAsync(VlrSAV);
	}

	public async Task DescricaoResumida(string DescResumida)
	{
		await Page.MainFrame.GetByLabel("Descrição resumida").ClearAsync();
		await Page.MainFrame.GetByLabel("Descrição resumida").FillAsync(DescResumida);
	}

	public async Task Descricao(string Descricao)
	{
		await Page.MainFrame.Locator("#Descricao").ClearAsync();
		await Page.MainFrame.Locator("#Descricao").FillAsync(Descricao);
	}

	public async Task ValorMaximo(string VlrMax)
	{
		await Page.MainFrame.GetByLabel("Valor máximo").ClearAsync();
		await Page.MainFrame.GetByLabel("Valor máximo").TypeAsync(VlrMax);
	}

	public async Task AdicionarAvaliacoes(string Tipo)
	{
		await this.ClicarEm("Avaliação");
		await this.TipoAvalicao(Tipo);
		await this.DescricaoResumida("N");
		await this.Descricao("Descricao N");
		await this.ValorMaximo("100");
		await this.ClicarEm("Confirmar");
	}
	public async Task FormulaAceitaNulo(string Opcao)
	{
		await Page.MainFrame.Locator("#FormulaPermiteNulo").GetByText(Opcao).ClickAsync();
		await Page.WaitForTimeoutAsync(500);
	}

	public async Task InserirFormula(string Formula)
	{
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Fórmula", Exact = true }).FillAsync(Formula);
	}

	public async Task ValidarMensagem(string mensagem)
	{
		var elementoMensagem = await Page.MainFrame.GetByText(mensagem).IsVisibleAsync();
		Assert.IsNotNull(elementoMensagem, "Elemento de mensagem não encontrado na tela.");
	}

	public async Task CadastrarPesquisando(string NomeTabela)
	{
		await this.ClicarEm("Cadastrar");
		await this.TipoAvalicao("Normal");
		await this.InformoONome(NomeTabela);
		await this.OpcaoTabelaAtiva("Sim");
		await this.OpcaoTabelaPadrao("Não");
		await this.CalcularAvaliacoesAnaliticas("Não");
		await this.OpcaoTemplatePortalDeProfessor("Não");
		await this.ClicarEm("Avaliação");
		await this.TipoAvalicao("Normal");
		await this.DescricaoResumida("N");
		await this.Descricao("Descricao N");
		await this.ValorMaximo("100");
		await this.ClicarEm("Confirmar");
		await this.AdicionarAvaliacoes("Normal");
		await this.AdicionarAvaliacoes("Normal");
		await this.FormulaAceitaNulo("Sim");
		await this.ClicarEm("Editar fórmula");
		await this.InserirFormula("(N1+N2+N3)/4");
		await this.ClicarEm("Confirmar");
		await this.ClicarEm("Gravar");
		await this.ValidarMensagem("Cadastro realizado com sucesso!");
		await this.ClicarEm("Voltar para a lista");
		await Page.WaitForTimeoutAsync(800);
		await this.Pesquisar(NomeUnico);
	}

	public async Task Pesquisar(string Nome)
	{
		// Garante que está na tela correta
		await Page.MainFrame.GetByPlaceholder("Pesquisar").TypeAsync(Nome, new() { Delay = 40 });
		Assert.IsTrue(Page.MainFrame.Url.Contains("CadastroTabelaAvaliacao"), "Não está na tela de manutenção de tabelas de avaliação.");
		await Page.WaitForTimeoutAsync(1000);

		var GridTabelasDeAvaliacoesLocator = Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container > div.em-grid-body-main");
		Assert.IsNotNull(GridTabelasDeAvaliacoesLocator, "Locator da grid não encontrada.");

		var GridTabelasDeAvaliacoes = await GridTabelasDeAvaliacoesLocator.ElementHandleAsync();
		Assert.IsNotNull(GridTabelasDeAvaliacoes, "Grid de tabelas de avaliação não encontrada");
	}

	public async Task AlterarNome(string alterarNome)
	{
		string dataHora = DateTime.Now.ToString("dd|HH:mm:ss");
		string descricao = $"{alterarNome} {dataHora}";
		this.NomeAlteradoUnico = descricao; // Corrigido: atribui à propriedade da classe
		await Page.MainFrame.GetByLabel("Nome *").FillAsync(this.NomeAlteradoUnico);
	}

	public async Task ValidarAlteracaoDoStatusDaTabela()
	{
		await this.ClicarEm("voltar para a lista");
		await Page.MainFrame.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new[] { "2" });
		await Page.WaitForTimeoutAsync(800);
		await this.Pesquisar(this.NomeUnico);
	}

	public async Task ClicoEmRemoverATabelaDeAvaliacaoDesejada()
	{
		await Page.MainFrame.Locator("div").Filter(new() { HasTextRegex = new Regex("^15TABELA QA EDITADANãoNormal$") }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task ClicoEmSim()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
	}
	public async Task ATabelaEhExcluidaComSucesso(string nome)
	{
		await Page.MainFrame.GetByText("Item excluído com sucesso.").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.MainFrame.GetByPlaceholder("Pesquisar").TypeAsync(nome);
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByPlaceholder("Pesquisar").TypeAsync(" ");
		await Page.WaitForTimeoutAsync(1000);
		await Expect(Page.MainFrame.Locator("css=.em-grid-row:nth-child(1) .btn-outline-danger")).ToBeHiddenAsync();
		await Page.WaitForTimeoutAsync(600);
	}

}