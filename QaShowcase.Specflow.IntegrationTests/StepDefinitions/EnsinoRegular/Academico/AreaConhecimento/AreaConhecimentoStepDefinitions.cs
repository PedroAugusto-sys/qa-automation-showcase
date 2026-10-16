using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QaShowcase.Specflow.IntegrationTests.PageObjects;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.AreaConhecimento;

[Binding]
public class AreaConhecimentoStepDefinitions : BaseStep
{
	private AreaConhecimentoPO _areaConhecimentoPage = null!;
	private readonly ScenarioContext _scenarioContext;

	// Injeção do ScenarioContext via construtor
	public AreaConhecimentoStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext; // Inicialize o ScenarioContext
	}

	// DADO

	[Given(@"que desejo cadastrar uma area de conhecimento"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task GivenQueDesejoCadastrarUmaAreaDeConhecimento()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo editar uma area de conhecimento"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task GivenQueDesejoEditarUmaAreaDeConhecimento()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo excluir uma area de conhecimento que nao esta em uso"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task GivenQueDesejoExcluirUmaAreaDeConhecimentoQueNaoEstaEmUso()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo excluir uma area de conhecimento que esta em uso"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task GivenQueDesejoExcluirUmaAreaDeConhecimentoQueEstaEmUso()
	{
		// O cenário precisa da automação de disciplinas para ser construído
	}

	[Given(@"que desejo pesquisar uma area de conhecimento"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task GivenQueDesejoPesquisarUmaAreaDeConhecimento()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	// E

	[Given(@"acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task GivenSelecionoOOperador(string nomeOperador)
	{
		await RealizeLogin(nomeOperador, "");
		_scenarioContext["NomeOperador"] = nomeOperador;
		_areaConhecimentoPage = await POFactory.Crie<AreaConhecimentoPO>(Context);
		await _areaConhecimentoPage.GotoAsync();
	}

	[Given(@"informo a descricao '([^']*)'"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task GivenInformoADescricao(string descricao)
	{
		await _areaConhecimentoPage.InformarDescricao(descricao);
	}

	[Given(@"pesquiso pela area de conhecimento '([^']*)'"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task GivenPesquisoPelaAreaDeConhecimento(string descricao)
	{
		await _areaConhecimentoPage.PesquisarAreaConhecimento(descricao);
	}

	[Given(@"clico no botao '([^']*)'"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task GivenClicoNoBotao(string botao)
	{
		await _areaConhecimentoPage.clicarEmOpcao(botao);
	}

	// QUANDO

	[When(@"clico em '([^']*)'"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task WhenClicoEm(string botao)
	{
		await _areaConhecimentoPage.clicarEmOpcao(botao);
	}

	[When(@"pesquiso pela area de conhecimento '([^']*)'"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task WhenPesquisoPelaAreaDeConhecimento(string descricao)
	{
		await _areaConhecimentoPage.PesquisarAreaConhecimento(descricao);
	}

	[When(@"clico em '([^']*)' para confirmar a operacao"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async void WhenClicoEmParaConfirmarAOperacao(string botao)
	{
		await _areaConhecimentoPage.clicarEmOpcao(botao);
	}

	// ENTÃO

	[Then(@"e apresentada a mensagem '([^']*)'"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task ThenEApresentadaAMensagem(string mensagem)
	{
		await _areaConhecimentoPage.ValidarMensagem(mensagem);
	}

	[Then(@"e apresentada somente as areas de conhecimentos que contenham no nome o termo informado"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task ThenEApresentadaSomenteAsAreasDeConhecimentosQueContenhamNoNomeOTermoInformado()
	{
		await _areaConhecimentoPage.ValidarPesquisaAreaConhecimento();
	}

	[Then(@"removo o operador"), Scope(Tag = "@AreaConhecimentoDisciplinas")]
	public async Task ThenRemovoOOperador()
	{
		// Recupera o nomeOperador armazenado no ScenarioContext
		string nomeOperador = _scenarioContext["NomeOperador"] as string;

		// Verifica se o nomeOperador foi encontrado
		Assert.NotNull(nomeOperador, "O nome do operador não foi encontrado no contexto.");

		// Realiza as operações de remoção
		await CrieContexto();
		await RealizeLoginAdmin();
		await RemoverOperador(nomeOperador);
	}

}
