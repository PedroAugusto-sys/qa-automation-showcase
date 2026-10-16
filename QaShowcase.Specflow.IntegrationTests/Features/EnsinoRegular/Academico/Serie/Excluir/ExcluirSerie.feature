#language: pt-br
Funcionalidade: Excluir serie

@Academico
@Serie
@SerieExcluir
Cenario: Exclusao de serie
	Dado que desejo excluir uma serie
	E pesquiso a serie que desejo excluir
	E clico em excluir a serie
	Quando clico em sim, para excluir a serie
	Entao serie eh excluida com sucesso