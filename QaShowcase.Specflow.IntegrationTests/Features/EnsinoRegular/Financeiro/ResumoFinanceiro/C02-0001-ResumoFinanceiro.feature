#language: pt-br
Funcionalidade: Resumo Financeiro xpto

Casos de uso relacionado ao resumo financeiro

@Financeiro
@ResumoFinanceiro
Cenário: Filtros do Resumo Financeiro
	Dado que preciso validar os filtros do resumo financeiro de um aluno
	E seleciono o aluno
	E clico em Carregar
	E seleciono uma finalidade
	Quando seleciono os filtros
	Então o sistema deve listar as informações de acordo com os filtros

@Financeiro
@ResumoFinanceiro
Cenário: Filtros do Resumo Financeiro sem finalidade
	Dado que preciso validar os filtros do resumo financeiro de um aluno
	E seleciono o aluno
	E clico em Carregar
	Quando seleciono os filtros
	Então o sistema deve listar as informações de acordo com os filtros