using QaShowcase.Specflow.IntegrationTests.PageObjects;
using System;
using System.Security.AccessControl;
using TechTalk.SpecFlow;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.Serie
{
	[Binding]
	public class SerieStepDefinitions : BaseStep
	{
		private SeriePO _serie = null!;


		[Given(@"que desejo cadastrar uma serie")]
		public async Task GivenQueDesejoCadastrarUmaSerie()
		{
			//SE��O DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
 



			//SE��O DO USU�RIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR SERIE";
			await RealizeLogin(nomeOperador, "");

			_serie = await POFactory.Crie<SeriePO>(Context);
			await _serie.GotoAsync();
		}

		[Given(@"clico em cadastrar a serie")]
		public async Task GivenClicoEmCadastrarASerie() => await _serie.ClicoEmCadastrarASerie();

		[Given(@"informo a carga horaria")]
		public async void GivenInformoACargaHoraria() => await _serie.InformoACargaHoraria();

		[When(@"clico em gravar o cadastro da serie")]
		public async Task WhenClicoEmGravarOCadastroDaSerie() => await _serie.ClicoEmGravarOCadastroDaSerie();

		[Then(@"o cadastro da serie eh realizado com sucesso")]
		public async Task ThenOCadastroDaSerieEhRealizadoComSucesso() => await _serie.OCadastroDaSerieEhRealizadoComSucesso();

		[Given(@"informo a descricao ""([^""]*)""")]
		public async Task GivenInformoADescricao(string p0) => await _serie.InformoADescricaoSerieDataAtual();

		[Given(@"adiciono disciplinas no cadastro da serie")]
		public async Task GivenAdicionoDisciplinasNoCadastroDaSerie() => await _serie.AdicionoDisciplinasNoCadastroDaSerie();


		[Given(@"que desejo editar o cadastro uma serie")]
		public async Task GivenQueDesejoEditarOCadastroUmaSerie()
		{
			//SE��O DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();




			//SE��O DO USU�RIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR SERIE";
			await RealizeLogin(nomeOperador, "");

			_serie = await POFactory.Crie<SeriePO>(Context);
			await _serie.GotoAsync();
		}

		[Given(@"clico em editar a serie")]
		public async Task GivenClicoEmEditarASerie() => await _serie.ClicoEmEditarASerie();

		[Given(@"edito a descricao da serie")]
		public async Task GivenEditoADescricaoDaSerie() => await _serie.EditoADescricaoDaSerie();

		[Then(@"serie eh editada com sucesso")]
		public async Task ThenSerieEhEditadaComSucesso() => await _serie.SerieEhEditadaComSucesso();


		[Given(@"que desejo excluir uma serie")]
		public async Task GivenQueDesejoExcluirUmaSerie()
		{
			//SE��O DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();




			//SE��O DO USU�RIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR SERIE";
			await RealizeLogin(nomeOperador, "");

			_serie = await POFactory.Crie<SeriePO>(Context);
			await _serie.GotoAsync();
		}

		[Given(@"pesquiso a serie que desejo excluir")]
		public async Task GivenPesquisoASerieQueDesejoExcluir() => await _serie.PesquisoASerieQueDesejoExcluir();

		[Given(@"clico em excluir a serie")]
		public async Task GivenClicoEmExcluirASerie() => await _serie.ClicoEmExcluirASerie();

		[When(@"clico em sim, para excluir a serie")]
		public async Task WhenClicoEmSimParaExcluirASerie() => await _serie.ClicoEmSimParaExcluirASerie();

		[Then(@"serie eh excluida com sucesso")]
		public async Task ThenSerieEhExcluidaComSucesso() => await _serie.SerieEhExcluidaComSucesso();
	}
}
