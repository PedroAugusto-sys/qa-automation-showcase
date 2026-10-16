#language: pt-br
Funcionalidade: Cadastrar Montagem de Turma

@Academico
@MontagemDeTurma
@MontagemDeTurmaCadastrar
Cenario: Cadastrar montagem de turma
	Dado Que desejo cadastrar uma montagem de turma
	E clico em cadastrar
	E seleciono a serie
	E seleciono a turma
	E informo o turno
	E informo a lotacao
	Quando clico em gravar
	Entao o cadastro eh realizado com sucesso