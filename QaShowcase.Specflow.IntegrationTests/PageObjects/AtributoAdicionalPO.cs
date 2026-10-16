using Microsoft.Playwright;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class AtributoAdicionalPO : POBase
{
	public AtributoAdicionalPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroAtributoAdicional";
	public string nomeAtributo = null!;
	public string nomePesquisa = null!;



	public async Task clicarEm(string botao)
	{
		// Dicionário para mapear os botões e suas ações
		var acoesBotao = new Dictionary<string, Func<Task>>
		{
			{ "Cadastrar", async () =>
				{
					await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
				}
			},
			{ "Excluir", async () =>
				{
					await Page.WaitForTimeoutAsync(1500);
					await Page.MainFrame.Locator("css=.em-grid-row:nth-child(1) .btn-outline-danger > .fal").ClickAsync();
					await Page.WaitForTimeoutAsync(1500);
					await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
					await Page.WaitForTimeoutAsync(1500);
					await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
				}
			},
			{ "Remover", async () =>
				{
					await Page.WaitForTimeoutAsync(1500);
					await Page.MainFrame.Locator("css=.em-grid-row:nth-child(1) .btn-outline-danger > .fal").ClickAsync();
				}
			},
			{ "Editar", async () =>
				{
					await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "" }).ClickAsync();
					await Page.WaitForTimeoutAsync(1000);
				}
			},
			{ "Gravar", async () =>
				{
					await Page.WaitForTimeoutAsync(500);
					await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
					await Page.WaitForTimeoutAsync(1000);
				}
			},
			{ "Voltar", async () =>
				{
					await Page.WaitForTimeoutAsync(1000);
					string cadastro = "Cadastro realizado com sucesso!";
					string exclusao = "Item excluído com sucesso.";
					string? mensagem;
					IElementHandle? elementoDIV = await Page.MainFrame.QuerySelectorAsync(".modal.show .modal-body.d-flex.flex-column.text-center");

					if (elementoDIV is null)
					{
						Console.WriteLine("Retorno null do modal");
						return;
					}

					mensagem = await elementoDIV.TextContentAsync();

					if (cadastro.Equals(mensagem))
					{
						await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
						await Page.WaitForTimeoutAsync(1000);
					}
					else if (exclusao.Equals(mensagem))
					{
						await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
						await Page.WaitForTimeoutAsync(1000);
					}
					else
					{
						Console.WriteLine("A mensagem recolhida foi: " + mensagem);
						return;
					}
				}
			},
			{ "Sim", async () =>
				{
					await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
					await Page.WaitForTimeoutAsync(1000);
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

	public async Task informarDescricao()
	{
		await Page.WaitForTimeoutAsync(500);
		string nomebase = "Aut.Atb.Adc";
		string dataHora = DateTime.Now.ToString("dd|HH:mm");
		string descricao = $"{nomebase} {dataHora}";
		nomeAtributo = descricao;
		await Page.MainFrame.GetByLabel("Descrição *").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição *").FillAsync(descricao);
	}

	public async Task informarADescricao(string descicaoInformada)
	{
		await Page.WaitForTimeoutAsync(500);
		string nomebase = descicaoInformada;
		string dataHora = DateTime.Now.ToString("dd|HH:mm");
		string descricao = $"{nomebase} {dataHora}";
		nomeAtributo = descricao;
		await Page.MainFrame.GetByLabel("Descrição *").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição *").FillAsync(descricao);
	}

	public async Task selecionarObrigatoriedade(string condicao)
	{
		if(condicao == "Sim")
		{
		    await Page.MainFrame.Locator("#EhObrigatorio").GetByText("Sim").ClickAsync();
		}
		else if(condicao == "Não")
		{
			await Page.MainFrame.Locator("#EhObrigatorio").GetByText("Não").ClickAsync();
		}
		else
		{
			Console.WriteLine("CONDIÇÃO INFORMADA NÃO É SIM OU NÃO");
			return;
		}

	}

	public async Task seleciononarUnicidade(string condicao)
	{
		if (condicao == "Sim")
		{
			await Page.MainFrame.Locator("#EhUnico").GetByText("Sim").ClickAsync();
		}
		else if(condicao == "Não")
		{
			await Page.MainFrame.Locator("#EhUnico").GetByText("Não").ClickAsync();
		}
		else
		{
			Console.WriteLine("CONDIÇÃO INFORMADA NÃO É SIM OU NÃO");
			return;
		}
	}

	public async Task selecionarConceito(string conceito)
	{
		if (conceito == "Aluno")
		{
			await Page.MainFrame.GetByLabel("Conceito *").SelectOptionAsync(new[] { "1" });
		}
		else if (conceito == "Ficha Médica")
		{
			await Page.MainFrame.GetByLabel("Conceito *").SelectOptionAsync(new[] { "2" });
		}
		else if (conceito == "Responsável")
		{
			await Page.MainFrame.GetByLabel("Conceito *").SelectOptionAsync(new[] { "3" });
		}
		else
		{
			Console.WriteLine("CONCEITO SELECIONADO: "+ conceito);
			return;
		}
	}

	public async Task selecionarTipo(string tipo)
	{
		if (tipo == "Texto")
		{
			await Page.MainFrame.GetByLabel("Tipo *").SelectOptionAsync(new[] { "1" });
		}
		else if (tipo == "Lista")
		{
			await Page.MainFrame.GetByLabel("Tipo *").SelectOptionAsync(new[] { "2" });
		}
		else
		{
			Console.WriteLine("TIPO SELECIONADO: " + tipo);
			return;
		}
	}

	public async Task definirTamanhoMaximo(string tamanho)
	{
        await Page.MainFrame.GetByLabel("Tamanho Máximo").FillAsync(""+tamanho);
	}

    public async Task adicionarItensNaLista(string qtdItens)
    {
        if (!int.TryParse(qtdItens, out int quantidade) || quantidade <= 0)
        {
            Console.WriteLine($"Erro: '{qtdItens}' não é um número válido ou é menor/igual a zero.");
            return;
        }

        for (int i = 0; i < quantidade; i++)
        {
            var campoDescricao = Page.MainFrame.GetByLabel("Descrição", new() { Exact = true });
            if (await campoDescricao.IsVisibleAsync())
            {
                await campoDescricao.ClickAsync();
                await campoDescricao.FillAsync((i + 1).ToString());
                await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Adicionar" }).ClickAsync();
            }
            else
            {
                Console.WriteLine("Erro: Campo 'Descrição' não está visível.");
                return;
            }
        }
    }

    public async Task cadastrarParaPesquisar(string atbPesquisar)
    {
        await this.clicarEm("Cadastrar");
        await this.informarADescricao(atbPesquisar);
        await this.selecionarObrigatoriedade("Não");
        await this.seleciononarUnicidade("Não");
        await this.selecionarConceito("Aluno");
        await this.selecionarTipo("Texto");
        await this.definirTamanhoMaximo("450");
        await this.clicarEm("Gravar");
        await this.clicarEm("Voltar");

		// Espera explícita: aguarda até que a grid de cadastros seja carregada
		await Page.MainFrame.WaitForSelectorAsync($"text= Cadastrar", new() { Timeout = 5000 });

		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClearAsync();
        await Page.MainFrame.GetByPlaceholder("Pesquisar").TypeAsync(nomeAtributo);
    }

    public async Task validarAtributoAdicionalPesquisado()
    {
        Assert.IsFalse(string.IsNullOrEmpty(nomeAtributo), "Erro: Nenhum valor foi definido para 'nomeAtributo'.");

        var elemento = Page.MainFrame.Locator($"text={nomeAtributo}");
        bool elementoVisivel = await elemento.IsVisibleAsync();

        Assert.IsTrue(elementoVisivel, $"Erro: Atributo adicional '{nomeAtributo}' não foi encontrado.");
        Console.WriteLine($"Atributo adicional '{nomeAtributo}' encontrado com sucesso.");
    }

    public async Task realizarAlteracao(string novaDescricao)
    {
        var campoDescricao = Page.MainFrame.GetByLabel("Descrição *");
        await campoDescricao.ClickAsync();
        await campoDescricao.ClearAsync();

		string nomebase = novaDescricao;
		string dataHora = DateTime.Now.ToString("dd|HH:mm");
		string descricao = $"{nomebase} {dataHora}";
		nomeAtributo = descricao;

		await campoDescricao.FillAsync(nomeAtributo);
    }

    public async Task validarMensagem(string mensagem)
    {
        // Busca o elemento que contém a mensagem
        var elementoMensagem = await Page.MainFrame.QuerySelectorAsync(".modal.show .modal-body.d-flex.flex-column.text-center");
        Assert.IsNotNull(elementoMensagem, "Elemento de mensagem não encontrado na tela.");

        // Captura o texto exibido
        var mensagemExibida = await elementoMensagem.TextContentAsync();

        // Compara o texto exibido com o esperado
        Assert.AreEqual(mensagem, mensagemExibida?.Trim(), $"Mensagem exibida diferente da esperada. Esperado: '{mensagem}', Obtido: '{mensagemExibida}'");
    }

}
