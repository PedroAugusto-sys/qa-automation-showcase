#language: pt-br
Funcionalidade: Matricula Individual

@Academico
@MatriculaIndividual
Cenario: Cancelar uma matrícula ativa
	Dado que desejo cancelar a matricula de um aluno
	E seleciono um aluno com a matrícula ativa
	E clico para cancelar a matricula
	E confirmo as consequencias
	E insiro uma data de cancelamento
	E confirmo o cancelamento
	E marco sim para cancelar
	Quando clico em voltar
	Entao noto que a matricula esta cancelada
