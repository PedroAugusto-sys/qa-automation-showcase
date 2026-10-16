Feature: Editar uma turma

@Academico
@Turmas
@EditarTurmasCadastrando
Scenario: Editar uma turma
	Given que desejo editar um turma
	And acesso o sistema utilizando o operador 'TESTE QA'
	And pesquiso pela turma 'TURMA TESTE'
	And clico no botao 'Editar'
	And altero a descricao para 'TESTE AAAA'
	When clico em 'Editar'
	Then e apresentada a mensagem 'Registro editado com sucesso!'
	Then removo o operador

