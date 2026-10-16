using FluentAssertions.Equivalency.Tracing;
using Microsoft.Playwright;
using Microsoft.Win32;
using NUnit;
using NUnit.Framework;
using System.Runtime.ConstrainedExecution;
using TechTalk.SpecFlow.Assist;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class TurmasPO : POBase
{
	public TurmasPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroTurma";

	private async Task OrdenarRegistroDescendentemente()
	{
		await Page.MainFrame.Locator("css=.em-grid-cell:nth-child(1) > .btn").ClickAsync();
		await Page.WaitForTimeoutAsync(500);
		await Page.MainFrame.Locator("css=.em-grid-cell:nth-child(1) > .btn").ClickAsync();
		await Page.WaitForTimeoutAsync(500);
	}
	public async Task PesquisarTurma(string turma)
	{
		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
		await Page.MainFrame.GetByPlaceholder("Pesquisar").FillAsync(turma);
		await Page.WaitForTimeoutAsync(500);

		if (await Page.MainFrame.Locator($".em-grid-cell.d-flex:has-text('{turma}')").IsVisibleAsync())
		{
			Assert.AreEqual(await Page.MainFrame.Locator($".em-grid-cell.d-flex:has-text('{turma}')").InnerTextAsync(), turma, "Nome da turma não é igual.");
			await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
			await Page.MainFrame.GetByPlaceholder("Pesquisar").FillAsync("");
			await Page.WaitForTimeoutAsync(500);
			await Page.MainFrame.GetByPlaceholder("Pesquisar").TypeAsync($"{turma}");

		}
		else
		{
			await ClicoEmObterOCodigoDaTurma();
			await InformoADescricaoDaTurmaQueDesejoCadastrar(turma);
			await ClicoEmCadastrarParaCadastrarATurma();
			await OCadastroDaTurmaEhRealizadoComSucesso();
			await PesquisarTurma(turma);
		}
		

	}
	public async Task ClicoEmObterOCodigoDaTurma()
	{
		await Page.MainFrame.ClickAsync("#BotaoCodigo");
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task InformoADescricaoDaTurmaQueDesejoCadastrar(string descricao)
	{

		await Page.MainFrame.GetByLabel("Descrição *").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição *").FillAsync(descricao);
		await Page.WaitForTimeoutAsync(1000);
	}


	public async Task ClicoEmCadastrarParaCadastrarATurma()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);

	}

	public async Task OCadastroDaTurmaEhRealizadoComSucesso()
	{
		string mensagem = "Registro cadastrado com sucesso!";
		string texto = await Page.MainFrame.Locator(".modal-dialog-centered .modal-body:has-text(\"Registro cadastrado com sucesso!\")").InnerTextAsync();
		Assert.AreEqual(mensagem, texto);
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task ClicoEmEditarATurmaDesejada()
	{
		await OrdenarRegistroDescendentemente();
		await Page.MainFrame.Locator("css=.em-grid-row:nth-child(1) .btn-outline-success > .fal").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task InformoANovaDescricaoDaTurma(string novaDescricao)
	{
		await Page.MainFrame.GetByLabel("Descrição *").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição *").FillAsync(novaDescricao);
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task ClicoEmEditarParaEditarATurma()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Editar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task ATurmaEhEditadaComSucesso()
	{
		string mensagem = "Registro editado com sucesso!";
		string texto = await Page.MainFrame.Locator(".modal-dialog-centered .modal-body:has-text(\"Registro editado com sucesso!\")").InnerTextAsync();
		Assert.AreEqual(mensagem, texto);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.WaitForTimeoutAsync(1000);
	}
	
	public async Task ClicoEmExcluirATurma()
	{
		await Page.MainFrame.Locator("css=.em-grid-row:nth-child(1) .btn-outline-danger").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task ClicoEmSimParaExcluirATurma()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}
	public async Task ATurmaEhExcluidaComSucesso()
	{
		string mensagem = "Item excluído com sucesso";
		string texto = await Page.MainFrame.Locator(".modal-dialog-centered .modal-body:has-text(\"Item excluído com sucesso\")").InnerTextAsync();
		Assert.AreEqual(mensagem, texto);
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);

	}

	public async Task ATurmaNaoEhExcluida()
	{
		string primeiraMensagem = "Turmas: Não é possível excluir a turma, pois há um coordenador do portal do professor associado a ela.";
		string segundaMensagem = "Turmas: Não é possível excluir a turma, pois há vínculos com séries.";

		string textoCompleto = await Page.MainFrame.Locator(".modal-dialog-centered .modal-body:has-text(\"Turmas: Não é possível excluir a turma, pois há um coordenador do portal do professor associado a ela.\")").InnerTextAsync();

		// Dividindo o texto pelo caractere de quebra de linha (com base no <br> no HTML)
		string[] partes = textoCompleto.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

		// Removendo espaços extras e verificando se as partes correspondem às mensagens esperadas
		string primeiroTexto = partes.Length > 0 ? partes[0].Trim() : string.Empty;
		string segundoTexto = partes.Length > 1 ? partes[1].Trim() : string.Empty;

		Assert.AreEqual(primeiroTexto, primeiraMensagem);
		Assert.AreEqual(segundoTexto, segundaMensagem);

		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task ClicoEm(string botao)
	{
		if (botao == "Cadastrar")
		{
			await ClicoEmCadastrarParaCadastrarATurma();
		}
		else if (botao == "Editar")
		{
			await ClicoEmEditarParaEditarATurma();
		}
		else if (botao == "Excluir")
		{
			await ClicoEmExcluirATurma();
		}
		else if (botao == "Sim")
		{
			await ClicoEmSimParaExcluirATurma();
		}
	}

	public async Task EntaoApresentaMensagem(string mensagem)
	{

		if (mensagem == "Registro cadastrado com sucesso!")
		{
			await OCadastroDaTurmaEhRealizadoComSucesso();
		}
		else if (mensagem == "Registro editado com sucesso!")
		{
			await ATurmaEhEditadaComSucesso();
		}
		else if (mensagem == "Item excluido com sucesso")
		{
			await ATurmaEhExcluidaComSucesso();
		}
		else if (mensagem == "Turmas: Nao e possivel excluir a turma, pois ha um coordenador do portal do professor associado a ela.Turmas: Nao e possivel excluir a turma, pois ha vinculos com series.")
		{
			await ATurmaNaoEhExcluida();
		}
	}

	public async Task ClicoNoBotao(string botao)
	{
		if (botao == "Editar")
		{
			await ClicoEmEditarATurmaDesejada();
		}
		else if (botao == "Excluir")
		{
			await ClicoEmExcluirATurma();
		}
	}



}