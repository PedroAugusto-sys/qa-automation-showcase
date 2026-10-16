using QaShowcase.Specflow.IntegrationTests.PageObjects.Components;
using Microsoft.Playwright;
using NuGet.Frameworks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QaShowcase.Specflow.IntegrationTests.PageObjects;
internal class CadastroLancamentoDiarioFrequenciaPO : POBase
{

	//Variavieis Locais:
	private string _codigoSerie { get; set; }
	private string _nomeSerie { get; set; }
	private string _codigoTurma { get; set; }
	private string _nomeTurma { get; set; }
	private string _mes { get; set; }
	private string _dia { get; set; }


	public CadastroLancamentoDiarioFrequenciaPO(IPage page) : base(page)
	{
	}
	public override string PageUrl => "CadastroLancamentoDiarioFrequencia";


	#region BLOCO SELECIONO
	public async Task SelecioneAnoOuMes(string id, string valor)
	{
		try
		{
			Assert.IsTrue(await Page.MainFrame.Locator($"#{id}").IsVisibleAsync(), "O campo esperado não está visível na tela.");
			string valorNormalizado = valor.ToLower().Trim();

			if (id == "Mes")
			{
				List<string> meses = new()
				{
					"janeiro", "fevereiro", "março", "abril", "maio", "junho",
					"julho", "agosto", "setembro", "outubro", "novembro", "dezembro"
				};

				if (meses.Contains(valorNormalizado))
				{
					Console.WriteLine($"O mês '{valor}' é válido.");
					_mes = valor;
				}
				else
				{
					Console.WriteLine($"O mês '{valor}' não é válido.");
				}
			}
			else if (id == "Ano")
			{
				bool anoValido = int.TryParse(valor, out int ano) && ano >= 200 && ano <= 3100;

				if (anoValido)
				{
					Console.WriteLine($"O ano '{valor}' é válido.");
				}
				else
				{
					Console.WriteLine($"O ano '{valor}' não é válido.");
				}
			}

			await Page.MainFrame.SelectOptionAsync($"#{id}", new SelectOptionValue { Label = valor });
			await Page.WaitForTimeoutAsync(1200);
		}
		catch (Exception ex)
		{
			Assert.Fail($"Ocorreu um erro ao selecionar o ano ou mês ou ao informar ano ou mês: {ex.Message}");
		}
	}

	public async Task SelecioneSerieTurma(string serieTurma)
	{
		try
		{
			string idSeletorSerie = "#TurmaMontada_Serie";
			string idSeletorTurma = "#TurmaMontada_Turma";

			var locatorSerie = Page.MainFrame.Locator(idSeletorSerie);
			var locatorTurma = Page.MainFrame.Locator(idSeletorTurma);

			Assert.IsTrue(await locatorSerie.IsVisibleAsync(), "O campo série não está visível.");
			Assert.IsTrue(await locatorTurma.IsVisibleAsync(), "O campo turma não está visível.");

			NormalizeFraseSerieTurma(serieTurma);

			Console.WriteLine($"Selecionando Série: Código = '{_codigoSerie}', Nome = '{_nomeSerie}'");
			Console.WriteLine($"Selecionando Turma: Código = '{_codigoTurma}', Nome = '{_nomeTurma}'");

			bool serieSelecionada = await SelecionarOpcaoAsync(Page.MainFrame, idSeletorSerie, _codigoSerie, _nomeSerie);
			Assert.IsTrue(serieSelecionada, "Série não encontrada na seleção de séries");

			bool turmaSelecionada = await SelecionarOpcaoAsync(Page.MainFrame, idSeletorTurma, _codigoTurma, _nomeTurma);
			Assert.IsTrue(turmaSelecionada, "Turma não encontrada na seleção de turmas");

			Console.WriteLine("Série e Turma selecionadas com sucesso.");
			await Page.MainFrame.WaitForTimeoutAsync(1000);
		}
		catch (Exception ex)
		{
			Assert.Fail($"Erro ao selecionar Série e Turma: {ex.Message}");
		}
	}

