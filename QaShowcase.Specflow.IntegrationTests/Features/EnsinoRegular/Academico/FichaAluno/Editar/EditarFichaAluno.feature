Feature: Editar Ficha do Aluno

@Academico
@FichaAluno
@EditarFichaAlunoNomeCadastrando
Scenario: Editar o nome de um aluno
	Given que desejo editar um aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And clico em 'Editar'
	And altero o nome do aluno para 'ALUNO TESTE NOME ALTERADO'
	When pesquiso pelo aluno alterado
	Then o aluno e apresentado na grid
	Then removo o operador

@Academico
@FichaAluno
@EditarFichaAlunoCadastrandoERemovendoCEP
Scenario: Editar ficha do aluno cadastrando e removendo CEP
	Given que desejo editar um aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And clico em 'Editar'
	And removo os dados existente para o 'CEP'
	When clico em 'Gravar'
	Then os campos obrigatorios pendentes do 'CEP' sao destacadados
	Then removo o operador

@Academico
@FichaAluno
@ValidarBotaoCancelarEdicao
Scenario: Cancelar a operacao de edicao
	Given que desejo editar um aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And pesquiso pelo aluno 'ALUNO TESTE QA'
	And clico em 'Editar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then e apresentada a tela de aterrissagem
	Then removo o operador