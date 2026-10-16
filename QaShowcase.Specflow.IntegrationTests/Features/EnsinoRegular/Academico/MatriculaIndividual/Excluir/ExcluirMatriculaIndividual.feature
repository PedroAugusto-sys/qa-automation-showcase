#language: pt-br
Funcionalidade: Matricula Individual

@Academico
@MatriculaIndividual
Cenario: Excluir uma matrícula
	Dado que acesso a tela de matricula
	E seleciono um aluno
	E clico para excluir a matricula
	Quando confirmo a exclusão da matrícula
	Entao a matricula e excluida com sucesso

@Academico
@MatriculaIndividual
Cenario: Excluir uma matrícula com vínculos
	Dado que acesso a tela de matricula
	E seleciono um aluno
	E clico para excluir a matricula
	Quando confirmo a exclusão da matrícula
	Entao e apresentada a mensagem que a matricula nao pode ser excluida