	public async Task SelecioneDisciplina(string codigodisciplinaOuNomeDaDisciplina)
	{
		try
		{
			await Page.MainFrame.WaitForTimeoutAsync(2000);
			await Page.MainFrame.WaitForTimeoutAsync(1000);
			await Page.MainFrame.WaitForTimeoutAsync(1000);

			var locatorInputPesquisarDisciplina = Page.MainFrame.Locator("#Disciplina_text");
			Assert.IsTrue(await locatorInputPesquisarDisciplina.IsVisibleAsync(), "Input disciplina não está na tela.");

			var locatorBotaoPesquisarDisciplina = Page.MainFrame.Locator("#DisciplinaPesquisar");
			Assert.IsTrue(await locatorBotaoPesquisarDisciplina.IsVisibleAsync(), "Botão Pesquisar não está na tela");

			// Espera e clica corretamente no botão pesquisar
			await locatorBotaoPesquisarDisciplina.ClickAsync();

			// Aguarda a grid estar disponível
			await Page.WaitForSelectorAsync("#grid-body");

			// Seleciona todas as linhas da grid
			var linhas = await Page.QuerySelectorAllAsync("#grid-body .em-grid-row");

			if (ContemSomenteNumeros(codigodisciplinaOuNomeDaDisciplina.Trim()))
			{
				foreach (var linha in linhas)
				{
					var codigoCell = await linha.QuerySelectorAsync("[data-label='Código']");
					if (codigoCell == null)
					{
						Assert.Fail("Linha do código da disciplina igual a nulo");
					}

					var codigoTexto = await codigoCell.InnerTextAsync();
					if (codigoTexto.Trim() == codigodisciplinaOuNomeDaDisciplina)
					{
						Console.WriteLine($"Disciplina com código {codigodisciplinaOuNomeDaDisciplina} encontrada. Selecionando...");
						await linha.ClickAsync();
						break;
					}
				}

				// Aguarda botão aparecer e clique
				await Page.MainFrame.WaitForSelectorAsync("#DisciplinaSelecionar", new()
				{
					State = WaitForSelectorState.Visible,
					Timeout = 5000
				});
				await CliqueNoBotao("#DisciplinaSelecionar");
			}
			else if (ContemSomenteLetrasNumerosEspeciaisEspacos(codigodisciplinaOuNomeDaDisciplina.Trim()))
			{
				foreach (var linha in linhas)
				{
					var disciplinaCell = await linha.QuerySelectorAsync("[data-label='Disciplina']");
					if (disciplinaCell == null)
					{
						Assert.Fail("Linha da disciplina está nula");
					}

					var textoDisciplina = await disciplinaCell.InnerTextAsync();

					if (textoDisciplina.Trim().Equals(codigodisciplinaOuNomeDaDisciplina, StringComparison.OrdinalIgnoreCase))
					{
						Console.WriteLine($"Disciplina encontrada com descrição: {textoDisciplina}. Selecionando...");
						await linha.ClickAsync();
						break;
					}
				}

				// Aguarda botão aparecer e clique
				await Page.MainFrame.WaitForSelectorAsync("#DisciplinaSelecionar", new()
				{
					State = WaitForSelectorState.Visible,
					Timeout = 5000
				});
				await CliqueNoBotao("#DisciplinaSelecionar");
			}
			else
			{
				Assert.Fail("Ocorreu algo inesperado na hora de selecionar a disciplina.");
			}
		}
		catch (Exception ex)
		{
			Assert.Fail($"Erro ao tentar selecionar a disciplina com código {codigodisciplinaOuNomeDaDisciplina}: {ex.Message}");
		}
	}



	public async Task SelecioneDiaEspecifico(string dia)
	{
		try
		{
			Assert.IsTrue(await Page.MainFrame.Locator("#DiaEspecifico").IsVisibleAsync(), "O campo esperado não está visível na tela.");

			bool diaSelecionado = await SelecionarOpcaoAsync(Page.MainFrame, "#DiaEspecifico", "", dia);
			Assert.IsTrue(diaSelecionado, "Descrição de dia especifico não foi encontrada.");

			this._dia = dia;

			await Page.MainFrame.WaitForTimeoutAsync(1000);
		}
		catch (Exception ex)
		{
			Assert.Fail($"Ocorreu um erro ao tentar selecionar um dia:  {ex.Message}");
		}

	}
	#endregion

    #region BLOCO: LANÇAMENTO
	public async Task LancePresencaParaTodosOsAlunos()
	{
		try
		{
			var grid = await Page.MainFrame.QuerySelectorAsync("#grid-body");
			Assert.IsNotNull(grid, "Grid não encontrada na página.");

			bool isVisible = await grid.IsVisibleAsync();
			Assert.IsTrue(isVisible, "Grid não está visível.");

			var checkboxContainers = await Page.MainFrame.QuerySelectorAllAsync(".em-grid-cell-checkbox");

			foreach (var container in checkboxContainers)
			{
				// Ignora checkboxes com a classe 'em-grid-cell-disabled'
				bool isDisabled = await container.EvaluateAsync<bool>("el => el.classList.contains('em-grid-cell-disabled')");
				if (isDisabled)
				{
					continue;
				}

				// Verifica se está habilitado pelo Playwright (extra segurança)
				bool isEnabled = await container.IsEnabledAsync();
				if (!isEnabled)
				{
					continue;
				}

				// Verifica se o checkbox já está marcado, usando o <i> interno
				var icon = await container.QuerySelectorAsync("i");
				if (icon is null)
				{
					continue;
				}

				bool isChecked = await icon.EvaluateAsync<bool>("el => el.classList.contains('fa-check-square')");

				if (!isChecked)
				{
					await container.ClickAsync();
				}
			}
		}
		catch (Exception ex)
		{
			Assert.Fail($"Ocorreu um erro ao tentar lançar presença: {ex.Message}");
		}
	}

