Feature: Cadastrar Planos de Pagamentos

@Financeiro
@PlanosDePagamentos
@CadastrarPlanoDePagamentosInformandoTipoDeContaContabilBancaria
Scenario: Cadastrar Plano De Pagamentos Informando Tipo De Conta Contabil Bancaria
	Given que desejo cadastrar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And clico em 'Cadastrar'
	And informo a descricao 'Plano de pagamentos teste automatizado'
	And clico em Ativo 'Sim'
	And clico em 'Adicionar'
	And seleciono a Finalidade 'MENSALIDADE'
	And insiro o valor mensal '1000'
	And insiro as Parcelas '10'
	And seleciono a Data Primeira parcela 'Data Atual'
	And insiro a Qtde dias a mais '1'
	And seleciono Outros vencimentos 'Fixo, todo dia' Dia '1'
	And seleciono o Pagamento 'Manter o vencimento'
	And seleciono Conta contábil de conf 'Não'
	And seleciono o Tipo de conta contabil 'Bancária'
	And seleciono a Informacao bancaria '1 - BRADESCO PRINCIPAL - T9'
	And seleciono a Conta contabil 'MENSALIDADES - UNID - T9 [1 - BRADESCO PRINCIPAL - T9]'
	And clico em 'Confirmar'
	And seleciono a Serie '1º ANO [Ensino Fundamental]'
	And seleciono a Turma 'Todos'
	And clico em 'Associar'
	And valido a simulacao das '10' duplicatas de valor '1000,00'
	And clico em 'Gravar'
	And clico em 'Sim'
	When e apresentada a mensagem 'Cadastro realizado com sucesso!'
	Then clico em 'Voltar para a lista'
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@CadastrarPlanoDePagamentosInformandoTipoDeContaContabilTesouraria
Scenario: Cadastrar Plano De Pagamentos Informando Tipo De Conta Contabil Tesouraria
	Given que desejo cadastrar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And clico em 'Cadastrar'
	And informo a descricao 'Plano de pagamentos teste automatizado'
	And clico em Ativo 'Sim'
	And clico em 'Adicionar'
	And seleciono a Finalidade 'MENSALIDADE'
	And insiro o valor mensal '1000'
	And insiro as Parcelas '10'
	And seleciono a Data Primeira parcela 'Data Atual'
	And insiro a Qtde dias a mais '1'
	And seleciono Outros vencimentos 'Fixo, todo dia' Dia '1'
	And seleciono o Pagamento 'Manter o vencimento'
	And seleciono Conta contábil de conf 'Não'
	And seleciono o Tipo de conta contabil 'Tesouraria'
	And seleciono a Conta contabil 'MENSALIDADES - UNID - T9'
	And clico em 'Confirmar'
	And seleciono a Serie '1º ANO [Ensino Fundamental]'
	And seleciono a Turma 'Todos'
	And clico em 'Associar'
	And valido a simulacao das '10' duplicatas de valor '1000'
	And clico em 'Gravar'
	And clico em 'Sim'
	When e apresentada a mensagem 'Cadastro realizado com sucesso!'
	Then clico em 'Voltar'
	Then removo o operador

@Financeiro
@PlanosDePagamentos
@CancelarCadastroDePlanoDePagamentos
Scenario: Cancelar Cadastro De Plano De Pagamentos
	Given que desejo cadastrar planos de pagamentos
	And seleciono o operador 'OPERADOR QA'
	And clico em 'Cadastrar'
	And clico em 'Gravar'
	And verifico a validacao de campo 'Descrição'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then e apresentada a tela de aterrissagem
	Then removo o operador