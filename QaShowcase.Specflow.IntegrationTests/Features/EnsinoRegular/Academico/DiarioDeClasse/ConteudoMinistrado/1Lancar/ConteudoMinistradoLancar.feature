#language: pt-br
Funcionalidade: : Lancamento de conteudo ministrado

@Academico
@DiarioDeClasse
Cenario: Lancar conteudo ministrado
    Dado que desejo realizar lancamento de conteudo ministrado
	E configuro os filtros do conteudo ministrado
	E clico em carregar, o conteudo ministrado
	E realizo o lancamento do conteudo ministrado
	Quando clico em gravar, o conteudo ministrado
	Entao a tela de conteudo ministrado apresenta a mensagem: "Conteudos ministrados gravados com sucesso!"

