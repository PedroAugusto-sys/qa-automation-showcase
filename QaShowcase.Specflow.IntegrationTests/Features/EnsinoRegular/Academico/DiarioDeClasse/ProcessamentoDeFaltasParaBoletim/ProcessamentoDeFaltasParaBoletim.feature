#language: pt-br
Funcionalidade: : Processar faltas para o boletim

@Academico
@DiarioDeClasse
Cenario: Processar faltas para o boletim
    Dado que desejo realizar o processamento de faltas para o boletim
	E configuro os filtros do processamento de faltas para o boletim
	Quando clico em processar, as faltas para o boletim
	Entao o sistema apresenta a mensagem: "Processamento de faltas concluído com sucesso."


@Academico
@DiarioDeClasse
Cenario: Processar faltas para o boletim por serie
    Dado que desejo realizar o processamento de faltas para o boletim
	E seleciono a serie para realizar o processamento de faltas
	Quando clico em processar, as faltas para o boletim
	Entao o sistema apresenta a mensagem: "Processamento de faltas concluído com sucesso."