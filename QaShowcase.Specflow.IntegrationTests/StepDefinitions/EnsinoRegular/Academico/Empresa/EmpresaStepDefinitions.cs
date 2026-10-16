using QaShowcase.Specflow.IntegrationTests.PageObjects;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.Empresa;

[Binding]
public class AtributoAdicionalStepDefinitions : BaseStep
{
	private EmpresaPO _empresaPage = null!;
	//private LoginPO _loginPage = null!;
	//private OperadorPO nomeOperador = null!;
	private string nomeOperador = null!;


	[Given(@"que desejo editar uma empresa")]
	public async Task GivenQueDesejoEditarUmaEmpresa()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();

	}

	[Given(@"acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@Empresa")]
	public async Task GivenAcessoOSistemaUtilizandoOOperador(string nomeOperador)
	{
		//SEÇÃO DO USUÁRIO
		this.nomeOperador = nomeOperador;
		await RealizeLogin(nomeOperador, "");

		_empresaPage = await POFactory.Crie<EmpresaPO>(Context);
		await _empresaPage.GotoAsync();
	}


	[Given(@"clico em editar no cadastro de empresa")]
	public async Task GivenClicoEmEditarNoCadastroDeEmpresa()
	{
		await _empresaPage.ClicoEmEditarNoCadastroDeEmpresa();
	}

	[Given(@"altero o apelido para '([^']*)'")]
	public async Task GivenAlteroOApelido(string apelido)
	{
		await _empresaPage.AlteroOApelido(apelido);
	}

	[When(@"clico em gravar"), Scope(Tag = "EditarApelidoEmpresa")]
	public async Task WhenClicoEmGravarAEdicaoDaEmpresa()
	{
		await _empresaPage.ClicoEmGravarAEdicaoDaEmpresa();
	}

	[Then(@"o cadastro e alterado com sucesso")]
	public async Task ThenOCadastroEAlteradoComSucesso()
	{
		await _empresaPage.OCadastroEAlteradoComSucesso();
	}

	[Then("removo o operador"), Scope(Tag = "EditarApelidoEmpresa")]
	public async Task ThenRemovoOOperador()
	{
		await CrieContexto();
		await RealizeLoginAdmin();
		await RemoverOperador(nomeOperador);
	}
}
