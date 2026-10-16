using Microsoft.Playwright;
using System.Globalization;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class EmissaoAtaRegMatriculaPO : POBase
{
	public EmissaoAtaRegMatriculaPO(IPage page) : base(page) { }

	public override string PageUrl => "EmissaoAtaRegistroMatricula";

	public async Task selecionarFiltro(string filtro)
	{

	}

	public async Task selecionarAno(string ano)
	{

	}

	public async Task selecionarEmpresa(string empresa)
	{

	}
	public async Task selecionarSerie(string serie)
	{

	}

	public async Task selecionarTurma(string turma)
	{

	}
	public async Task escreverTitulo(string titulo)
	{

	}

	public async Task selecionarLeiaute(string opcao)
	{

	}

	public async Task selecionarLeiauteAssinatura(string opcao)
	{

	}
	public async Task selecionarAssinaturas(string assinatura)
	{

	}

	public async Task clicarEmitir()
	{

	}

	public async Task fecharRelatorio()
	{
		await Page.WaitForTimeoutAsync(3000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Fechar" }).ClickAsync();

	}

}
