Feature: Excluir professor

@Academico
@Professor
@ExcluirProfessorSemVinculoCadastrando
Scenario: Excluir professor
	Given que preciso excluir um professor
	And acesso o sistema utilizando o operador 'TESTE JADER'
	And pesquiso pelo professor 'Professor Teste Jader'
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
    #Then removo o operador


###################################################### CENARIO DEPENDE DA PRODUCAO DA AUTOMACAO DE HORARIOS ESCOLARES
@Academico
@Professor
@ExcluirProfessorComVinculoCadastrando
Scenario: Excluir professor com vinculo em histórico escolares
	Given que preciso excluir um professor
	And acesso o sistema utilizando o operador 'TESTE JADER'
	And pesquiso pelo professor 'Professor Teste Jader'
	And clico em 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Horários escolares: O professor possui vínculo com os horários escolares.'
    #Then removo o operador