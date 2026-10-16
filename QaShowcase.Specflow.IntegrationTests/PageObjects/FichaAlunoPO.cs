using QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
﻿using Microsoft.Playwright;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;

internal class FichaAlunoPO : POBase
{

	public FichaAlunoPO(IPage page) : base(page)
	{
		ProjectRoot = AppContext.BaseDirectory;
		FilePath = Path.Combine(ProjectRoot, "Features", "EnsinoRegular", "Academico", "FichaAluno", "ResourcesFichaAluno", "PNGTests.png");
	}

	public override string PageUrl => "CadastroFichaAluno";



	// VARIÁVEIS GLOBAIS ==========================================================================

	public string? MatriculaAluno = null;
	public string? NomeDoAluno = null;
	public string? CPFAluno = null;



	// VARIÁVEIS LOCAIS ===========================================================================

	private string? NomeAnexo = null;
	private readonly string? ProjectRoot;
	private readonly string? FilePath;




	// CADASTROS ===========================================================================

	public async Task CadastrarAlunoEVoltar(string NomeAluno)
	{
		await this.CadastrarCidade("GOIANIA", "GO", "Cadastrar");
		await this.ClicarEm("Cadastrar");
		await this.PreechoONomeCivil(NomeAluno);
		await this.SelecionoOSexo("Masculino");
		await this.InsiroONascimento("01/01/2000");
		await this.SelecionoANaturalidade("GOIANIA");
		await this.PreenchoOCEP("74375-230");
		await this.InserirNovoCPF(GeradorDeCPF.GerarCpf());
		await this.InserirAtributoAdicional();
		await this.ClicarEm("Gravar");
		await this.ValidarMensagem("Cadastro realizado, deseja ir para a tela de matrícula?");
		await this.ClicarEm("Não");
		await this.ClicarEm("Voltar para a lista");
	}

	public async Task CadastrarAlunoComResponsavel(string NomeAluno)
	{
		// Cadastra o responsável e obtém o CPF
		var CpfDoResponsavel = await CadastrarResponsavelInternoAsync();

		// Agora segue o fluxo do aluno normalmente
		await this.GotoAsync();
		await this.ClicarEm("Cadastrar");
		await this.PreechoONomeCivil(NomeAluno);
		await this.SelecionoOSexo("Masculino");
		await this.InsiroONascimento("01/01/2000");
		await this.SelecionoANaturalidade("GOIANIA");
		await this.PreenchoOCEP("74375-230");
		await this.InserirNovoCPF(GeradorDeCPF.GerarCpf());
		await this.InformarCpfResponsavel(CpfDoResponsavel);
		await this.InserirAtributoAdicional();
		await this.ClicarEm("Gravar");
		await this.ValidarMensagem("Cadastro realizado, deseja ir para a tela de matrícula?");
		await this.ClicarEm("Não");
		await this.ClicarEm("Voltar para a lista");
		await this.ClicarEm("Responsáveis");
		await Page.MainFrame.GetByLabel("CPF").FillAsync(CpfDoResponsavel);
		await this.ClicarEm("Todos");
	}

	public async Task<string> CadastrarResponsavelInternoAsync()
	{
		await this.CadastrarCidade("GOIANIA", "GO", "Cadastrar");
		var NovaAba = await Page.Context.NewPageAsync();
		var responsavelPO = new ResponsavelPO(NovaAba);

		//var responsavelPO = new ResponsavelPO(this.Page);
		await responsavelPO.GotoAsync();
		await responsavelPO.ClicarEm("Cadastrar");
		await responsavelPO.InformarNome("Responsável gerado internamente");
		await responsavelPO.InformarDtNascimento("01/01/1980");
		await responsavelPO.SelecionarSexo("Masculino");
		await responsavelPO.SelecionarEstadoCivil("Casado(a)");
		await responsavelPO.PreencherCEP("74375-230");
		await responsavelPO.InformarCPF();
		await responsavelPO.InserirAtributoAdicional();
		await responsavelPO.ClicarEm("Gravar");
		await responsavelPO.ValidarMensagem("Cadastro realizado com sucesso!");
		await NovaAba.CloseAsync();


		// Retorna o CPF gerado
		return responsavelPO.CPFResponsavel!;
	}

