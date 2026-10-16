Feature: : Editar Professor

@Academico
@Professor
@EditarProfessorNomeCadastrando
Scenario: Editar o nome de um professor
	Given que desejo editar o nome de um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE JADER'
	And pesquiso pelo professor 'PROFESSOR TESTE'
	And clico em 'editar'
	And altero o 'PROFESSOR TESTE' do professor para 'PROFESSOR TESTE ALTERADO'
 	When clico em 'gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador

@Academico
@Professor
@EditarProfessorCadastrandoERemovendoCEP
Scenario: Editar o nome de um professor removendo dados obrigatorios
	Given que desejo editar o cep de um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE JADER'
	And pesquiso pelo professor 'PROFESSOR TESTE'
	And clico em editar
	And removo os dados existente para o 'CEP'
 	When clico em gravar
	Then o campo 'CEP' é destacado em vermelho
	#Then removo o operador

@Academico
@Professor
@ValidarBotãoCancelarEdicao
Scenario: Cancelar a operação de edicao
	Given que preciso editar um professor
	And acesso o sistema utilizando o operador 'TESTE JADER'
	And clico em 'Cadastrar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then é apresentada a tela de aterrizagem
    #Then removo o operador