using Bogus;
using Bogus.Extensions.Brazil;
using QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
using FluentAssertions.Primitives;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Configuration.Internal;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class ProfessorPO : POBase
{
	string nomeUnicoProfessor;
	string nomeUnicoAnexo;
	public ProfessorPO(IPage page) : base(page)
	{
		GridProfessores = new GridComponent(Page, "#GridAterrissagem");
	}

	public override string PageUrl => "CadastroProfessor";

	public GridComponent GridProfessores { get; }

	public async Task ClicoEmCadastrarProfessor() => await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Cadastrar" }).ClickAsync();

	public async Task InformoONomeDoProfessor(string nomeProfessor)
	{
		string nomeBase = nomeProfessor;
		string dataHora = DateTime.Now.ToString("dd|HH:mm");
		string nomeCompleto = $"{nomeBase} {dataHora}";
		nomeUnicoProfessor = nomeCompleto;

		await Page.MainFrame.GetByLabel("Nome *").PressSequentiallyAsync(nomeCompleto);
	}

	public async Task selecionaSexoDoProfessor(string sexo)
	{
		//Esperar e Verificar se esta elemento esta visivel
		var sexoSelect = await Page.MainFrame.WaitForSelectorAsync("#Sexo", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(sexoSelect);

		// Normalizar entrada (remover espaços e deixar tudo em minúsculas)
		var sexoNormalizado = sexo?.Trim().ToLowerInvariant();

		if (sexoNormalizado is "masculino" or "m")
		{
			await sexoSelect.SelectOptionAsync("0");
		}
		else if (sexoNormalizado is "feminino" or "f")
		{
			await sexoSelect.SelectOptionAsync("1");
		}
		else
		{
			throw new Exception($"Sexo informado '{sexo}' não está na lista de seleção");
		}

	}

	public async Task SelecionoANaturalidadeDoProfessor(string naturalidadeProfessor)
	{
		await Page.MainFrame.Locator("#Naturalidade_text").ClickAsync();
		await Page.MainFrame.Locator("#Naturalidade_text").PressSequentiallyAsync(naturalidadeProfessor);
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}

	public async Task SelecionoADataDeNascimentoDoProfessor(string dataNascimentoProfessor)
	{
		await Page.MainFrame.GetByLabel("Nascimento *").ClickAsync();
		await Page.MainFrame.GetByLabel("Nascimento *").FillAsync(dataNascimentoProfessor);
	}

	public async Task InformoAFormacaoDoProfessor(string formacaoProfessor)
	{
		// Aguarda o select estar visível
		var select = await Page.MainFrame.WaitForSelectorAsync("#GrauDeFormacao", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(select);

		// Captura todas as opções (filhos <option>)
		var options = await select.QuerySelectorAllAsync("option");

		// Normaliza a entrada do usuário
		var formacaoNormalizado = formacaoProfessor?.Trim().ToLowerInvariant();

		// Mapeia os valores do select: Dictionary<texto visível, valor>
		var mapaDeOpcoes = new Dictionary<string, string>();

		foreach (var option in options)
		{
			string label = (await option.InnerTextAsync()).Trim().ToLowerInvariant();
			string value = await option.GetAttributeAsync("value");
			mapaDeOpcoes[label] = value;
		}

		// Verifica se o texto informado existe entre os labels das opções
		if (mapaDeOpcoes.TryGetValue(formacaoNormalizado, out var valorParaSelecionar))
		{
			await select.SelectOptionAsync(valorParaSelecionar);
		}
		else
		{
			throw new Exception($"O grau de formação '{formacaoNormalizado}' não está entre as opções disponíveis.");
		}

	}


	public async Task InformoCPFDoProfessor(string cpfProfessor)
	{
		cpfProfessor = TestConfig.Resolve(cpfProfessor); 

		//Pegar o CPF da feature
		string novoCPF = cpfProfessor.Trim().ToLowerInvariant();

		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Documentos" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Identificação" }).ClickAsync();

		//Aguardar e verificar se o campo esta visivel
		var inputCPF = await Page.MainFrame.WaitForSelectorAsync("#CPF", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(inputCPF);

		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CPF" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "CPF" }).FillAsync(novoCPF);
	}

	public async Task InformoORGDoProfessor()
	{
		Random random = new();
		int numeroAleatorio = random.Next(100000000, 999999999);
		string novoRg = string.Format("{0:D9}", numeroAleatorio);

		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Documentos" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = "Identificação" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "RG" }).ClickAsync();
		await Page.MainFrame.GetByRole(AriaRole.Textbox, new() { Name = "RG" }).FillAsync(novoRg);
	}

	public async Task InformoOEstadoCivilDoProfessor(string estadoCivilProfessor)
	{
		//Acessar o menu
		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " Relacionamentos" }).ClickAsync();


		//Esperar e validar se o elemento esta visivel
		var selectEstadoCivil = await Page.MainFrame.WaitForSelectorAsync("#EstadoCivil", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(selectEstadoCivil);

		// Captura todas as opções (filhos <option>)
		var options = await selectEstadoCivil.QuerySelectorAllAsync("option");

		// Normaliza a entrada do usuário
		var estadoCivilNormalizado = estadoCivilProfessor?.Trim().ToLowerInvariant();

		// Encontra a opção que contém o texto informado
		foreach (var option in options)
		{
			string label = (await option.InnerTextAsync()).Trim().ToLowerInvariant();
			if (label.Contains(estadoCivilNormalizado))
			{
				string value = await option.GetAttributeAsync("value");
				await selectEstadoCivil.SelectOptionAsync(value);
				return;
			}
		}

		// Caso nenhuma opção seja compatível
		throw new Exception($"O estado civil '{estadoCivilNormalizado}' não está entre as opções disponíveis.");

	}

	public async Task InformoOEnderecoDoProfessor(string cepProfessor)
	{
		await Page.MainFrame.GetByRole(AriaRole.Tab, new() { Name = " End. e Contatos" }).ClickAsync();
		await Page.MainFrame.GetByLabel("CEP *").ClickAsync();
		await Page.MainFrame.GetByLabel("CEP *").FillAsync(cepProfessor);
		await Page.MainFrame.GetByLabel("CEP *").PressAsync("Tab");
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Selecionar" }).ClickAsync();
	}

	public async Task ClicoEm(string botaoClicadoProfessor)
	{
		string botaoNormalizado = RemoverAcentos(botaoClicadoProfessor).ToLowerInvariant().Trim();
		
		// Se for o botão de editar, precisa estar na primeira linha da grid
		if (botaoNormalizado == "editar")
		{
			// Aguarda a grid estar visível e carregada
			var gridBody = Page.MainFrame.Locator(".em-grid-body");
			await gridBody.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

			// Aguarda que pelo menos uma linha apareça na grid
			var primeiraLinha = Page.MainFrame.Locator(".em-grid-row:not(.em-grid-header)").First;
			await primeiraLinha.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

			// Localiza o botão de editar na primeira linha usando o atributo data-role
			var botaoEditar = primeiraLinha.Locator("button[data-role='em-grid-editar']");
			
			// Aguarda o botão estar visível e habilitado antes de clicar
			await botaoEditar.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
			await botaoEditar.ClickAsync();
			return;
		}

		// Para outros botões, usa o método padrão
		var botaoClicavel = await ObterBotaoPorNomeAsync(botaoClicadoProfessor);
		if (botaoClicavel == null)
		{
			throw new Exception($"Botão '{botaoClicadoProfessor}' não encontrado ou não visível na página.");
		}
		await botaoClicavel.ClickAsync();
	}

	public async Task AlteroANacionalidadeDoProfessor()
	{
		await Page.MainFrame.GetByLabel("Nacionalidade *").ClickAsync();
		await Page.MainFrame.GetByLabel("Nacionalidade *").FillAsync("BRASILEIRA TESTE");
	}

	public async Task ConfirmoAExclusaoDoProfessor()
	{
		await Page.MainFrame.GetByRole(AriaRole.Button, new() { Name = " Sim" }).ClickAsync();
	}

	public async Task preencheratributoadicionalobrigatorios()
	{
		await ClicoEm("atributosadctab");

		var tabPaneAtbAdc = Page.MainFrame.Locator("#FormAtributosAdicionaisRespostasAtributosAdicionais > div > div > div");

		if (await tabPaneAtbAdc.IsVisibleAsync())
		{
			//debug
			//string texto = await tabPaneAtbAdc.InnerTextAsync();
			//Console.WriteLine(texto);

			var botaoNovoAtributoAdc = await Page.MainFrame.WaitForSelectorAsync("button.btn.btn-success.py-2.my-0.px-4", new() { State = WaitForSelectorState.Visible });
			Assert.NotNull(botaoNovoAtributoAdc);
		}
		else if(!await tabPaneAtbAdc.IsVisibleAsync())
		{
			await InserirAtributoAdicional();
		}
		else
		{
			throw new Exception("Erro ao buscar o campo de atributos adicionais");
		}
	}


	public async Task addAnexoProfessor(string tipoDocProfessor)
	{
		string tipoDocProfessorNormalizado = RemoverAcentos(tipoDocProfessor.Trim().ToLowerInvariant());

		// Diretório raiz do projeto (onde está o .csproj)
		string projectDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		// Subir pastas até sair de bin/Debug
		string solutionDir = Path.GetFullPath(Path.Combine(projectDir, @"..\..\.."));

		string arquivo = tipoDocProfessorNormalizado.Contains("imagem")
			? Path.Combine(solutionDir, @"Features\EnsinoRegular\Academico\Professor\Anexos\image-teste-1.jpg")
			: tipoDocProfessorNormalizado.Contains("pdf")
				? Path.Combine(solutionDir, @"Features\EnsinoRegular\Academico\Professor\Anexos\doc-teste.pdf")
				: throw new ArgumentException($"Tipo de documento não mapeado: {tipoDocProfessor}");


		if (!File.Exists(arquivo))
		{
			throw new FileNotFoundException($"Arquivo não encontrado: {arquivo}");
		}
			

		var inputFile = await Page.QuerySelectorAsync("#objeto_ImporteArquivo");
		await inputFile.SetInputFilesAsync(arquivo);
		await Page.ClickAsync("#AnexosCadastroProfessorAdicionarEditarAdd");



	}

	public async Task informoTituloAnexo(string tituloAnexo)
	{
		var campoTexto = await Page.MainFrame.WaitForSelectorAsync("#AnexosCadastroProfessorCampoTitulo", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(campoTexto);

		string nomeBase = tituloAnexo;
		string dataHora = DateTime.Now.ToString("dd|HH:mm");
		string tituloAnexoUnico = $"{nomeBase} {dataHora}";
		nomeUnicoAnexo = tituloAnexoUnico;

		await campoTexto.ClickAsync();

		var campo = Page.MainFrame.Locator("#AnexosCadastroProfessorCampoTitulo");
		await campo.PressSequentiallyAsync(tituloAnexoUnico, new() { Delay = 100 });
		await Page.WaitForTimeoutAsync(2000);

	}

	public async Task alterarNomeProfessor(string nomeAlterado)
	{
		await apagarCampoTexto(nomeUnicoProfessor);
		await InformoONomeDoProfessor(nomeAlterado);
		await Page.WaitForTimeoutAsync(2000);

	}

	//########################################################## PESQUISAS

	public async Task pesquisaProfessor(string nomeProfessor)
	{
		var campoPesquisa = await Page.MainFrame.WaitForSelectorAsync("input[placeholder='Pesquisar']", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(campoPesquisa);

		await Page.MainFrame.GetByPlaceholder("Pesquisar").PressSequentiallyAsync(nomeProfessor);
		await Page.WaitForTimeoutAsync(1500);

		//Ordenar grid maior para o menor
		await ClicoEm("ordenarcodigo");
		await ClicoEm("ordenarcodigo");
		await Page.WaitForTimeoutAsync(2000);

		await Page.MainFrame.WaitForSelectorAsync("#grid-body .em-grid-row", new() { State = WaitForSelectorState.Visible });

		var primeiroItemGrid = await Page.MainFrame.WaitForSelectorAsync("#grid-body .em-grid-row:nth-of-type(1) [data-label='Nome']", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(primeiroItemGrid);

		string textoContidoNaGrid = "" + await primeiroItemGrid.InnerTextAsync();

		Assert.True(validarString(nomeProfessor, textoContidoNaGrid));

	}


	public async Task pesquisaProfessorCadastrando(string nomeProfessor)
	{
		string nomeCompletoUnico = await cadastroInternoProfessor(nomeProfessor);

		await pesquisaProfessor(nomeCompletoUnico);

	}

	//*********** METODO DEPENDENTE DA TELA DE HORARIOS ESCOLARES ****************
	public async Task pesquisaProfessorCadastrandoEVinculando(string nomeProfessor)
	{
		throw new NotImplementedException();
	}


	//######################################################################## VALIDADORES
	public async Task validarStringNaGrid(string TextoInformado)
	{

		await AguardeExibirProfessores();

		var linhas = await GridProfessores.ObtenhaLinhasGrid();

		int linhaIndex = 1;

		foreach (var linha in linhas)
		{
			var conteudo = await linha.InnerTextAsync();

			if (!conteudo.Contains(TextoInformado, StringComparison.OrdinalIgnoreCase))
			{
				Assert.Fail($"A linha {linhaIndex} da grid **não contém** o texto '{TextoInformado}'. Conteúdo da linha: '{conteudo}'");
			}

			linhaIndex++;
		}


	}

	public async Task validaItensNaGridAnexos()
	{
		await AguardeExibirAnexosProfessor();

		//Verifica se tem alguma linha com o titulo do anexo passado
		var anexoAdicionado = await Page.MainFrame.WaitForSelectorAsync($"div[data-label='Titulo']:text('{nomeUnicoAnexo}')", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(anexoAdicionado);

		Console.WriteLine($"O texto do titulo é {nomeUnicoAnexo} e esta conforme foi informado no cadastro");

		await Page.WaitForTimeoutAsync(2000);
	}


	public async Task validamensagem(string mensagemProfessor)
	{
		var modalApresentado = await ObterModalMensagem(mensagemProfessor);
		Assert.NotNull(modalApresentado);

		var mensagemNoModal = await modalApresentado.InnerTextAsync();
		string mensagemProfessorNormalizada = mensagemProfessor.Trim().ToLowerInvariant();
		string mensagemNoModalNormalizada = mensagemNoModal.Trim().ToLowerInvariant();
		await Page.WaitForTimeoutAsync(2000);

		Assert.True(mensagemNoModalNormalizada.Contains(mensagemProfessorNormalizada), $"Esperava encontrar '{mensagemProfessorNormalizada}' em '{mensagemNoModalNormalizada}'");

	}

	public async Task validarBotaoComCampoObrigatorio(string nomeBotao)
	{
		if (nomeBotao is "trabalho" or "Trabalho")
		{
			var botaoDestacado = await ObterBotaoPorNomeAsync("trabalhotabinvalido");
			Assert.NotNull(botaoDestacado);
			await botaoDestacado.ClickAsync();
		}
	}

	//REVISAR METODO, IDEIA DE PEGAR O CAMPO E PROCURAR QUEM ESTA COM A CLASS INVALIDO SERIA PARA IDENTIFICAR SE O CAMPO ESTA DESTACADO
	public async Task validaCampoObrigatorio(string nomeCampo)
	{
		string nomeCampoNormalizado = RemoverAcentos(nomeCampo.ToLowerInvariant().Trim());

		if (nomeCampoNormalizado == "cep")
		{
			var campoCEP = ObterCampoPorNomeAsync("cepinvalido");
			Assert.NotNull(campoCEP);

		}
	}

	public async Task validarEApresentadaTelaAterrisagem()
	{
		var telaAterrisagem = await Page.MainFrame.WaitForSelectorAsync("#formulario_aterrissagem", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(telaAterrisagem);

		Console.WriteLine("Tela de aterrizagem esta visivel");
	}


	//####################################### METODOS INTERNOS DE VALIDACAO E BUSCA ###########################################################


	internal async Task AguardeExibirProfessores()
	{
		var grid = await Page.MainFrame.WaitForSelectorAsync("#GridAterrissagem", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(grid);
	}

	internal async Task AguardeExibirAnexosProfessor()
	{
		var grid = await Page.MainFrame.WaitForSelectorAsync("#GridAnexosCadastroProfessorItensGrid", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(grid);
	}

	internal async Task apagarCampoTexto(string textoAtual)
	{
		//Verifica se tem alguma linha com o titulo do anexo passado
		var campoInput = await Page.MainFrame.WaitForSelectorAsync($"input[value='{textoAtual}']", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(campoInput);

		await campoInput.FillAsync("");
		await Page.WaitForTimeoutAsync(1500);
	}

	internal Boolean validarString(string passada, string capturada)
	{
		string passadaNormalizada = RemoverAcentos(passada.ToUpperInvariant().Trim());
		string capturadaNormalizada = RemoverAcentos(capturada.ToUpperInvariant().Trim());

		Assert.AreEqual(passadaNormalizada, capturadaNormalizada);
		return true;
	} 

    internal async Task<String> cadastroInternoProfessor(string nomeProfessor)
	{
		string cpfGerado = GeradorDeCPF.GerarCpf(true); //true com mascara (000.000.000-00) e deixar em branco para sem mascara (00000000000)

		var botaoaCadastrar = await Page.MainFrame.WaitForSelectorAsync("button.botao-cadastrar", new() { State = WaitForSelectorState.Visible });
		Assert.NotNull(botaoaCadastrar);

		await ClicoEm("Cadastrar");
		await InformoONomeDoProfessor(nomeProfessor);
		await selecionaSexoDoProfessor("M");
		await SelecionoADataDeNascimentoDoProfessor("07/07/1990");
		await SelecionoANaturalidadeDoProfessor("BR");
		await InformoAFormacaoDoProfessor("Ensino Superior (Completo)");
		await InformoCPFDoProfessor(cpfGerado);
		await InformoOEstadoCivilDoProfessor("casado");
		await InformoOEnderecoDoProfessor("74330600");
		await preencheratributoadicionalobrigatorios();
		await ClicoEm("gravar");
		await validamensagem("Cadastro realizado com sucesso!");
		await ClicoEm("Voltar para Lista");
		await Page.WaitForTimeoutAsync(1500);
		
		return nomeUnicoProfessor;
	}


	internal async Task<IElementHandle?> ObterBotaoPorNomeAsync(string nomeBotao)
	{
		string botaoNormalizado = RemoverAcentos(nomeBotao).ToLowerInvariant().Trim();
		;
		string seletor = botaoNormalizado switch
		{
			"cadastrar" => "button.botao-cadastrar",
			"editar" => "button[data-role='em-grid-editar']",
			"gravar" => "button.botao-confirmar",
			"excluir" => "button[data-role='em-grid-remover']",
			"sim" => "button.btn-outline-warning:has-text('Sim')",
			"nao" => "button.btn-warning:has-text('Não')",
			"voltar" => "button.btn-primary",
			"cancelar" => "button.botao-cancelar",
			"anexo" or "ver anexos" => "button[data-bs-toggle='tooltip-info']",
			"ordenarcodigo" or "codigo" => "(//button[contains(@class, 'btn') and contains(@class, 'btn-link')])[1]",
			"ordenarnome" => "(//button[contains(@class, 'btn') and contains(@class, 'btn-link')])[2]",
			"ordenarcpf" => "(//button[contains(@class, 'btn') and contains(@class, 'btn-link')])[3]",
			"ordenarativo" => "(//button[contains(@class, 'btn') and contains(@class, 'btn-link')])[4]",
			"voltartolista" or "voltar para lista" => "button.btn-outline-success:has-text('Voltar para a lista')",
			"novocadastro" => "button.btn-success",
			"atributosadctab" => "#Atributos_adicionais-tab",
			"cadastrarnovoatributoadc" => "button.btn.btn-success.py-2.my-0.px-4",
			"trabalhotabinvalido" => "button.nav-link.invalido",
			"enderecotabinvalido" or "enderecoinvalido" => "button#End__e_Contatos-tab.nav-link.invalido",
			"adicionar" => "#AnexosCadastroProfessorAdicionarEditarAdd",
			_ => throw new ArgumentException($"Botão '{nomeBotao}' não mapeado.")
		};

		return await Page.MainFrame.WaitForSelectorAsync(seletor, new() { State = WaitForSelectorState.Visible });
	}


	internal async Task<IElementHandle> ObterCampoPorNomeAsync(string nomeCampo)
	{

		string campoNormalizado = RemoverAcentos(nomeCampo).ToLowerInvariant().Trim();
		;
		string seletor = campoNormalizado switch
		{
			"cepinvalido" => "div.form-group.invalido:has(input#EnderecoCep)",
			"logradouroinvalido" => "div.form-group.invalido:has(input#EnderecoLogradouro)",
			"bairroinvalido" => "div.form-group.invalido:has(input#EnderecoBairroCodigo)",
			"cidadeinvalido" => "div.form-group.invalido:has(input#Endereco_Cidade_text)",
			_ => throw new ArgumentException($"Botão '{nomeCampo}' não mapeado.")
		};

		return await Page.MainFrame.WaitForSelectorAsync(seletor, new() { State = WaitForSelectorState.Visible });
	}

	internal async Task<IElementHandle> ObterModalMensagem(string mensagem)
	{
		string mensagemNomalizada = RemoverAcentos(mensagem).ToLowerInvariant().Trim();

		string seletor = mensagemNomalizada switch
		{
			"cadastro realizado com sucesso!" => "div.modal-body:has-text('Cadastro realizado com sucesso!')",/*voltar ou cadastrar*/
			"professor: ja existe professor cadastrado com mesmo cpf." => "div.modal-body:has-text('Professor: Já existe professor cadastrado com mesmo CPF.')",/*sim ou não*/
			"tem certeza que deseja excluir este item?" => "div.modal-body:has-text('Tem certeza que deseja excluir este item?')",/*sim ou não*/
			"programaçao de avaliaçoes: O professor possui vinculo com a programação de avaliaçoes." => "div.modal-body:has-text('Programação de avaliações: O professor possui vínculo com a programação de avaliações.')",/*VOLTAR*/
			"associaçao com RFID: O professor possui associaçao com um número de RFID." => "div.modal-body:has-text('Associação com RFID: O professor possui associação com um número de RFID.')",/*voltar*/
			"item excluido com sucesso." => "div.modal-body:has-text('Item excluído com sucesso.')",/*voltar*/
			_ => throw new ArgumentException($"Mensagem '{mensagem}' não mapeada.")
		};

		return await Page.MainFrame.WaitForSelectorAsync(seletor, new() { State = WaitForSelectorState.Visible });
	}

	private string RemoverAcentos(string texto)
	{
		if (string.IsNullOrWhiteSpace(texto))
		{
			return string.Empty;
		}

		var normalized = texto.Normalize(NormalizationForm.FormD);
		var sb = new StringBuilder();

		foreach (var c in normalized)
		{
			var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			{
				sb.Append(c);
			}
		}

		return sb.ToString().Normalize(NormalizationForm.FormC);
	}


	internal async Task InserirAtributoAdicional()
	{
		await Page.MainFrame.WaitForTimeoutAsync(500); // Aguarda a seção ser expandida

		var atributosDivLocator = Page.MainFrame.Locator("#Atributos_adicionanis");
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
