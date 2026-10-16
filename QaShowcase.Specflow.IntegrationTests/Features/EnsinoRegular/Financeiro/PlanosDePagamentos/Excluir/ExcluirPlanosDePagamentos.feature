Feature: Exclir Planos de Pagamentos

@Financeiro
@PlanosDePagamentos
@ExcluirPlanosDePagamentosCadastrandoDoTipoDeContaContabilBancaria
Scenario: Excluir Planos De Pagamentos Cadastrando Do Tipo De Conta Contabil Bancaria
	Given que desejo excluir planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Remover' no plano de pagamentos
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@ExcluirPlanosDePagamentosCadastrandoDoTipoDeContaContabilTesouraria
Scenario: Excluir Planos De Pagamentos Cadastrando Do Tipo De Conta Contabil Tesouraria
	Given que desejo excluir planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Remover' no plano de pagamentos
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@ExcluirPlanosDePagamentosCadastrandoComStatusInativo
Scenario: Excluir Planos De Pagamentos Cadastrando Com Status Inativo
	Given que desejo excluir planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And pesquiso pelo plano de pagamentos 'plano de pagamento inativo'
	And clico em 'Remover' no plano de pagamentos
	When clico em 'Sim'
	Then e apresentada a mensagem 'Item excluído com sucesso.'
	Then removo o operador

	#Essa feature será produzida após a automação de geração de duplicatas ser produzida
@Financeiro
@PlanosDePagamentos
@ExcluirPlanosDePagamentosQuePossuiVinculoComMatriculas
Scenario: Excluir Planos De Pagamentos Que Possui Vinculo Com Matriculas
	Given que desejo excluir planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And cadastro o plano de pagamentos 'plano de pagamento vinculado'
	And gero duplicatas para o aluno 'Aluno qa' utlizando o plano de pagamentos
	And pesquiso pelo plano de pagamentos 'plano de pagamento vinculado'
	And clico em 'Remover' no plano de pagamentos
	When clico em 'Sim'
	Then e apresentada a mensagem 'Não foi possível excuir, pois existem matrículas vinculada a este plano de pagamento.'
	#Then removo o operador