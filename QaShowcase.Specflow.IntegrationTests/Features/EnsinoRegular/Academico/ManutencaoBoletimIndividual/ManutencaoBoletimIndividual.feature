#language: pt-br
Funcionalidade: Realizar manutenção do boletim individual

@Academico
@ManutencaoBoletimIndividual
@ManutencaoBoletimIndividualLancarNotas
Cenario: Validar lancamento de notas
Dado que preciso realizar o lancamento de notas na manutencao do boletim individual
E seleciono o ano letivo
E seleciono o aluno
E clico em Carregar
E realizo o lancamento de notas
E clico em Gravar
Quando clico em Sim
Entao o boletim e gravado com sucesso

@Academico
@ManutencaoBoletimIndividual
@ManutencaoBoletimIndividualLancarFaltas
Cenario: Validar lancamento de faltas
Dado que preciso realizar o lancamento de faltas na manutencao do boletim individual
E seleciono o ano letivo
E seleciono o aluno
E clico em Carregar
E realizo o lancamento de faltas
E clico em Gravar
Quando clico em Sim
Entao o boletim e gravado com sucesso

@Academico
@ManutencaoBoletimIndividual
@ManutencaoBoletimIndividualEmitirBoletim
Cenario: Validar a emissao do boletim individual
Dado que preciso realizar a emissao do boletim individual
E seleciono o ano letivo
E seleciono o aluno
E clico em Carregar
E clico em Emitir
Quando clico em Emitir novamente
Entao o boletim e emitido com sucesso

@Academico
@ManutencaoBoletimIndividual
@ManutencaoBoletimIndividualRecalcularBoletim
Cenario: Validar o recalculo do boletim
Dado que preciso recalcular as nota e faltas do boletim
E seleciono o ano letivo
E seleciono o aluno
E clico em Carregar
E realizo o lancamento de notas
E realizo o lancamento de faltas
Quando clico em Recalcular
Entao o recalculo do boletim e realizado