using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using QaShowcase.Specflow.IntegrationTests.Features.EnsinoRegular.Logins.Cadastrar;
using FluentAssertions.Primitives;
using Microsoft.Playwright;
using NUnit.Framework;
using TechTalk.SpecFlow.CommonModels;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class OperadorPO : POBase
{

	public string nomeOp = null!;
	public string senhaOp = null!;

	public OperadorPO(IPage page) : base(page)
	{

	}
	public override string PageUrl => "CadastroOperador";


	public async Task ClicoEmCadastrar()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
	}

	public async Task PreencherCampoNome(string nomeOperador)
	{
		await Page.MainFrame.GetByLabel("Nome *").ClickAsync();
		await Page.MainFrame.GetByLabel("Nome *").FillAsync(nomeOperador);
		nomeOp = nomeOperador;
	}
	public async Task PreencherCampoLogin(string nomeOperadorLogin)
	{
		await Page.MainFrame.GetByLabel("Login *").ClickAsync();
		await Page.MainFrame.GetByLabel("Login *").FillAsync(nomeOperadorLogin);
	}

	public async Task PreencherCampoSenha(string senhaOperador)
	{
		senhaOperador = TestConfig.Resolve(senhaOperador);

		// Verifica se o campo de senha está desabilitado
		var senhaDesabilitada = await Page.MainFrame.Locator("css=#SenhaInicial").IsDisabledAsync();

		if (!senhaDesabilitada)
		{
			// Clica no campo "Senha" e preenche com a senha fornecida
			await Page.MainFrame.Locator("css=#SenhaInicial").ClickAsync();
			await Page.MainFrame.Locator("css=#SenhaInicial").FillAsync(senhaOperador);
			senhaOp = senhaOperador;
		}
		else
		{
			// Se o campo de senha estiver desabilitado, pula a etapa
			Console.WriteLine("------ Senha do campo bloqueado obtido com sucesso! " + senhaOp);
			senhaOp = await Page.MainFrame.Locator("css=#SenhaInicial").GetAttributeAsync("value");
		}



	}

	public async Task HabilitarCampo(string ativo)
	{
		await Page.MainFrame.Locator("#EstaAtivo").GetByText(ativo).ClickAsync();
	}

	public async Task SelecionarEmpresasDisponiveis()
	{
		// Aguarda o frame principal estar disponível
		var mainFrame = Page.MainFrame;

		// Localiza o container que contém todas as empresas
		var empresasContainer = await mainFrame.QuerySelectorAsync(".lista-empresas-checkbox");

		if (empresasContainer != null)
		{
			// Busca todos os checkboxes dentro do container
			var checkboxes = await empresasContainer.QuerySelectorAllAsync("input[type='checkbox'].form-check-input");

			// Itera sobre cada checkbox e marca se não estiver marcado
			foreach (var checkbox in checkboxes)
			{
				var isChecked = await checkbox.IsCheckedAsync();
				if (!isChecked)
				{
					await checkbox.ClickAsync(); // Usar "await" para garantir que a ação seja concluída
					// Aguarda após cada clique para garantir que a ação foi processada completamente
					await Page.WaitForTimeoutAsync(100);
				}
			}
		}
		else
		{
			Console.WriteLine("Não existem empresas associadas ao registro da empresa.");
			Debug.Assert(false);
		}
	}

	public async Task SelecionarEmpresaPadrao()
	{
		// Aguarda o frame principal estar disponível
		var mainFrame = Page.MainFrame;

		// Localiza o elemento <select> da combobox
		var selectElement = await mainFrame.QuerySelectorAsync("select#EmpresaPadrao");

		if (selectElement != null)
		{
			// Clica na combobox para abrir o dropdown
			await selectElement.ClickAsync();

			// Aguarda um breve momento para garantir que o dropdown seja renderizado
			await Page.WaitForTimeoutAsync(300);

			// Obtém todas as opções disponíveis no <select>
			var options = await selectElement.QuerySelectorAllAsync("option");

			// Verifica se há pelo menos uma opção além da opção padrão "Selecione"
			if (options.Count > 1) // options[0] é "Selecione"
			{
				// Seleciona a primeira opção disponível (ignorando a opção "Selecione")
				var primeiraOpcao = options[1];
				var valorPrimeiraOpcao = await primeiraOpcao.GetAttributeAsync("value");

				// Verifica se o valor da primeira opção é válido
				if (!string.IsNullOrEmpty(valorPrimeiraOpcao))
				{
					// Define o valor da primeira opção no <select>
					await selectElement.SelectOptionAsync(new[] { valorPrimeiraOpcao });
				}
				else
				{
					Console.WriteLine("O valor da primeira opção é inválido. Nenhuma ação foi realizada.");
				    Debug.Assert(false);
				}
			}
			else
			{
				Console.WriteLine("Nenhuma opção disponível para selecionar.");
				Debug.Assert(false);
			}
		}
		else
		{
			Console.WriteLine("Combobox de empresa padrão não encontrada.");
			Debug.Assert(false);
		}
	}

	public async Task SelecionarPermissoesNecessarias()
	{
		var mainFrame = Page.MainFrame;

		// Expande todas as opções (se necessário)
		var botaoExpandir = mainFrame.GetByRole(AriaRole.Button, new() { Name = " Expandir todos" });
		await botaoExpandir.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
		await botaoExpandir.ClickAsync();

		// Aguarda a árvore ser expandida completamente antes de continuar
		await Page.WaitForTimeoutAsync(300);

		// Localiza a árvore de permissões pelo ID "divTreeVIewContainer"
		var elemento = await mainFrame.QuerySelectorAsync("#divTreeVIewContainer");

		if (elemento != null)
		{
			// Lista de IDs das checkboxes que devem ser clicadas novamente
			var idsParaClicar = new List<string>
		{
			"1010045", "1010053", "1010050", "1010052", "1010049", "1010059",
			"1010010", "1010023", "1010038", "1010051", "1010039", "1010011"
		};

			// Busca todas as checkboxes dentro da hierarquia correta
			var checkboxesPermissoes = await elemento.QuerySelectorAllAsync(".custom-control.form-check.checkbox-tree-view input[type='checkbox'].form-check-input");

			foreach (var checkbox in checkboxesPermissoes)
			{
				// Verifica se a checkbox está desabilitada
				var isDisabled = await checkbox.EvaluateAsync<bool>("el => el.disabled");
				if (!isDisabled)
				{
					// Marca a checkbox se não estiver marcada
					var isChecked = await checkbox.IsCheckedAsync();
					if (!isChecked)
					{
						await checkbox.ClickAsync();
					}
				}
			}

			// Clica nas checkboxes específicas
			foreach (var id in idsParaClicar)
			{
				var checkbox = await elemento.QuerySelectorAsync($"input[type='checkbox'][id='{id}']");
				if (checkbox != null)
				{
					await checkbox.ClickAsync();
				}
			}
		}
	}

	// Escolha Sim , Não ou Entrar
	public async Task ClicarEm(string escolha)
	{
		await Page.WaitForTimeoutAsync(2000);

		if (escolha == "Sim")
		{
			await Page.WaitForTimeoutAsync(1000);
			await Page.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
			await Page.WaitForTimeoutAsync(2000);
		}
		else if (escolha == "Entrar")
		{
			await Page.WaitForTimeoutAsync(1000);

			// Usando Locator com um seletor específico
			await Page.Locator("xpath=(//button[@type='button'])[4]").ClickAsync();
			await Page.WaitForTimeoutAsync(2000);
		}
		else if (escolha == "Nao")
		{
			await Page.WaitForTimeoutAsync(1000);
			await Page.GetByRole(AriaRole.Button, new() { Name = " Não" }).ClickAsync();
			await Page.WaitForTimeoutAsync(2000);

		}
	}

	//Clica no campo de editar na grid
	public async Task ClicoEmEditar()
	{
		// Aguarda a grid estar visível e carregada
		var gridBody = Page.MainFrame.Locator(".em-grid-body");
		await gridBody.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

		// Aguarda que pelo menos uma linha apareça na grid
		var primeiraLinha = Page.MainFrame.Locator(".em-grid-row:not(.em-grid-header)").First;
		await primeiraLinha.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

		// Localiza o botão de editar na primeira linha usando o atributo data-role (mais confiável)
		var botaoEditar = primeiraLinha.Locator("button[data-role='em-grid-editar']");

		// Aguarda o botão estar visível e habilitado antes de clicar
		await botaoEditar.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
		await botaoEditar.ClickAsync();
	}

	//Altera o nome adicionando a string enviada pela feature
	public async Task InformoANovaDescricao(string novaDescricao)
	{
		var campoNome = Page.MainFrame.GetByLabel("Nome *");
		await campoNome.ClickAsync();

		var nomeAtual = await campoNome.InputValueAsync();
		var nomeAlterado = nomeAtual + " " + novaDescricao;
		await campoNome.FillAsync(nomeAlterado);

		await Page.WaitForTimeoutAsync(1000);
	}

	//Somente clica no campo de pesquisa
	public async Task ClicoNoCampoPesquisar()
	{
		await Page.WaitForTimeoutAsync(2000);
		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
	}


	//Digita o nome do Operador no campo de pesquisa - Referencia

	//Atualmente método não é utilizado
	public async Task DigitarNomeOperador(string NomeOperador)
	{
		nomeOp = NomeOperador;
		var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");

		// Usamos o PressSequentiallyAsync para digitar o nome completo
		await campoPesquisa.FillAsync(""); // Limpa o campo de pesquisa
		await campoPesquisa.PressSequentiallyAsync(NomeOperador);

		// Aguarda um tempo suficiente para garantir que a pesquisa foi processada e a grid foi atualizada
		await Page.WaitForTimeoutAsync(2000); // Espera de 2 segundos após digitar o nome completo (ajuste conforme necessário)
	}

	public async Task DigitarNomeOperadorCadastrado()
	{
		var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");

		// Usamos o PressSequentiallyAsync para digitar o nome completo
		await campoPesquisa.FillAsync(""); // Limpa o campo de pesquisa
		await campoPesquisa.PressSequentiallyAsync(nomeOp);

		// Aguarda um tempo suficiente para garantir que a pesquisa foi processada e a grid foi atualizada

		await Page.WaitForTimeoutAsync(2000); // Espera de 2 segundos após digitar o nome completo (ajuste conforme necessário)
	}

	//Cadastra o operador antes de pesquisar-Matheus
	//Nota-se que a pesquisa é realizada porém não existe validação sobre o que foi pesquisado
	//Se não ouver validação sobre o itens que esta sendo apresentado na grid, o proximo passo pode operar de forma inadequada
	public async Task PesquisarOperador(string nomeOperador)
	{
		//A condicao para ser null é o nomeOperador não ter sido informada na feature
		//Temos dois métodos diferentes no StepDefinitions que chama o mesmo método do PageObjets
		//Porem um passa a string com valor null e outro passa a string com o nome do operador
		//Permitindo assim que seja feita a validação para qual cenário esta sendo reproduzido
		if (nomeOperador == null)
		{
			// Gera um nome único com timestamp (ano, data, hora e minuto) para evitar conflitos
			var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
			var nomeUnico = $"nome_{timestamp}";

			await ClicoEmCadastrar();

			await PreencherCampoNome(nomeUnico);
			await PreencherCampoLogin(nomeUnico);
			await PreencherCampoSenha(TestConfig.DefaultOperatorPassword);
			await HabilitarCampo("Sim");
			await SelecionarEmpresasDisponiveis();
			await SelecionarEmpresaPadrao();
			await SelecionarPermissoesNecessarias();
			await GravarOperador();
			await EApresentadaAMensagem("Cadastro realizado com sucesso!");

			var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar"); //Armazena o campo de pesquisa
			await campoPesquisa.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
			await campoPesquisa.ClearAsync(); //Limpa o campo de pesquisa
			await Page.MainFrame.GetByPlaceholder("Pesquisar").PressSequentiallyAsync(nomeUnico);//Informa o nome único gerado com timestamp

			// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
			await Page.WaitForTimeoutAsync(3000);

			// Aguarda a grid estar visível e carregada
			var gridBody = Page.MainFrame.Locator(".em-grid-body");
			await gridBody.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

			// Aguarda um pouco mais para garantir que as linhas foram renderizadas
			await Page.WaitForTimeoutAsync(1000);
		}
		else
		{
			// Aguarda o campo de pesquisa estar visível antes de tentar usá-lo
			var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");
			await campoPesquisa.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
			await campoPesquisa.ClearAsync();
			await campoPesquisa.PressSequentiallyAsync(nomeOperador);

			// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
			await Page.WaitForTimeoutAsync(3000);

			// Aguarda a grid estar visível e carregada
			var gridBody = Page.MainFrame.Locator(".em-grid-body");
			await gridBody.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

			// Aguarda um pouco mais para garantir que as linhas foram renderizadas
			await Page.WaitForTimeoutAsync(1000);
		}
	}


	//Cadastra o operador antes de pesquisar-Vinicius
	//Nota-se que a pesquisa é realizada porém não existe validação sobre o que foi pesquisado
	//Se não ouver validação sobre o itens que esta sendo apresentado na grid, o proximo passo pode operar de forma inadequada
	public async Task PesquisarUsuario(string nomeOperador)
	{
		//A condicao para ser null é o nomeOperador não ter sido informada na feature
		//Temos dois métodos diferentes no StepDefinitions que chama o mesmo método do PageObjets
		//Porem um passa a string com valor null e outro passa a string com o nome do operador
		//Permitindo assim que seja feita a validação para qual cenário esta sendo reproduzido

		if (nomeOperador == null)
		{
			await this.ClicoEmCadastrar();
			await Page.WaitForTimeoutAsync(2000);
			await PreencherCampoNome("PesquisarUsuario");
			await PreencherCampoLogin("PesquisarUsuario");
			await PreencherCampoSenha(TestConfig.DefaultOperatorPassword);
			await HabilitarCampo("sim");
			await SelecionarEmpresasDisponiveis();
			await SelecionarEmpresaPadrao();
			await SelecionarPermissoesNecessarias();
			await Page.WaitForTimeoutAsync(14000);
			await GravarOperador();
			await EApresentadaAMensagem("Cadastro realizado com sucesso!");
			var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");
			await campoPesquisa.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
			await campoPesquisa.ClearAsync();
			await Page.MainFrame.GetByPlaceholder("Pesquisar").PressSequentiallyAsync("OPERADOR QA");

			// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
			await Page.WaitForTimeoutAsync(3000);

			// Aguarda a grid estar visível e carregada
			var gridBody = Page.MainFrame.Locator(".em-grid-body");
			await gridBody.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

			// Aguarda um pouco mais para garantir que as linhas foram renderizadas
			await Page.WaitForTimeoutAsync(1000);
		}
		else
		{
			// Aguarda o campo de pesquisa estar visível antes de tentar usá-lo
			var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");
			await campoPesquisa.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });

			// Limpa o campo completamente - usa múltiplas estratégias para garantir limpeza
			await campoPesquisa.ClickAsync();
			await Page.WaitForTimeoutAsync(200);
			await campoPesquisa.FillAsync(""); // Limpa usando FillAsync
			await campoPesquisa.ClearAsync(); // Limpa usando ClearAsync
			await Page.WaitForTimeoutAsync(300);

			// Preenche o campo com o nome do operador
			await campoPesquisa.FocusAsync();
			await Page.WaitForTimeoutAsync(200);
			await campoPesquisa.PressSequentiallyAsync(nomeOperador);

			// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
			await Page.WaitForTimeoutAsync(3000);

			// Aguarda a grid estar visível e carregada
			var gridBody = Page.MainFrame.Locator(".em-grid-body");
			await gridBody.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

			// Aguarda um pouco mais para garantir que as linhas foram renderizadas
			await Page.WaitForTimeoutAsync(1500);
		}
	}

	//Validar Operador apresentado no grid

	//Cadastra o operador antes de pesquisar - Janayna
	//Nota-se que a pesquisa é realizada porém não existe validação sobre o que foi pesquisado
	//Se não ouver validação sobre o itens que esta sendo apresentado na grid, o proximo passo pode operar de forma inadequada
	public async Task CadastroOperadorPesquisar(string nomeOperador)
	{
		//A condicao para ser null é o NOMEOperador não ter sido informada na feature
		//Temos dois métodos diferentes no StepDefinitions que chama o mesmo método do PageObjets
		//Porem um passa a string com valor null e outro passa a string com o nome do operador
		//Permitindo assim que seja feita a validação para qual cenário esta sendo reproduzido
		if (nomeOperador == null)
		{
			await ClicoEmCadastrar();
			await PreencherCampoNome("CadastroOperadorPesquisar");
			await PreencherCampoLogin("CadastroOperadorPesquisar");
			await PreencherCampoSenha(TestConfig.DefaultOperatorPassword);
			await HabilitarCampo("Sim");
			await SelecionarEmpresasDisponiveis();
			await SelecionarEmpresaPadrao();
			await SelecionarPermissoesNecessarias();
			await GravarOperador();
			await EApresentadaAMensagem("Cadastro realizado com sucesso!");

			var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar"); //Armazena o campo de pesquisa
			await campoPesquisa.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
			await campoPesquisa.ClearAsync(); //Limpa o campo de pesquisa
			await Page.MainFrame.GetByPlaceholder("Pesquisar").PressSequentiallyAsync("nome");//Informa o nome de forma sequencial

			// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
			await Page.WaitForTimeoutAsync(3000);

			// Aguarda a grid estar visível e carregada
			var gridBody = Page.MainFrame.Locator(".em-grid-body");
			await gridBody.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

			// Aguarda um pouco mais para garantir que as linhas foram renderizadas
			await Page.WaitForTimeoutAsync(1000);
		}
		else
		{
			// Aguarda o campo de pesquisa estar visível antes de tentar usá-lo
			var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");
			await campoPesquisa.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 30000 });
			await campoPesquisa.ClearAsync();
			await campoPesquisa.PressSequentiallyAsync("nome");

			// Aguarda mais tempo para garantir que a pesquisa foi processada e a grid foi atualizada
			await Page.WaitForTimeoutAsync(3000);

			// Aguarda a grid estar visível e carregada
			var gridBody = Page.MainFrame.Locator(".em-grid-body");
			await gridBody.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

			// Aguarda um pouco mais para garantir que as linhas foram renderizadas
			await Page.WaitForTimeoutAsync(1000);
		}

	}

	//Validar Operador apresentado na grid
	//Porém se obeserva que o método atual só funciona se for informado no nome completo
	//E método deve ser utilizado para validar o avanço ou não nas steps
	public async Task ValidarOperadorApresentado()
	{
		// Aguarda até que o nome do operador esteja presente na grid
		var nomeGridSelector = "div.em-grid-cell[data-label='Nome']"; // Seletor correto baseado no data-label
		await Page.WaitForSelectorAsync(nomeGridSelector, new PageWaitForSelectorOptions { Timeout = 5000 }); // Espera até 5 segundos

		// Pega o nome da grid
		var nomeNaGrid = await Page.MainFrame.QuerySelectorAsync(nomeGridSelector) ?? throw new Exception("O nome do usuário não foi encontrado na grid.");

		// Pega o texto do nome da grid
		var nomeTexto = await nomeNaGrid.TextContentAsync();

		// Remove espaços extras, se houver, e faz a comparação
		nomeTexto = nomeTexto?.Trim();

		if (string.IsNullOrEmpty(nomeTexto))
		{
			throw new Exception("O nome apresentado na grid está vazio ou não foi carregado corretamente.");
		}

		// Comparação de texto entre o nome digitado e o nome que apareceu na grid
		if (!nomeTexto.Contains(nomeOp, StringComparison.OrdinalIgnoreCase)) // Considera a comparação sem diferenciação de maiúsculas/minúsculas
		{
			throw new Exception($"O nome apresentado na grid '{nomeTexto}' não é o mesmo que o nome pesquisado '{nomeOp}'.");
		}
	}

	//Clicar em Gravar

	//Espera e clica em Gravar
	public async Task GravarOperador()
	{
		await Page.WaitForTimeoutAsync(2000);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
	}

	//Clica em gravar e espera
	public async Task Gravar()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Gravar" }).ClickAsync();
		await Page.WaitForTimeoutAsync(2000);
	}

	// Clicar em Excluir Operador (sobrecarga sem parâmetro - usa nomeOp armazenado)
	public async Task ClicarEmRemover()
	{
		if (string.IsNullOrEmpty(nomeOp))
		{
			throw new Exception("Nome do operador não foi definido. Use ClicarEmRemover(string nomeOperador) ou defina nomeOp antes de chamar este método.");
		}
		await ClicarEmRemover(nomeOp);
	}

	// Clicar em Excluir Operador
	// Localiza a linha da grid que contém o nome do operador e clica no botão de remover dessa linha específica
	public async Task ClicarEmRemover(string nomeOperador)
	{
		await Page.WaitForTimeoutAsync(2000);

		// Garante que o campo de pesquisa está preenchido com o nome do operador
		// Isso filtra a grid para mostrar apenas linhas relevantes
		var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");
		var valorAtual = await campoPesquisa.InputValueAsync();

		if (string.IsNullOrEmpty(valorAtual) || !valorAtual.Contains(nomeOperador, StringComparison.OrdinalIgnoreCase))
		{
			await campoPesquisa.ClearAsync();
			await campoPesquisa.PressSequentiallyAsync(nomeOperador);
			await Page.WaitForTimeoutAsync(1000); // Aguarda a grid filtrar
		}

		// Localiza a linha da grid filtrando pela célula específica de Nome
		// Usa o seletor que busca a célula com data-label='Nome' que contém o texto exato
		var linhasGrid = Page.MainFrame.Locator(".em-grid-row:not(.em-grid-header)");

		// Itera pelas linhas para encontrar a que tem o nome exato na célula de Nome
		var countLinhas = await linhasGrid.CountAsync();
		ILocator? linhaEncontrada = null;

		for (int i = 0; i < countLinhas; i++)
		{
			var linha = linhasGrid.Nth(i);
			var celulaNome = linha.Locator("div.em-grid-cell[data-label='Nome']");

			if (await celulaNome.CountAsync() > 0)
			{
				var textoNome = await celulaNome.InnerTextAsync();
				textoNome = textoNome?.Trim() ?? string.Empty;

				// Compara o texto exato (case-insensitive)
				if (textoNome.Equals(nomeOperador, StringComparison.OrdinalIgnoreCase))
				{
					linhaEncontrada = linha;
					break;
				}
			}
		}

		if (linhaEncontrada == null)
		{
			throw new Exception($"Não foi possível encontrar a linha do operador '{nomeOperador}' na grid.");
		}

		// Localiza o botão de remover dentro dessa linha específica
		var botaoRemover = linhaEncontrada.Locator("button[data-role='em-grid-remover']");

		// Aguarda o botão estar visível
		await botaoRemover.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

		// Clica no botão de remover da linha específica
		await botaoRemover.ClickAsync();
	}

	public async Task ValidarMensagemEAcaoCorrespondente()
	{
		// pega o texto da tela
		var mensagemElement = await Page.MainFrame.Locator("css=.modal-dialog-centered .modal-body:nth-child(2)").TextContentAsync();
		string mensagem = mensagemElement?.Trim() ?? string.Empty;

		switch (mensagem)
		{
			case "Tem certeza que deseja cancelar este cadastro?":
				await Page.MainFrame.Locator("css=.btn-outline-warning > .fa-check").ClickAsync(); // Clica em sim
				break;

			case "Cadastro realizado com sucesso!":
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).ClickAsync();
				break;

			case "Tem certeza que deseja excluir este item?":
				await Page.MainFrame.Locator("css=.btn-outline-warning > .fa-check").ClickAsync(); // Clica em sim
				break;

			case "Item excluído com sucesso.":
				await Page.MainFrame.GetByText("Item excluído com sucesso.").ClickAsync();
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
				await Page.WaitForTimeoutAsync(1000);
				break;

			default:
				throw new Exception($"Mensagem inesperada: {mensagem}");
		}
	}

	//Mensagem de confirmação de exclusão tratamento no objeto
	public async Task EApresentadaAMensagemVoltar()
	{
		try
		{
			await Page.MainFrame.GetByText("Item excluído com sucesso.").ClickAsync();
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
		//Futuramente revisar para melhor forma de finalizar a execeção
		catch (Exception ex)
		{
			Console.WriteLine(ex + " Detectado uma exclusão do usuário com auditoria vinculada. Ignorando a exclusão.");
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
			await Page.WaitForTimeoutAsync(1000);
		}
	}

	//Mensagem de confirmação de cadastro com validação de diferentes tipos de mensagens

	public async Task EApresentadaAMensagem(string mensagem)
	{
		// Aguarda o modal aparecer
		await Page.WaitForTimeoutAsync(1000);

		// Localiza o modal-body filtrando pela mensagem específica para evitar ambiguidade
		// Usa HasText para encontrar o modal que contém exatamente a mensagem esperada
		var modalBodyLocator = Page.MainFrame.Locator(".modal-body.d-flex.flex-column.text-center")
			.Filter(new() { HasText = mensagem })
			.First;

		await modalBodyLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

		// Captura o texto do modal (pode conter <br> ou quebras de linha)
		var textoModal = await modalBodyLocator.InnerTextAsync();
		var textoNormalizado = textoModal?.Trim().Replace("\r\n", "\n").Replace("\r", "\n") ?? string.Empty;


		// Normaliza a mensagem esperada também
		_ = mensagem.Trim();

		// Verifica se a mensagem de sucesso foi exibida
		if (textoNormalizado.Contains("Cadastro realizado com sucesso!", StringComparison.OrdinalIgnoreCase))
		{
			// Clica no botão "Voltar para a lista" para mensagem de sucesso
			// Localiza o botão dentro do mesmo modal-content que contém o modal-body encontrado
			var modalContent = modalBodyLocator.Locator("xpath=ancestor::div[contains(@class, 'modal-content')]");
			var botaoVoltar = modalContent.Locator("button").Filter(new() { HasText = "Voltar para a lista" }).First;

			// Se não encontrar no modal-content, tenta o seletor global
			if (await botaoVoltar.CountAsync() == 0)
			{
				botaoVoltar = Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).First;
			}

			await botaoVoltar.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
			await botaoVoltar.ClickAsync();
			return;
		}

		// Verifica mensagens de erro individuais ou combinadas
		bool temErroLogin = textoNormalizado.Contains("Login: Login já cadastrado.", StringComparison.OrdinalIgnoreCase);
		bool temErroSenha = textoNormalizado.Contains("Senha: Campo Senha é obrigatório.", StringComparison.OrdinalIgnoreCase);

		if (temErroLogin || temErroSenha)
		{
			// Para mensagens de erro, clica no botão "Voltar" (btn-warning)
			// Localiza o botão dentro do mesmo modal-content
			var modalContent = modalBodyLocator.Locator("xpath=ancestor::div[contains(@class, 'modal-content')]");
			var botaoVoltar = modalContent.Locator(".modal-footer button.btn.btn-warning").First;

			// Se não encontrar no modal-content, tenta o seletor global
			if (await botaoVoltar.CountAsync() == 0)
			{
				botaoVoltar = Page.MainFrame.Locator(".modal-footer button.btn.btn-warning").First;
			}

			await botaoVoltar.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
			await botaoVoltar.ClickAsync();
			return;
		}

		// Se nenhuma das mensagens conhecidas foi encontrada, tenta o comportamento padrão
		// (para manter compatibilidade com outros casos)
		try
		{
			// Localiza o botão dentro do mesmo modal-content
			var modalContent = modalBodyLocator.Locator("xpath=ancestor::div[contains(@class, 'modal-content')]");
			var botaoVoltar = modalContent.Locator("button").Filter(new() { HasText = "Voltar para a lista" }).First;

			if (await botaoVoltar.CountAsync() > 0)
			{
				await botaoVoltar.ClickAsync();
			}
			else
			{
				await Page.MainFrame.GetByText(mensagem).ClickAsync();
				await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar para a lista" }).First.ClickAsync();
			}
		}
		catch
		{
			// Se falhar, tenta apenas clicar em "Voltar" no modal encontrado
			var modalContent = modalBodyLocator.Locator("xpath=ancestor::div[contains(@class, 'modal-content')]");
			var botaoVoltar = modalContent.Locator(".modal-footer button.btn").First;

			if (await botaoVoltar.CountAsync() == 0)
			{
				botaoVoltar = Page.MainFrame.Locator(".modal-footer button.btn").First;
			}

			await botaoVoltar.ClickAsync();
		}
    }

	public async Task clicarEmOpcao(string botao)
	{
		await Page.WaitForTimeoutAsync(2000);

		if (botao == "Cancelar")
		{
			await Page.MainFrame.Locator("css=.botao-cancelar").ClickAsync();
		}
		else if (botao == "Gravar")
		{
			await GravarOperador();
		}
	}

	public async Task selecionoFiltroSituacao(string filtroSituacao)
	{
		await Page.WaitForTimeoutAsync(2000);
		await Page.MainFrame.Locator("css=#DropDownAtivoFiltro").ClickAsync();
		await Page.WaitForTimeoutAsync(2000);
		if (filtroSituacao == "Ativas")
		{
			await Page.MainFrame.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new[] { "1" });
		}
		else if(filtroSituacao == "Inativas")
		{
			await Page.MainFrame.Locator("xpath=//div[@id='GridAterrissagem']/div[2]/div[2]/div[2]/select").SelectOptionAsync(new[] { "2" });
		}
		else
		{
			await Page.MainFrame.GetByPlaceholder("Pesquisar").ClearAsync();
			await Page.MainFrame.Locator("#DropDownAtivoFiltro").SelectOptionAsync(new[] { "3" });
		}
		await Page.WaitForTimeoutAsync(8000);
	}

    //Metodos utilizados no BaseStep
	//Visando cadastrar um operador sempre que precisar
 	public async Task CadastroOperadorFinal(string nomeOperador)
	{
		await ClicoEmCadastrar();
		await PreencherCampoNome(nomeOperador);
		await PreencherCampoLogin(nomeOperador);
		await PreencherCampoSenha(TestConfig.DefaultOperatorPassword);

		await HabilitarCampo("Sim");
		await SelecionarEmpresasDisponiveis();
		await SelecionarEmpresaPadrao();
		await SelecionarPermissoesNecessarias();
		await GravarOperador();
		await EApresentadaAMensagem("Cadastro realizado com sucesso!");
	}

    //Metodos utilizados no BaseStep
	//Visando retornar verdadeiro se o operador existir e falso se não existir
	//Verifica na coluna Login e Nome, pois o login é único e identifica o operador
	public async Task<bool> operadorExiste(string operador)
	{
		// Normaliza o operador para comparação (remove espaços extras e converte para minúsculas)
		var operadorNormalizado = operador?.Trim().ToLowerInvariant() ?? string.Empty;

		if (string.IsNullOrEmpty(operadorNormalizado))
		{
			return false;
		}

		// Aguarda o campo de pesquisa estar visível antes de tentar usá-lo
		var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");
		await campoPesquisa.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

		// Limpa o campo completamente
		await campoPesquisa.ClickAsync();
		await campoPesquisa.FillAsync("");
		await campoPesquisa.ClearAsync();
		await Page.WaitForTimeoutAsync(200); // Pequeno delay após limpar

		// Preenche o campo com o nome do operador
		await campoPesquisa.FocusAsync();
		await campoPesquisa.PressSequentiallyAsync(operador);

		// Aguarda a pesquisa ser processada (dá tempo para a aplicação processar a busca)
		await Page.WaitForTimeoutAsync(2000); // Delay para processamento da pesquisa

		// Aguarda a grid ser atualizada após a pesquisa - usa wait condicional em vez de timeout fixo
		var gridBody = Page.MainFrame.Locator(".em-grid-body");
		await gridBody.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

		// Aguarda que pelo menos uma linha apareça na grid ou que a pesquisa seja concluída
		// Usa wait com condição mas com timeout maior para garantir processamento
		try
		{
			await Page.WaitForFunctionAsync(
				@"() => {
					const rows = document.querySelectorAll('.em-grid-row:not(.em-grid-header)');
					return rows.length > 0 || document.querySelector('.em-grid-body') !== null;
				}",
				new PageWaitForFunctionOptions { Timeout = 8000 }
			);
		}
		catch
		{
			// Se o wait falhar, aguarda um tempo curto e continua (fallback)
			await Page.WaitForTimeoutAsync(1500);
		}

		// Aguarda um pouco mais para garantir que as linhas foram completamente renderizadas
		await Page.WaitForTimeoutAsync(1000);

		// Localiza todas as linhas da grid (excluindo o cabeçalho)
		var linhasGrid = Page.MainFrame.Locator(".em-grid-row:not(.em-grid-header)");

		var countLinhas = await linhasGrid.CountAsync();

		// Se não há linhas na grid, o operador não existe
		if (countLinhas == 0)
		{
			return false;
		}

		// Itera pelas linhas até encontrar o operador - para assim que encontrar
		// Verifica primeiro o Login (mais rápido e único) e depois o Nome como fallback
		for (int i = 0; i < Math.Min(countLinhas, 50); i++) // Limita a 50 linhas para performance
		{
			var linha = linhasGrid.Nth(i);

			// Verifica a célula de Login (login é único e identifica o operador)
			var celulaLogin = linha.Locator("div.em-grid-cell[data-label='Login']");
			if (await celulaLogin.CountAsync() > 0)
			{
				var loginTexto = await celulaLogin.InnerTextAsync();
				var loginNormalizado = loginTexto?.Trim().ToLowerInvariant() ?? string.Empty;

				// Compara o texto normalizado (case-insensitive e sem espaços extras)
				if (loginNormalizado == operadorNormalizado)
				{
					return true; // Encontrou, retorna imediatamente
				}
			}

			// Também verifica pelo Nome como fallback (caso o login não seja encontrado)
			var celulaNome = linha.Locator("div.em-grid-cell[data-label='Nome']");
			if (await celulaNome.CountAsync() > 0)
			{
				var nomeTexto = await celulaNome.InnerTextAsync();
				var nomeNormalizado = nomeTexto?.Trim().ToLowerInvariant() ?? string.Empty;

				// Compara o texto normalizado (case-insensitive e sem espaços extras)
				if (nomeNormalizado == operadorNormalizado)
				{
					return true; // Encontrou, retorna imediatamente
				}
			}
		}

		// Se não encontrou nenhuma correspondência exata no Login ou Nome
		return false;
	}

    //Metodos utilizados no BaseStep
	//Visando remover o operador
	public async Task ExcluirOperador(string operador)
	{
		if (await this.operadorExiste(operador))
		{
			await this.ClicarEmRemover(operador);
			await this.ClicarEm("Sim");
			await this.EApresentadaAMensagemVoltar();
			return;

		}
		return;
	}

}
