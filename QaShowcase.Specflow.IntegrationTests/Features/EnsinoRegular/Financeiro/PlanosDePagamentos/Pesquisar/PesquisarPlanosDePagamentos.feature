Feature: Pesquisar Planos de Pagamentos

@Financeiro
@PlanosDePagamentos
@PesquisarPlanosDePagamentosCadastrandoAtivo
Scenario: Pesquisar Planos De Pagamentos Cadastrando Ativo
	Given que desejo pesquisar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	When pesquiso pelo plano de pagamentos ativo 'plano de pagamento pesquisa'
	Then e apresentado o plano de pagamentos pesquisado
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@PesquisarPlanosDePagamentosCadastrandoInativo
Scenario: Pesquisar Planos De Pagamentos Cadastrando Inativo
	Given que desejo pesquisar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	When pesquiso pelo plano de pagamentos inativo 'plano de pagamento pesquisa'
	Then e apresentado o plano de pagamentos pesquisado
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@PesquisarPlanosDePagamentosCadastrandoClonado
Scenario: Pesquisar Planos De Pagamentos Cadastrando Clonado
	Given que desejo pesquisar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	When pesquiso pelo plano de pagamentos clonado 'plano de pagamento clonado'
	Then e apresentado o plano de pagamentos pesquisado
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@PesquisarPlanosDePagamentosCadastrandoEmAnoDiferenteDoSelecionado
Scenario: Pesquisar Planos De Pagamentos Cadastrando Em Ano Diferente Do Selecionado
	Given que desejo pesquisar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	When pesquiso pelo plano de pagamentos de ano diferente 'plano de pagamento ano diferente'
	Then nao e apresentado o plano de pagamentos pesquisado
	Then removo o operador