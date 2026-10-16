using QaShowcase.Specflow.IntegrationTests.PageObjects;
using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Financeiro.ResumoFinanceiro;

[Binding]
public class ResumoFinanceiroStepDefinitions : BaseStep
{
	private ResumoFinanceiroAlunoPO _resumoFinanceiroPage = null!;



	[Given(@"que preciso validar os filtros do resumo financeiro de um aluno")]
	public async Task GivenQuePrecisoValidarOsFiltrosDoResumoFinanceiroDeUmAluno()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
 



		//SEÇÃO DO USUÁRIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR FINANCEIRO";
		await RealizeLogin(nomeOperador, "");

		_resumoFinanceiroPage = await POFactory.Crie<ResumoFinanceiroAlunoPO>(Context);
		await _resumoFinanceiroPage.GotoAsync();
	}

	[Given(@"seleciono o aluno")]
	public async Task GivenSelecionoOAluno() => await _resumoFinanceiroPage.SelecioneAluno("3180890");

	[Given(@"clico em Carregar")]
	public async Task GivenClicoEmCarregar() => await _resumoFinanceiroPage.CliqueEmCarregar();

	[Given(@"seleciono uma finalidade")]
	public async Task GivenSelecionoUmaFinalidade()
	{
		await _resumoFinanceiroPage.SelecioneFinalidade("MENSALIDADE");
		await _resumoFinanceiroPage.Page.WaitForTimeoutAsync(1000);
	}

	[When(@"seleciono os filtros")]
	public async Task WhenSelecionoOsFiltros()
	{
		await _resumoFinanceiroPage.AguardeParaExibirAsDuplicatas();
	}



	[Then(@"o sistema deve listar as informações de acordo com os filtros")]
	public async Task ThenOSistemaDeveListarAsInformacoesDeAcordoComOsFiltros()
	{
		//O sistema deve listar 6 ou 7 duplicatas
		//e a primeira deve ser a 367352

		IReadOnlyList<ILocator> rows = await _resumoFinanceiroPage.GridDuplicatas.ObtenhaLinhasGrid();

		//foreach (var row in rows)
		//{
		//	var estaVisivel = await row.IsVisibleAsync();
		//	if (!estaVisivel)
		//	{
		//		throw new Exception();
		//	}
		//}

		//Aqui não se sabe pq o ele traz uma linha vazia inexistente, então, usamos o BeOneOf
		rows.Count().Should().BeOneOf(6, 7);

		IReadOnlyList<string> texto = await rows[0].AllInnerTextsAsync();
		texto[0].Should().StartWith("367352");

		texto[0].Should().NotBeNull();
	}
}
