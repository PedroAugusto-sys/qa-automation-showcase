using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QaShowcase.Specflow.IntegrationTests.PageObjects;
using NUnit.Framework;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.CadastroTiposProfissionais
{
	[Binding]
	public class TipoProfissionalStepDefinitions : BaseStep
	{
		private TipoProfissionalPO _cadastroDeTipoProfissionalPage;

		private OutrosProfissionaisPO _cadastroDeProfissionalPage;
	

		private readonly ScenarioContext _scenarioContext;

		// Injeção do ScenarioContext via construtor
		public TipoProfissionalStepDefinitions(ScenarioContext scenarioContext)
		{
			_scenarioContext = scenarioContext; // Inicialize o ScenarioContext
		}


		[Given("que desejo cadastrar um tipo de profissional")]
		public async Task GivenQueDesejoCadastrarUmTipoDeProfissional()
		{
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		[Given("que desejo editar um tipo de profissional")]
		public async Task GivenQueDesejoEditarUmTipoDeProfissional()
		{
			await CrieContexto();
			await RealizeLoginAdmin();
		}


		[Given("que desejo excluir um tipo de profissional que nao esta em uso")]
		public async Task GivenQueDesejoExcluirUmTipoDeProfissionalQueNaoEstaEmUso()
		{
			{
				await CrieContexto();
				await RealizeLoginAdmin();
			}
		}

		[Given("que desejo excluir um tipo de profissional que esta em uso")]
		public async Task GivenQueDesejoExcluirUmTipoDeProfissionalQueEstaEmUso()
		{
			await CrieContexto();
			await RealizeLoginAdmin();
		}


		[Given("que desejo pesquisar um tipo de profissional")]
		public async Task GivenQueDesejoPesquisarUmTipoDeProfissional()
		{
			await CrieContexto();
			await RealizeLoginAdmin();
		}





		[Given("acesso o sistema utilizando o operador '([^']*)'") , Scope(Tag = "TiposProfissionais")]
		public async Task GivenAcessoOSistemaUtilizandoOOperador(string nomeOperador)
		{

			//this.nomeOperador = nomeOperador;
			await RealizeLogin(nomeOperador, "");

			// Armazena o nomeOperador no ScenarioContext

			_scenarioContext["NomeOperador"] = nomeOperador;
			                                    
		
			_cadastroDeTipoProfissionalPage = await POFactory.Crie<TipoProfissionalPO>(Context);
		}

		[Given("informo a descricao '([^']*)'"), Scope(Tag = "TiposProfissionais")]
		public async Task GivenInformoADescricao(string tipoDeProfissional)
		{
			// Navega para a página definida em 
			await _cadastroDeTipoProfissionalPage.GotoAsync();

			// Preenche a descrição do tipo de profissional
			await _cadastroDeTipoProfissionalPage.PreencherDescricaoDoTipoProfissional(tipoDeProfissional);

			// Armazena o tipoDeProfissional no ScenarioContext
			_scenarioContext["TipoDeProfissional"] = tipoDeProfissional;

		}



		[Given("pesquiso pelo tipo de profissional '([^']*)'")]
		public async Task GivenPesquisoPeloTipoDeProfissional(string tipoDeProfissional)
		{

			await _cadastroDeTipoProfissionalPage.GotoAsync();
			await _cadastroDeTipoProfissionalPage.PreencherDescricaoDoTipoProfissional(tipoDeProfissional);

			// Realiza a pesquisa usando o tipoDeProfissional passado diretamente
			await _cadastroDeTipoProfissionalPage.PesquisarTipoDeProfissional(tipoDeProfissional);
			_scenarioContext["TipoDeProfissionalPesquisado"] = tipoDeProfissional; // <-- Aqui você armazena
		}

		[Given("pesquiso pelo tipo de profissional em uso '([^']*)'")]
		public async Task GivenPesquisoPeloTipoDeProfissionalEmUso(string tipoDeProfissional)
		{
			_cadastroDeProfissionalPage = await POFactory.Crie<OutrosProfissionaisPO>(Context);
			await _cadastroDeProfissionalPage.GotoAsync();
			await _cadastroDeProfissionalPage.CadastroCompletoDeProfissional(tipoDeProfissional);
			_cadastroDeTipoProfissionalPage = await POFactory.Crie<TipoProfissionalPO>(Context);
			await _cadastroDeTipoProfissionalPage.GotoAsync();
			await _cadastroDeTipoProfissionalPage.PesquisarTipoDeProfissionalApenas(tipoDeProfissional);

		}


		[Given("clico em '([^']*)'"), Scope(Tag = "TiposProfissionais")]
		public async Task WhenClicoEmAnd(string opcao)
		{

			await _cadastroDeTipoProfissionalPage.ClicoEmAnd(opcao);
		}


		[When("pesquiso pelo tipo de profissional '([^']*)'")]
		public async Task GivenApenasPesquisoPeloTipoDeProfissional(string tipoDeProfissional)
		{
			// Navega para a página e realiza a pesquisa
			await _cadastroDeTipoProfissionalPage.GotoAsync();

			await _cadastroDeTipoProfissionalPage.PesquisarTipoDeProfissional(tipoDeProfissional);

			_scenarioContext["TipoDeProfissionalPesquisado"] = tipoDeProfissional;
		}



		[When("clico em '([^']*)' para confirmar a operacao")]
		public async Task WhenClicoEmParaConfirmarAOperacao(string opcao)
		{
			await _cadastroDeTipoProfissionalPage.ClicoEmWhen(opcao);
		}



		[When("clico em '([^']*)'"), Scope(Tag = "TiposProfissionais")]
		public async Task WhenClicoEmWhen(string opcao)
		{
			await _cadastroDeTipoProfissionalPage.ClicoEmWhen(opcao);
		}


		[Then("e apresentada a mensagem '([^']*)'"), Scope(Tag = "TiposProfissionais")]

		public async Task ThenEApresentadaAMensagem(string p0)
		{
			await Task.Delay(2000);
			await _cadastroDeTipoProfissionalPage.EApresentadaAMensagem(p0);

		}


		[Then("e apresentada somente os tipos de profissional que contenham no nome o termo informado")]
		public async Task ThenEApresentadaSomenteOsTiposDeProfissionalQueContenhamNoNomeOTermoInformado()
		{

			// Aguarda um tempo para garantir que a grid seja carregada
			await Task.Delay(2000);

			// Recupera o tipoDeProfissional do ScenarioContext
			var tipoDeProfissional = _scenarioContext["TipoDeProfissionalPesquisado"].ToString();

			var encontrado = await _cadastroDeTipoProfissionalPage.ContemNaGrid(tipoDeProfissional);


			// Validação (usando Assert ou outro mecanismo de teste)
			Assert.True(encontrado, $"O tipo de profissional '{tipoDeProfissional}' não foi encontrado na grid.");

		}




	}
}
