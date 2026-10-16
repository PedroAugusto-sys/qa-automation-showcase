using QaShowcase.Specflow.IntegrationTests.PageObjects;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.Responsavel;

[Binding]
public class ResponsavelStepDefinitions : BaseStep
{
	private ResponsavelPO _responsavelPage = null!;
	private readonly ScenarioContext _scenarioContext;

	// Injeção do ScenarioContext via construtor
	public ResponsavelStepDefinitions(ScenarioContext scenarioContext)
	{
		_scenarioContext = scenarioContext; // Inicialize o ScenarioContext
	}

	// GIVEN

	[Given(@"que desejo cadastrar um responsavel"), Scope(Tag = "@Responsavel")]
	public async Task GivenQueDesejoCadastrarUmResponsavel()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo editar um responsavel"), Scope(Tag = "@Responsavel")]
	public async Task GivenQueDesejoEditarUmResponsavel()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	[Given(@"que desejo excluir um responsavel"), Scope(Tag = "@Responsavel")]
	public async Task GivenQueDesejoExcluirUmResponsavel()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
	}

	// AND

	[Given(@"acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenAcessoOSistemaUtilizandoOOperador(string nomeOperador)
	{
		await RealizeLogin(nomeOperador, "");
		_scenarioContext["NomeOperador"] = nomeOperador;
		_responsavelPage = await POFactory.Crie<ResponsavelPO>(Context);
		await _responsavelPage.GotoAsync();
	}

	[Given(@"clico em '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenClicoEm(string Botao)
	{
		await _responsavelPage.ClicarEm(Botao);
	}

	[Given(@"informo o nome '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenInformoONome(string Nome)
	{
		await _responsavelPage.InformarNome(Nome);
	}

	[Given(@"informo a data de nascimento '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenInformoADataDeNascimento(string DataNascimento)
	{
		await _responsavelPage.InformarDtNascimento(DataNascimento);
	}

	[Given(@"seleciono o sexo '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenSelecionoOSexo(string Sexo)
	{
		await _responsavelPage.SelecionarSexo(Sexo);
	}

	[Given(@"informo o estado civil '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenInformoOEstadoCivil(string EstadoCivil)
	{
		await _responsavelPage.SelecionarEstadoCivil(EstadoCivil);
	}

	[Given(@"informo o cep '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenInoformoOEndereco(string CEP)
	{
		await _responsavelPage.PreencherCEP(CEP);
	}

	[Given(@"informo o cpf"), Scope(Tag = "@Responsavel")]
	public async Task GivenInformoOCPF()
	{
		await _responsavelPage.InformarCPF();
	}

	[Given(@"preencho os atributos adicionais obrigatorios"), Scope(Tag = "@Responsavel")]
	public async Task GivenPreenchoOsAtributosAdicionaisObrigatorios()
	{
		await _responsavelPage!.InserirAtributoAdicional();
	}

	[Given(@"altero o nome do responsavel para '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenAlteroONomeDoResponsavelPara(string NomeResponsavel) // altero o 'nome' do responsavel para '<nome_alterado>'
	{
		await _responsavelPage.AlterarNomeResponsavel(NomeResponsavel);
	}

	[Given(@"pesquiso pelo responsavel '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenPesquisoPeloResponsavel(string NomeResponsavel)
	{
		await _responsavelPage.CadastrarResponsavel(NomeResponsavel);
		await _responsavelPage.PesquisarResponsavelPorID(NomeResponsavel);
	}

	[Given(@"pesquiso o responsavel '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenPesquisoOResponsavel(string NomeResponsavel)
	{
		await _responsavelPage.PesquisarResponsavelPorID(NomeResponsavel);
	}

	[Given(@"o responsavel '([^']*)' possui vinculo com o aluno '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task GivenEsseResponsavelPossuiVinculoComOAluno(string NomeResponsavel, string NomeAluno)
	{
		await _responsavelPage.CadastrarResponsavelEAlunoVinculando(NomeResponsavel, NomeAluno);
	}

	// WHEN

	[When(@"clico em '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task WhenClicoEm(string botao)
	{
		await _responsavelPage.ClicarEm(botao);
	}

	// THEN

	[Then(@"e apresentada a mensagem '([^']*)'"), Scope(Tag = "@Responsavel")]
	public async Task ThenEApresentadaAMensagemDeConfirmacao(string Mensagem)
	{
		await _responsavelPage.ValidarMensagem(Mensagem);
	}

	[Then(@"e apresentada a tela de aterrissagem"), Scope(Tag = "@Responsavel")]
	public async Task ThenEApresentadaATelaDeAterrissagem()
	{
		await _responsavelPage.ApresentarTelaDeAterrissagem();
	}
	[Then(@"removo o operador"), Scope(Tag = "@Responsavel")]
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
