using Microsoft.Playwright;
using NUnit.Framework;
using System;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class LoginPO : POBase
{
	public LoginPO(IPage page) : base(page)
	{

	}

	public override string PageUrl => "Login";

	public ILocator Usuarios => Page.GetByRole(AriaRole.Combobox, new() { Name = "Operador" });

	public ILocator UsuarioPreencher => Page.GetByRole(AriaRole.Textbox, new() { Name = "Operador" });

	public ILocator SenhaAcesso => Page.GetByLabel("Senha");
	public ILocator BotaoEntrar => Page.GetByRole(AriaRole.Button, new() { Name = " Entrar" });

	internal async Task LogueComAdmin()
	{
		var tokenUrl = Environment.GetEnvironmentVariable("QA_TOKEN_URL") 
			?? throw new InvalidOperationException("QA_TOKEN_URL environment variable is not set");
		var username = Environment.GetEnvironmentVariable("QA_USER") 
			?? throw new InvalidOperationException("QA_USER environment variable is not set");
		var password = Environment.GetEnvironmentVariable("QA_PASSWORD") 
			?? throw new InvalidOperationException("QA_PASSWORD environment variable is not set");

		// Navegar até o site de geração de token
		await Page.GotoAsync(tokenUrl, new PageGotoOptions
		{
			WaitUntil = WaitUntilState.NetworkIdle,
			Timeout = 30000
		});

		// Preencher o campo de Login
		var loginInput = Page.Locator("#Login");
		await loginInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
		await loginInput.FillAsync(username);

		// Preencher o campo de Senha
		var senhaInput = Page.Locator("#Senha");
		await senhaInput.FillAsync(password);

		// Clicar no botão "Gerar Token"
		var gerarTokenButton = Page.Locator("#btnGerarToken");
		await gerarTokenButton.ClickAsync();

		// Esperar o token ser exibido
		var tokenLocator = Page.Locator("#token");
		await tokenLocator.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

		// Capturar o texto do token
		var token = await tokenLocator.InnerTextAsync();

		// Copiar o token para a área de transferência
		await Page.EvaluateAsync($"navigator.clipboard.writeText('{token.Trim()}');");

		// Após pegar o token, navegamos para a página de login
		// Aguarda um pouco para garantir que o token foi copiado
		await Task.Delay(500);
		await GotoAsync();
		
		// Aguarda a página de login estar totalmente carregada
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
		
		// Verifica se realmente está na página de login (não foi redirecionado)
		// Se não estiver na página de login, tenta navegar novamente
		var currentUrl = Page.Url;
		if (!currentUrl.Contains("Login", StringComparison.OrdinalIgnoreCase))
		{
			// Se foi redirecionado, navega explicitamente para a página de login
			await GotoAsync();
			await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
			await Task.Delay(1000);
		}

		// Verifica se o combobox de "Operador" está visível
		// Aguarda o campo estar disponível antes de tentar usá-lo
		var usuariosVisible = await Usuarios.IsVisibleAsync();
		if (usuariosVisible)
		{
			// Caso o combobox esteja visível, seleciona a opção "1"
			await Page.WaitForTimeoutAsync(500);
			await Usuarios.SelectOptionAsync(new[] { "1" });
		}
		else
		{
			// Caso o combobox não esteja visível, aguarda o campo de texto estar disponível
			await UsuarioPreencher.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
			// Preenche o campo de texto com "ADMINISTRADOR"
			await UsuarioPreencher.FillAsync("ADMINISTRADOR");
		}

		// Aguarda o campo de senha estar disponível
		await SenhaAcesso.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
		// Preenche o campo de senha com o token obtido
		await SenhaAcesso.FillAsync(token);

		// Aguarda o botão estar disponível e clica
		await BotaoEntrar.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
		// Clica no botão "Entrar"
		await BotaoEntrar.ClickAsync();
		
		// Aguarda a navegação após o login (pode redirecionar para a tela inicial)
		await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
	}
	internal async Task LogueComUsuario(string nomeOperador, string senhaOp)
	{
		// Normaliza o nome do operador
		var nomeOperadorNormalizado = nomeOperador.ToUpper().Trim();

		// Verifica se o elemento é um combobox (<select>)
		var comboboxOperador = Page.MainFrame.Locator("select[name='Operador'], select#Usuarios");

		// Verifica se o elemento é um campo de texto (<input>)
		var campoTextoOperador = Page.MainFrame.Locator("input[name='Operador'], input#Login");

		// Verifica se o combobox está presente e visível (verificação rápida)
		var comboboxOperadorVisible = await comboboxOperador.IsVisibleAsync();

		if (comboboxOperadorVisible)
		{
			// Busca o value diretamente nas opções (mais rápido que usar Label)
			// Aguarda no máximo 1 segundo para as opções carregarem
			bool selecionado = false;
			string? valueEncontrado = null;
			
			for (int i = 0; i < 5 && valueEncontrado == null; i++) // 5 tentativas de 200ms
			{
				try
				{
					var options = await comboboxOperador.Locator("option").AllAsync();
					foreach (var option in options)
					{
						var optionText = await option.TextContentAsync();
						if (!string.IsNullOrEmpty(optionText))
						{
							var textNormalizado = optionText.Trim().ToUpper();
							if (textNormalizado == nomeOperadorNormalizado || 
							    textNormalizado.Contains(nomeOperadorNormalizado))
							{
								var value = await option.GetAttributeAsync("value");
								if (!string.IsNullOrEmpty(value))
								{
									valueEncontrado = value;
									break;
								}
							}
						}
					}
					
					if (valueEncontrado != null) break;
				}
				catch { }
				
				if (i < 4) await Task.Delay(200);
			}

			// Se encontrou o value, seleciona diretamente (mais rápido)
			if (valueEncontrado != null)
			{
				try
				{
					await comboboxOperador.SelectOptionAsync(new[] { valueEncontrado });
					selecionado = true;
				}
				catch { }
			}

			// Se não conseguiu por value, tenta por Label (fallback)
			if (!selecionado)
			{
				try
				{
					await comboboxOperador.SelectOptionAsync(new SelectOptionValue { Label = nomeOperadorNormalizado });
					selecionado = true;
				}
				catch
				{
					try
					{
						await comboboxOperador.SelectOptionAsync(new SelectOptionValue { Label = nomeOperador });
						selecionado = true;
					}
					catch (Exception ex)
					{
						throw new Exception($"Não foi possível selecionar o operador '{nomeOperador}'. Erro: {ex.Message}");
					}
				}
			}
		}
		else
		{
			// Verifica se o campo de texto está presente e visível
			var campoTextoOperadorVisible = await campoTextoOperador.IsVisibleAsync();

			if (campoTextoOperadorVisible)
			{
				// Preenche diretamente sem clicar (FillAsync já faz isso)
				await campoTextoOperador.FillAsync(nomeOperador);
			}
			else
			{
				throw new Exception("Nenhum campo de 'Operador' foi encontrado na página.");
			}
		}

		// Preenche a senha diretamente (FillAsync já aguarda e clica se necessário)
		await Page.MainFrame.GetByLabel("Senha").FillAsync(senhaOp);
	}

	internal async Task ClicoEntrar()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Entrar" }).ClickAsync();
	}

	internal async Task ConfirmarTela(string nomeOperador)
	{
		try
		{
			// Espera até que o iframe seja anexado ao DOM
			var iframeElement = await Page.WaitForSelectorAsync("iframe", new PageWaitForSelectorOptions
			{
				Timeout = 2000, // Tempo maior para garantir carregamento
				State = WaitForSelectorState.Attached
			});

			if (iframeElement == null)
			{
				Assert.Fail("O iframe não foi encontrado.");
			}

			// Obtém o frame correto dentro do iframe
			var frame = await iframeElement.ContentFrameAsync();
			if (frame == null)
			{
				Assert.Fail("Não foi possível acessar o conteúdo do iframe.");
			}

			// Espera até que a tela inicial esteja visível dentro do iframe
			var telaInicial = await frame.WaitForSelectorAsync("#div-tela-inicial", new FrameWaitForSelectorOptions
			{
				Timeout = 2000,
				State = WaitForSelectorState.Visible
			});

			if (telaInicial == null)
			{
				Assert.Fail("A tela inicial não foi encontrada dentro do iframe.");
			}

			// Verifica se o elemento da tela inicial está visível
			bool telaVisivel = await telaInicial.IsVisibleAsync();
			Assert.True(telaVisivel, "A tela inicial não está visível.");

			// Se a tela estiver visível, busca o texto de saudação dentro do iframe
			var texto = await frame.QuerySelectorAsync("span.tela-inicial-saudacao");
			if (texto == null)
			{
				Assert.Fail("O elemento de saudação não foi encontrado.");
			}

			var textoSd = await texto.TextContentAsync();

			// Verifica se o nome do operador está presente no texto
			Assert.True(textoSd.Contains(nomeOperador), "O nome do operador não foi encontrado na saudação.");
		}
		catch (TimeoutException)
		{
			Assert.Fail("Timeout excedido. O elemento não foi encontrado ou não ficou visível no tempo esperado.");
		}
	}

}
