using Microsoft.Playwright;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class TipoProfissionalPO : POBase
{
	public TipoProfissionalPO(IPage page) : base(page)
	{

	}




	public override string PageUrl => "CadastroTiposProfissionais";

	public async Task PreencherDescricaoDoTipoProfissional(string tipoDeProfissional)
	{

		await Page.WaitForTimeoutAsync(2000);
		await Page.MainFrame.GetByLabel("Descrição").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição").FillAsync(tipoDeProfissional);

	}

	//AND

	public async Task ClicoEmAnd(string opcao)
	{
		if (opcao.Equals("Cadastrar", StringComparison.OrdinalIgnoreCase))
		{
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync(); //Clica no botão de cadastrar
		}
		else if (opcao.Equals("Editar", StringComparison.OrdinalIgnoreCase))
		{
			await Page.WaitForTimeoutAsync(2000); // Espera 2 segundos
			await Page.MainFrame.Locator("css=.em-grid-row:nth-child(1) .btn-outline-success > .fal").ClickAsync(); // Clica no botão de editar

			// Obtém o valor atual do campo "Descrição"
			var descricaoAtual = await Page.GetByLabel("Descrição").InputValueAsync();

			// Adiciona "ALTERADO" ao final do texto existente
			var novaDescricao = descricaoAtual + " ALTERADO";

			// Preenche o campo com o novo valor
			await Page.GetByLabel("Descrição").FillAsync(novaDescricao);
		}
		else if (opcao.Equals("Excluir", StringComparison.OrdinalIgnoreCase))
		{

			await Page.MainFrame.Locator(".em-grid-row:nth-child(1) .btn-outline-danger > .fal").ClickAsync();

		}
		else
		{
			throw new NotImplementedException($"A opção '{opcao}' não é suportada.");
		}
	}

	//WHEN

	public async Task ClicoEmWhen(string opcao)
	{
		if (opcao.Equals("Cadastrar", StringComparison.OrdinalIgnoreCase))
		{
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
		}
		else if (opcao.Equals("Editar", StringComparison.OrdinalIgnoreCase))
		{
			await Page.WaitForTimeoutAsync(2000); // Espera 2 segundos
			await Page.GetByRole(AriaRole.Button, new() { Name = " Editar" }).ClickAsync();
		}
		else if (opcao.Equals("Sim", StringComparison.OrdinalIgnoreCase))
		{
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
		}
		else
		{
			throw new NotImplementedException($"A opção '{opcao}' não é suportada.");
		}
	}

	public async Task EApresentadaAMensagem(string mensagemEsperada)
	{
		// Aguarda um tempo para garantir que o modal seja exibido
		await Page.WaitForTimeoutAsync(2000);


		// Localiza o modal que contém a mensagem esperada
		_ = Page.MainFrame.Locator(".modal-body", new() { HasText = mensagemEsperada });

		// Verifica se o modal está visível
		//var modalVisivel = await modal.IsVisibleAsync();

		// Exibe a mensagem esperada no console
		Console.WriteLine($"---- Mensagem esperada: {mensagemEsperada}");

		// Lógica condicional para cada mensagem esperada
		if (mensagemEsperada == "Registro cadastrado com sucesso!")
		{
			// Clica no botão "Voltar" dentro do modal
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		}
		else if (mensagemEsperada == "Registro editado com sucesso!")
		{
			// Clica no botão "Voltar" dentro do modal
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		}
		else if (mensagemEsperada == "Item excluído com sucesso")
		{
			// Clica no botão "Voltar" dentro do modal
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		}
		else if (mensagemEsperada == "Tipo de profissional: Exclusão não permitida! Existe profissional cadastrado com esse tipo.")
		{
			// Pausa por 2 segundos
			await Page.WaitForTimeoutAsync(2000);

			// Clica no botão "Voltar" dentro do modal
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();

		}
		else if (mensagemEsperada == "Lançamentos: Não é possível excluir profissional, pois o mesmo possui registros em caixa.")
		{
			await Page.WaitForTimeoutAsync(2000);
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		}
		else
		{
			throw new Exception($"---- Mensagem inesperada: {mensagemEsperada}");
		}
	}

	public async Task PesquisarTipoDeProfissionalCadastrando(string tipoDeProfissional)
	{
		try
		{
			// Aguarda o campo de descrição estar visível
			var campoDescricao = Page.MainFrame.GetByLabel("Descrição");
			await campoDescricao.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

			// Cadastra o tipo de profissional
			await campoDescricao.ClickAsync();
			await campoDescricao.FillAsync(tipoDeProfissional);
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();

			// Valida o modal de sucesso do cadastro
			var modalCadastro = Page.MainFrame.Locator(".modal-body", new() { HasText = "Registro cadastrado com sucesso!" });
			await modalCadastro.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
			if (!await modalCadastro.IsVisibleAsync())
			{
				throw new Exception("---- Modal de sucesso de cadastro não foi exibido.");
			}

			// Clica no botão "Voltar" e aguarda o modal fechar
			await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
			await modalCadastro.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 5000 });

			// Aguarda o campo de pesquisa estar visível
			var campoPesquisa = Page.MainFrame.GetByPlaceholder("Pesquisar");
			await campoPesquisa.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

			// Realiza a pesquisa
			await campoPesquisa.ClickAsync();
			await campoPesquisa.PressSequentiallyAsync(tipoDeProfissional);

			// Aguarda o contêiner da grid estar visível
			await Page.MainFrame.Locator("#GridAterrissagem .em-grid-container").WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

			// Clica duas vezes no botão de ordenação "Código" para garantir o mais recente primeiro
			var botaoOrdenacao = Page.MainFrame.Locator("#GridAterrissagem .em-grid-header button.btn.btn-link", new() { HasText = "Código" });
			if (await botaoOrdenacao.CountAsync() > 1)
			{
				throw new Exception("---- Múltiplos botões de ordenação 'Código' encontrados. Refinar o seletor.");
			}
			await botaoOrdenacao.ClickAsync();
			await botaoOrdenacao.ClickAsync();

			// Verifica se pelo menos uma linha de dados está presente
			var linhasDados = Page.MainFrame.Locator("#GridAterrissagem .em-grid-row:not(:has-text('CódigoDescrição'))");
			if (await linhasDados.CountAsync() == 0)
			{
				throw new Exception("---- Nenhuma linha de dados encontrada na grid após ordenação.");
			}

			// Localiza a linha da grid que contém o tipo de profissional
			var linhaGrid = Page.MainFrame.Locator("#GridAterrissagem .em-grid-row", new()
			{
				Has = Page.MainFrame.Locator("[data-label='Descrição']", new() { HasText = tipoDeProfissional })
			});
			var rowCount = await linhaGrid.CountAsync();
			if (rowCount == 0)
			{
				throw new Exception($"---- Item '{tipoDeProfissional}' não encontrado na grid após pesquisa.");
			}
			if (rowCount > 1)
			{
				Console.WriteLine($"---- Aviso: Múltiplas linhas encontradas para '{tipoDeProfissional}'. Selecionando a primeira (mais recente).");
			}

			// Valida o texto na linha
			var celulaDescricao = linhaGrid.First.Locator("[data-label='Descrição']");
			string textoItem = await celulaDescricao.InnerTextAsync();
			if (!textoItem.Equals(tipoDeProfissional, StringComparison.OrdinalIgnoreCase))
			{
				throw new Exception($"---- Item na grid não corresponde ao esperado. Esperado: '{tipoDeProfissional}', Encontrado: '{textoItem}'");
			}

			Console.WriteLine($"---- Tipo de profissional '{tipoDeProfissional}' cadastrado e pesquisado com sucesso.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"---- Erro em PesquisarTipoDeProfissionalCadastrando: {ex.Message}");
			throw;

		}
	}

	public async Task PesquisarTipoDeProfissional(string tipoDeProfissional)
	{

		await Page.WaitForTimeoutAsync(2000);

		//Cadastrar
		await Page.MainFrame.GetByLabel("Descrição").ClickAsync();
		await Page.MainFrame.GetByLabel("Descrição").FillAsync(tipoDeProfissional);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Voltar" }).ClickAsync();
		//Pesquisar
		await Page.WaitForTimeoutAsync(2000);
		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
		await Page.MainFrame.GetByPlaceholder("Pesquisar").PressSequentiallyAsync(tipoDeProfissional);
	}

	public async Task PesquisarTipoDeProfissionalApenas(string tipoDeProfissional)
	{

		//Pesquisar
		await Page.WaitForTimeoutAsync(2000);
		await Page.MainFrame.GetByPlaceholder("Pesquisar").ClickAsync();
		await Page.MainFrame.GetByPlaceholder("Pesquisar").PressSequentiallyAsync(tipoDeProfissional);
	}


	public async Task<bool> ContemNaGrid(string tipoDeProfissional)
	{
		try
		{
			// Aguarda até que as linhas da grid estejam visíveis
			await Page.WaitForSelectorAsync("div.em-grid-row", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

			// Seleciona o container da grid
			var gridContainer = await Page.QuerySelectorAsync("div.em-grid-container");

			if (gridContainer == null)
			{
				Console.WriteLine("Grid não encontrada.");
				return false;
			}

			// Obtém todas as linhas da grid
			var linhasGrid = await gridContainer.QuerySelectorAllAsync("div.em-grid-row");

			// Itera sobre as linhas da grid
			foreach (var linha in linhasGrid)
			{
				// Obtém o conteúdo da célula de descrição
				var descricaoCell = await linha.QuerySelectorAsync("div.em-grid-cell[data-label='Descrição']");
				if (descricaoCell != null)
				{
					var descricao = await descricaoCell.InnerTextAsync();

					// Verifica se o tipoDeProfissional está contido na descrição
					if (descricao.Contains(tipoDeProfissional, StringComparison.OrdinalIgnoreCase))
					{
						Console.WriteLine($"Tipo de profissional '{tipoDeProfissional}' encontrado na grid: {descricao}");
						return true;
					}
				}
			}

			Console.WriteLine($"Tipo de profissional '{tipoDeProfissional}' não encontrado na grid.");
			return false;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro ao verificar a grid: {ex.Message}");
			return false;

		}
	}

	public async Task CadastroCompletoTipoDeProfissional(string tipoDeProfissional)
	{
		try
		{
			// Pesquisa o tipo de profissional cadastrando

			await PesquisarTipoDeProfissional(tipoDeProfissional);

			// Verifica se o item está na grid
			bool encontrado = await ContemNaGrid(tipoDeProfissional);

			if (!encontrado)
			{
				throw new Exception("O tipo de profissional não foi encontrado na grid após o cadastro.");
			}


			Console.WriteLine($"Cadastro completo do tipo de profissional '{tipoDeProfissional}' realizado com sucesso.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro durante o cadastro completo: {ex.Message}");

			throw; 
		}


	}

}



