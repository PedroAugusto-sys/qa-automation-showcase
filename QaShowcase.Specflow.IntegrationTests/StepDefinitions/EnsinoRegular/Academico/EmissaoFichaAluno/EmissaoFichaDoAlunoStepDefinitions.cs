using QaShowcase.Specflow.IntegrationTests.PageObjects;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.EmissaoFichaAluno;

[Binding]

public class EmissaoFichaDoAlunoStepDefinitions : BaseStep
{
	private EmissaoFichaAlunoPO _emissaofichaAlunoPage = null!;


	[Given(@"emitir a ficha do aluno"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenEmitirAFichaDoAluno()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();




		//SEÇÃO DO USUÁRIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR FICHA ALUNO";
		await RealizeLogin(nomeOperador, "");

		_emissaofichaAlunoPage = await POFactory.Crie<EmissaoFichaAlunoPO>(Context);
		await _emissaofichaAlunoPage.GotoAsync();
	}

	[Given(@"filtro por '([^']*)'"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenFiltroPor(string filtro)
	{
		await _emissaofichaAlunoPage.selecionoFiltro(filtro);
	}

	[Given(@"seleciono a empresa '([^']*)'"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenSelecionoAEmpresa(string empresa)
	{
		await _emissaofichaAlunoPage.selecionarEmpresa();
	}

	[Given(@"seleciono a série '([^']*)'"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenSelecionoASerie(string p0)
	{
		await _emissaofichaAlunoPage.selecionarSerie(p0);
	}

	[Given(@"seleciono a turma '([^']*)'"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenSelecionoATurma(string p0)
	{
		await _emissaofichaAlunoPage.selecionarTurma(p0);
	}

	[Given(@"seleciono o aluno (.*)"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenSelecionoOAluno(int matricula)
	{
		await _emissaofichaAlunoPage.selecionarAluno(matricula);
	}


	[Given(@"aplico um filtro"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenAplicoUmFiltro()
	{
		await _emissaofichaAlunoPage.selecionoFiltro("Aluno");
	}

	[Given(@"seleciono um item do filtro"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenSelecionoUmItemDoFiltro()
	{
		await _emissaofichaAlunoPage.selecionarAluno(240476);
	}

	[Given(@"seleciono uma visibilidade para empresa"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenSelecionoUmaVisibilidadeParaEmpresa()
	{
		await _emissaofichaAlunoPage.alterarVisibilidade("Ocultar");
	}

	[Given(@"clico em '([^']*)'"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task GivenClicoEm(string opcao)
	{
		await _emissaofichaAlunoPage.alterarVisibilidade(opcao);
	}

	[When(@"clico em '([^']*)'"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task WhenClicoEm(string botao)
	{
		await _emissaofichaAlunoPage.clicaEmitir();
	}


	[Then(@"é gerado um relatório"), Scope(Tag = "EmissãoFichaAluno")]
	public async Task ThenEGeradoUmRelatorio()
	{
		await _emissaofichaAlunoPage.fecharRelatorio();
	}

}
