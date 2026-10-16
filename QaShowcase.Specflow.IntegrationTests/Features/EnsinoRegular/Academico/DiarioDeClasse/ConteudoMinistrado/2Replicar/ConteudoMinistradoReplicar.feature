#language: pt-br
Funcionalidade: : Replicar conteudo ministrado

@Academico
@DiarioDeClasse
Cenario: Replicar conteudo ministrado
    Dado que desejo realizar lancamento de conteudo ministrado
	E configuro os filtros do conteudo ministrado
	E clico em carregar, o conteudo ministrado
	E seleciono para replicar o conteudo ministrado
	E realizo o lancamento do conteudo ministrado replicado
	Quando clico em gravar, o conteudo ministrado
	Entao o conteudo ministrado eh replicado com sucesso