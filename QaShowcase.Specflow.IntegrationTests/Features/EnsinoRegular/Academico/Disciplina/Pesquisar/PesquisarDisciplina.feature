Feature: Pesquisar Disciplina

@Academico
@Disciplinas
@PesquisarDisciplinasCadastrando
Scenario: Pesquisar por uma disciplina
Given que desejo localizar uma disciplina
And acesso o sistema utilizando o operador 'TESTE QA'
When pesquiso pela disciplina 'DISCIPLINA TESTE QA'
Then e apresentada somente disciplinas que contenham o texto informado na pesquisa
Then removo o operador

@Academico
@Disciplinas
@FiltrarGridDisciplinasCadastrando
Scenario: Filtrar apresentação de uma disciplina 
Given que desejo localizar uma disciplina
And acesso o sistema utilizando o operador 'TESTE QA'
When altero o filtro de situacao para 'Inativas'
Then e apresentada somente disciplinas 'Inativas'
Then removo o operador