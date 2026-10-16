using QaShowcase.Specflow.IntegrationTests.PageObjects;
using Microsoft.Playwright;
using System;
using System.Security.AccessControl;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace QaShowcase.Specflow.IntegrationTests.StepDefinitions.EnsinoRegular.Academico.Disciplina
{
	[Binding]
	public class DisciplinaStepDefinitions : BaseStep
	{
		private DisciplinaPO _disciplina = null!;
		private string nomeOperador = null!;


		// DADO

		// DADO - Cadastrar uma disciplina comum
		[Given(@"que desejo cadastrar uma disciplina comum"), Scope(Tag = "@CadastrarDisciplinasObrigatorio")]
		public async Task GivenQueDesejoPesquisarUmAtributoAdicional()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		// DADO - Alterar o nome de uma disciplina

		[Given("que desejo alterar o nome de uma disciplina"), Scope(Tag = "@EditarNomeDisciplinasCadastrando")]
		public async Task GivenQueDesejoAlterarONomeDeUmaDisciplina()
		{
			//SEÇÃO DO ADMIN
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		// DADO - Alterar a situação de uma disciplina
		[Given("que desejo alterar a situacao de uma disciplina"), Scope(Tag = "@AlterarSituacaoDisciplinasCadastrando")]
		public async Task GivenQueDesejoAlterarASituacaoDeUmaDisciplina()
		{
			//SEÇÃO DO ADMIN
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		// DADO - Excluir uma disciplina sem vínculos
		[Given("que desejo excluir uma disciplina")]
		[Scope(Tag = "@ExcluirDisciplinasSemVinculosCadastrando")]
		[Scope(Tag = "@ExcluirDisciplinasComVinculosCadastrando")]
		public async Task QueDesejoExcluirUmaDisciplina()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		// DADO - Excluir uma disciplina com vínculos
		[Given("acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@ExcluirDisciplinasComVinculosCadastrando")]
		public async Task GivenAcessoOSistemaUtilizandoOOperadorExcluirDisciplinaComVinculos(string nomeOperador)
		{
			//SEÇÃO DO USUÁRIO
			this.nomeOperador = nomeOperador;
			await RealizeLogin(nomeOperador, "");

			_disciplina = await POFactory.Crie<DisciplinaPO>(Context);
			await _disciplina.GotoAsync();
		}

		// DADO - Pesquiso por uma disciplina
		[Given("que desejo localizar uma disciplina"), Scope(Tag = "@PesquisarDisciplinasCadastrando")]
		public async Task GivenQueDesejoLocalizarUmaDisciplina()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		// DADO - Filtrar disciplinas cadastradas
		[Given("que desejo localizar uma disciplina"), Scope(Tag = "@FiltrarGridDisciplinasCadastrando")]
		public async Task GivenQueDesejoLocalizarUmaDisciplinaParaFiltrar()
		{
			//SEÇÃO DO ADMIN 
			await CrieContexto();
			await RealizeLoginAdmin();
		}

		// E

		// E - Acesso o sistema utilizando o operador desejado
		[Given("acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@CadastrarDisciplinasObrigatorio")]
		public async Task GivenAcessoOSistemaUtilizandoOOperador(string nomeOperador)
		{
			//SEÇÃO DO USUÁRIO
			await RealizeLogin(nomeOperador, "");
			//_scenarioContext["NomeOperador"] = nomeOperador;
			this.nomeOperador = nomeOperador;
			_disciplina = await POFactory.Crie<DisciplinaPO>(Context);
			await _disciplina.GotoAsync();
		}

		// E - Clico no botão 'Cadastrar' para iniciar o cadastro de disciplina
		[Given(@"informo o nome '([^']*)'"), Scope(Tag = "@CadastrarDisciplinasObrigatorio")]
		public async Task GivenInformoADescricaoDaDisciplina(string descricaoDisciplina)
		{
			await _disciplina.InformoADescricaoDaDisciplina(descricaoDisciplina);
		}

		// E - Seleciono a situação para a disciplina
		[Given("seleciono a situacao '([^']*)'"), Scope(Tag = "@CadastrarDisciplinasObrigatorio")]
		public async Task GivenSelecionoASituacao(string ativo)
		{
			await _disciplina.InformoASituacao(ativo);
		}

		/// <summary>
		/// ///////////
		/// </summary>
		/// <param name="nomeOperador"></param>
		/// <returns></returns>

		// E - Informo a área de conhecimento
		[Given("informo a area de conhecimento '([^']*)'"), Scope(Tag ="CadastrarDisciplinasObrigatorio")]
		public async Task GivenInformoAAreaDeConhecimento(string areaDeConhecimento)
		{
			await _disciplina.InformoAAreaDeConhecimento(areaDeConhecimento);
		}
		// E - Seleciono o tipo de disciplina 
		[Given("seleciono o tipo '([^']*)'"), Scope(Tag ="CadastrarDisciplinasObrigatorio")]
		public async Task GivenSelecionoOTipo(string comum)
		{
			await _disciplina.SelecionoTipoDeDisciplina(comum);
		}

		// E - Seleciono o grupo de histórico escolar
		[Given("seleciono o grupo de historico escolar '([^']*)'"), Scope(Tag ="CadastrarDisciplinasObrigatorio")]
		public async Task GivenSelecionoOGrupoDeHistoricoEscolar(string grupoHistoricoEscolar)
		{
			await _disciplina.SelecionoGrupoHistoricoEscolar(grupoHistoricoEscolar);
		}

		// E - Seleciono o tipo de agregação
		[Given("informo o Tipo de agregacao '([^']*)'"), Scope(Tag ="CadastrarDisciplinasObrigatorio")]
		public async Task GivenInformoOTipoDeAgregacao(string tipoAgregacao)
		{
			await _disciplina.SelecionoTipoDeAgregacao(tipoAgregacao);
		}

		// E - Seleciono em histórico escolar a opção entre 'Sim' ou 'Não'
		[Given("em Historico escolar seleciono a opcao '([^']*)'"), Scope(Tag ="@CadastrarDisciplinasObrigatorio")]
		public async Task GivenSelecionoHistoricoEscolar(string historicoEscolar)
		{
			await _disciplina.SelecionoHistoricoEscolar(historicoEscolar);
		}

		// E - Acesso o sistema utilizando o operador desejado

		[Given("acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@EditarNomeDisciplinasCadastrando")]
		public async Task GivenSelecionoOOperador(string nomeOperador)
		{

			//SEÇÃO DO USUÁRIO
			this.nomeOperador = nomeOperador;
			await RealizeLogin(nomeOperador, "");

			_disciplina = await POFactory.Crie<DisciplinaPO>(Context);
			await _disciplina.GotoAsync();

		}

		// E - Pesquiso pela disciplina que desejo alterar o nome
		[Given("pesquiso pela disciplina '([^']*)'")]
		[Scope(Tag = "@EditarNomeDisciplinasCadastrando")]
		[Scope(Tag = "@AlterarSituacaoDisciplinasCadastrando")]
		[Scope(Tag = "@ExcluirDisciplinasSemVinculosCadastrando")]
		public async Task GivenPesquisoPelaDisciplina(string descricao)
		{
			await _disciplina.PesquisaCadastrando(descricao);
		}

		// E - Altero o nome da disciplina para o novo nome informado
		[Given("altero o nome da disciplina para '([^']*)'"), Scope(Tag = "@EditarNomeDisciplinasCadastrando")]
		public async Task GivenAlteroONomeDaDisciplina(string novaDescricao)
		{
			await _disciplina.AlteroADescricao(novaDescricao);
		}

		// E - Clico no botão 'Salvar' para concluir a alteração
		[Given("clico em '([^']*)'")]
		[Scope(Tag = "@CadastrarDisciplinasObrigatorio")]
		[Scope(Tag = "@EditarNomeDisciplinasCadastrando")]
		[Scope(Tag = "@AlterarSituacaoDisciplinasCadastrando")]
		[Scope(Tag = "@ExcluirDisciplinasSemVinculosCadastrando")]
		[Scope(Tag = "@ExcluirDisciplinasComVinculosCadastrando")]
		public async Task GivenClicoEm(string botao)
		{
			await _disciplina.ClicoEm(botao);
		}

		// E - Acesso o sistema utilizando o operador desejado
		[Given("acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@AlterarSituacaoDisciplinasCadastrando")]
		public async Task GivenAcessoOSistemaUtilizandoOOperadorParaAlterarSituacaoDisciplina(string nomeOperador)
		{
			//SEÇÃO DO USUÁRIO
			this.nomeOperador = nomeOperador;
			await RealizeLogin(nomeOperador, "");

			_disciplina = await POFactory.Crie<DisciplinaPO>(Context);
			await _disciplina.GotoAsync();
		}

		// E - Altero a situação da disciplina para a nova situação informada
		[Given("altero a situacao para '([^']*)'"), Scope(Tag = "@AlterarSituacaoDisciplinasCadastrando")]
		public async Task GivenAlteroASituacaoPara(string situacao)
		{
			await _disciplina.AlteraSituacao(situacao);
		}

		// E - Acesso o sistema utilizando o operador desejado
		[Given("acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@ExcluirDisciplinasSemVinculosCadastrando")]
		public async Task GivenAcessoOSistemaUtilizandoOOperadorExcluirDisciplina(string nomeOperador)
		{
			//SEÇÃO DO USUÁRIO
			this.nomeOperador = nomeOperador;
			await RealizeLogin(nomeOperador, "");

			_disciplina = await POFactory.Crie<DisciplinaPO>(Context);
			await _disciplina.GotoAsync();
		}

		// E - Pesquiso pela disciplina que desejo excluir
		[Given("pesquiso pela disciplina '([^']*)'"), Scope(Tag = "@ExcluirDisciplinasComVinculosCadastrando")]
		public async Task GivenPesquisoPelaDisciplinaComVinculo(string disciplinaComVinculo)
		{
			await _disciplina.PesquisaDisciplinaComVinculos(disciplinaComVinculo);
		}

		// E - Acesso o sistema utilizando o operador desejado
		[Given("acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@PesquisarDisciplinasCadastrando")]
		public async Task GivenAcessoOSistemaUtilizandoOOperadorPesquisarDisciplina(string nomeOperador)
		{
			//SEÇÃO DO USUÁRIO
			this.nomeOperador = nomeOperador;
			await RealizeLogin(nomeOperador, "");

			_disciplina = await POFactory.Crie<DisciplinaPO>(Context);
			await _disciplina.GotoAsync();
		}

		// E - Acesso o sistema utilizando o operador desejado
		[Given("acesso o sistema utilizando o operador '([^']*)'"), Scope(Tag = "@FiltrarGridDisciplinasCadastrando")]
		public async Task GivenAcessoOSistemaUtilizandoOOperadorParaFiltrarDisciplina(string nomeOperador)
		{
			//SEÇÃO DO USUÁRIO
			this.nomeOperador = nomeOperador;
			await RealizeLogin(nomeOperador, "");

			_disciplina = await POFactory.Crie<DisciplinaPO>(Context);
			await _disciplina.GotoAsync();
		}

		// QUANDO

		// QUANDO - Clico no botão de acordo com o cenário
		[When("clico em '([^']*)'")]
		[Scope(Tag = "@EditarNomeDisciplinasCadastrando")]
		[Scope(Tag = "@AlterarSituacaoDisciplinasCadastrando")]
		[Scope(Tag = "@CadastrarDisciplinasObrigatorio")]
		[Scope(Tag = "@ExcluirDisciplinasSemVinculosCadastrando")]
		[Scope(Tag = "@ExcluirDisciplinasComVinculosCadastrando")]
		public async Task WhenClicoEM(string botao)
		{
			await _disciplina.ClicoEm(botao);

		}

		// QUANDO - Confirmo a operação de exclusão
		[When("confirmo a operacao clicando em '([^']*)'"), Scope(Tag = "@ExcluirDisciplinasSemVinculosCadastrando")]
		public async Task WhenConfirmoAOperacao(string botao)
		{
			await _disciplina.ClicoEm(botao);
		}

		// QUANDO - Clico em confirmar a operação de exclusão
		[When("confirmo a operacao clicando em '([^']*)'"), Scope(Tag = "@ExcluirDisciplinasComVinculosCadastrando")]
		public async Task WhenConfirmoAOperacaoDeExclusaoDeDisciplinaComVinculo(string botaoOpcao)
		{
			await _disciplina.ClicoEm(botaoOpcao);
		}

		// QUANDO - Pesquiso pela disciplina que desejo localizar
		[When("pesquiso pela disciplina '([^']*)'"), Scope(Tag = "@PesquisarDisciplinasCadastrando")]
		public async Task WhenPesquisoPelaDisciplina(string descricaoDisciplina)
		{
			await _disciplina.PesquisaCadastrando(descricaoDisciplina);
		}

		// QUANDO - Altero o filtro de situação para a opção informada
		[When("altero o filtro de situacao para '([^']*)'"), Scope(Tag = "@FiltrarGridDisciplinasCadastrando")]
		public async Task WhenAlteroOFiltroDeSituacaoPara(string opcao)
		{
			await _disciplina!.GotoAsync();

			await _disciplina.AlterarFiltroGridCadastrando(opcao);
		}

		// ENTÃO

		// ENTÃO - É apresentada a mensagem de acordo com o cenário
		[Then("e apresentada a mensagem '([^']*)'")]
		[Scope(Tag = "@CadastrarDisciplinasObrigatorio")]
        [Scope(Tag = "@EditarNomeDisciplinasCadastrando")]
        [Scope(Tag = "@AlterarSituacaoDisciplinasCadastrando")]
		[Scope(Tag = "@ExcluirDisciplinasSemVinculosCadastrando")]
		[Scope(Tag = "@ExcluirDisciplinasComVinculosCadastrando")]
		public async Task ThenEApresentadaAMensagem(string mensagemApresentada)
		{
			await _disciplina.ValidarMensagem(mensagemApresentada);
		}


		// ENTÃO - É apresentada somente disciplinas que contenham o texto informado na pesquisa
		[Then("e apresentada somente disciplinas que contenham o texto informado na pesquisa"), Scope(Tag = "@PesquisarDisciplinasCadastrando")]
		public async Task ThenEApresentadaSomenteDisciplinasQueContenhamOTextoInformadoNaPesquisa()
		{
			await _disciplina.ValidarDisciplinasContemTextoPesquisado();

		}

		// ENTÃO - É apresentada somente disciplinas com a situação informada no filtro
		[Then("e apresentada somente disciplinas '([^']*)'"), Scope(Tag = "@FiltrarGridDisciplinasCadastrando")]
		public async Task ThenEApresentadaSomenteDisciplinas(string situacao)
		{
			await _disciplina.ValidarSomenteDisciplinasComSituacaoInativa(situacao);
		}

		// ENTÃO - Remove o operador utilizado para os testes
		[Then("removo o operador")]
		[Scope(Tag = "@CadastrarDisciplinasObrigatorio")]
		[Scope(Tag = "@EditarNomeDisciplinasCadastrando")]
		[Scope(Tag = "@AlterarSituacaoDisciplinasCadastrando")]
		[Scope(Tag = "@ExcluirDisciplinasSemVinculosCadastrando")]
		[Scope(Tag = "@ExcluirDisciplinasComVinculosCadastrando")]
		[Scope(Tag = "@PesquisarDisciplinasCadastrando")]
		[Scope(Tag = "@FiltrarGridDisciplinasCadastrando")]
		public async Task ThenRemovoOOperador()
		{
			await CrieContexto();
			await RealizeLoginAdmin();
			await RemoverOperador(nomeOperador);
		}
	}

}