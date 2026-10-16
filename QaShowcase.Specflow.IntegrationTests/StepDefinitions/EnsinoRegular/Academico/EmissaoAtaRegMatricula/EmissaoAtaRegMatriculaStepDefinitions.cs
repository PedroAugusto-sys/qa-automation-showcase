using QaShowcase.Specflow.IntegrationTests.PageObjects;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.EmissaoAtaRegMatricula;

[Binding]
public class EmissaoAtaRegMatriculaStepDefinitions : BaseStep
{
	private EmissaoAtaRegMatriculaPO _emissaoAtaRegMatriculaPage = null!;



	[Given(@"que desejo emitir a ata de registro de matriculas por empresa"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenQueDesejoEmitirAAtaDeRegistroDeMatriculasPorEmpresa()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();




		//SEÇÃO DO USUÁRIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR ATA MATRICULA";
		await RealizeLogin(nomeOperador, "");

		_emissaoAtaRegMatriculaPage = await POFactory.Crie<EmissaoAtaRegMatriculaPO>(Context);
		await _emissaoAtaRegMatriculaPage.GotoAsync();
	}

	[Given(@"que desejo emitir a ata de registro de matriculas por série/turma"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenQueDesejoEmitirAAtaDeRegistroDeMatriculasPorSerieTurma()
	{
		//SEÇÃO DO ADMIN 
		await CrieContexto();
		await RealizeLoginAdmin();




		//SEÇÃO DO USUÁRIO
		// RealizeLogin cria o operador automaticamente se não existir e já faz o login
		string nomeOperador = "OPERADOR ATA MATRICULA";
		await RealizeLogin(nomeOperador, "");

		_emissaoAtaRegMatriculaPage = await POFactory.Crie<EmissaoAtaRegMatriculaPO>(Context);
		await _emissaoAtaRegMatriculaPage.GotoAsync();
	}

	[Given(@"clico em '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenClicoEm(string filtro)
	{
		await _emissaoAtaRegMatriculaPage.selecionarFiltro(filtro);
	}

	[Given(@"seleciono o ano '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenSelecionoOAno(string p0)
	{
		await _emissaoAtaRegMatriculaPage.selecionarAno(p0);
	}

	[Given(@"seleciono a série '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenSelecionoASerie(string p0)
	{
		await _emissaoAtaRegMatriculaPage.selecionarSerie(p0);
	}

	[Given(@"filtro por '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenFiltroPor(string p0)
	{
		await _emissaoAtaRegMatriculaPage.selecionarFiltro(p0);
	}

	[Given(@"seleciono a empresa '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenSelecionoAEmpresa(string p0)
	{
		await _emissaoAtaRegMatriculaPage.selecionarEmpresa(p0);
	}

	[Given(@"seleciono a turma '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenSelecionoATurma(string p0)
	{
		await _emissaoAtaRegMatriculaPage.selecionarTurma(p0);
	}

	[Given(@"defino o título no leiaute '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenDefinoOTituloNoLeiaute(string p0)
	{
		await _emissaoAtaRegMatriculaPage.escreverTitulo(p0);
	}

	[Given(@"defino o leiaute '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenDefinoOLeiaute(string mostrar)
	{
		await _emissaoAtaRegMatriculaPage.selecionarLeiaute(mostrar);
	}

	[Given(@"defino as Assinaturas"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenDefinoAsAssinaturas()
	{
		await _emissaoAtaRegMatriculaPage.selecionarLeiauteAssinatura("Mostrar");
	}

	[Given(@"defino as Assinaturas para mostrar o '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task GivenDefinoAsAssinaturasParaMostrarO(string p0)
	{
		await _emissaoAtaRegMatriculaPage.selecionarAssinaturas(p0);
	}

	[When(@"clico em '([^']*)'"), Scope(Tag = "EmissãoAtaRegistroMatricula")]
	public async Task WhenClicoEm(string emitir)
	{
		await _emissaoAtaRegMatriculaPage.clicarEmitir();	
	}

	[Then(@"é gerado um relatório")]
	public async Task ThenEGeradoUmRelatorio()
	{
		await _emissaoAtaRegMatriculaPage.fecharRelatorio();
	}




}
