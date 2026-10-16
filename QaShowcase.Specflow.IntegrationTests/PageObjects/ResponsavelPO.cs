using QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
﻿using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Playwright;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class ResponsavelPO : POBase
{

	public ResponsavelPO(IPage page) : base(page) { }

	public override string PageUrl => "CadastroResponsavel";

	// Propriedades globais
	public string? CodigoResponsavel;
	public string? CPFResponsavel;
	public string? NomeResponsavel;

	// Strings de botões
	private const string BotaoCancelar = " Cancelar";
	private const string BotaoGravar = " Gravar";
	private const string BotaoVoltarParaALista = " Voltar para a lista";
	private const string BotaoVoltar = " Voltar";
	private const string BotaoSim = " Sim";
	private const string BotaoSelecionar = " Selecionar";

	// CADASTROS ========================================================================

	public async Task CadastraUmaCidade(string NomeCidade, string NomeUF, string? NomeBotao) // Cadastra Goiânia 5208707
	{
		var cidadePO = new CidadesPO(this.Page);
		await cidadePO.CadastrarCidadeSemCodIbge(NomeCidade, NomeUF, NomeBotao);
		await this.GotoAsync();
	}

	public async Task CadastrarResponsavel(string Responsavel)
	{
		await this.CadastraUmaCidade("GOIANIA", "GO", "Cadastrar");

		await this.ClicarEm("Cadastrar");
		await this.InformarNome(Responsavel);
		await this.InformarDtNascimento("01/01/1980");
		await this.SelecionarSexo("Masculino");
		await this.SelecionarEstadoCivil("Casado(a)");
		await this.PreencherCEP("74375-230");
		await this.InformarCPF();
		await this.InserirAtributoAdicional();
		await this.ClicarEm("Gravar");
		await this.ValidarMensagem("Cadastro realizado com sucesso!");
		await this.ClicarEm("Voltar para a lista");
	}

	public async Task CadastrarResponsavelEAlunoVinculando(string NomeResponsavel, string NomeAluno)
	{
		await this.CadastrarResponsavel(NomeResponsavel);

		var fichaAlunoPO = new FichaAlunoPO(Page);

		await fichaAlunoPO.GotoAsync();
		await fichaAlunoPO.ClicarEm("Cadastrar");
		await fichaAlunoPO.PreechoONomeCivil(NomeAluno);
		await fichaAlunoPO.SelecionoOSexo("Masculino");
		await fichaAlunoPO.InsiroONascimento("01/01/2000");
		await fichaAlunoPO.SelecionoANaturalidade("GOIANIA");
		await fichaAlunoPO.PreenchoOCEP("74375-230");
		await fichaAlunoPO.InserirNovoCPF(GeradorDeCPF.GerarCpf());
		await fichaAlunoPO.InformarCpfResponsavel(CPFResponsavel);
		await fichaAlunoPO.InserirAtributoAdicional();
		await fichaAlunoPO.ClicarEm("Gravar");
		await fichaAlunoPO.ValidarMensagem("Cadastro realizado, deseja ir para a tela de matrícula?");
		await fichaAlunoPO.ClicarEm("Não");
		await fichaAlunoPO.ClicarEm("Voltar para a lista");

		await this.GotoAsync();
	}

	// PESQUISAS ========================================================================

	public async Task PesquisarResponsavelPorID(string NomeResponsavel)
	{
		await Page.MainFrame.GetByPlaceholder("Pesquisar").TypeAsync(CodigoResponsavel);
		await Page.WaitForTimeoutAsync(1000);
		// Validar resultado apresentado  
		var resultado = await Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container").ElementHandleAsync();
		Assert.IsNotNull(resultado, "Grid de responsáveis não encontrada");
		string textoGrid = await resultado.InnerTextAsync();
		Assert.IsTrue(textoGrid.Contains(CodigoResponsavel), $"O código '{CodigoResponsavel}' do responsável '{NomeResponsavel}' não foi encontrado na grid. Texto atual: '{textoGrid}'");
	}

	// VALIDAÇÕES ========================================================================

	public async Task ValidarMensagem(string Mensagem)
	{
		//await Page.WaitForTimeoutAsync(600);
		var TextoMensagem = Page.MainFrame.GetByText(Mensagem);
		await TextoMensagem.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
		Assert.IsTrue(await TextoMensagem.IsVisibleAsync(), "Texto da mensagem não encontrado na tela.");
	}

	public async Task ValidarValorApresentadoNaGrid(string valorEsperado)
	{
		await Page.MainFrame.WaitForSelectorAsync("#GridAterrissagem .em-grid-container");
		var gridElemento = await Page.MainFrame.QuerySelectorAsync("css=#GridAterrissagem > .em-grid-container");
		Assert.IsNotNull(gridElemento, "Elemento da grid não encontrado.");
		string textoGrid = await gridElemento.InnerTextAsync();
		Assert.IsTrue(textoGrid.Contains(CodigoResponsavel), $"O código '{CodigoResponsavel}' não foi encontrado na grid. Texto atual: '{textoGrid}'");
		Assert.IsTrue(textoGrid.Contains(valorEsperado), $"O nome '{valorEsperado}' não foi encontrado na grid. Texto atual: '{textoGrid}'");
	}

	public async Task ValidarMensagemInformativaVinculo()
	{
		string mensagemImpeditiva = "Responsável não pode ser excluído, pois existe relação com as seguinte(s) matricula(s):";
		string? mensagem;

		IElementHandle? elementoDIV = await Page.MainFrame.QuerySelectorAsync(".modal.show .modal-body.d-flex.flex-column.text-center");
		if (elementoDIV is null)
		{
			Console.WriteLine("Retorno null do modal");
			return;
		}

		mensagem = await elementoDIV.TextContentAsync();

		if (!string.IsNullOrWhiteSpace(mensagem) && mensagem.StartsWith(mensagemImpeditiva, StringComparison.OrdinalIgnoreCase))
		{
			await this.ClicarEm(BotaoVoltar);
		}
		else
		{
			Console.WriteLine("Mensagem diferente da esperada: " + mensagem);
		}
	}

	public async Task ApresentarTelaDeAterrissagem()
	{
		var CampoDePesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");
		await CampoDePesquisa.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
		Assert.IsTrue(await CampoDePesquisa.IsVisibleAsync(), "O campo de pesquisa da tela de aterrissagem não está visível");
		var gridDeResponsaveis = await Page.MainFrame.Locator("css=#GridAterrissagem > .em-grid-container").IsVisibleAsync();
		Assert.IsTrue(gridDeResponsaveis, "A grid de aterrissagem não está visível.");
	}

	// ACÕES GERAIS =======================================================================

	public async Task InformarNome(string Nome)
	{
		NomeResponsavel = Nome;
		await Page.MainFrame.GetByLabel("Nome *").FillAsync(Nome);

		// capturar código do responsável
		var CodigoElemento = await Page.MainFrame.GetByLabel("Código").ElementHandleAsync();
		Assert.IsNotNull(CodigoElemento, "Elemento '#Codigo' não encontrado.");
		string? Codigo = await CodigoElemento.GetAttributeAsync("value");
		Assert.IsNotNull(Codigo, "O campo '#Codigo' não possui valor.");
		CodigoResponsavel = Codigo;
	}

	public async Task InformarDtNascimento(string DataNascimento)
	{
		await Page.MainFrame.GetByLabel("Nascimento").FillAsync(DataNascimento);
	}

	public async Task SelecionarSexo(string Sexo)
	{
		await Page.MainFrame.GetByLabel("Sexo *").SelectOptionAsync(new[] { Sexo });
	}

	public async Task SelecionarEstadoCivil(string EstadoCivil)
	{
		await Page.MainFrame.GetByLabel("Estado Civil *").SelectOptionAsync(new[] { EstadoCivil });
	}

	public async Task PreencherCEP(string CEP)
	{



		await Page.MainFrame.Locator("xpath=(//button[@type='button'])[27]").ClickAsync();
		await Page.MainFrame.Locator("id=EnderecoPesquisarLupa").ClickAsync();
		await Page.MainFrame.Locator("id=EnderecoPesquisaCep").ClickAsync();
		await Page.MainFrame.Locator("id=EnderecoPesquisaCep").FillAsync(CEP);
		await Page.MainFrame.Locator("id=EnderecoPesquisaLogradouro").ClickAsync();
		await Page.WaitForTimeoutAsync(500);
		await this.ClicarEm("Selecionar");
	}

	public async Task InformarCPF()
	{
		Faker faker = new();
		string novoCPF = faker.Person.Cpf();
		CPFResponsavel = novoCPF; // Armazena o CPF gerado
		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Documentos" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CPF/CNPJ" }).FillAsync(novoCPF);
	}

	public async Task AlterarNomeResponsavel(string NomeResponsavel)
	{
		await Page.MainFrame.GetByLabel("Nome *").FillAsync(NomeResponsavel);
	}

	public async Task ClicarEm(string botao)
	{
		switch (botao)
		{
			case "Cadastrar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
				break;
			case "Gravar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoGravar }).ClickAsync();
				break;
			case "Voltar para a lista":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoVoltarParaALista }).ClickAsync();
				break;
			case "Editar":
				await Page.MainFrame.Locator("//*[@id=\"grid-body\"]/div[1]/div[4]/button[3]").ClickAsync();
				break;
			case "Remover":
				await Page.MainFrame.Locator("//*[@id=\"grid-body\"]/div[1]/div[4]/button[4]").ClickAsync();
				break;
			case "Sim":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoSim }).ClickAsync();
				break;
			case "Selecionar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoSelecionar }).ClickAsync();
				break;
			case "Cancelar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoCancelar }).ClickAsync();
				break;
			case "Adicionar":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Adicionar" }).ClickAsync();
				break;
			case "CampoPesquisar":
				await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
				break;
			case "Enter":
				await Page.MainFrame.GetByLabel("Nome ou número de matrícula").PressAsync("Enter");
				break;
			default:
				Console.WriteLine($"Botão '{botao}' não está programado.");
				break;
		}
	}

	public async Task InserirAtributoAdicional()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Atributos Adicionais" }).ClickAsync();
		await Page.MainFrame.WaitForTimeoutAsync(500); // Aguarda a seção ser expandida

		var atributosDivLocator = Page.MainFrame.Locator("#DocumentosAtributos_Adicionais");
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
