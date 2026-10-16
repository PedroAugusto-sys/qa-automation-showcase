Feature: Clonar Planos de Pagamentos

@Financeiro
@PlanosDePagamentos
@ClonarPlanoDePagamentosCadastrandoComStatusAtivo
Scenario: Clonar Plano De Pagamentos Cadastrando Com Status Ativo
	Given que desejo clonar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And pesquiso pelo plano de pagamentos ativo 'plano de pagamento ativo'
	And clico em 'Clonar'
	And clico em 'Gravar'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@ClonarPlanoDePagamentosCadastrandoComStatusInativo
Scenario: Clonar Plano De Pagamentos Cadastrando Com Status Inativo
	Given que desejo clonar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And pesquiso pelo plano de pagamentos inativo 'plano de pagamento inativo'
	And clico em 'Clonar'
	And clico em 'Gravar'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	Then removo o operador