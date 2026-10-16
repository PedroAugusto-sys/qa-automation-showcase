Feature: Cadastro de Ficha do Aluno

@Academico
@FichaAluno
@CadastrarFichaAlunoCamposObrigatorios
Scenario: Cadastro de nova ficha de aluno
	Given que preciso cadastrar um novo aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And clico em 'Cadastrar'
	And preecho o Nome civil 'ALUNO TESTE QA'
	And seleciono o sexo 'Masculino'
	And insiro o nascimento '01/01/2000'
	And seleciono a naturalidade 'ABADIA DE GOIAS'
	And preencho o CEP '74375-230'
	And preencho o CPF '<GERAR_CPF>'
	And preencho os atributos adicionais obrigatorios
	When clico em 'Gravar'
	Then e apresentada a mensagem 'Cadastro realizado, deseja ir para a tela de matrícula?'
	Then removo o operador

@Academico
@FichaAluno
@CadastrarFichaAlunoSemCEP
Scenario: Cadastro de nova ficha de aluno sem informar CEP
	Given que preciso cadastrar um novo aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And clico em 'Cadastrar'
	And preecho o Nome civil 'ALUNO TESTE QA'
	And seleciono o sexo 'Masculino'
	And insiro o nascimento '01/01/2000'
	And seleciono a naturalidade 'ABADIA DE GOIAS'
	And preencho o CPF '<GERAR_CPF>'
	When clico em 'Gravar'
	Then os campos obrigatorios pendentes do CEP sao destacadados
	Then removo o operador

@Academico
@FichaAluno
@ValidarBotaoCancelarCadastro
Scenario: Cancelar a operação de cadastro
	Given que preciso cadastrar um novo aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And clico em 'Cadastrar'
	And clico em 'Cancelar'
	When clico em 'Sim' para confirmar a operacao
	Then e apresentada a tela de aterrissagem
	Then removo o operador

@Academico
@FichaAluno
@ValidarAnexoCadastro
Scenario: Anexar arquivo na ficha do aluno
	Given que preciso adiconar uma anexo a uma ficha do aluno
	And acesso o sistema utilizando o operador 'OPERADOR QA'
	And clico em anexo na ficha do aluno 'ALUNO TESTE QA'
	And insiro o titulo 'anexo teste'
	And seleciono um anexo
	When clico em 'Adicionar'
	Then o anexo e apresentado na grid de anexos
	Then removo o operador