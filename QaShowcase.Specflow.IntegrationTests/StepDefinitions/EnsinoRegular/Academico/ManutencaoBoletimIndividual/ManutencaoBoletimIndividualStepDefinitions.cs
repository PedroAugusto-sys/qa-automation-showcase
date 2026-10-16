using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QaShowcase.Specflow.IntegrationTests.PageObjects;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.ManutencaoBoletimIndividual;

[Binding]

internal class ManutencaoBoletimIndividualStepDefinitions : BaseStep
{
	private ManutencaoBoletimIndividualPO _manutencaoBoletimIndividualPage = null!;



	[Given(@"que preciso realizar o lancamento de notas na manutencao do boletim individual"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenQuePrecisoRealizarOLancamentoDeNotasNaManutencaoDoBoletimInidvidual()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();




		//SEÇÃO DO USUÁRIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR BOLETIM";
		await RealizeLogin(nomeOperador, "");

		_manutencaoBoletimIndividualPage = await POFactory.Crie<ManutencaoBoletimIndividualPO>(Context);
		await _manutencaoBoletimIndividualPage.GotoAsync();
	}

	[Given(@"que preciso realizar o lancamento de faltas na manutencao do boletim individual"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenQuePrecisoRealizarOLancamentoDeFaltasNaManutencaoDoBoletimInidvidual()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();



		//SEÇÃO DO USUÁRIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR BOLETIM";
		await RealizeLogin(nomeOperador, "");

		_manutencaoBoletimIndividualPage = await POFactory.Crie<ManutencaoBoletimIndividualPO>(Context);
		await _manutencaoBoletimIndividualPage.GotoAsync();
	}

	[Given(@"que preciso realizar a emissao do boletim individual"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenQuePrecisoRealizarAEmissaoDoBoletimIndividual()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();


		//SEÇÃO DO USUÁRIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR BOLETIM";
		await RealizeLogin(nomeOperador, "");

		_manutencaoBoletimIndividualPage = await POFactory.Crie<ManutencaoBoletimIndividualPO>(Context);
		await _manutencaoBoletimIndividualPage.GotoAsync();
	}

	[Given(@"que preciso recalcular as nota e faltas do boletim"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenQuePrecisoRecalcularAsNotaEFaltasDoBoletim()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();


		//SEÇÃO DO USUÁRIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR BOLETIM";
		await RealizeLogin(nomeOperador, "");

		_manutencaoBoletimIndividualPage = await POFactory.Crie<ManutencaoBoletimIndividualPO>(Context);
		await _manutencaoBoletimIndividualPage.GotoAsync();
	}

	[Given(@"seleciono o ano letivo"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenSelecionoOAnoLetivo()
	{
		await _manutencaoBoletimIndividualPage.selecionoOAnoLetivo("2024");
	}

	[Given(@"seleciono o aluno"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenSelecionoOAluno()
	{
		await _manutencaoBoletimIndividualPage.selecionoOAluno();
	}

	[Given(@"clico em Carregar"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenClicoEmCarregar()
	{
		await _manutencaoBoletimIndividualPage.clicoEmCarregar();
	}

	[Given(@"realizo o lancamento de notas"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenRealizoOLancamentoDeNotas()
	{
		await _manutencaoBoletimIndividualPage.realizoOLancamentoDeNotas();
	}

	[Given(@"clico em Gravar"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenClicoEmGravar()
	{
		await _manutencaoBoletimIndividualPage.clicoEmGravar();
	}

	[When(@"clico em Sim"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task WhenClicoEmSim()
	{
		await _manutencaoBoletimIndividualPage.clicoEmSim();
	}

	[Then(@"o boletim e gravado com sucesso"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task ThenOBoletimEGravadoComSucesso()
	{
		await _manutencaoBoletimIndividualPage.oBoletimEGravadoComSucesso();
	}

	[Given(@"clico em Emitir"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenClicoEmEmitir()
	{
		await _manutencaoBoletimIndividualPage.clicoEmEmitir();
	}

	[When(@"clico em Emitir novamente"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task WhenClicoEmEmitirNovamente()
	{
		await _manutencaoBoletimIndividualPage.clicoEmEmitirNovamente();
	}

	[Then(@"o boletim e emitido com sucesso"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task ThenOBoletimEEmitidoComSucesso()
	{
		await _manutencaoBoletimIndividualPage.oBoletimEEmitidoComSucesso();
	}

	[Given(@"realizo o lancamento de faltas"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task GivenRealizoOLancamentoDeFaltas()
	{
		await _manutencaoBoletimIndividualPage.realizoOLancamentoDeFaltas();
	}

	[When(@"clico em Recalcular"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task WhenClicoEmRecalcular()
	{
		await _manutencaoBoletimIndividualPage.clicoEmRecalcular();
	}

	[Then(@"o recalculo do boletim e realizado"), Scope(Tag = "ManutencaoBoletimIndividual")]
	public async Task ThenORecalculoDoBoletimERealizado()
	{
		await _manutencaoBoletimIndividualPage.oRecalculoDoBoletimERealizado();
	}
}