	public async Task LanceFaltaParaTodosOsAlunos()
	{
		try
		{
			var grid = await Page.MainFrame.QuerySelectorAsync("#grid-body");
			Assert.IsNotNull(grid, "Grid não encontrada na página.");

			bool isVisible = await grid.IsVisibleAsync();
			Assert.IsTrue(isVisible, "Grid não está visível.");

			var checkboxContainers = await Page.MainFrame.QuerySelectorAllAsync(".em-grid-cell-checkbox");

			foreach (var container in checkboxContainers)
			{
				// Ignora checkboxes com a classe 'em-grid-cell-disabled'
				bool isDisabled = await container.EvaluateAsync<bool>("el => el.classList.contains('em-grid-cell-disabled')");
				if (isDisabled)
				{
					continue;
				}

				// Verifica se está habilitado pelo Playwright (extra segurança)
				bool isEnabled = await container.IsEnabledAsync();
				if (!isEnabled)
				{
					continue;
				}

				// Verifica se o checkbox já está marcado, usando o <i> interno
				var icon = await container.QuerySelectorAsync("i");
				if (icon is null)
				{
					continue;
				}

				bool isChecked = await icon.EvaluateAsync<bool>("el => el.classList.contains('fa-check-square')");

				if (isChecked)
				{
					await container.ClickAsync(); // desmarca
				}
			}
		}
		catch (Exception ex)
		{
			Assert.Fail($"Ocorreu um erro ao tentar lançar presença: {ex.Message}");
		}
	}
	#endregion

	#region BLOCO: VALIDAR MENSAGEM
	public async Task ValideMensagem(string mensagemEsperada)
	{
		try
		{
			// Mensagens válidas
			var mensagensMap = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"Cadastro realizado com sucesso!",
				"Item excluído com sucesso.",
				"Lançamento excluído com sucesso",
				$"Deseja excluir todos os lançamentos do mês de {_mes.Trim()}?",
				$"Deseja excluir os lançamentos do dia {_dia.Trim()} do mês de {_mes.Trim()}?"
			};

			if (!mensagensMap.Contains(mensagemEsperada))
			{
				throw new Exception($"Mensagem '{mensagemEsperada}' não está mapeada.");
			}

			// 1️⃣ Localiza APENAS a modal que contém a mensagem esperada
			var modalLocator = Page.Locator($".modal-content:has(.modal-body:has-text('{mensagemEsperada}'))");

			await modalLocator.First.WaitForAsync(new LocatorWaitForOptions
			{
				State = WaitForSelectorState.Visible,
				Timeout = 5000
			});

			// 2️⃣ Captura o texto exibido
			var modalTexto = await modalLocator.Locator(".modal-body").InnerTextAsync();

			// 3️⃣ Compara mensagem
			if (!string.Equals(modalTexto.Trim(), mensagemEsperada.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				throw new Exception($"Mensagem exibida diferente da esperada.\nEsperado: '{mensagemEsperada}'\nObtido: '{modalTexto}'");
			}

			string msg1 = $"Deseja excluir todos os lançamentos do mês de {_mes.Trim()}?";
			string msg2 = $"Deseja excluir os lançamentos do dia {_dia.Trim()} do mês de {_mes.Trim()}?";

			if (mensagemEsperada.Equals(msg1, StringComparison.OrdinalIgnoreCase)
				|| mensagemEsperada.Equals(msg2, StringComparison.OrdinalIgnoreCase))
			{
				
			}
			else
			{
				await CliqueNoBotao("css=.btn-success");
			}	

		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ocorreu um erro ao validar a mensagem. Erro: {ex}");
			throw;
		}
	}
	#endregion

	#region BLOCO: CLIQUE NO BOTAO 
	public async Task CliqueNoBotao(string valor)
	{
		try
		{
			Assert.IsTrue(await Page.MainFrame.Locator($"{valor}").IsVisibleAsync(), "Botão não está visivel.");
			await Page.MainFrame.Locator($"{valor}").ClickAsync();
		}
		catch (Exception ex)
		{
			Assert.Fail($"Ocorreu um erro ao clicar no botão: {ex.Message}");
		}
	}

	#endregion

	#region BLOCO: CLICO EM

