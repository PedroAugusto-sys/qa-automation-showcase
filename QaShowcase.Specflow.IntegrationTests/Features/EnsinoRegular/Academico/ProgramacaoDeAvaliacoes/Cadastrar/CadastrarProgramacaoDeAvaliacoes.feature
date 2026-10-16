#language: pt-br
Funcionalidade: Cadastrar Programacao de avaliacoes

@Academico
@ProgramacaoDeAvaliacoes
@ProgramacaoDeAvaliacoesCadastrar
Cenario: Cadastrar programacao de avaliacoes
	Dado Que desejo cadastrar uma programacao de avaliacoes
	E clico em novo
	E seleciono a serie
	E seleciono a turma
	E informo a etapa
	E informo a tabela de avaliacao
	E informo a disciplina
	E clico em carregar
	E informo o professor
	Quando clico em gravar
	Entao a programacao eh realizada com sucesso