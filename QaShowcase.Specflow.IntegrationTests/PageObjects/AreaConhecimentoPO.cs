using System.Text;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Globalization;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class AreaConhecimentoPO : POBase
{
	public AreaConhecimentoPO(IPage page) : base(page)
	{
	}
	public override string PageUrl => "CadastroAreaConhecimentoDisciplina";

	// Propriedades para armazenar os dados do teste
	public string codigoAreaConhecimento = null!;
	public string nomeAreaConhecimento = null!;

	// Constantes para os rótulos dos campos
	private const string Descricao = "Descrição *";
	private const string BotaoCadastrar = " Cadastrar";
	private const string BotaoVoltar = " Voltar";
	private const string BotaoSim = " Sim";

	public async Task InformarDescricao(string descricao)
	{
		nomeAreaConhecimento = descricao;
		await Page.MainFrame.GetByLabel(Descricao).FillAsync(descricao);
	}

	public async Task clicarEmOpcao(string botao)
	{
		var acoesBotao = new Dictionary<string, Func<Task>>
		{
			{ "Cadastrar", async () =>
				{
					await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoCadastrar }).ClickAsync();
				}
			},
			{ "Icone Editar", async () =>
				{
					var botaoEditar = await Page.MainFrame.WaitForSelectorAsync("#grid-body > div:nth-child(1) > div.em-grid-cell.em-grid-action.d-flex > button.btn.btn-outline-success");
					Assert.IsNotNull(botaoEditar, "Botão de editar não encontrado ou não visível na grid.");
					await botaoEditar!.ClickAsync();
					await Page.WaitForTimeoutAsync(500);
				}
			},
			{ "Editar", async () =>
				{
					await Page.MainFrame.GetByLabel("Descrição *").ClickAsync();
					await Page.MainFrame.GetByLabel("Descrição *").PressAsync("ArrowDown");
					string nomeEditado = $"Editado {nomeAreaConhecimento}";
					await Page.MainFrame.GetByLabel("Descrição *").FillAsync(nomeEditado);
					nomeAreaConhecimento = nomeEditado; // Atualiza a propriedade para refletir o novo valor

					var botaoEditar = await Page.MainFrame.WaitForSelectorAsync("#AddEditEntidadeNegocioEdit");
					await botaoEditar!.ClickAsync();
				}
			},
			{ "Excluir", async () =>
				{
					var botaoExcluir= await Page.MainFrame.WaitForSelectorAsync("css=.em-grid-row:nth-child(1) .btn-outline-danger > .fal");
					await botaoExcluir!.ClickAsync();

					await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoSim }).ClickAsync();
				}
			}
		};

		// Verifica se o botão existe no dicionário e executa a ação correspondente
		if (acoesBotao.TryGetValue(botao, out var acao))
		{
			await acao();
		}
		else
		{
			Console.WriteLine($"Botão '{botao}' não está programado.");
		}
	}

	public async Task PesquisarAreaConhecimento(string descricao)
	{
		// Captura o valor do campo #Codigo antes do cadastro
		var codigoElemento = await Page.MainFrame.QuerySelectorAsync("#Codigo");
		Assert.IsNotNull(codigoElemento, "Elemento '#Codigo' não encontrado.");
		string? codigo = await codigoElemento!.GetAttributeAsync("value");
		Assert.IsNotNull(codigo, "O campo '#Codigo' não possui valor.");
		codigoAreaConhecimento = codigo!;

		await this.InformarDescricao(descricao);
		await this.clicarEmOpcao("Cadastrar");
		await Page.WaitForTimeoutAsync(500);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = BotaoVoltar }).ClickAsync();
		await Page.MainFrame.GetByPlaceholder("Pesquisar").TypeAsync(nomeAreaConhecimento);
	}

	public async Task ValidarPesquisaAreaConhecimento()
	{
		var gridElemento = await Page.MainFrame.QuerySelectorAsync("#GridAterrissagem > div.em-grid-container");
		Assert.IsNotNull(gridElemento, "Elemento da grid não encontrado.");
		string textoGrid = await gridElemento.InnerTextAsync();

		Assert.IsTrue(textoGrid.Contains(codigoAreaConhecimento), $"O código '{codigoAreaConhecimento}' não foi encontrado na grid. Texto atual: '{textoGrid}'");
		Assert.IsTrue(textoGrid.Contains(nomeAreaConhecimento), $"O nome '{nomeAreaConhecimento}' não foi encontrado na grid. Texto atual: '{textoGrid}'");
	}

	public async Task ValidarMensagem(string mensagem)
	{
		await Page.WaitForTimeoutAsync(2000); // Se remover esse await vai quebrar a automação
		var elementoMensagem = await Page.MainFrame.QuerySelectorAsync("#form0 > div.modal.fade.show > div > div > div.modal-body.d-flex.flex-column.text-center");
		Assert.IsNotNull(elementoMensagem, "Elemento de mensagem não encontrado na tela.");

		// Captura o texto exibido
		var mensagemExibida = await elementoMensagem.TextContentAsync();

		// Remove acentos das duas mensagens para comparação
		string mensagemExibidaSemAcento = RemoverAcentos(mensagemExibida?.Trim() ?? "");
		string mensagemEsperadaSemAcento = RemoverAcentos(mensagem.Trim());

		// Compara as mensagens sem acentuação
		Assert.AreEqual(mensagemEsperadaSemAcento, mensagemExibidaSemAcento,
			$"Mensagem exibida diferente da esperada. Esperado: '{mensagemEsperadaSemAcento}', Obtido: '{mensagemExibidaSemAcento}'");
	}

	private string RemoverAcentos(string texto)
	{
		var normalized = texto.Normalize(NormalizationForm.FormD);
		var builder = new StringBuilder();

		foreach (var c in normalized)
		{
			if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
			{
				builder.Append(c);
			}
		}

		return builder.ToString().Normalize(NormalizationForm.FormC);
	}

}
