using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using QaShowcase.Specflow.IntegrationTests.Features.EnsinoRegular.Academico.ManutencaoBoletimColetivo;
using QaShowcase.Specflow.IntegrationTests.PageObjects;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.ManutencaoBoletimColetivo;

[Binding]
internal class ManutencaoBoletimColetivoStepDefinitions : BaseStep
{
	private ManutencaoBoletimColetivoPO _manutencaoBoletimColetivoPage = null!;



	[Given(@"que acesso a tela de Manutencao de Boletim Coletivo"), Scope(Tag = "@ManutencaoBoletimColetivo")]
	public async Task GivenQueAcessoATelaDeManutencaoDeBoletimColetivo()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();




		//SEÇÃO DO USUÁRIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR BOLETIM COLETIVO";
		await RealizeLogin(nomeOperador, "");

		_manutencaoBoletimColetivoPage = await POFactory.Crie<ManutencaoBoletimColetivoPO>(Context);
		await _manutencaoBoletimColetivoPage.GotoAsync();
	}

	[Given(@"seleciono a Serie"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task GivenSelecionoASerie()
	{
		await _manutencaoBoletimColetivoPage.selecionoASerie();
	}

	[Given(@"seleciono a Turma"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task GivenSelecionoATurma()
	{
		await _manutencaoBoletimColetivoPage.selecionoATurma();
	}

	[Given(@"seleciono o Ano"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task GivenSelecionoOAno()
	{
		await _manutencaoBoletimColetivoPage.selecionoOAno();
	}

	[Given(@"seleciono a Disciplina"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task GivenSelecionoADisciplina()
	{
		await _manutencaoBoletimColetivoPage.selecionoADisciplina();
	}

	[Given(@"clico em Carregar"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task GivenClicoEmCarregar()
	{
		await _manutencaoBoletimColetivoPage.clicoEmCarregar();
	}

	[Given(@"realizo o lancamento de notas"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task GivenRealizoOLancamentoDeNotas()
	{
		await _manutencaoBoletimColetivoPage.realizoOLancamentoDeNotas();
	}

	[Given(@"clico em Gravar"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task GivenClicoEmGravar()
	{
		await _manutencaoBoletimColetivoPage.clicoEmGravar();
	}	

	[Given(@"realizo o lancamento de faltas"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task GivenRealizoOLancamentoDeFaltas()
	{
		await _manutencaoBoletimColetivoPage.realizoOlancamentoDeFaltas();
	}

	[Given(@"seleciono um Aluno"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task GivenSelecionoUmAluno()
	{
		await _manutencaoBoletimColetivoPage.selecionoUmAluno();
	}

	[When(@"clico em Recalcular"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task WhenClicoEmRecalcular()
	{
		await _manutencaoBoletimColetivoPage.clicoEmRecalcular();
	}

	[When(@"clico em Sim"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task WhenClicoEmSim()
	{
		await _manutencaoBoletimColetivoPage.clicoEmSim();
	}

	[When(@"clico em Emitir"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task WhenClicoEmEmitir()
	{
		await _manutencaoBoletimColetivoPage.clicoEmEmitir();
	}	

	[When(@"clico em Boletim individual"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task WhenClicoEmBoletimIndividual()
	{
		await _manutencaoBoletimColetivoPage.clicoEmBoletimIndividual();
	}

	[Then(@"e apresentado um alerta"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task ThenEApresentadoUmAlerta()
	{
		await _manutencaoBoletimColetivoPage.eApresentadoUmAlerta();
	}

	[Then(@"o boletim e gravado com sucesso"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task ThenOBoletimEGravadoComSucesso()
	{
		await _manutencaoBoletimColetivoPage.oBoletimEGravadoComSucesso();
	}

	[Then(@"o recalculo do boletim e realizado"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task ThenORecalculoDoBoletimERealizado()
	{
		await _manutencaoBoletimColetivoPage.oRecalculoDoBoletimERealizado();
	}

	[Then(@"a tela de Emissao do Boletim Escolar e carregada"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task ThenATelaDeEmissaoDoBoletimEscolarECarregada()
	{
		await _manutencaoBoletimColetivoPage.aTelaDeEmissaoDoBoletimEscolarECarregada();
	}

	[Then(@"a tela de Manutencao de Boletim Individual e carregada"), Scope(Tag = "ManutencaoBoletimColetivo")]
	public async Task ThenATelaDeManutencaoDeBoletimIndividualECarregada()
	{
		
		await _manutencaoBoletimColetivoPage.aTelaDeManutencaoDeBoletimIndividualECarregada();
	}

}