	public async Task CadastrarCidade(string NomeCidade, string NomeUF, string? NomeBotao)
	{
		var cidadePO = new CidadesPO(this.Page);
		// Cadastra uma cidade
		await cidadePO.CadastrarCidadeSemCodIbge(NomeCidade, NomeUF, NomeBotao);
		await this.GotoAsync();
	}




	// PESQUISAS ===========================================================================

	public async Task PesquisarAlunoPorID(string NomeAluno)
	{
		// Cadastrar o aluno
		await this.CadastrarAlunoEVoltar(NomeAluno);

		// Pesquisar o aluno cadastrado
		await Page.MainFrame.Locator("#PesquisePorMatriculados").GetByText("Todos").ClickAsync();
		await Page.WaitForTimeoutAsync(500); // Aguarda após clicar no filtro

		var campoPesquisa = Page.MainFrame.GetByLabel("Nome ou número de matrícula");
		await campoPesquisa.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
		await campoPesquisa.ClickAsync();
		await Page.WaitForTimeoutAsync(300); // Aguarda após clicar no campo

		// Foca no campo antes de preencher
		await campoPesquisa.FocusAsync();
		await Page.WaitForTimeoutAsync(200); // Aguarda após focar

		await campoPesquisa.FillAsync(MatriculaAluno);
		await Page.WaitForTimeoutAsync(500); // Aguarda após preencher

		// Garante que o campo está focado antes de pressionar Enter
		await campoPesquisa.FocusAsync();
		await Page.WaitForTimeoutAsync(300); // Aguarda após focar novamente
		await campoPesquisa.PressAsync("Enter");

		// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
		await Page.WaitForTimeoutAsync(2000); // Aguarda a pesquisa ser processada

		// Aguarda a grid estar visível e carregada
		var gridAterrissagem = Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container");
		await gridAterrissagem.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

		// Aguarda um pouco mais para garantir que as linhas foram renderizadas
		await Page.WaitForTimeoutAsync(1000);

		// Validar se o aluno foi encontrado - verifica o conteúdo da grid
		var gridAlunosElement = await gridAterrissagem.ElementHandleAsync();
		Assert.IsNotNull(gridAlunosElement, "Grid de alunos não encontrada");

		// Aguarda até que o aluno apareça na grid (pela matrícula ou pelo nome)
		int tentativas = 0;
		bool encontrado = false;
		while (tentativas < 10 && !encontrado)
		{
			string resultado = await gridAlunosElement.InnerTextAsync();
			encontrado = resultado.Contains(MatriculaAluno) || resultado.Contains(NomeAluno);
			if (!encontrado)
			{
				await Page.WaitForTimeoutAsync(500);
				tentativas++;
			}
		}

		Assert.IsTrue(encontrado, $"O aluno '{NomeAluno}' (matrícula: {MatriculaAluno}) não foi encontrado na grid após {tentativas + 1} tentativas");
	}