	public async Task ClicoEm(string botao)
	{
		switch (botao)
		{
			case "Carregar":
				await CliqueNoBotao("css=.botao-carregar-lancamento");
				break;
			case "Gravar":
				await CliqueNoBotao("css=.botao-confirmar");
				break;
			case "Excluir":
				await CliqueNoBotao("css=.botao-excluir");
				await Page.MainFrame.WaitForTimeoutAsync(1000);
				break;
			case "Sim":
				await CliqueNoBotao("button:has-text('Sim')");
				await Page.MainFrame.WaitForTimeoutAsync(1000);
				break;
		}
	}
	#endregion

	#region BLOCO: NORMALIZADORES DE STRING

	public void NormalizeFraseSerieTurma(string frase)
	{
		if (string.IsNullOrWhiteSpace(frase))
		{
			throw new ArgumentException("A frase não pode estar vazia ou nula.", nameof(frase));
		}

		var partes = frase.Split(" / ");
		if (partes.Length != 2)
		{
			throw new FormatException("Formato inválido. Esperado dois blocos separados por ' / '");
		}

		// ----- PARTE DA SÉRIE -----
		string codigoSerie = null;
		string nomeSerie = null;

		var parteSerie = partes[0].Trim();
		var seriePartes = parteSerie.Split(" - ", 2);

		if (seriePartes.Length == 2 && seriePartes[0].All(char.IsDigit))
		{
			// Formato com código
			codigoSerie = seriePartes[0].Trim();
			nomeSerie = seriePartes[1].Trim();
		}
		else
		{
			// Formato sem código
			nomeSerie = parteSerie;
		}

		// ----- PARTE DA TURMA -----
		string codigoTurma = null;
		string nomeTurma = null;

		var parteTurma = partes[1].Trim();
		var turmaPartes = parteTurma.Split(" - ", 2);

		if (turmaPartes.Length == 2 && turmaPartes[0].All(char.IsDigit))
		{
			// Formato com código
			codigoTurma = turmaPartes[0].Trim();
			nomeTurma = turmaPartes[1].Trim();
		}
		else
		{
			// Formato sem código
			nomeTurma = parteTurma;
		}

		// Atribuições finais
		_codigoSerie = codigoSerie;
		_nomeSerie = nomeSerie;
		_codigoTurma = codigoTurma;
		_nomeTurma = nomeTurma;
	}
	#endregion

	#region BLOCO: CADASTRO INTERNO DE LANÇAMENTO DE FREQUENCIA

	public async Task CadastreInternamenteLancamentoDeFrequenciaPorDia()
	{
		await SelecioneAnoOuMes("Ano", "2025");
		await SelecioneAnoOuMes("Mes", "agosto");
		await SelecioneSerieTurma("2º ANO [Ensino Fundamental] / A - ANGLO");
		await ClicoEm("Carregar");
		await SelecioneDiaEspecifico("Todos");
		await LancePresencaParaTodosOsAlunos();
		await ClicoEm("Gravar");
		await ValideMensagem("Cadastro realizado com sucesso!");

	}

	public async Task CadastreInternamenteLancamentoDeFrequenciaPorDisciplina()
	{
		await SelecioneAnoOuMes("Ano", "2024");
		await SelecioneAnoOuMes("Mes", "setembro");
		await SelecioneSerieTurma("10 - 7º ANO [Ensino Fundamental] / 7 - A - CJ");
		await SelecioneDisciplina("GEOGRAFIA");
		await ClicoEm("Carregar");
		await SelecioneDiaEspecifico("Todos");
		await LancePresencaParaTodosOsAlunos();
		await ClicoEm("Gravar");
		await ValideMensagem("Cadastro realizado com sucesso!");

	}
	#endregion

	#region BLOCO: METODOS AUXILIARES
	private async Task<bool> SelecionarOpcaoAsync(IFrame frame, string seletor, string valor = "", string label = "")
	{
		if (!string.IsNullOrEmpty(valor))
		{
			var resultado = await frame.SelectOptionAsync(seletor, new SelectOptionValue { Value = valor });
			if (resultado.Count > 0)
			{
				return true;
			}
		}

		if (!string.IsNullOrEmpty(label))
		{
			var resultado = await frame.SelectOptionAsync(seletor, new SelectOptionValue { Label = label });
			if (resultado.Count > 0)
			{
				return true;
			}
		}

		return false;
	}


	private static bool ContemSomenteLetrasNumerosEspeciaisEspacos(string texto)
	{
		return !string.IsNullOrWhiteSpace(texto) && texto.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsSymbol(c));
	}

	private static bool ContemSomenteNumeros(string texto)
	{
		return !string.IsNullOrWhiteSpace(texto) && texto.All(char.IsDigit);
	}
	#endregion


}
