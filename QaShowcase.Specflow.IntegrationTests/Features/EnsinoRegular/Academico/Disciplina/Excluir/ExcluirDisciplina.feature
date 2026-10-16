Feature: Excluir Disciplina

@Academico
@Disciplinas
@ExcluirDisciplinasSemVinculosCadastrando
Scenario: Excluir uma disciplina
Given que desejo excluir uma disciplina
And acesso o sistema utilizando o operador 'TESTE QA'
And pesquiso pela disciplina 'DISCIPLINA TESTE QA'
And clico em 'Excluir'
When confirmo a operacao clicando em 'Sim'
Then e apresentada a mensagem 'Item excluido com sucesso.'
#Then removo o operador


##################### REFATORAR CENÁRIO QUANDO POSSUI AUTOMACAO DAS TELAS DE VINCULOS

@Academico
@Disciplinas
@ExcluirDisciplinasComVinculosCadastrando
Scenario: Excluir uma disciplina vinculada
Given que desejo excluir uma disciplina
And acesso o sistema utilizando o operador 'TESTE QA'
And pesquiso pela disciplina 'TRILHAS - APROF. EM FISICA' 
And clico em 'Excluir'
When confirmo a operacao clicando em 'Sim'
Then e apresentada a mensagem 'A disciplina nao pode ser excluida pois ja esta sendo utilizada por alguma das funcoes ou conceitos:'
#Then removo o operador