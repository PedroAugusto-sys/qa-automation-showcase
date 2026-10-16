using Microsoft.Playwright;
using NUnit.Framework;
using TechTalk.SpecFlow.CommonModels;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class EmpresaPO : POBase
{
	public EmpresaPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroEmpresa";

	public async Task ClicoEmEditarNoCadastroDeEmpresa()
	{
		await Page.MainFrame.Locator("css=.fa-edit").ClickAsync();
	}

	public async Task AlteroOApelido(string apelido)
	{
		await Page.MainFrame.GetByLabel("Apelido").ClickAsync();
		await Page.MainFrame.GetByLabel("Apelido").FillAsync(apelido);
	}

	public async Task ClicoEmGravarAEdicaoDaEmpresa()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task OCadastroEAlteradoComSucesso()
	{
		string mensagem = "Edição realizada com sucesso!";
		var text = await Page.MainFrame.Locator("css=.modal-dialog-centered > .border-0 > .modal-body:nth-child(2)").InnerTextAsync();
		Assert.AreEqual(mensagem, text);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}
}
