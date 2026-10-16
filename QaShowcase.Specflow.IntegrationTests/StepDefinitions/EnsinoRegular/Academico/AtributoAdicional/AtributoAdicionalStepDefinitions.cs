using QaShowcase.Specflow.IntegrationTests.PageObjects;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.AtributoAdicional;

[Binding]
public class AtributoAdicionalStepDefinitions : BaseStep
{
	private AtributoAdicionalPO _atributoAdicionalPage = null!;

	private readonly ScenarioContext _scenarioContext;

	// Injeção do ScenarioContext via construtor
	public AtributoAdicionalStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext; // Inicialize o ScenarioContext
	}

	//DADO

	[Given(@"que desejo cadastrar um atributo adicional"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenQueDesejoCadastrarAtributosAdicionais()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo editar um atributo adicional"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenQueDesejoEditarUmAtributoAdicional()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo editar um atributo que possui resposta"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenQueDesejoEditarUmAtributoAdicionalQuePossuiResposta()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo excluir um atributo adicional"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenQueDesejoExcluirUmAtributoAdicional()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo pesquisar um atributo adicional"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenQueDesejoPesquisarUmAtributoAdicional()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	//E

	[Given(@"seleciono o operador '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenSelecionoOOperador(string nomeOperador)
	{
		await RealizeLogin(nomeOperador, "");
		_scenarioContext["NomeOperador"] = nomeOperador;
		_atributoAdicionalPage = await POFactory.Crie<AtributoAdicionalPO>(Context);
		await _atributoAdicionalPage.GotoAsync();
	}

	[Given(@"clico em '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenClicoEm(string botao)
	{
		await _atributoAdicionalPage.clicarEm(botao);
	}

	[Given(@"informo a descricao '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenInformoUmaDescricao(string descicaoInformada)
	{
		await _atributoAdicionalPage.informarADescricao(descicaoInformada);
	}

	[Given(@"insiro a descricao '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenInformoDescricao(string descicaoInformada)
	{
		await _atributoAdicionalPage.informarADescricao(descicaoInformada);
	}

	[Given(@"seleciono a obrigatoriedade '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenSelecionoAObrigatoriedade(string condicao)
	{
		await _atributoAdicionalPage.selecionarObrigatoriedade(condicao);
	}

	[Given(@"seleciono a unicidade '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenSelecionoAEmUnico(string condicao)
	{
		await _atributoAdicionalPage.seleciononarUnicidade(condicao);
	}

	[Given(@"seleciono o conceito '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenSelecionoConceito(string conceito)
	{
		await _atributoAdicionalPage.selecionarConceito(conceito);
	}

	[Given(@"seleciono o tipo '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenSelecionoOTipo(string tipo)
	{
		await _atributoAdicionalPage.selecionarTipo(tipo);
	}

	[Given(@"defino o tamanho maximo '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenDefinoOTamanhoMaximo(string tamanho)
	{
		await _atributoAdicionalPage.definirTamanhoMaximo(tamanho);
	}

	[Given(@"adiciono '([^']*)' itens na lista"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenAdicionoItensNaLista(string qtdItens)
	{
		await _atributoAdicionalPage.adicionarItensNaLista(qtdItens);
	}

	[Given(@"pesquiso pelo atributo adicional '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenPesquisoPeloAtributo(string atbPesquisar)
	{
		await _atributoAdicionalPage.cadastrarParaPesquisar(atbPesquisar);
	}

	[Given("pesquiso pelo atributo adicional com resposta '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task GivenPesquisoPeloAtributoAdicionalComResposta(string p0)
	{
		// Esta step será feita após a automação da ficha do aluno
	}

	[Given(@"altero a descricao para '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
    public async Task GivenAlteroADescricaoIncluindo(string novaDescricao)
    {
        await _atributoAdicionalPage.realizarAlteracao(novaDescricao);
    }

	//QUANDO

	[When(@"clico em '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task WhenClicoEm(string botao)
	{
		await _atributoAdicionalPage.clicarEm(botao);
	}

	[When(@"pesquiso pelo atributo adicional '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task WhenPesquisoPeloAtributoAdicional(string atbPesquisar)
	{
		await _atributoAdicionalPage.cadastrarParaPesquisar(atbPesquisar);
	}

	//ENTÃO

	[Then(@"e apresentado o atributo adicinal pesquisado"), Scope(Tag = "@AtributoAdicional")]
	public async Task ThenEApresentadoOAtributoAdicinalPesquisado()
	{
		await _atributoAdicionalPage.validarAtributoAdicionalPesquisado();
	}

	[Then(@"e apresenta a mensagem de confirmação"), Scope(Tag = "@AtributoAdicional")]
	public async Task ThenEApresentaAMensagemDeConfirmacao()
	{
		string botao = "Voltar";
		await _atributoAdicionalPage.clicarEm(botao);
	}

	[Then(@"e apresentada a mensagem '([^']*)'"), Scope(Tag = "@AtributoAdicional")]
	public async Task ThenEApresentaAMensagem(string mensagem)
	{
		await _atributoAdicionalPage.validarMensagem(mensagem);
	}

	[Then("removo o operador"), Scope(Tag = "@AtributoAdicional")]
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
