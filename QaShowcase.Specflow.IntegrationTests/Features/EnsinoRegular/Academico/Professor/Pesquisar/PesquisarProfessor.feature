Feature: Pesquisar professor

@Academico
@Professor
@PesquisarProfessorNomeCadastrando
Scenario: Pesquisar professor
	Given que preciso pesquisar um professor
	And acesso o sistema utilizando o operador 'TESTE JADER'
	When pesquiso pelo professor 'PROFESSOR TESTE JADER'
	Then é apresentado somente professores que contenham em seu nome 'PROFESSOR TESTE JADER'
    #Then removo o operador
