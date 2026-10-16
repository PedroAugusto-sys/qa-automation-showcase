Feature: Excluir

@Academico
@Turmas
@ExcluirTurmaSemUsoCadastrando
Scenario: Excluir uma turma
	Given que desejo excluir uma turma
	And acesso o sistema utilizando o operador 'TESTE QA'
	And pesquiso pela turma 'MARISA'
	And clico no botao 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluido com sucesso'
	Then removo o operador

#Cenário Mocado (Ajustar novamente quando automação do portal do professor for automatizado ou cadastro de séries)
@Academico
@Turmas
@ExcluirTurmaEmUsoCadastrando
Scenario: Excluir Turma Em Uso Cadastrando
	Given que desejo excluir uma turma
	And acesso o sistema utilizando o operador 'TESTE QA'
	And pesquiso pela turma 'EF2VB_4_1_4_19'
	And clico no botao 'Excluir'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Turmas: Nao e possivel excluir a turma, pois ha um coordenador do portal do professor associado a ela.Turmas: Nao e possivel excluir a turma, pois ha vinculos com series.'
	Then removo o operador

