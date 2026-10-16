using QaShowcase.Specflow.IntegrationTests.PageObjects;
using TechTalk.SpecFlow;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.DiarioDeClasse
{
	[Binding]
	public class DiarioDeClasseStepDefinitions : BaseStep
	{
		private ProcessamentoDeFaltasParaBoletimPO _processamentoDeFaltasParaBoletim = null!;
		private ConteudoMinistradoPo _conteudoMinistradoPo;


		[Given(@"que desejo realizar o processamento de faltas para o boletim")]
		public async Task GivenQueDesejoRealizarOProcessamentoDeFaltasParaOBoletim()
		{
			//SE��O DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();



			//SE��O DO USU�RIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR DIARIO";
			await RealizeLogin(nomeOperador, "");

			_processamentoDeFaltasParaBoletim = await POFactory.Crie<ProcessamentoDeFaltasParaBoletimPO>(Context);
			await _processamentoDeFaltasParaBoletim.GotoAsync();
		}

		[Given(@"que desejo realizar lancamento de conteudo ministrado")]
		public async Task GivenQueDesejoRealizarLancamentoDeConteudoMinistrado()
		{
			//SE��O DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();



			//SE��O DO USU�RIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR DIARIO";
			await RealizeLogin(nomeOperador, "");

			_conteudoMinistradoPo = await POFactory.Crie<ConteudoMinistradoPo>(Context);
			await _conteudoMinistradoPo.GotoAsync();
		}

		[Given(@"que desejo editar o lancamento de conteudo ministrado")]
		public async Task GivenQueDesejoEditarOLancamentoDeConteudoMinistrado()
		{
			//SE��O DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();



			//SE��O DO USU�RIO
			// RealizeLogin cria o operador automaticamente se não existir e já faz o login
			string nomeOperador = "OPERADOR DIARIO";
			await RealizeLogin(nomeOperador, "");

			_conteudoMinistradoPo = await POFactory.Crie<ConteudoMinistradoPo>(Context);
			await _conteudoMinistradoPo.GotoAsync();
		}

		[Given(@"seleciono a serie para realizar o processamento de faltas")]
		public async Task GivenSelecionoASerieParaRealizarOProcessamentoDeFaltas() => await _processamentoDeFaltasParaBoletim.SelecionoASerieParaRealizarOProcessamentoDeFaltas();

		[Given(@"configuro os filtros do processamento de faltas para o boletim")]
		public void GivenConfiguroOsFiltrosDoProcessamentoDeFaltasParaOBoletim()
		{
		}

		[Given(@"configuro os filtros do conteudo ministrado")]
		public async Task GivenConfiguroOsFiltrosDoConteudoMinistrado() => await _conteudoMinistradoPo.ConfiguroOsFiltrosDoConteudoMinistrado();

		[Given(@"clico em carregar, o conteudo ministrado")]
		public async Task GivenClicoEmCarregarOConteudoMinistrado() => await _conteudoMinistradoPo.ClicoEmCarregarOConteudoMinistrado();

		[Given(@"seleciono para replicar o conteudo ministrado")]
		public async Task GivenSelecionoParaReplicarOConteudoMinistrado() => await _conteudoMinistradoPo.SelecionoParaReplicarOConteudoMinistrado();

		[Given(@"realizo o lancamento do conteudo ministrado replicado")]
		public async Task GivenRealizoOLancamentoDoConteudoMinistradoReplicado() => await _conteudoMinistradoPo.RealizoOLancamentoDoConteudoMinistradoReplicado();

		[Given(@"realizo o lancamento do conteudo ministrado")]
		public async Task GivenRealizoOLancamentoDoConteudoMinistrado() => await _conteudoMinistradoPo.RealizoOLancamentoDoConteudoMinistrado();

		[Given(@"edito o conteudo ministrado")]
		public async Task GivenEditoOConteudoMinistrado() => await _conteudoMinistradoPo.EditoOConteudoMinistrado();

		[When(@"clico em processar, as faltas para o boletim")]
		public async Task WhenClicoEmProcessarAsFaltasParaOBoletim() => await _processamentoDeFaltasParaBoletim.ClicoEmProcessarAsFaltasParaOBoletim();

		[When(@"clico em gravar, o conteudo ministrado")]
		public async Task WhenClicoEmGravarOConteudoMinistrado() => await _conteudoMinistradoPo.ClicoEmGravarOConteudoMinistrado();

		[Then(@"o sistema apresenta a mensagem: ""([^""]*)""")]
		public async Task ThenOSistemaApresentaAMensagem(string p0) => await _processamentoDeFaltasParaBoletim.OSistemaApresentaAMensagem();

		[Then(@"a tela de conteudo ministrado apresenta a mensagem: ""([^""]*)""")]
		public async Task ThenATelaDeConteudoMinistradoApresentaAMensagem(string p0) => await _conteudoMinistradoPo.ATelaDeConteudoMinistradoApresentaAMensagem();

		[Then(@"o conteudo ministrado eh replicado com sucesso")]
		public async Task ThenOConteudoMinistradoEhReplicadoComSucesso() => await _conteudoMinistradoPo.OConteudoMinistradoEhReplicadoComSucesso();

		[Then(@"o conteudo ministrado eh editado")]
		public async Task ThenOConteudoMinistradoEhEditado() => await _conteudoMinistradoPo.OConteudoMinistradoEhEditado();
	}
}