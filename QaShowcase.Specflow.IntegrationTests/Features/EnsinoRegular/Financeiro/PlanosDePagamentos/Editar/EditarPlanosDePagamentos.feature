Feature: Editar Planos de Pagamentos

@Financeiro
@PlanosDePagamentos
@EditarPlanosDePagamentosCadastrandoAlterandoFinalidadeETipoDeContaContabil
Scenario: Editar Planos De Pagamentos Cadastrando Alterando Finalidade E Tipo De Conta Contabil
	Given que desejo editar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Editar' no plano de pagamentos
	And na grid Finalidade clico em 'Editar'
	And altero a Finalidade para 'MATRÍCULA'
	And seleciono o Tipo de conta contabil 'Tesouraria'
	And seleciono a Conta contabil 'MATRICULA - UNID - T9'
	And clico em 'Confirmar'
	And clico em 'Gravar'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@EditarPlanosDePagamentosCadastrandoAlterandoAnoReferenciaEDescricao
Scenario: Editar Planos De Pagamentos Cadastrando Alterando Ano Referencia E Descricao
	Given que desejo editar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Editar' no plano de pagamentos
	And altero o ano de referencia para '2024'
	And altero a descricao para 'plano de pagamento editado'
	And clico em 'Gravar'
	When clico em 'Sim'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@EditarPlanosDePagamentosAlterandoStatus
Scenario: Editar Planos De Pagamentos Alterando Status
	Given que desejo editar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa'
	And clico em 'Editar' no plano de pagamentos
	And altero o campo Ativo para 'Não'
	And clico em 'Gravar'
	And clico em 'Sim'
	And e apresentada a mensagem 'Cadastro realizado com sucesso!'
	When pesquiso pelo plano de pagamentos 'plano de pagamento pesquisa' com status alterado
	Then e apresentado o plano de pagamento com status 'Inativas'
	#Then removo o operador