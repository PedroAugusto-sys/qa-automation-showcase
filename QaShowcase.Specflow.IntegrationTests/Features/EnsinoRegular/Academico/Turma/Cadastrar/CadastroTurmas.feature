Feature: Cadastrar uma turma

@Academico
@Turmas
@CadastrarTurmas
Scenario: Cadastrar uma turma
	Given que desejo cadastrar uma turma
	And acesso o sistema utilizando o operador 'TESTE QA'
	And clico para gerar um codigo
	And informo a descricao 'TURMA TESTE'
	When clico em 'Cadastrar'
	Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
	Then removo o operador


