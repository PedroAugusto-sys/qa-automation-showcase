using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class RemanejamentoIndividualPO : POBase
{
	private string? Aluno = null;
	private string? SerieDestino = null;
	public RemanejamentoIndividualPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "RemanejamentoAlunoIndividual";

	public async Task SelecionoOAlunoNaTelaDeRemanejamento()
	{
		string aluno = "2200617";
		Aluno = aluno;

		await Page.MainFrame.Locator("#AlunoPesquisar").ClickAsync();
		await Page.MainFrame.GetByLabel("Aluno").ClickAsync();
		await Page.MainFrame.GetByLabel("Aluno").FillAsync(Aluno);
		await Page.MainFrame.Locator("#DropDownAtivoFiltroAluno").SelectOptionAsync(new[] { "1" });
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}
	public async Task SelecionoASerieTurmaParaRemanejamento()
	{
		string serieDestino = "#Serie_Descricao_75";
		SerieDestino = serieDestino;

		await Page.MainFrame.Locator("#TurmaMontadaPesquisar").ClickAsync();
		await Page.MainFrame.GetByLabel("Série / Turma para remanejamento").ClickAsync();
		await Page.MainFrame.Locator(SerieDestino).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}
	public async Task ClicoEmCarregarRemanejamento()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Carregar" }).ClickAsync();
	}

	public async Task EConfiguroAsOpcoesDeRemanejamento()
	{
		await Page.MainFrame.Locator("#RegistrarHistoricoRemanejamento").GetByText("Sim").ClickAsync();
	}
	public async Task ClicoEmGravarRemanejamento()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Confirmar" }).ClickAsync();
	}

	public async Task ORemanejamentoEhRealizadoComSucesso()
	{
		await Page.MainFrame.GetByText("Remanejamento concluído com sucesso.").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await SelecionoOAlunoNaTelaDeRemanejamento();
		await VerificoSerieAtual("2ª SÉRIE / EM2MA");
	}

	public async Task ClicoEmEstornarORemanejamento()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Estornar" }).ClickAsync();
	}

	public async Task ClicoEmSim()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
	}

	public async Task OEstornoDoRemanejamentoEhRealizadoComSucesso()
	{
		await Page.MainFrame.GetByText("Estorno da transferência realizada com SUCESSO! Verifique os LANÇAMENTOS DE FREQ").ClickAsync();
	}
	public async Task ClicoNoBotaoFinanceiroNaTelaDeRemanejamentoIndividual()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Financeiro" }).ClickAsync();
	}

	public async Task ATelaDeGeracaoDeDuplicatasEhAberta()
	{
		var page1 = await Page.RunAndWaitForPopupAsync(async () =>
		{
			await Page.GetByRole(AriaRole.Button, new() { Name = " Financeiro" }).ClickAsync();
		});
	}

	private async Task VerificoSerieAtual(string serie)
	{
		await Page.MainFrame.Locator("#AlunoInfoRapidas").ClickAsync();
		await Page.MainFrame.GetByText(serie).ClickAsync();
	}
}