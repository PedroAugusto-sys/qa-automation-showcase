using System;
using TechTalk.SpecFlow;
using QaShowcase.Specflow.IntegrationTests.PageObjects;
using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.Professor;

[Binding]
public class ProfessorStepDefinitions : BaseStep
{
	private ProfessorPO _cadastroProfessorPage = null!;
	private readonly ScenarioContext _scenarioContext;
	private string OperadorLogado;

	public ProfessorStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext;
	}

	//########################################################## BLOCO DADO (GIVEN) #################################################

	[Given(@"que desejo cadastrar um professor no sistema")]
	public async Task GivenQueDesejoCadastrarUmProfessorNoSistema()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo editar o cadastro de um professor no sistema")]
	public async Task GivenQueDesejoEditarOCadastroDeUmProfessorNoSistema()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();

	}

	[Given(@"que desejo excluir o cadastro de um professor no sistema")]
	public async Task GivenQueDesejoExcluirOCadastroDeUmProfessorNoSistema()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();

	}


	[Given("que preciso adiconar uma anexo a uma professor")]
	public async Task GivenQuePrecisoAdiconarUmaAnexoAUmaProfessor()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo editar o nome de um professor no sistema")]
	public async Task GivenQueDesejoEditarONomeDeUmProfessorNoSistema()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que desejo editar o cep de um professor no sistema")]
	public async Task GivenQueDesejoEditarOCepDeUmProfessorNoSistema()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que preciso editar um professor")]
	public async Task GivenQuePrecisoEditarUmProfessor()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que preciso excluir um professor")]
	public async Task GivenQuePrecisoExcluirUmProfessor()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given("que preciso pesquisar um professor")]
	public async Task GivenQuePrecisoPesquisarUmProfessor()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}




	//################################################### BLOCO E (AND) ##############################################################

	[Given("acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@Professor")]
	public async Task GivenAcessoOSistemaUtilizandoOOperador(string nomeOperador)
	{
		OperadorLogado = nomeOperador;
		//SEÇÃO DO USUÁRIO
		await RealizeLogin(nomeOperador, "");
		_cadastroProfessorPage = await POFactory.Crie<ProfessorPO>(Context);
		await _cadastroProfessorPage.GotoAsync();
	}


	[Given(@"clico em cadastrar professor")]
	public async Task GivenClicoEmCadastrarProfessor()
	{
		await _cadastroProfessorPage.ClicoEmCadastrarProfessor();
	}

	[Given("preencho o nome '([^']*)'")]
	public async Task GivenPreenchoONome(string nomeProfessor)
	{
		await _cadastroProfessorPage.InformoONomeDoProfessor(nomeProfessor);
	}

	[Given("seleciono o sexo '([^']*)'")]
	public async Task GivenSelecionoOSexo(string sexoProfessor)
	{
		await _cadastroProfessorPage.selecionaSexoDoProfessor(sexoProfessor);
	}

	[Given("seleciono a naturalidade '([^']*)'")]
	public async Task GivenSelecionoANaturalidade(string naturalidadeProfessor)
	{
		await _cadastroProfessorPage.SelecionoANaturalidadeDoProfessor(naturalidadeProfessor);
	}

	[Given("insiro o nascimento '([^']*)'")]
	public async Task GivenInsiroONascimento(string dataNascimentoProfessor)
	{
		await _cadastroProfessorPage.SelecionoADataDeNascimentoDoProfessor(dataNascimentoProfessor);
	}

	[Given("seleciono uma formacao '([^']*)'")]
	public async Task GivenSelecionoUmaFormacao(string formacaoProfessor)
	{
		await _cadastroProfessorPage.InformoAFormacaoDoProfessor(formacaoProfessor);
	}

	[Given("informo o CPF '([^']*)'")]
	public async Task GivenInformoOCPF(string cpfProfessor)
	{
		await _cadastroProfessorPage.InformoCPFDoProfessor(cpfProfessor);
	}

	[Given("seleciono o estado civil '([^']*)'")]
	public async Task GivenSelecionoOEstadoCivil(string estadoCivilProfessor)
	{
		await _cadastroProfessorPage.InformoOEstadoCivilDoProfessor(estadoCivilProfessor);
	}

	[Given("preencho o CEP '([^']*)'")]
	public async Task GivenPreenchoOCEP(string cepProfessor)
	{
		await _cadastroProfessorPage.InformoOEnderecoDoProfessor(cepProfessor);
	}

	[Given("informo o atributo adicional obrigatorio")]
	public async Task GivenInformoOAtributoAdicionalObrigatorio()
	{
		await _cadastroProfessorPage.preencheratributoadicionalobrigatorios();
	}

	[Given(@"altero a Nacionalidade do professor")]
	public async Task GivenAlteroANacionalidadeDoProfessor()
	{
		await _cadastroProfessorPage.AlteroANacionalidadeDoProfessor();
	}

	[Given("pesquiso pelo professor '([^']*)'")]
	[Scope (Tag = "@Professor")]
	public async Task GivenPesquisoPeloProfessor(string nomeProfessor)
	{
		if(_scenarioContext.ScenarioInfo.Tags.Contains("ExcluirProfessorSemVinculoCadastrando"))
		{
			//METODO DEPENDENTE DA PAGE HORARIOS ESCOLARES
			await _cadastroProfessorPage.pesquisaProfessorCadastrandoEVinculando(nomeProfessor);
		}
		else if(_scenarioContext.ScenarioInfo.Tags.Contains("EditarProfessorCadastrandoERemovendoCEP"))
		{
			await _cadastroProfessorPage.pesquisaProfessorCadastrando(nomeProfessor);
		}
		else if(_scenarioContext.ScenarioInfo.Tags.Contains("PesquisarProfessorNomeCadastrando"))
		{
			await _cadastroProfessorPage.pesquisaProfessorCadastrando(nomeProfessor);
		}
		else if (_scenarioContext.ScenarioInfo.Tags.Contains("EditarProfessorNomeCadastrando"))
		{
			await _cadastroProfessorPage.pesquisaProfessorCadastrando(nomeProfessor);
		}
		else if (_scenarioContext.ScenarioInfo.Tags.Contains("ValidarAnexoCadastroProfessor"))
		{
			await _cadastroProfessorPage.pesquisaProfessorCadastrando(nomeProfessor);
		}
		else
		{
			throw new Exception("Pesquisa não foi mapeada");
		}
		
	}

	[Given("adiciono um anexo do tipo '([^']*)'")]
	public async Task GivenAdicionoUmAnexoDoTipo(string tipoAnexo)
	{
		await _cadastroProfessorPage.addAnexoProfessor(tipoAnexo);
	}

	[Given("informo o titulo do anexo '([^']*)'")]
	public async Task GivenInformoOTituloDoAnexo(string tituloAnexo)
	{
		await _cadastroProfessorPage.informoTituloAnexo(tituloAnexo);
		_scenarioContext["TituloAnexo"] = tituloAnexo;
	}


	[Given("altero o '([^']*)' do professor para '([^']*)'")]
	public async Task GivenAlteroODoProfessorPara(string textoAtual, string novoTexto)
	{
		//Atualmente PO do professor tem um nome unico para tratamento das pesquisas, portanto não vamos usar o textoAtual nesse step
		await _cadastroProfessorPage.alterarNomeProfessor(novoTexto);
	}


	[Given("removo os dados existente para o '([^']*)'")]
	public async Task GivenRemovoOsDadosExistenteParaO(string cEP)
	{
		await _cadastroProfessorPage.apagarCampoTexto(cEP);
	}


	//CLICO EM
	[Given("clico em '([^']*)'")]
	public async Task GivenClicoEm(string botaoClicadoProfessor)
	{
		await _cadastroProfessorPage.ClicoEm(botaoClicadoProfessor);
	}

	[Given("clico em editar"), Scope(Tag = "@Professor")]
	public async Task GivenClicoEmEditar()
	{
		await _cadastroProfessorPage.ClicoEm("editar");
	}

	


	//################################################### BLOCO QUANDO (WHEN) ##############################################################


	[When("clico em '([^']*)'")]
	public async Task WhenClicoEm(string botaoClicadoProfessor)
	{
		await _cadastroProfessorPage.ClicoEm(botaoClicadoProfessor);
	}

	[When(@"confirmo a exclusao do professor")]
	public async Task WhenConfirmoAExclusaoDoProfessor()
	{
		await _cadastroProfessorPage.ConfirmoAExclusaoDoProfessor();
	}

	[When("pesquiso pelo professor '([^']*)'")]
	[Scope (Tag = "@Professor")]
	public async Task WhenPesquisoPeloProfessor(string nomeProfessor)
	{
		if (_scenarioContext.ScenarioInfo.Tags.Contains("ExcluirProfessorSemVinculoCadastrando"))
		{
			await _cadastroProfessorPage.pesquisaProfessorCadastrando(nomeProfessor);
		}
		else if (_scenarioContext.ScenarioInfo.Tags.Contains("EditarProfessorCadastrandoERemovendoCEP"))
		{
			await _cadastroProfessorPage.pesquisaProfessorCadastrando(nomeProfessor);
		}
		else if (_scenarioContext.ScenarioInfo.Tags.Contains("PesquisarProfessorNomeCadastrando"))
		{
			await _cadastroProfessorPage.pesquisaProfessorCadastrando(nomeProfessor);
		}
		else
		{
			throw new Exception("Pesquisa não foi mapeada");
		}
	}



	//################################################### BLOCO ENTAO (THEN) ##############################################################


	[Then("e apresentada a mensagem '([^']*)'")]
	[Scope (Tag= "@Professor")]
	public async Task ThenEApresentadaAMensagem(string mensagemProfessor)
	{
		await _cadastroProfessorPage.validamensagem(mensagemProfessor);
	}


	[Then("removo o operador")]
	[Scope (Tag = "@Professor")]
	public async Task ThenRemovoOOperador()
	{
		// Realiza as operações de remoção
		await CrieContexto();
		await RealizeLoginAdmin();
		await RemoverOperador(OperadorLogado);
	}

	[Then("o botao '([^']*)' é destacado em vermelho")]
	public async Task ThenOBotaoEDestacadoEmVermelho(string nomeBotao)
	{
		await _cadastroProfessorPage.validarBotaoComCampoObrigatorio(nomeBotao);
	}

	[Then("o campo '([^']*)' é destacado em vermelho")]
	public async Task ThenOCampoEDestacadoEmVermelho(string campo)
	{
		await _cadastroProfessorPage.validaCampoObrigatorio(campo);
	}


	[Then("é apresentada a tela de aterrizagem")]
	public async Task ThenEApresentadaATelaDeAterrizagem()
	{
		await _cadastroProfessorPage.validarEApresentadaTelaAterrisagem();
	}

	[Then("ele é apresentado na grid de anexos")]
	public async Task ThenEleEApresentadoNaGridDeAnexos()
	{
		await _cadastroProfessorPage.validaItensNaGridAnexos();
	}

	[Then("é apresentado somente professores que contenham em seu nome '([^']*)'")]
	public async Task ThenEApresentadoSomenteProfessoresAQueContenhamEmSeuNome(string nomeProfessorPesquisado)
	{
		await _cadastroProfessorPage.validarStringNaGrid(nomeProfessorPesquisado);
	}



}
