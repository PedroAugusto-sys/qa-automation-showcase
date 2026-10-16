#language: pt-br
Funcionalidade: Editar serie

@Academico
@Serie
@SerieEditar
Cenario: Cadastro de serie
	Dado que desejo editar o cadastro uma serie
	E clico em editar a serie
	E edito a descricao da serie
	Quando clico em gravar o cadastro da serie
	Entao serie eh editada com sucesso