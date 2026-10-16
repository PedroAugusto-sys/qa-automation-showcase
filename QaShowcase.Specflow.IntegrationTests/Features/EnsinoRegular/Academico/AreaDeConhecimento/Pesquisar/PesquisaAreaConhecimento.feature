Feature: Pesquisar Area de Conhecimento Disciplinas

@Academico
@AreaConhecimentoDisciplinas
@PesquisarAreaConhecimentoCadastrando
Scenario: Pesquisar Area de Conhecimento
Given que desejo pesquisar uma area de conhecimento
And acesso o sistema utilizando o operador 'OPERADOR QA'
When pesquiso pela area de conhecimento 'area de conhecimento teste qaaa'
Then e apresentada somente as areas de conhecimentos que contenham no nome o termo informado
Then removo o operador