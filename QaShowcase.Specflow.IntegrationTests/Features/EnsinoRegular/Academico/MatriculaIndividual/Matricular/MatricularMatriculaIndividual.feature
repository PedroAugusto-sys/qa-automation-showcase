#language: pt-br
Funcionalidade: Matricula Individual

@Academico
@MatriculaIndividual
Cenario: Matricular um aluno sem financeiro
	Dado que desejo matricular um aluno
	E seleciono um aluno que nao possui matricula
	E seleciono uma serie para matricular
	E marco o aluno como proprio responsavel
	Quando clico em gravar matricula
	Entao a matricula e realizada

@Academico
@MatriculaIndividual
Cenario: Matricular um aluno com financeiro
	Dado que desejo matricular um aluno gerando financeiro
	E seleciono um aluno que nao possui matricula ativa
	E seleciono uma serie para matricular
	E marco o aluno como proprio responsavel
	E preencho os dados das duplicatas
	Quando clico em gravar matricula
	Entao a matricula e realizada

@Academico
@MatriculaIndividual
Cenario: Matricular um aluno com financeiro por plano de pagamento
	Dado que desejo matricular um aluno com financeiro por plano de pagamento
	E seleciono um aluno que nao possui matricula no ano atual
	E seleciono uma serie para matricular
	E marco o aluno como proprio responsavel
	E seleciono um plano de pagamento
	Quando clico em gravar matricula
	Entao a matricula e realizada
