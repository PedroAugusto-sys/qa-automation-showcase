Feature: Pesquisar

@Academico
@Turmas
@PesquisarTurmasCadastrando
Scenario: Pesquisar por uma turma
	Given que desejo pesquisar uma turma
	And acesso o sistema utilizando o operador 'TESTE QA'
	When pesquiso pela turma 'TTTTTTTTT'
	Then e apresentada somente as turmas que contenham no nome o termo informado
	Then removo o operador