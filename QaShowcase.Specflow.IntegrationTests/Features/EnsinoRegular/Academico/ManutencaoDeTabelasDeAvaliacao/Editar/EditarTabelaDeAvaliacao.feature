Feature: Editar Tabela de Avaliacao

@Academico
@Avaliacoes
@EditarTabelasDeAvaliacaoCadastrando
Scenario: Editar uma tabela de avaliacao
	Given que desejo editar uma tabela de avaliacao cadastrando
	And acesso o sistema utilizando o operador 'TESTE QA'
	And pesquiso a tabela 'tabela teste'
	And clico em 'Editar'
	And altero o nome para 'TABELA AVALIACAO TESTE ALTERADA'
	When clico em 'Gravar'
	Then e trago a mensagem 'Cadastro realizado com sucesso!'
	Then removo o operador

@Academico
@Avaliacoes
@EditarStatusTabelasDeAvaliacaoCadastrando
Scenario: Inativar uma tabela de Avaliação Cadastrando
	Given que desejo editar uma tabela de avaliacao cadastrando
	And acesso o sistema utilizando o operador 'TESTE QA'
	And pesquiso a tabela 'tabela teste'
	And clico em 'Editar'
	And altero a opcao de tabela ativa para 'Não'
	And clico em 'Gravar'
	When e trago a mensagem 'Cadastro realizado com sucesso!'
	Then valido a alteracao do status da tabela filtrando
	Then removo o operador