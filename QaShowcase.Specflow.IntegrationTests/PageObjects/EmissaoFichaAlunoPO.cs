using Microsoft.Playwright;
using System.Security.Cryptography.X509Certificates;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class EmissaoFichaAlunoPO : POBase
{
	private string filtroSelecionado = null;
	public EmissaoFichaAlunoPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "EmissaoFichaAluno";

	public async Task selecionoFiltro(string filtro)
	{
		filtroSelecionado = filtro;

		switch (filtro)
		{
			case "Empresa":
				await Page.MainFrame.GetByLabel("Filtrar por").SelectOptionAsync(new[] { "empresa" });
			break;

			case "Série":
			    await Page.MainFrame.GetByLabel("Filtrar por").SelectOptionAsync(new[] { "serie" });
			break;

			case "Série/Turma":
				await Page.MainFrame.GetByLabel("Filtrar por").SelectOptionAsync(new[] { "serie-turma" });
			break;

			case "Aluno":
				await Page.MainFrame.GetByLabel("Filtrar por").SelectOptionAsync(new[] { "aluno" });
			break;

			default:
				await Page.MainFrame.GetByLabel("Filtrar por").SelectOptionAsync(new[] { "123" });
			break;
		}

		await Page.WaitForTimeoutAsync(2000);

	}


	public async Task selecionarEmpresa()
	{
		await Page.MainFrame.GetByLabel("Empresa", new() { Exact = true }).ClickAsync();

	}

	public async Task selecionarSerie(string serie)
	{
		if(filtroSelecionado == "Série")
		{
			await Page.MainFrame.Locator("#Filtro_Serie").SelectOptionAsync(new[] { "" + serie });
			await Page.WaitForTimeoutAsync(2000);
		}
		else if(filtroSelecionado == "Série/Turma")
		{
			await Page.MainFrame.Locator("#Filtro_SerieETurma_Serie").SelectOptionAsync(new[] { "" + serie });
			await Page.WaitForTimeoutAsync(2000);
		}
		else
		{
			//Gerar erro
			await Page.MainFrame.Locator("#form-control form-select").SelectOptionAsync(new[] { "" + serie });
			await Page.WaitForTimeoutAsync(2000);
		}
		
	}

	public async Task selecionarTurma(string turma)
	{
		await Page.MainFrame.GetByLabel("Turma", new() { Exact = true }).SelectOptionAsync(new[] { ""+turma });
		await Page.WaitForTimeoutAsync(2000);
	}

	public async Task selecionarAluno(int matricula)
	{
	    await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "" }).ClickAsync();
		await Page.MainFrame.GetByLabel("Aluno", new() { Exact = true }).ClickAsync();
		await Page.MainFrame.GetByLabel("Aluno", new() { Exact = true }).PressSequentiallyAsync(""+matricula);
		await Page.WaitForTimeoutAsync(2000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();

	}

	public async Task alterarVisibilidade(string visibilidade)
	{
		switch (visibilidade)
		{
			case "Mostrar":
				await Page.MainFrame.GetByText("Mostrar").ClickAsync();
			break;

			case "Ocultar":
				await Page.MainFrame.GetByText("Ocultar").ClickAsync();
			break;

			default:
				//Gerar erro
				await Page.MainFrame.GetByText("Gerar Erro").ClickAsync();
			break;
		}
	}

	public async Task clicaEmitir()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Emitir" }).ClickAsync();
		await Page.WaitForTimeoutAsync(2000);
	}

	public async Task fecharRelatorio()
	{
		await Page.WaitForTimeoutAsync(3000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Fechar" }).ClickAsync();

	}
}
