Feature: : Pesquisar Profissional


@Academico
@OutrosProfissionais
@PesquisarOutrosProfissionaisCadastrando
Scenario: Pesquisar por um Profissional

Given que desejo localizar um profissional
And acesso o sistema utilizando o operador 'nome_operador'
When pesquiso pelo profissional 'nome'
Then e apresentada somente profissionais que contenham o texto informado na pesquisa
Then removo o operador



@Academico
@OutrosProfissionais
@FiltrarGridOutrosProfissionaisSemVinculosCadastrando
Scenario: Filtrar apresentacao de um profissional
Given que desejo localizar um profissional
And acesso o sistema utilizando o operador 'nome_operador'
When altero o filtro de situacao para 'inativo'
Then e apresentada somente profissionais 'inativo'
Then removo o operador