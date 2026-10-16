Feature: Cadastrar Tabela de Avaliacao

@Academico
@Avaliacoes
@CadastrarTabelasDeAvaliacao
Scenario: Cadastrar uma tabela de avaliacao
	Given que desejo cadastrar uma tabela de avaliacao
	And acesso o sistema utilizando o operador 'TESTE AUTOMACAO ELETRON'
	And clico em 'Cadastrar'
	And altero o tipo de tabela de avaliacao para 'Normal'
	And preencho com o nome de 'TABELA AVALIACAO TESTE'
	And altero a opcao de tabela ativa para 'Sim'
	And altero a tabela padrao para 'Não'
	And altero a opcao calcular avaliacoes analiticas para 'Não'
	And altero a opcao template portal de professor para 'Não'
	And clico em 'Avaliação'
	And insiro o tipo de avaliacao 'Normal'
	And insiro a descricao resumida 'N'
	And insiro a descricao 'Descrição N'
	And insiro o valor maximo '100'
	And clico em 'Confirmar'
	And repito a adicao de avaliacoes 'Normal' mais '2' vezes
	And altero a opcao formula aceita nulo para 'Sim'
	And clico em 'Editar fórmula'
	And insiro a formula '(N1+N2+N3)/4'
	And clico em 'Confirmar'
	When clico em 'Gravar'
	Then e trago a mensagem 'Cadastro realizado com sucesso!'
	Then removo o operador

@Academico
@Avaliacoes
@CadastrarTabelasDeAvaliacaoPortalDoProfessor
Scenario: Cadastro de uma tabela de avaliacao com template no portal de professsor
	Given que desejo cadastrar uma tabela de avaliacao
	And acesso o sistema utilizando o operador 'TESTE QA'
	And clico em 'Cadastrar'
	And altero o tipo de tabela de avaliacao para 'Normal'
	And preencho com o nome de 'TABELA AVALIACAO TESTE'
	And altero a opcao de tabela ativa para 'Sim'
	And altero a opcao calcular avaliacoes analiticas para 'Não'
	And altero a opcao template portal de professor para 'Sim'
	And insiro o valor SAV '10'
	And clico em 'Avaliação'
	And insiro o tipo de avaliacao 'Normal'
	And insiro a descricao resumida 'N'
	And insiro a descricao 'Descricao N'
	And insiro o valor maximo '10'
	And clico em 'Confirmar'
	And repito a adicao de avaliacoes 'Normal' mais '2' vezes
	And altero a opcao formula aceita nulo para 'Sim'
	And clico em 'Editar fórmula'
	And insiro a formula '(N1+N2+N3)/4'
	And clico em 'Confirmar'
	When clico em 'Gravar'
	Then e trago a mensagem 'Cadastro realizado com sucesso!'
	Then removo o operador