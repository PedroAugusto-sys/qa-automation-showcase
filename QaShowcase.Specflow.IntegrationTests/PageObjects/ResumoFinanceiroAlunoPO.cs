using QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class ResumoFinanceiroAlunoPO : POBase
{
	private readonly PesquisaFinalidadeDuplicataComponent _pesquisaFinalidadeDuplicataComponent;
	private readonly PesquisaAlunoComponent _pesquisaAlunoComponent;

	public ResumoFinanceiroAlunoPO(IPage page) : base(page)
	{
		_pesquisaFinalidadeDuplicataComponent = new PesquisaFinalidadeDuplicataComponent(page);
		_pesquisaAlunoComponent = new PesquisaAlunoComponent(page);
		GridDuplicatas = new GridComponent(page, "#GridDuplicatas");
	}

	public override string PageUrl => "ResumoFinanceiroAluno";

	public GridComponent GridDuplicatas { get; }

	public async Task AguardeParaExibirAsDuplicatas() => await Page.WaitForSelectorAsync("#GridDuplicatas");
	public async Task CliqueEmCarregar() => await Page.GetByRole(AriaRole.Button, new() { Name = " Carregar" }).ClickAsync();
	public async Task SelecioneAluno(string matricula) => await _pesquisaAlunoComponent.SelecioneAluno(matricula);
	internal async Task SelecioneFinalidade(string nome) => await _pesquisaFinalidadeDuplicataComponent.SelecioneFinalidade(nome);

}