	public async Task PesquisarAlunoPorNome(string NomeCampoTexto, string NomeAluno)
	{
		// Cadastrar o aluno
		await this.CadastrarAlunoEVoltar(NomeAluno);

		await Page.MainFrame.Locator("#PesquisePorAluno").GetByText("Alunos").ClickAsync();

		var campoTextoEncontrado = Page.MainFrame.GetByText(NomeCampoTexto);
		await campoTextoEncontrado.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

		if (!await campoTextoEncontrado.IsVisibleAsync())
		{
			string nomeCampoTextoNormalizado = NomeCampoTexto;
			await Page.MainFrame.GetByText(nomeCampoTextoNormalizado).IsVisibleAsync();
		}

		// Pesquisar o aluno cadastrado
		await Page.MainFrame.Locator("#PesquisePorMatriculados").GetByText("Todos").ClickAsync();
		await Page.WaitForTimeoutAsync(500); // Aguarda após clicar no filtro

		var campoPesquisa = Page.MainFrame.GetByLabel("Nome ou número de matrícula");
		await campoPesquisa.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
		await campoPesquisa.ClickAsync();
		await Page.WaitForTimeoutAsync(300); // Aguarda após clicar no campo

		// Foca no campo antes de preencher
		await campoPesquisa.FocusAsync();
		await Page.WaitForTimeoutAsync(200); // Aguarda após focar

		await campoPesquisa.FillAsync(NomeAluno);
		await Page.WaitForTimeoutAsync(500); // Aguarda após preencher

		// Garante que o campo está focado antes de pressionar Enter
		await campoPesquisa.FocusAsync();
		await Page.WaitForTimeoutAsync(300); // Aguarda após focar novamente
		await campoPesquisa.PressAsync("Enter");

		// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
		await Page.WaitForTimeoutAsync(2000); // Aguarda a pesquisa ser processada

		// Aguarda a grid estar visível e carregada
		var gridAterrissagem = Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container");
		await gridAterrissagem.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

		// Aguarda um pouco mais para garantir que as linhas foram renderizadas
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task PesquisarAlunoAlterado()
	{
		// Pesquisar o aluno cadastrado
		await Page.MainFrame.Locator("#PesquisePorMatriculados").GetByText("Todos").ClickAsync();
		await Page.WaitForTimeoutAsync(500); // Aguarda após clicar no filtro

		var campoPesquisa = Page.MainFrame.GetByLabel("Nome ou número de matrícula");
		await campoPesquisa.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
		await campoPesquisa.ClickAsync();
		await Page.WaitForTimeoutAsync(300); // Aguarda após clicar no campo

		// Foca no campo antes de preencher
		await campoPesquisa.FocusAsync();
		await Page.WaitForTimeoutAsync(200); // Aguarda após focar

		await campoPesquisa.FillAsync(MatriculaAluno);
		await Page.WaitForTimeoutAsync(500); // Aguarda após preencher

		// Garante que o campo está focado antes de pressionar Enter
		await campoPesquisa.FocusAsync();
		await Page.WaitForTimeoutAsync(300); // Aguarda após focar novamente
		await campoPesquisa.PressAsync("Enter");

		// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
		await Page.WaitForTimeoutAsync(2000); // Aguarda a pesquisa ser processada

		// Aguarda a grid estar visível e carregada
		var gridAterrissagem = Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container");
		await gridAterrissagem.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

		// Aguarda um pouco mais para garantir que as linhas foram renderizadas
		await Page.WaitForTimeoutAsync(1000);

		// Validar se o aluno foi encontrado
		var CampoTexto = Page.MainFrame.GetByText(MatriculaAluno);
		await CampoTexto.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
		Assert.IsTrue(await CampoTexto.IsVisibleAsync(), $"Campo de texto {CampoTexto} não encontrado");
	}

	public async Task PesquisarResponsavelDoAluno(string cpfResponsavel)
	{
		await Page.MainFrame.GetByText("Responsáveis").ClickAsync();
		await Page.MainFrame.GetByLabel("CPF").TypeAsync(cpfResponsavel);
	}




	// VALIDAÇÕES ===========================================================================

	public async Task ValidarMensagem(string Mensagem)
	{
		await Page.WaitForTimeoutAsync(2000);
		var TextoMensagem = await Page.MainFrame.GetByText(Mensagem).IsVisibleAsync();
		Assert.IsTrue(TextoMensagem, "Texto da mensagem não encontrado na tela.");
	}

	public async Task ValidarAlunoNaGrid()
	{
		// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
		await Page.WaitForTimeoutAsync(3000);

		// Aguarda a grid estar visível e carregada
		var gridAlunosLocator = Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container");
		await gridAlunosLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
		Assert.IsNotNull(gridAlunosLocator, "Grid de alunos não encontrada");

		// Aguarda um pouco mais para garantir que as linhas foram renderizadas
		await Page.WaitForTimeoutAsync(1000);

		var gridAlunos = await gridAlunosLocator.ElementHandleAsync();
		Assert.IsNotNull(gridAlunos, "Grid de alunos não encontrada");

		string resultado = await gridAlunos.InnerTextAsync();
		Assert.IsTrue(resultado.Contains(MatriculaAluno), $"O código '{MatriculaAluno}' não foi encontrado na grid");
	}

	public async Task ValidarAlunosFiltrados()
	{
		await Page.WaitForTimeoutAsync(1000);
		var gridAlunosLocator = Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container");
		Assert.IsNotNull(gridAlunosLocator, "Grid de alunos não encontrada");

		var gridAlunos = await gridAlunosLocator.ElementHandleAsync();
		Assert.IsNotNull(gridAlunos, "Grid de alunos não encontrada");

		string resultado = await gridAlunos.InnerTextAsync();
		Assert.IsTrue(resultado.Contains(NomeDoAluno), $"O código '{NomeDoAluno}' não foi encontrado na grid");
		await Page.WaitForTimeoutAsync(1000);
	}

	public async Task ValidarCamposDestacados(string CEP)
	{
		await Page.WaitForTimeoutAsync(500);
		var cepObrigatorioLocator = Page.MainFrame.Locator("#EnderecoContainer > div:nth-child(1) > div.col-xl-3.col-lg-12 > label > span.validationMessage.ms-auto");
		await cepObrigatorioLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
		bool cepObrigatorioVisivel = await cepObrigatorioLocator.IsVisibleAsync();
		Assert.IsTrue(cepObrigatorioVisivel, $"Validação de {CEP} falhou!");
	}

	public async Task ValidarCamposDestacados()
	{
		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Endereço e telefone" }).ClickAsync();

		// Validação do CEP
		var cepObrigatorioLocator = Page.MainFrame.Locator("#EnderecoContainer > div:nth-child(1) > div.col-xl-3.col-lg-12 > label > span.validationMessage.ms-auto");
		await cepObrigatorioLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
		bool cepObrigatorioVisivel = await cepObrigatorioLocator.IsVisibleAsync();
		Assert.IsTrue(cepObrigatorioVisivel, "Validação de CEP falhou!");

		// Validação do Logradouro
		var logradouroObrigatorioLocator = Page.MainFrame.Locator("#EnderecoContainer > div:nth-child(1) > div.col-xl-9.col-lg-12 > label > span.validationMessage.ms-auto");
		await logradouroObrigatorioLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
		bool logradouroObrigatorioVisivel = await logradouroObrigatorioLocator.IsVisibleAsync();
		Assert.IsTrue(logradouroObrigatorioVisivel, "Validação de Logradouro falhou!");

		// Validação do Bairro
		var bairroObrigatorioLocator = Page.MainFrame.Locator("#EnderecoContainer > div:nth-child(2) > div:nth-child(1) > label > span.validationMessage.ms-auto");
		await bairroObrigatorioLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
		bool bairroObrigatorioVisivel = await bairroObrigatorioLocator.IsVisibleAsync();
		Assert.IsTrue(bairroObrigatorioVisivel, "Validação de bairro falhou!");
	}

	public async Task ValidarAnexoInserido()
	{
		var ValidarAnexo = Page.MainFrame.Locator("#GridAnexosFichaAlunoItensGrid > div.em-grid-container");
		await ValidarAnexo.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
		Assert.IsTrue(await ValidarAnexo.IsVisibleAsync(), $"Anexo {NomeAnexo} não encontrado na grid");
	}

	public async Task ValidarTelaDeAterrissagem()
	{
		var telaDeAterrissagem = Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container");
		await telaDeAterrissagem.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
		Assert.IsTrue(await telaDeAterrissagem.IsVisibleAsync(), "Tela de aterrissagem não encontrada");
	}




	// AÇÕES GERAIS =========================================================================

	public async Task PreechoONomeCivil(string NomeCivil)
	{
		// Obter o valor do campo '#Matricula'
		var MatriculaElemento = await Page.MainFrame.GetByLabel("Matrícula *").ElementHandleAsync();
		Assert.IsNotNull(MatriculaElemento, "Elemento '#Matricula' não encontrado.");
		string? Matricula = await MatriculaElemento.GetAttributeAsync("value");
		Assert.IsNotNull(Matricula, "O campo '#Matricula' não possui valor.");
		MatriculaAluno = Matricula;
		NomeDoAluno = NomeCivil;

		await Page.MainFrame.GetByLabel("Nome civil *").FillAsync(NomeCivil);
	}

	public async Task AlterarONomeDoAluno(string NomeAluno)
	{
		await this.PreechoONomeCivil(NomeAluno);
		await this.ClicarEm("Gravar");
		await this.ValidarMensagem("Cadastro realizado, deseja ir para a tela de matrícula?");
		await this.ClicarEm("Não");
		await this.ClicarEm("Voltar para a lista");
	}

	public async Task ClicarEmAnexo(string NomeAluno)
	{
		// Localiza o aluno na grid
		var GridAlunos = Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container");
		await GridAlunos.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
		Assert.IsTrue(await GridAlunos.IsVisibleAsync(), "Grid de alunos não encontrada");
		string Resultado = await GridAlunos.InnerTextAsync();
		Assert.IsTrue(Resultado.Contains(NomeAluno), $"O aluno '{NomeAluno}' não foi encontrado na grid");

		await Page.MainFrame.Locator("//*[@id=\"grid-body\"]/div[1]/div[5]/button[2]").ClickAsync();
	}

	public async Task InserirTituloAnexo(string TituloAnexo)
	{
		NomeAnexo = TituloAnexo;
		await Page.MainFrame.GetByLabel("Título").FillAsync(TituloAnexo);
	}

	public async Task SelecionarAnexo()
	{
		await Page.SetInputFilesAsync("#objeto_ImporteArquivo", FilePath);
	}

	public async Task SelecionoOSexo(string Sexo)
	{
		await Page.MainFrame.GetByRole(AriaRole.Combobox, new() { Name = "Sexo *" }).SelectOptionAsync(new[] { Sexo });
	}

	public async Task InsiroONascimento(string Nascimento)
	{
		await Page.MainFrame.GetByLabel("Nascimento *").FillAsync(Nascimento);
	}

	public async Task SelecionoANaturalidade(string Naturalidade)
	{
		// Cadastra a cidade com a naturalidade desejada
		var NovaAba = await Page.Context.NewPageAsync();
		var cidadePO = new CidadesPO(NovaAba);
		await cidadePO.CadastrarCidadeSemCodIbge(Naturalidade, "GO", "Cadastrar");
		await NovaAba.CloseAsync();

		// Seleciona a naturalidade
		await Page.MainFrame.Locator("#NaturalidadePesquisar").ClickAsync();
		await Page.MainFrame.GetByLabel("Naturalidade").TypeAsync(Naturalidade);
		await Page.MainFrame.Locator("#GridNaturalidade #Descricao_0").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}

	public async Task PreenchoOCEP(string CEP)
	{
		await Page.MainFrame.GetByLabel("CEP *").ClickAsync();
		await Page.MainFrame.GetByLabel("CEP *").FillAsync(CEP);
		await Page.MainFrame.Locator("#EnderecoPesquisarLupa").ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(500);
	}

	public async Task InserirNovoCPF(string CPF)
	{
		CPF = TestConfig.Resolve(CPF);
		CPFAluno = CPF;
		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Documentos" }).ClickAsync();
		await Page.WaitForTimeoutAsync(600);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Identificação" }).ClickAsync();
		await Page.WaitForTimeoutAsync(400);
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CPF" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CPF" }).FillAsync(CPF);
	}

	public async Task RemoverCEP(string RemoverCEP)
	{
		await Page.MainFrame.GetByLabel($"{RemoverCEP} *").FillAsync("");
		await Page.WaitForTimeoutAsync(500);
	}

	public async Task ClicarEm(string botao)
	{
		switch (botao)
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
			case "Editar":
				await Page.MainFrame.Locator("//*[@id=\"grid-body\"]/div[1]/div[5]/button[3]").ClickAsync();
				break;
			case "Remover":
				await Page.MainFrame.Locator("//*[@id=\"grid-body\"]/div[1]/div[5]/button[4]").ClickAsync();
				break;
			case "Sim":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
				break;
			case "Não":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Não" }).ClickAsync();
				break;
			case "Cancelar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cancelar" }).ClickAsync();
				break;
			case "Adicionar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Adicionar" }).ClickAsync();
				break;
			case "Enter":
				await Page.MainFrame.GetByLabel("Nome ou número de matrícula").PressAsync("Enter");
				break;
			case "Responsáveis":
				await Page.MainFrame.GetByText("Responsáveis").ClickAsync();
				break;
			case "Todos":
				await Page.MainFrame.Locator("#PesquisePorMatriculados").GetByText("Todos").ClickAsync();
				break;
			default:
				Console.WriteLine($"Botão '{botao}' não está programado.");
				break;
		}
	}

	public async Task InformarCpfResponsavel(string cpfResponsavel)
	{
		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Responsáveis" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Filiação e outros responsáveis" }).ClickAsync();
		await Page.MainFrame.GetByLabel("Responsáveis").GetByRole(AriaRole.Button, new() { Name = "" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "Nome", Exact = true }).TypeAsync(cpfResponsavel);
		await Page.WaitForTimeoutAsync(1000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Adicionar" }).ClickAsync();
	}

	public async Task InserirAtributoAdicional()
	{
		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Atributos adicionais" }).ClickAsync();
		await Page.MainFrame.WaitForTimeoutAsync(500); // Aguarda a seção ser expandida

		var atributosDivLocator = Page.MainFrame.Locator("#Atributos_adicionais");
		Assert.NotNull(atributosDivLocator, "Div de atributos adicionais não localizada");

		var todosCampos = await atributosDivLocator.Locator("textarea, select, input").AllAsync();
		Assert.NotNull(todosCampos, "Todos os campos de atributos adicionais não encontrados");


		foreach (var aux in todosCampos)
		{
			bool isRequired = await aux.GetAttributeAsync("required") != null;
			if (!isRequired)
			{
				continue;
			}

			string tagName = await aux.EvaluateAsync<string>("el => el.tagName");

			if (tagName == "SELECT")
			{
				try
				{
					await aux.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
					await aux.ClickAsync();
					await Page.WaitForTimeoutAsync(500);
					var options = await aux.Locator("option:not([disabled])").AllAsync();
					foreach (var option in options)
					{
						string optionValue = await option.GetAttributeAsync("value");
						if (!string.IsNullOrEmpty(optionValue))
						{
							await aux.SelectOptionAsync(new[] { optionValue });
							var selectedValue = await aux.EvaluateAsync<string>("el => el.value");
							if (selectedValue == optionValue)
							{
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception($"Erro ao preencher SELECT obrigatório: {ex.Message}");
				}
			}
			else if (tagName == "TEXTAREA")
			{
				try
				{
					string texto = DateTime.Now.ToString("dd|HH:mm:ss");
					await aux.FillAsync($"Atributo obrigatório automatizado - {texto}");
				}
				catch (Exception ex)
				{
					throw new Exception($"Erro ao preencher TEXTAREA obrigatório: {ex.Message}");
				}
			}
			else if (tagName == "INPUT")
			{
				var role = await aux.GetAttributeAsync("data-role");
				if (role == "ItemMultiplaEscolha_text")
				{
					try
					{
						var lupaButton = aux.Locator("xpath=following-sibling::button[contains(@id, 'Pesquisar')]");
						if (await lupaButton.IsVisibleAsync())
						{
							await lupaButton.ClickAsync();
							var modalLocator = Page.MainFrame.Locator("div.modal.fade.show");
							await modalLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
							var selecionarTodos = modalLocator.Locator(".em-grid-header .em-grid-select-box");
							await selecionarTodos.ClickAsync();
							var btnSelecionar = modalLocator.GetByRole(AriaRole.Button, new() { Name = "Selecionar" });
							await btnSelecionar.ClickAsync();
							await modalLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden });
						}
					}
					catch (Exception ex)
					{
						throw new Exception($"Erro em campo de múltipla escolha obrigatório: {ex.Message}");
					}
				}
			}
		}
	}

}
