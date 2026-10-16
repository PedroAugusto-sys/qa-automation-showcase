#language: pt-br
Funcionalidade: Realizar manutenção do boletim coletivo 

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Cenario: Validar lancamento de notas
Dado que acesso a tela de Manutencao de Boletim Coletivo
E seleciono a Serie
E seleciono a Turma
E seleciono o Ano
E seleciono a Disciplina
E clico em Carregar
E realizo o lancamento de notas
E clico em Gravar
Quando clico em Sim
Entao o boletim e gravado com sucesso

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Cenario: Validar lancamento de faltas
Dado que acesso a tela de Manutencao de Boletim Coletivo
E seleciono a Serie
E seleciono a Turma
E seleciono o Ano
E seleciono a Disciplina
E clico em Carregar
E realizo o lancamento de faltas
E clico em Gravar
Quando clico em Sim
Entao o boletim e gravado com sucesso

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Cenario: Validar a funcionalidade Recalcular
Dado que acesso a tela de Manutencao de Boletim Coletivo
E seleciono a Serie
E seleciono a Turma
E seleciono o Ano
E seleciono a Disciplina
E clico em Carregar
E realizo o lancamento de notas
E realizo o lancamento de faltas
Quando clico em Recalcular
Entao o recalculo do boletim e realizado

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Cenario: Validar atalho Emitir
Dado que acesso a tela de Manutencao de Boletim Coletivo
Quando clico em Emitir
Entao a tela de Emissao do Boletim Escolar e carregada

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Cenario: Validar alerta ao clicar em Boletim individual sem selecionar aluno
Dado que acesso a tela de Manutencao de Boletim Coletivo
Quando clico em Boletim individual
Entao e apresentado um alerta

@Academico
@ManutencaoBoletimColetivo
@ManutencaoBoletimColetivoLancarNotas
Cenario: Validar atalho Boletim individual selecionando um aluno
Dado que acesso a tela de Manutencao de Boletim Coletivo
E seleciono a Serie
E seleciono a Turma
E seleciono o Ano
E seleciono a Disciplina
E clico em Carregar
E seleciono um Aluno
Quando clico em Boletim individual
Entao a tela de Manutencao de Boletim Individual e carregada