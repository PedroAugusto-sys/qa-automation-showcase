Feature: Excluir Tabela de Avaliacao

@Academico
@Avaliacoes
@ExcluirTabelasDeAvaliacaoCadastrando
Scenario: Excluir uma tabela de avaliacao cadastrando
	Given que desejo excluir uma tabela de avaliacao
	And acesso o sistema utilizando o operador 'TESTE QA'
	And pesquiso a tabela 'TABELA TESTE QA'
	And clico em 'Remover'
	When clico em 'Sim'
	Then e trago a mensagem 'Item excluído com sucesso.'
	Then removo o operador


# Necessário estar finalizada a automação de Programação de Avaliações para Professores para que este cenário funcione corretamente.
@Academico
@Avaliacoes
@ExcluirTabelasDeAvaliacaoEmUsoCadastrando
Scenario: Excluir de uma tabela de avaliacao em uso
	Given que desejo excluir uma tabela de avaliacao em uso
	And acesso o sistema utilizando o operador 'TESTE QA'
	And pesquiso a tabela 'tabela teste'
	When clico em 'Remover'
	Then e trago a mensagem 'Exclusão não permitida pois há registros de distribuição inserido no sistema.'
	Then removo o operador