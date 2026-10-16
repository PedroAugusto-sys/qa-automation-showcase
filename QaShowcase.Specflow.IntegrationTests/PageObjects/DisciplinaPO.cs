using Microsoft.Playwright;
using NUnit.Framework;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using TechTalk.SpecFlow.Assist;
using QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
using System.Diagnostics;
using NuGet.Frameworks;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class DisciplinaPO : POBase
{
	string nomeDisciplina = null;

	public string? codigoDisciplina;
	public string? descricaoDisciplina;
	public DisciplinaPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroDisciplina";


	//Métodos para cadastrar uma disciplina

	//Método para informar a situação da disciplina
	public async Task InformoASituacao(string ativo)
	{
		if (ativo == "Ativo")
		{
			await Page.Locator("label[for='EstaAtivo_sim']").ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
		else if (ativo == "Inativo")
		{
			await Page.Locator("label[for='EstaAtivo_nao']").ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
	}

	//Método para informar a área de conhecimento da disciplina
	public async Task InformoAAreaDeConhecimento(string descricao)
	{
		await this.CadastrarAreaConhecimentoDisciplinaInterno(descricao);
		await Page.MainFrame.Locator("#AreaConhecimentoPesquisar").ClickAsync();
		await Page.MainFrame.GetByLabel("Área de conhecimento").ClickAsync();
		await Page.MainFrame.GetByLabel("Área de conhecimento").TypeAsync(descricao);
		await Page.MainFrame.Locator("#Nome_0").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1000);
	}

	//Método para selecionar o tipo de disciplina
	public async Task SelecionoTipoDeDisciplina(string tipoDisciplina)
	{
		try
		{

			if (tipoDisciplina == "Comum")
			{
				await Page.SelectOptionAsync("#EnumOpcoesDisciplina", "EhComum");
			}
			else if (tipoDisciplina == "Multidisciplinar")
			{
				await Page.SelectOptionAsync("#EnumOpcoesDisciplina", "EhMultiDisciplinar");
			}
			else if (tipoDisciplina == "Disciplina Extra")
			{
				await Page.SelectOptionAsync("#EnumOpcoesDisciplina", "EhDisciplinaExtra");
			}
			else
			{
				throw new ArgumentException($"Tipo de disciplina '{tipoDisciplina}' não reconhecido. Use 'Comum', 'Multidisciplinar' ou 'Disciplina Extra'.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro ao selecionar o tipo de disciplina: {ex.Message}");
			throw;
		}
	}

	//Método para selecionar o grupo do histórico escolar
	public async Task SelecionoGrupoHistoricoEscolar(string grupoHistoricoEscolar)
	{
		try
		{
			if (grupoHistoricoEscolar == "Parte nacional comum")
			{
				await Page.SelectOptionAsync("#EnumGrupoHistoricoEscolar", "EhParteComum");
			}
			else if (grupoHistoricoEscolar == "Parte diversificada")
			{
				await Page.SelectOptionAsync("#EnumGrupoHistoricoEscolar", "EhParteDiversificada");
			}
			else
			{
				throw new ArgumentException($"Grupo escolar {grupoHistoricoEscolar} não encontrado");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro ao selecionar o grupo escolar: {ex.Message}");
			throw;
		}
	}

	//Método para selecionar o tipo de agregação
	public async Task SelecionoTipoDeAgregacao(string tipoAgregacao)
	{
		if (tipoAgregacao == "Não há agregação")
		{
			await Page.SelectOptionAsync("#EnumTipoDisciplina", "Não há agregação");
		}
		else if (tipoAgregacao == "É agregadora")
		{
			await Page.SelectOptionAsync("#EnumTipoDisciplina", "É agregadora");
		}
		else if (tipoAgregacao == "É agregada")
		{
			await Page.SelectOptionAsync("#EnumTipoDisciplina", "É agregada");
		}
	}

	//Método para selecionar se a disciplina aparecerá no histórico escolar
	public async Task SelecionoHistoricoEscolar(string historicoEscolar)
	{
		if (historicoEscolar == "Sim")
		{
			await Page.Locator("label[for='MostrarSomenteNoBoletim_sim']").ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
		else if (historicoEscolar == "Nao")
		{
			await Page.Locator("label[for='MostrarSomenteNoBoletim_nao']").ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
	}


	//Método para validar a mensagem de confirmação após o cadastro de uma disciplina
	public async Task ValidarMensagemDeConfirmacaoDeCadastro(string mensagemConfirmacao)
	{
		var mensagemDeValidacaoLocator = Page.MainFrame.Locator("#form0 > div.modal.fade.show > div > div");
		string mensagemApresentada = await mensagemDeValidacaoLocator.InnerTextAsync();

		// Valida se a mensagem esperada está contida no texto apresentado
		Assert.IsTrue(mensagemApresentada.Contains(mensagemConfirmacao),
			$"Falha ao cadastrar disciplina! Mensagem esperada: '{mensagemConfirmacao}', Mensagem apresentada: '{mensagemApresentada}'");

		await Page.WaitForTimeoutAsync(1000);

		var botaoVoltarLocator = Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" });
		await botaoVoltarLocator.ClickAsync();
	}

	// Métodos para editar a descrição de uma disciplina

	//Método para alterar a descrição de uma disciplina
	public async Task AlteroADescricao(string novaDescricao)
	{
		await Page.MainFrame.GetByLabel("Descrição *").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição *").FillAsync(novaDescricao);
		await Page.WaitForTimeoutAsync(1000);
	}

	//Método para informar a descrição da disciplina
	public async Task InformoADescricaoDaDisciplina(string descricaoDisciplina)
	{
		await Page.MainFrame.GetByLabel("Descrição *").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição *").FillAsync(descricaoDisciplina);
		await Page.WaitForTimeoutAsync(1000);
		nomeDisciplina = descricaoDisciplina; // Armazena a descrição da disciplina para uso posterior
	}

	//Métodos para editar a situação de uma disciplina

	//Método para alterar a situação da disciplina
	public async Task AlteraSituacao(string situacao)
	{
		if (situacao == "Ativo")
		{
			await Page.MainFrame.Locator("#EstaAtivo_sim + label").ClickAsync();
		}
		else
		{
			await Page.MainFrame.Locator("#EstaAtivo_nao + label").ClickAsync();
		}
	}


	//Métodos para excluir uma disciplina com vínculos

	//Método para pesquisar uma disciplina com vínculos na grid
	public async Task<bool> PesquisaDisciplinaComVinculos(string textoPesquisado)
	{
		var linhas = await Page.MainFrame.QuerySelectorAllAsync(".em-grid-row");

		foreach (var linha in linhas)
		{
			string textoLinha = await linha.InnerTextAsync();
			if (textoLinha.Contains(textoPesquisado, StringComparison.OrdinalIgnoreCase))
			{
				return true; // Encontrou pelo menos uma linha com o texto pesquisado
			}
		}
		return false; // Nenhuma linha contém o texto pesquisado
	}


	//Métodos para pesquisar uma disciplina

	//Método para validar que todas as disciplinas apresentadas na grid contém o texto pesquisado
	public async Task ValidarDisciplinasContemTextoPesquisado()
	{
		var linhas = await Page.Locator("div[id^='Descricao']").AllInnerTextsAsync();

		// Pega o texto pesquisado no campo de pesquisa
		var campoPesquisa = await Page.MainFrame.GetByPlaceholder("Pesquisar").InputValueAsync();
		await Page.MainFrame.WaitForSelectorAsync($".em-grid-row:has-text('{campoPesquisa}')", new() { Timeout = 5000 });
		Assert.IsTrue(linhas.Count > 0, "Nenhuma disciplina foi apresentada na grid.");

		foreach (var linha in linhas)
		{
			Assert.IsTrue(linha.Contains(nomeDisciplina), $"A linha '{linha}' não contém o texto pesquisado '{nomeDisciplina}'.");
			Console.WriteLine($"{nomeDisciplina} contem em {linha}\n\n" );
		}
	}

	//Métodos para filtrar disciplinas por situação

	//Método para alterar o filtro da grid de disciplinas
	public async Task AlterarFiltroGrid(string opcao)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(opcao))
			{
				throw new ArgumentNullException(nameof(opcao));
			}

			await Page.Locator("#DropDownAtivoFiltro").WaitForAsync(new() { Timeout = 5000 });

			if (opcao.Equals("Inativas", StringComparison.OrdinalIgnoreCase))
			{
				await Page.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new SelectOptionValue { Value = "2" });
				Console.WriteLine($"Filtro alterado para valor 2 (opção: {opcao})");
			}
			else if (opcao.Equals("Ativas", StringComparison.OrdinalIgnoreCase))
			{
				await Page.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new SelectOptionValue { Value = "1" });
				Console.WriteLine($"Filtro alterado para valor 1 (opção: {opcao})");
			}
			else if (opcao.Equals("Todas", StringComparison.OrdinalIgnoreCase))
			{
				await Page.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new SelectOptionValue { Value = "3" });
				Console.WriteLine($"Filtro alterado para valor 3 (opção: {opcao})");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro ao alterar filtro: {ex.Message}");
			throw;
		}
	}

	//Método para alterar o filtro da grid cadastrando disciplinas ativas e inativas
	public async Task AlterarFiltroGridCadastrando(string opcaoFiltro)
	{
		await PesquisaCadastrando("DISCIPLINA TESTE QA ATIVA");
		await PesquisaCadastrando("DISCIPLINA TESTE QA INATIVA");
		await EditarInterno("Editar", "Gravar", "Inativo", "Cadastro realizado com sucesso!");
		await AlterarFiltroGrid(opcaoFiltro);
	}

	//Método para validar que todas as disciplinas apresentadas na grid estão com a situação selecionada no filtro
	public async Task ValidarSomenteDisciplinasComSituacaoInativa(string situacao)
    {
		await Page.WaitForTimeoutAsync(5000);
        var linhas = await Page.MainFrame.QuerySelectorAllAsync(".em-grid-row");
        Assert.IsTrue(linhas.Count > 1, "Nenhuma disciplina foi apresentada na grid.");

        foreach (var linha in linhas)
        {
			string textoLinha = await linha.InnerTextAsync();

			// Ignora cabeçalhos e linhas vazias
			if (string.IsNullOrWhiteSpace(textoLinha) ||
				(textoLinha.Contains("Código") && textoLinha.Contains("Descrição")))
			{
				continue;
			}
			if (situacao.Equals("Inativa", StringComparison.OrdinalIgnoreCase) || situacao.Equals("Inativas", StringComparison.OrdinalIgnoreCase))
            {
                // Verifica se o ícone de inativo está presente na linha
                var iconeInativo = await linha.QuerySelectorAsync("[data-label='Ativo'] i.fa-times-circle");
                Assert.IsTrue(iconeInativo != null && await iconeInativo.IsVisibleAsync(),"Foi encontrada uma disciplina que não está marcada como inativa (ícone não encontrado).");
			}
			else
            {
                // Para outras situações, mantém a validação textual
                string textoLinhaGrid = await linha.InnerTextAsync();
                Assert.IsTrue(textoLinhaGrid.Contains(situacao, StringComparison.OrdinalIgnoreCase),$"Foi encontrada uma disciplina que não está com a situação '{situacao}'. Texto da linha: '{textoLinha}'");
            }
		}
	}

	//Método para clicar em botões da tela de disciplina
	public async Task ClicoEm(string botao)
	{
		if (botao == "Cadastrar")
		{
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
		else if (botao == "Gravar")
		{
			//await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
			await Page.Locator("button:text('Gravar')").ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
		else if (botao == "Editar")
		{
			var primeiraLinha = Page.MainFrame.Locator("div.em-grid-body > div.em-grid-row").First;

			// Dentro da primeira linha, encontra o botão de editar
			var botaoEditar = primeiraLinha.Locator("button[data-role='em-grid-editar']");

			// Clica no botão
			await botaoEditar.ClickAsync();

			Console.WriteLine("Clique no botão 'Editar' realizado com sucesso.");
			await Page.WaitForTimeoutAsync(1000);
		}
		else if (botao == "Excluir")
		{
			var primeiraLinha = Page.MainFrame.Locator("div.em-grid-body > div.em-grid-row").First;
			// Dentro da primeira linha, encontra o botão de excluir
			var botaoExcluir = primeiraLinha.Locator("button[data-role='em-grid-remover']");

			// Clica no botão
			await botaoExcluir.ClickAsync();

			Console.WriteLine("Clique no botão 'Excluir' realizado com sucesso.");
			await Page.WaitForTimeoutAsync(1000);
		}
		else if (botao == "Sim")
		{
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
	}

	//Método para pesquisar uma disciplina cadastrada na grid
	public async Task PesquisaCadastrando(string disciplina)
	{
		
		await CadastroInterno(disciplina);

		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
		await Page.MainFrame.GetByPlaceholder("Pesquisar").PressSequentiallyAsync(disciplina);
		await Page.WaitForTimeoutAsync(2000);


		var botaoCodigo = Page.MainFrame.Locator("button:has-text(\"Código\")");
		await botaoCodigo.ClickAsync();
		await botaoCodigo.ClickAsync();
		var primeiraLinha = Page.MainFrame.Locator("div.em-grid-body > div.em-grid-row").First;



		// Verifica se a disciplina aparece na grid
		if (await primeiraLinha.IsVisibleAsync())
		{
			var descricaoElemento = primeiraLinha.Locator("div[data-label='Descrição']");

			string textoGrid = await descricaoElemento.InnerTextAsync();
			Assert.IsTrue(textoGrid.Contains(disciplina), $"A disciplina '{disciplina}' não foi encontrada na grid.");
		}
		else
		{
			Assert.Fail($"A disciplina '{disciplina}' não foi encontrada na grid.");
		}
	}

	//Método para validar mensagens de confirmaçõa de cadastro e exclusão de disciplinas
	public async Task ValidarMensagem(string mensagem)
	{
		var mensagemConfirmacao = "Cadastro realizado com sucesso!";
		var mensagemExclusao = "Item excluido com sucesso.";
		var mensagemExclusaoComVinculos = "A disciplina nao pode ser excluida pois ja esta sendo utilizada por alguma das funcoes ou conceitos:";

		if (mensagemConfirmacao == mensagem)
		{
			await Page.MainFrame.GetByText(mensagemConfirmacao).IsVisibleAsync();
			await Page.WaitForTimeoutAsync(1000);

			var botaoVoltarLocator = Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" });
			await botaoVoltarLocator.ClickAsync();
		}
		else if (mensagemExclusao == mensagem)
		{
			await Page.MainFrame.GetByText(mensagemExclusao).IsVisibleAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
		else if (mensagemExclusaoComVinculos == mensagem)
		{
			await Page.MainFrame.GetByText(mensagemExclusaoComVinculos).IsVisibleAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
		else
		{
			Assert.Fail($"Mensagem '{mensagem}' não reconhecida.");
		}

	}

	//########################################################### MÉTODOS INTERNOS ###########################################################//

	//Métodos internos para cadastro e edição de disciplinas

	//Método interno para cadastrar uma disciplina
	internal async Task CadastroInterno(string disciplina)
	{
		await ClicoEm("Cadastrar");
		await InformoADescricaoDaDisciplina(disciplina);
		await InformoASituacao("Ativo");
		await InformoAAreaDeConhecimento("Área de Conhecimento Teste QA");
		await SelecionoTipoDeDisciplina("Comum");
		await SelecionoGrupoHistoricoEscolar("Parte nacional comum");
		await SelecionoTipoDeAgregacao("Não há agregação");
		await SelecionoHistoricoEscolar("Sim");
		await ClicoEm("Gravar");
		await ValidarMensagemDeConfirmacaoDeCadastro("Cadastro realizado com sucesso!");
	}

	//Método interno para editar a situação de uma disciplina
	internal async Task EditarInterno(string botaoEditar, string botaoGravar, string situacao, string mensagemSucesso)
	{
		await ClicoEm(botaoEditar);
		await AlteraSituacao(situacao);
		await ClicoEm(botaoGravar);
		await ValidarMensagem(mensagemSucesso);
	}

	//Método interno para cadastrar uma área de conhecimento para a disciplina
	internal async Task CadastrarAreaConhecimentoDisciplinaInterno(string descricao)
	{
		AreaConhecimentoPO _areaConhecimentoPO = await POFactory.Crie<AreaConhecimentoPO>(Page.Context);
		await _areaConhecimentoPO.GotoAsync();
		await _areaConhecimentoPO.InformarDescricao(descricao);
		await _areaConhecimentoPO.clicarEmOpcao("Cadastrar");
		await _areaConhecimentoPO.Page.WaitForTimeoutAsync(2000);
		await _areaConhecimentoPO.Page.CloseAsync();
	}
}
