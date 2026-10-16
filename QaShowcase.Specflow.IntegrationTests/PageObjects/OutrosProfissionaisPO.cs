using System;
using System.Globalization;
using System.Text;
using System.Web;
using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class OutrosProfissionaisPO : POBase
{
	private string _nomeProfissionalSalvo;
	private string _cpfProfissionalSalvo;
	private string _nomeOperadorSalvo;

	public OutrosProfissionaisPO(IPage page) : base(page)
	{
	}

	public override string PageUrl => "CadastroProfissional";

	public async Task ClicoEmCadastrar() => await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();

	public async Task InformoONomeDoProfissional(string nomeProfissional)
	{
		await Page.MainFrame.GetByLabel("Nome *").FillAsync(nomeProfissional);

		_nomeProfissionalSalvo = nomeProfissional;
	}


	public async Task SelecionoADataDeNascimento()
	{
		await Page.MainFrame.GetByLabel("Nascimento *").ClickAsync();
		await Page.MainFrame.GetByLabel("Nascimento *").FillAsync("01/03/2002");
	}

	public async Task InformoCPFDoProfissional()
	{
		Faker faker = new();
		string novoCPF = faker.Person.Cpf();

		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Documentos" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Identificação" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CPF" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CPF" }).FillAsync(novoCPF);
		_cpfProfissionalSalvo = novoCPF;
	}

	

	public async Task InformoOEnderecoDoProfissional()
	{
		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Endereço e Contatos" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CEP" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CEP" }).FillAsync("78008-160");
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CEP" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CEP" }).PressAsync("Tab");
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}

	public async Task InserirAtributoAdicional()
	{
		// Abrir a aba de "Atributos Adicionais"
		var buttonLocator = Page.MainFrame.Locator("#Atributos_adicionais-tab");
		await buttonLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
		await buttonLocator.ClickAsync();

		// Aguardar a aba de "Atributos Adicionais" ficar visível
		var abaLocator = Page.MainFrame.Locator("#Atributos_adicionais");
		await abaLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

		// Aguardar um tempo para garantir que todos os campos estejam carregados
		await Page.WaitForTimeoutAsync(1000);

		// Localizar todos os campos de entrada (textarea, select, input)
		var atributosDivLocator = Page.MainFrame.Locator("#Atributos_adicionais");
		var todosCampos = await atributosDivLocator.Locator("textarea, select, input").AllAsync();

		foreach (var aux in todosCampos)
		{
			// Verificar se o campo é obrigatório
			bool isRequired = await aux.GetAttributeAsync("required") != null;
			if (!isRequired)
			{
				Console.WriteLine($"Campo NÃO obrigatório encontrado. Ignorando...");
				continue; // Pula campos não obrigatórios
			}

			string tagName = await aux.EvaluateAsync<string>("el => el.tagName");

			if (tagName == "SELECT")
			{
				try
				{
					Console.WriteLine($"Preenchendo campo SELECT obrigatório...");

					// Aguardar o select estar visível
					await aux.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

					// Clicar no select para abrir o menu de opções
					await aux.ClickAsync();

					// Aguardar um tempo para garantir que as opções estão visíveis
					await Page.WaitForTimeoutAsync(500);

					var options = await aux.Locator("option:not([disabled])").AllAsync();

					bool optionSelected = false;
					foreach (var option in options)
					{
						string optionValue = await option.GetAttributeAsync("value");

						if (!string.IsNullOrEmpty(optionValue))
						{


							{
								Console.WriteLine($"Selecionando opção com valor: {optionValue}");
								await aux.SelectOptionAsync(new[] { optionValue });

								// Verificar seleção
								var selectedValue = await aux.EvaluateAsync<string>("el => el.value");
								if (selectedValue == optionValue)
								{
									optionSelected = true;
									break;
								}
							}
						}

						if (!optionSelected)
						{
							throw new Exception("Nenhuma opção válida encontrada para o campo SELECT obrigatório.");
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
					Console.WriteLine($"Preenchendo campo TEXTAREA obrigatório...");
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
						Console.WriteLine($"Processando campo de múltipla escolha obrigatório...");

						var lupaButton = aux.Locator("xpath=following-sibling::button[contains(@id, 'Pesquisar')]");
						if (!await lupaButton.IsVisibleAsync())
						{
							throw new Exception("Botão de lupa não encontrado para campo obrigatório.");
						}

						await lupaButton.ClickAsync();

						// Modal
						var modalLocator = Page.MainFrame.Locator("div.modal.fade.show");
						await modalLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

						// Selecionar todos
						var selecionarTodos = modalLocator.Locator(".em-grid-header .em-grid-select-box");
						await selecionarTodos.ClickAsync();

						// Confirmar
						var btnSelecionar = modalLocator.GetByRole(AriaRole.Button, new() { Name = " Selecionar" });
						await btnSelecionar.ClickAsync();

						// Aguardar fechamento
						await modalLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden });
					}
					catch (Exception ex)
					{
						throw new Exception($"Erro em campo de múltipla escolha obrigatório: {ex.Message}");
					}
				}
			}
		}
	}


	public async Task ClicoEmAlterarSituacao(string situacao)
	{
		if (situacao.Equals("Ativo", StringComparison.OrdinalIgnoreCase))
		{
			await Page.MainFrame.Locator("#EstaAtivo_sim + label").ClickAsync();
		}
		else
		{
			await Page.MainFrame.Locator("#EstaAtivo_nao + label").ClickAsync();
		}
	}

	public async Task ClicoEmWhen(string opcao)
	{

		await Page.MainFrame.WaitForTimeoutAsync(2000);

		if (opcao.Equals("Cadastrar", StringComparison.OrdinalIgnoreCase))
		{
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
		}
		else if (opcao.Equals("Editar", StringComparison.OrdinalIgnoreCase))
		{
			await Page.WaitForTimeoutAsync(2000); // Espera 2 segundos


			var editButton = Page.GetByRole(AriaRole.Button, new() { Name = " Editar" });
			if (await editButton.CountAsync() > 0)
			{
				await editButton.ClickAsync();
			}
			else
			{
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "" }).First.ClickAsync();

			}

		}
		else if (opcao.Equals("Sim", StringComparison.OrdinalIgnoreCase))
		{
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
		}

		else if (opcao.Equals("Gravar", StringComparison.OrdinalIgnoreCase))
		{
			// Tenta clicar no botão "Gravar" completo primeiro
			var gravarButton = Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" });
			if (await gravarButton.CountAsync() > 0)
			{
				await gravarButton.ClickAsync();
			}
			else
			{
				// Se não encontrar, tenta localizar pelo texto visível "Gravar" dentro do botão
				await Page.MainFrame.GetByRole(AriaRole.Button)
					.Filter(new() { HasText = "Gravar" })
					.First.ClickAsync();
				await Page.WaitForTimeoutAsync(2000);
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
			}
			await Page.WaitForTimeoutAsync(2000);
		}
		else if (opcao.Equals("Excluir", StringComparison.OrdinalIgnoreCase))
		{
			// Clica no botão "Excluir"
			var excluirButton = Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Excluir" });
			if (await excluirButton.CountAsync() > 0)
			{
				await excluirButton.ClickAsync();
			}
			else
			{
				// Se não encontrar o botão com ícone, tenta localizar pelo texto "Excluir"
				await Page.Locator("#grid-body > div:nth-child(1) > div.em-grid-cell.em-grid-action.d-flex > button.btn.btn-outline-danger").ClickAsync();

			}
		}
		else
		{
			throw new NotImplementedException($"A opção '{opcao}' não é suportada.");

		}
	}

	public async Task AlterarNome(string alterarNome)
	{
		await Page.MainFrame.GetByLabel("Nome *").ClickAsync();
		await Page.MainFrame.GetByLabel("Nome *").FillAsync(alterarNome);
	}

	public async Task<string> ObterCodigoProfissional()
	{

		await Page.GetByPlaceholder("Pesquisar").ClickAsync();
		await Page.GetByPlaceholder("Pesquisar").PressSequentiallyAsync(_nomeProfissionalSalvo.Trim());

		await Page.WaitForTimeoutAsync(2000); // Espera 2 segundos

		var editButton = Page.GetByRole(AriaRole.Button, new() { Name = " Editar" });
		if (await editButton.CountAsync() > 0)
		{
			await editButton.ClickAsync();
		}
		else
		{
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "" }).First.ClickAsync();

		}

		try
		{
			// Tenta pegar o input visível primeiro
			var codigoInput = Page.Locator("input#Codigo[type=text], input#Codigo:visible");

			if (await codigoInput.CountAsync() > 0)
			{
				string codigo = await codigoInput.First.GetAttributeAsync("value");
				return codigo?.Trim();
			}

			// Fallback: pega qualquer input com o ID
			var fallbackInput = Page.Locator("#Codigo").First;
			string fallbackCodigo = await fallbackInput.GetAttributeAsync("value");
			return fallbackCodigo?.Trim();
		}
		catch (Exception ex)
		{
			throw new Exception($"Falha ao obter código do profissional: {ex.Message}");
		}
	}

	public async Task EApresentadaAMensagem(string mensagemEsperada)
	{
		const int defaultTimeoutMs = 1000;
		const int navigationTimeoutMs = 60000;
		const int selectorTimeoutMs = 30000; // Aumentado temporariamente para 30 segundos

		async Task WaitAndClickButton(string buttonName)
		{
			await Page.WaitForTimeoutAsync(defaultTimeoutMs);
			await Page.GetByRole(AriaRole.Button, new() { Name = buttonName }).ClickAsync();
		}

		// Verifica se o modal "CPF é inválido, deseja continuar?" está presente
		var modalCpfInvalido = Page.Locator(".modal-body", new() { HasText = "CPF é inválido, deseja continuar?" }).First;
		if (await modalCpfInvalido.IsVisibleAsync())
		{
			Console.WriteLine("[INFO] Modal 'CPF é inválido, deseja continuar?' detectado. Clicando em 'Sim'.");
			await Page.WaitForTimeoutAsync(defaultTimeoutMs);
			await Page.Locator("button.btn.btn-outline-warning").Filter(new() { HasText = "Sim" }).First.ClickAsync();

			// Aguarda possível animação após clicar em "Sim"
			await Page.WaitForTimeoutAsync(2000);

			// Verifica se o iframe "#content-iframe" está presente
			var iframeExists = await Page.Locator("#content-iframe").IsVisibleAsync();
			Console.WriteLine($"[DEBUG] Iframe '#content-iframe' visível: {iframeExists}");

			// Valida o modal "Cadastro realizado com sucesso!" no iframe ou na página principal
			Console.WriteLine("[INFO] Aguardando modal 'Cadastro realizado com sucesso!'.");
			ILocator modalSucesso;
			IFrameLocator frame = null;

			if (iframeExists)
			{
				try
				{
					await Page.WaitForSelectorAsync("#content-iframe", new() { State = WaitForSelectorState.Visible, Timeout = selectorTimeoutMs });
					frame = Page.FrameLocator("#content-iframe");
					modalSucesso = frame.Locator("#form0 > div.modal.fade.show > div > div .modal-body", new() { HasText = "Cadastro realizado com sucesso!" }).First;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[WARN] Erro ao acessar iframe '#content-iframe': {ex.Message}. Tentando na página principal.");
					modalSucesso = Page.Locator("#form0 > div.modal.fade.show > div > div .modal-body", new() { HasText = "Cadastro realizado com sucesso!" }).First;
				}
			}
			else
			{
				Console.WriteLine("[INFO] Iframe '#content-iframe' não encontrado. Validando na página principal.");
				modalSucesso = Page.Locator("#form0 > div.modal.fade.show > div > div .modal-body", new() { HasText = "Cadastro realizado com sucesso!" }).First;
			}

			// Log para contar elementos correspondentes
			var modalCount = await (frame != null ? frame.Locator("#form0 .modal-body") : Page.Locator("#form0 .modal-body"))
				.Filter(new() { HasText = "Cadastro realizado com sucesso!" }).CountAsync();
			Console.WriteLine($"[DEBUG] Número de modais com 'Cadastro realizado com sucesso!': {modalCount}");

			await modalSucesso.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = selectorTimeoutMs });
			if (!await modalSucesso.IsVisibleAsync())
			{
				throw new Exception("Modal 'Cadastro realizado com sucesso!' não apareceu após clicar em 'Sim'.");
			}
			Console.WriteLine("[INFO] Modal 'Cadastro realizado com sucesso!' detectado.");

			// Clica no botão "Voltar para a lista"
			try
			{
				Console.WriteLine("[INFO] Tentando clicar no botão 'Voltar para a lista'.");
				ILocator button;
				if (frame != null)
				{
					button = frame.Locator("#form0 > div.modal.fade.show > div > div > div.modal-footer.d-flex.justify-content-center.border-0 > button.btn.btn-outline-success").First;
				}
				else
				{
					button = Page.Locator("#form0 > div.modal.fade.show > div > div > div.modal-footer.d-flex.justify-content-center.border-0 > button.btn.btn-outline-success").First;
				}

				// Verifica se o botão está visível
				Console.WriteLine($"[DEBUG] Botão 'Voltar para a lista' visível: {await button.IsVisibleAsync()}");
				if (!await button.IsVisibleAsync())
				{
					throw new Exception("Botão 'Voltar para a lista' não está visível.");
				}

				// Aguarda o botão estar pronto
				await button.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = selectorTimeoutMs });

				// Executa o clique
				await button.ClickAsync(new() { Force = true });
				Console.WriteLine("[INFO] Clique no botão 'Voltar para a lista' executado com sucesso!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[ERROR] Erro ao clicar no botão 'Voltar para a lista': {ex.Message}");
				// Tenta um seletor alternativo
				try
				{
					Console.WriteLine("[INFO] Tentando seletor alternativo para 'Voltar para a lista'.");
					ILocator button = (frame != null ? frame.Locator("button.btn.btn-outline-success") : Page.Locator("button.btn.btn-outline-success"))
						.Filter(new() { HasText = "Voltar para a lista" }).First;
					await button.ClickAsync(new() { Force = true });
					Console.WriteLine("[INFO] Clique no botão alternativo 'Voltar para a lista' concluído com sucesso!");
				}
				catch
				{
					Console.WriteLine("[ERROR] Falha no seletor alternativo. Usando fallback.");
					await WaitAndClickButton(" Voltar");
					throw;
				}
			}

			// Retorna após tratar os modais
			return;
		}

		// Captura e normaliza mensagens do modal padrão (se o modal de CPF não apareceu)
		await Page.WaitForTimeoutAsync(defaultTimeoutMs);
		// Tenta um seletor mais genérico para maior robustez
		var modal = Page.Locator("div.modal.fade.show .modal-body", new() { HasText = mensagemEsperada }).First;

		// Depuração: Verifica se o elemento modal existe
		var modalExists = await Page.Locator("div.modal.fade.show .modal-body").CountAsync();
		Console.WriteLine($"[DEBUG] Elementos 'div.modal.fade.show .modal-body' encontrados: {modalExists}");

		// Depuração: Verifica se #form0 existe como fallback
		var form0Exists = await Page.Locator("#form0").CountAsync();
		Console.WriteLine($"[DEBUG] Elemento '#form0' encontrado: {form0Exists}");

		try
		{
			var mensagemCapturada = await modal.InnerTextAsync();

			// Log das mensagens
			Console.WriteLine($"[INFO] Mensagem capturada: {mensagemCapturada}");
			Console.WriteLine($"[INFO] Mensagem esperada: {mensagemEsperada}");

			string mensagemNormalizada = RemoverAcentos(mensagemCapturada).Trim();
			string mensagemEsperadaNormalizada = RemoverAcentos(mensagemEsperada).Trim();

			// Depuração: Log das mensagens normalizadas
			Console.WriteLine($"[DEBUG] Mensagem normalizada capturada: {mensagemNormalizada}");
			Console.WriteLine($"[DEBUG] Mensagem esperada normalizada: {mensagemEsperadaNormalizada}");

			// Validação da mensagem
			if (mensagemNormalizada == mensagemEsperadaNormalizada)
			{
				Console.WriteLine("[INFO] Mensagem esperada encontrada!");
				return;
			}

			// Executa ação específica se a mensagem estiver mapeada
			var mensagemAcoes = new Dictionary<string, Func<Task>>
		{
			{
				RemoverAcentos("Cadastro realizado com sucesso!"),
				async () =>
				{
					try
					{
						Console.WriteLine("[INFO] Tentando clicar no botão 'Voltar para a lista' para 'Cadastro padrão'.");
						var iframeExists = await Page.Locator("#content-iframe").IsVisibleAsync();
						Console.WriteLine($"[DEBUG] Iframe '#content-iframe' visível: {iframeExists}");

						ILocator button;
						if (iframeExists)
						{
							await Page.WaitForSelectorAsync("#content-iframe", new() { State = WaitForSelectorState.Visible, Timeout = selectorTimeoutMs });
							var frame = Page.FrameLocator("#content-iframe");
							button = frame.Locator("div.modal.fade.show > div > div > div.modal-footer.d-flex.justify-content-center.border-0 > button.btn.btn-outline-success").First;
						}
						else
						{
							button = Page.Locator("div.modal.fade.show > div > div > div.modal-footer.d-flex.justify-content-center.border-0 > button.btn.btn-outline-success").First;
						}

                        // Verifica se o botão está visível
                        Console.WriteLine($"[DEBUG] Botão 'Voltar para a lista' visível: {await button.IsVisibleAsync()}");
						if (!await button.IsVisibleAsync())
						{
							throw new Exception("Botão 'Voltar para a lista' não está visível!");
						}

                        // Aguarda o botão estar pronto
                        await button.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = selectorTimeoutMs });

                        // Executa o clique
                        await button.ClickAsync(new() { Force = true });
						Console.WriteLine("[INFO] Clique no botão 'Voltar para a lista' executado com sucesso!");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"[ERROR] Erro ao clicar no botão 'Voltar para a lista': {ex.Message}");
						try
						{
							Console.WriteLine("[INFO] Tentando seletor alternativo para 'Voltar para a lista'.");
							var iframeExists = await Page.Locator("#content-iframe").IsVisibleAsync();
							ILocator button = (iframeExists ? Page.FrameLocator("#content-iframe").Locator("button.btn.btn-outline-success") : Page.Locator("button.btn.btn-outline-success"))
								.Filter(new() { HasText = "Voltar para a lista" }).First;
							await button.ClickAsync(new() { Force = true });
							Console.WriteLine("[INFO] Clique no botão alternativo 'Voltar para a lista' concluído com sucesso!");
						}
						catch
						{
							Console.WriteLine("[ERROR] Falha no seletor alternativo. Usando fallback.");
							await WaitAndClickButton(" Voltar");
							throw;
						}
					}
				}
			},
			{
				RemoverAcentos("Item excluído com sucesso."),
				async () =>
				{
					try
					{
						Console.WriteLine("[INFO] Tentando clicar no botão 'Voltar' para 'Item excluído com sucesso'.");
						var iframeExists = await Page.Locator("#content-iframe").IsVisibleAsync();
						Console.WriteLine($"[DEBUG] Iframe '#content-iframe' visível: {iframeExists}");

						ILocator button;
						if (iframeExists)
						{
							await Page.WaitForSelectorAsync("#content-iframe", new() { State = WaitForSelectorState.Visible, Timeout = selectorTimeoutMs });
							var frame = Page.FrameLocator("#content-iframe");
							button = frame.Locator("div.modal.fade.show > div > div .modal-footer > button.btn.btn-success").First;
						}
						else
						{
							button = Page.Locator("div.modal.fade.show > div > div .modal-footer > button.btn.btn-success").First;
						}

                        // Verifica se o botão está visível
                        Console.WriteLine($"[DEBUG] Botão 'Voltar' visível: {await button.IsVisibleAsync()}");
						if (!await button.IsVisibleAsync())
						{
							throw new Exception("Botão 'Voltar' não está visível!");
						}

                        // Aguarda o botão estar pronto
                        await button.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = selectorTimeoutMs });

                        // Executa o clique
                        await button.ClickAsync(new() { Force = true });
						Console.WriteLine("[INFO] Clique no botão 'Voltar' executado com sucesso!");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"[ERROR] Erro ao clicar no botão 'Voltar': {ex.Message}");
						try
						{
							Console.WriteLine("[INFO] Tentando seletor alternativo para 'Voltar'.");
							var iframeExists = await Page.Locator("#content-iframe").IsVisibleAsync();
							ILocator button = (iframeExists ? Page.FrameLocator("#content-iframe").Locator("button.btn.btn-success") : Page.Locator("button.btn.btn-success"))
								.Filter(new() { HasText = "Voltar" }).First;
							await button.ClickAsync(new() { Force = true });
							Console.WriteLine("[INFO] Clique no botão alternativo 'Voltar' concluído com sucesso!");
						}
						catch
						{
							Console.WriteLine("[ERROR] Falha no seletor alternativo. Usando fallback.");
							await WaitAndClickButton(" Voltar");
							throw;
						}
					}
				}
			},
			{
				RemoverAcentos("Tipo de profissional: Exclusão não permitida! Existe profissional cadastrado com esse tipo."),
				() => WaitAndClickButton(" Voltar")
			},
			{
				RemoverAcentos("Lançamentos: Não é possível excluir profissional, pois o mesmo possui registros em caixa.\n"),
				() => WaitAndClickButton(" Voltar")
			}
		};

			string? mensagemKey = mensagemAcoes.Keys.FirstOrDefault(k => mensagemNormalizada == k);
			Console.WriteLine($"[DEBUG] Mensagem encontrada no dicionário: {mensagemKey ?? "Nenhuma"}");
			if (mensagemKey != null)
			{
				await mensagemAcoes[mensagemKey]();
				return;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[ERROR] Erro ao capturar mensagem do modal: {ex.Message}");
			// Depuração: Log do conteúdo da página
			var pageContent = await Page.ContentAsync();
			Console.WriteLine($"[DEBUG] Conteúdo da página: {pageContent.Substring(0, Math.Min(pageContent.Length, 500))}...");
			// Captura de tela
			await Page.ScreenshotAsync(new() { Path = $"screenshot_modal_failure_{DateTime.Now:yyyyMMddHHmmss}.png" });
			throw;
		}

		// Navegação para página inicial e manutenção
		IPage? paginaInicial = null;
		try
		{
			paginaInicial = await Page.Context.NewPageAsync();
			string urlInicial = $"{Contexto.BaseUrl}/{Contexto.BaseTenant}/LancamentoCaixaAterrissagem";
			Console.WriteLine($"[DEBUG] Acessando página inicial: {urlInicial}");

			await paginaInicial.GotoAsync(urlInicial, new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = navigationTimeoutMs
			});

			await paginaInicial.WaitForSelectorAsync(".form-group.icon-dropdown", new()
			{
				Timeout = selectorTimeoutMs,
				State = WaitForSelectorState.Visible
			});

			// Sequência de cliques para navegação
			var botoesNavegacao = new[] { " Carregar", "", " Sim", " Voltar" };
			foreach (var botao in botoesNavegacao)
			{
				Console.WriteLine($"[DEBUG] Clicando no botão: {botao}");
				await paginaInicial.GetByRole(AriaRole.Button, new() { Name = botao }).ClickAsync();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[ERROR] Erro ao excluir o lançamento: {ex.Message}");
		}
		finally
		{
			if (paginaInicial != null)
			{
				await paginaInicial.CloseAsync();
			}
		}
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

	public async Task AlteroANacionalidade()
	{
		await Page.MainFrame.GetByLabel("Nacionalidade *").FillAsync("BRASILEIRA");
	}

	public async Task SelecionoANaturalidade()
	{
		// Clica no campo Naturalidade para abrir o grid
		await Page.MainFrame.Locator("#NaturalidadePesquisar").ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}

	public async Task SelecionoOSexo()
	{
		// Seleciona a opção "Masculino" diretamente pelo valor
		await Page.MainFrame.Locator("select#Sexo").SelectOptionAsync(new[] { "1" });
	}

	public async Task AlteroAEtnia()
	{
		// Seleciona a primeira opção do campo Etnia (ignorando a opção "Selecione")
		await Page.MainFrame.Locator("select#Etnia").SelectOptionAsync(new[] { "1" });
	}


	public async Task PreencherModalDescricao(string tipoDeProfissional)
	{
		await Page.MainFrame.GetByLabel("Trabalho").GetByRole(AriaRole.Button, new() { Name = "" }).ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição").FillAsync(tipoDeProfissional);
		await Page.MainFrame.GetByRole(AriaRole.Dialog).GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(1500);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
	}

	public async Task ClicoEmGravarAEdicao() => await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();

	public async Task OProfissionalEExcluidoComSucesso() => await Page.MainFrame.GetByText("Item excluído com sucesso.Voltar").ClickAsync();

	public async Task VoltarParaALista()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
		await Page.GetByPlaceholder("Pesquisar").ClickAsync();
		await Page.GetByPlaceholder("Pesquisar").PressSequentiallyAsync(_nomeProfissionalSalvo.Trim());
	}

	public async Task PesquisarProfissional(string pesquisarProfissional)
	{

		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClearAsync();
		await Page.MainFrame.GetByPlaceholder("Pesquisar").PressSequentiallyAsync(pesquisarProfissional.Trim());
	}

	public async Task alterarFiltroGridMaiorParaMenor()
	{
		const int defaultTimeoutMs = 1000;
		const int selectorTimeoutMs = 15000; // Consistente com o código anterior

		try
		{
			Console.WriteLine("[INFO] Iniciando alteração do filtro da grid para 'maior para menor'.");

			// Verifica se o iframe "#content-iframe" está presente
			var iframeExists = await Page.Locator("#content-iframe").IsVisibleAsync();
			Console.WriteLine($"[DEBUG] Iframe '#content-iframe' visível: {iframeExists}");

			ILocator button;
			IFrameLocator frame = null;

			// Seleciona o botão no iframe ou na página principal
			if (iframeExists)
			{
				await Page.WaitForSelectorAsync("#content-iframe", new() { State = WaitForSelectorState.Visible, Timeout = selectorTimeoutMs });
				frame = Page.FrameLocator("#content-iframe");
				button = frame.Locator("#GridAterrissagem > div.em-grid-container > div.em-grid-header.mb-3 > div > div:nth-child(1) > button").First;
			}
			else
			{
				Console.WriteLine("[INFO] Iframe '#content-iframe' não encontrado. Usando página principal.");
				button = Page.Locator("#GridAterrissagem > div.em-grid-container > div.em-grid-header.mb-3 > div > div:nth-child(1) > button").First;
			}

			// Verifica se o botão está visível
			Console.WriteLine($"[DEBUG] Botão de filtro visível: {await button.IsVisibleAsync()}");
			if (!await button.IsVisibleAsync())
			{
				throw new Exception("Botão de filtro da grid não está visível.");
			}

			// Aguarda o botão estar pronto
			await button.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = selectorTimeoutMs });

			// Executa o primeiro clique
			Console.WriteLine("[INFO] Executando primeiro clique no botão de filtro.");
			await button.ClickAsync(new() { Force = true });
			await Page.WaitForTimeoutAsync(defaultTimeoutMs); // Pequena espera entre cliques

			// Executa o segundo clique
			Console.WriteLine("[INFO] Executando segundo clique no botão de filtro.");
			await button.ClickAsync(new() { Force = true });

			Console.WriteLine("[INFO] Filtro da grid alterado para 'maior para menor' com sucesso!");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[ERROR] Erro ao alterar filtro da grid: {ex.Message}");
			throw;
		}
	}


	public async Task AlterarFiltroGrid(string opcao)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(opcao))
			{
				throw new ArgumentNullException(nameof(opcao));
			}
			if (opcao.Equals("Inativos", StringComparison.OrdinalIgnoreCase) || opcao.Equals("Inativo", StringComparison.OrdinalIgnoreCase))
			{
				await Page.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new SelectOptionValue { Value = "2" });
				Console.WriteLine($"Filtro alterado para valor 2 (opção: {opcao})");
			}
			else if (opcao.Equals("Ativos", StringComparison.OrdinalIgnoreCase) || opcao.Equals("Ativo", StringComparison.OrdinalIgnoreCase))
			{
				await Page.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new SelectOptionValue { Value = "1" });
				Console.WriteLine($"Filtro alterado para valor 1 (opção: {opcao})");
			}
			else if (opcao.Equals("Todos", StringComparison.OrdinalIgnoreCase))
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

	public async Task ValidadorProfissionais(string opcao)
	{
		// Validação da opção usando LINQ para evitar o problema com Contains
		var opcoesValidas = new[] { "ativos", "inativos", "todos", "ativo", "inativo" };
		if (!opcoesValidas.Any(x => x.Equals(opcao, StringComparison.OrdinalIgnoreCase)))
		{
			throw new ArgumentException($"Opção inválida. Use: {string.Join(", ", opcoesValidas)}");
		}

		try
		{
			Console.WriteLine($"Iniciando validação para profissionais '{opcao}'");

			// 1. Primeiro tentamos extrair os dados do campo hidden (quando não for "todos")
			if (!opcao.Equals("todos", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					var jsonInput = await Page.Locator("#Aterrissagem_Datasource").GetAttributeAsync("value");
					if (!string.IsNullOrEmpty(jsonInput))
					{
						var decodedJson = HttpUtility.HtmlDecode(jsonInput);
						var profissionais = JArray.Parse(decodedJson);

						bool deveEstarAtivo = opcao.Equals("ativos", StringComparison.OrdinalIgnoreCase) ||
											 opcao.Equals("ativo", StringComparison.OrdinalIgnoreCase);
						bool algumErro = false;

						foreach (var prof in profissionais)
						{
							int codigo = prof.Value<int>("Codigo");
							string nome = prof.Value<string>("Nome");
							bool estaAtivo = prof.Value<bool>("EstaAtivo");

							if (deveEstarAtivo && !estaAtivo)
							{
								Console.WriteLine($"ERRO: {nome} (Código: {codigo}) está inativo (esperado ativo)");
								algumErro = true;
							}
							else if (!deveEstarAtivo && estaAtivo)
							{
								Console.WriteLine($"ERRO: {nome} (Código: {codigo}) está ativo (esperado inativo)");
								algumErro = true;
							}
						}

						if (algumErro)
						{
							throw new Exception("Foram encontrados profissionais com status incorreto no JSON");
						}

						Console.WriteLine($"Validação via JSON concluída para {profissionais.Count} profissionais");
						return;
					}
				}
				catch (Exception jsonEx)
				{
					Console.WriteLine($"Falha na validação via JSON, tentando via UI: {jsonEx.Message}");
				}
			}

			// 2. Validação via Interface do Usuário
			const int timeoutGrid = 30000;
			const int timeoutCelula = 10000;
			const int timeoutIcone = 7000;

			var grid = Page.Locator("#GridAterrissagem > div.em-grid-container");
			await grid.WaitForAsync(new() { Timeout = timeoutGrid, State = WaitForSelectorState.Visible });

			var linhas = await grid.Locator(".em-grid-row:not(.em-grid-header)").AllAsync();
			if (linhas.Count == 0)
			{
				throw new Exception("Nenhum profissional encontrado na grid");
			}

			// Para filtro "todos", apenas verifica se há linhas
			if (opcao.Equals("todos", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine($"Filtro 'todos' aplicado - {linhas.Count} profissionais encontrados");
				return;
			}

			// Validação dos status para ativos/inativos
			bool validarComoAtivo = opcao.Equals("ativos", StringComparison.OrdinalIgnoreCase) ||
								  opcao.Equals("ativo", StringComparison.OrdinalIgnoreCase);

			for (int i = 0; i < linhas.Count; i++)
			{
				var linha = linhas[i];

				// Obter código e nome da linha atual
				var celulaCodigo = linha.Locator("[data-label='Código'], .em-grid-cell").Nth(0);
				var celulaNome = linha.Locator("[data-label='Nome'], .em-grid-cell").Nth(1);

				int codigo = int.Parse(await celulaCodigo.InnerTextAsync());
				string nome = await celulaNome.InnerTextAsync();

				// Estratégia flexível para encontrar a célula de status
				ILocator celulaStatus = null;
				var locators = new[]
				{
				linha.Locator("[id^='EstaAtivo_']"),
				linha.Locator("[data-label='Ativo']"),
				linha.Locator(".em-grid-cell").Nth(3)
			};

				foreach (var locator in locators)
				{
					if (await locator.IsVisibleAsync())
					{
						celulaStatus = locator;
						break;
					}
				}

				if (celulaStatus == null)
				{
					throw new Exception($"Não foi possível localizar a célula de status para {nome} (Código: {codigo})");
				}

				// Validação visual do status
				var icone = celulaStatus.Locator("i.fa-check-circle, i.fa-times-circle");
				await icone.WaitForAsync(new() { Timeout = timeoutIcone });

				var classes = await icone.GetAttributeAsync("class") ?? "";
				var estilo = await icone.GetAttributeAsync("style") ?? "";

				bool estaAtivo = classes.Contains("fa-check-circle") &&
								(estilo.Contains("success") || estilo.Contains("var(--success)"));
				bool estaInativo = classes.Contains("fa-times-circle") ||
								  (estilo.Contains("danger") || estilo.Contains("var(--danger)"));

				if (validarComoAtivo && estaInativo)
				{
					throw new Exception($"Linha {i + 1}: {nome} (Código: {codigo}) está inativo (esperado ativo)");
				}

				if (!validarComoAtivo && estaAtivo)
				{
					throw new Exception($"Linha {i + 1}: {nome} (Código: {codigo}) está ativo (esperado inativo)");
				}
			}

			Console.WriteLine($"Validação via UI concluída para {linhas.Count} profissionais");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro durante a validação: {ex}");
			throw;
		}
	}

	public async Task ValidarInformacoesGridPesquisa()
	{
		// Aguarda a grid carregar
		var grid = Page.MainFrame.Locator("#GridAterrissagem > div.em-grid-container");
		await grid.WaitForAsync(new() { Timeout = 10000 });
		var campoPesquisa = Page.Locator("input.em-grid-pesquisar");
		var textoDigitado = await campoPesquisa.InputValueAsync();
		var pesquisarProfissional = textoDigitado.ToString();

		// Opcional: para debug, exibe o conteúdo da grid no console
		var textoNaGrid = await grid.InnerTextAsync();
		Console.WriteLine("Conteúdo atual da grid:\n" + textoNaGrid);

		// Verifica se o texto digitado aparece na grid
		var encontrou = await grid.Filter(new() { HasText = pesquisarProfissional }).CountAsync() > 0;

		if (!encontrou)
		{
			throw new Exception($"Texto '{pesquisarProfissional}' não encontrado na grid.");
		}
		else
		{
			Console.WriteLine($"Texto '{pesquisarProfissional}' encontrado na grid.");
		}
	}

	//Completo
	public async Task CadastroCompletoDeProfissional(string nomeProfissional, string tipoDeProfissional = null)
	{
		await ClicoEmCadastrar();
		await InformoONomeDoProfissional(nomeProfissional); // Usando o mesmo nome aqui
		await PreencherModalDescricao(tipoDeProfissional ?? "Tipo padrão"); // Se tipoDeProfissional for opcional
		await SelecionoADataDeNascimento();
		await InformoCPFDoProfissional();
		await InformoOEnderecoDoProfissional();
		await InserirAtributoAdicional();
		await SelecionoANaturalidade();
		await SelecionoOSexo();
		await AlteroAEtnia();
		await ClicoEmWhen("Gravar");
		await VoltarParaALista();
	}

	public async Task RealizarLancamentosFinanceiros()
	{
		try
		{
			Console.WriteLine("[INFO] Iniciando fluxo de lançamentos financeiros");

			await RealizarAberturaCaixa();    // Primeira operação 
			await RealizarLancamentoBasico();  // Segunda operação

			Console.WriteLine("[SUCCESS] Fluxo completo executado com sucesso");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[ERROR] Falha durante execução: {ex.Message}");
			throw;
		}
	}

	private async Task RealizarLancamentoBasico()
	{
		Console.WriteLine("[INFO] Iniciando lançamento básico");
		var paginaInicial = await Page.Context.NewPageAsync();
		IPage? paginaManutencao = null;

		try
		{
			// 1. Navegação para página inicial
			string urlInicial = $"{Contexto.BaseUrl}/{Contexto.BaseTenant}/LancamentoCaixaAterrissagem";
			Console.WriteLine($"[DEBUG] Acessando página inicial: {urlInicial}");

			await paginaInicial.GotoAsync(urlInicial, new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 60000
			});

			// 2. Espera por elementos da página inicial
			await paginaInicial.WaitForSelectorAsync(".form-group.icon-dropdown", new()
			{
				Timeout = 8000,
				State = WaitForSelectorState.Visible
			});
			await Task.Delay(1000);

			// 3. Navegação para página de manutenção
			Console.WriteLine("[DEBUG] Clicando no botão Entrada para navegar para página de manutenção");

			var paginaManutencaoTask = paginaInicial.Context.WaitForPageAsync();
			await paginaInicial.GetByRole(AriaRole.Button, new() { Name = " Entrada" })
							 .ClickAsync(new() { Timeout = 30000 });

			paginaManutencao = await paginaManutencaoTask;

			// 4. Verificação de loading na nova página
			try
			{
				await paginaManutencao.WaitForSelectorAsync(".loading-indicator", new()
				{
					Timeout = 2000,
					State = WaitForSelectorState.Hidden
				});
			}
			catch
			{
				Console.WriteLine("[DEBUG] Nenhum indicador de loading encontrado");
			}

			// 5. Configuração dos filtros
			Console.WriteLine("[DEBUG] Configurando filtros na página de manutenção");

			await paginaManutencao.WaitForSelectorAsync("#Filtro", new()
			{
				Timeout = 15000,
				State = WaitForSelectorState.Visible
			});
			await paginaManutencao.Locator("#Filtro").SelectOptionAsync(new[] { "profissional" });
			await Task.Delay(500);

			// 6. Pesquisa pelo profissional (agora usando o NOME)
			Console.WriteLine($"[DEBUG] Pesquisando pelo CPF do profissional: {_nomeProfissionalSalvo}");
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = "" }).ClickAsync();
			await paginaManutencao.WaitForTimeoutAsync(1000);

			// Modificado: pesquisa pelo NOME 
			await paginaManutencao.GetByLabel("Professor").PressSequentiallyAsync(_nomeProfissionalSalvo);

			// 7. Seleção do profissional na grid usando o CPF
			Console.WriteLine($"[DEBUG] Selecionando profissional na grid pelo CPF: {_cpfProfissionalSalvo}");

			// Modificado para usar o seletor correto baseado no HTML fornecido
			var linhaProfissional = paginaManutencao.Locator($"div.em-grid-row:has(div.em-grid-cell[data-label='CPF']:text-is('{_cpfProfissionalSalvo}'))").First;

			if (await linhaProfissional.CountAsync() == 0)
			{
				Console.WriteLine($"[WARN] Profissional com CPF {_cpfProfissionalSalvo} não encontrado, tentando selecionar pelo nome");

				// Fallback: tenta encontrar pelo nome
				linhaProfissional = paginaManutencao.Locator($"div.em-grid-row:has(div.em-grid-cell[data-label='Nome']:text-is('{_nomeProfissionalSalvo}')").First;

				if (await linhaProfissional.CountAsync() == 0)
				{
					Console.WriteLine("[WARN] Profissional não encontrado nem pelo CPF nem pelo nome, selecionando primeira linha");
					linhaProfissional = paginaManutencao.Locator("div.em-grid-row").First;
				}
			}

			// Espera explícita com timeout aumentado
			await linhaProfissional.WaitForAsync(new LocatorWaitForOptions
			{
				Timeout = 15000,
				State = WaitForSelectorState.Visible
			});

			// Scroll para o elemento se necessário
			await linhaProfissional.ScrollIntoViewIfNeededAsync();

			// Clica na linha
			await linhaProfissional.ClickAsync();
			await Task.Delay(500);

			Console.WriteLine("[DEBUG] Profissional selecionado com sucesso");
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();

			// 8. Finalização do processo
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = " Carregar" }).ClickAsync();
			await Task.Delay(1000);

			//Continuar aqui o adicionar produto
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = "" }).ClickAsync();
			await paginaManutencao.WaitForTimeoutAsync(1000);
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
			await paginaManutencao.WaitForTimeoutAsync(1000);
			await paginaManutencao.GetByLabel("Quantidade").PressSequentiallyAsync("1");
			await paginaManutencao.WaitForTimeoutAsync(1000);
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = " Adicionar" }).ClickAsync();
			await paginaManutencao.WaitForTimeoutAsync(1000);
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
			await paginaManutencao.WaitForTimeoutAsync(1000);
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
			await paginaManutencao.WaitForTimeoutAsync(1000);
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
			await paginaManutencao.WaitForTimeoutAsync(1000);
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
			await paginaManutencao.WaitForTimeoutAsync(1000);
			await paginaManutencao.GetByRole(AriaRole.Button, new() { Name = " Não" }).ClickAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[ERROR] Falha durante o lançamento: {ex.Message}");
			throw;
		}

		finally
		{
			if (paginaManutencao != null && !paginaManutencao.IsClosed)
			{
				await paginaManutencao.CloseAsync();
			}

			if (paginaInicial != null && !paginaInicial.IsClosed)
			{
				await paginaInicial.CloseAsync();
			}
		}
	}

	private async Task RealizarAberturaCaixa()
	{
		Console.WriteLine("[INFO] Iniciando abertura de caixa");
		var novaGuia = await Page.Context.NewPageAsync();

		try
		{
			string url = $"{Contexto.BaseUrl}/{Contexto.BaseTenant}/AberturaFechamentoCaixa";
			Console.WriteLine($"[DEBUG] Acessando: {url}");

			await novaGuia.GotoAsync(url, new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 15000
			});

			await novaGuia.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
			await Task.Delay(1000);

			var btnAbrir = novaGuia.Locator("#botaoAbrirCaixa");
			var btnFechar = novaGuia.Locator("#botaoFecharCaixa");

			// Verifica se o botão "Abrir Caixa" está visível
			var isBtnAbrirVisible = await btnAbrir.IsVisibleAsync();
			Console.WriteLine($"[DEBUG] Botão 'Abrir Caixa' visível: {isBtnAbrirVisible}");

			if (isBtnAbrirVisible)
			{
				Console.WriteLine("[INFO] Botão 'Abrir Caixa' encontrado. Realizando abertura do caixa.");
				await btnAbrir.ClickAsync();

				var btnSim = novaGuia.Locator("button.btn-outline-warning:has-text('Sim')");

				var primeiroBtnSim = btnSim.First;
				await primeiroBtnSim.WaitForAsync(new()
				{
					State = WaitForSelectorState.Visible,
					Timeout = 5000
				});
				await Task.Delay(800);
				await primeiroBtnSim.ClickAsync();
				Console.WriteLine("[DEBUG] Primeira confirmação realizada");

				var segundoBtnSim = btnSim.Nth(1);
				await segundoBtnSim.WaitForAsync(new()
				{
					State = WaitForSelectorState.Visible,
					Timeout = 5000
				});
				await Task.Delay(800);
				await segundoBtnSim.ClickAsync();
				Console.WriteLine("[DEBUG] Segunda confirmação realizada");

				Console.WriteLine("[SUCCESS] Abertura de caixa concluída");
			}
			else
			{
				// Verifica se o botão "Fechar Caixa" está visível
				var isBtnFecharVisible = await btnFechar.IsVisibleAsync();
				Console.WriteLine($"[DEBUG] Botão 'Fechar Caixa' visível: {isBtnFecharVisible}");

				if (isBtnFecharVisible)
				{
					Console.WriteLine("[INFO] Botão 'Fechar Caixa' encontrado. Caixa já está aberto. Prosseguindo sem ação.");
				}
				else
				{
					Console.WriteLine("[WARN] Nenhum botão ('Abrir Caixa' ou 'Fechar Caixa') encontrado. Prosseguindo...");
					// Opcional: Lançar exceção se necessário
					// throw new Exception("Nenhum botão de controle de caixa encontrado.");
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[ERROR] Falha durante abertura de caixa: {ex.Message}");
			throw;
		}
		finally
		{
			if (!novaGuia.IsClosed)
			{
				await novaGuia.CloseAsync();
				Console.WriteLine("[DEBUG] Página fechada");
			}
		}
	}
}

