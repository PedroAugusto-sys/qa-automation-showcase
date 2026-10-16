Feature: Cadastrar Professor

@Academico
@Professor
@CadastrarProfessorCamposObrigatorios
Scenario: Cadastro de novo professor
	Given que desejo cadastrar um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE JADER'
	And clico em 'Cadastrar'
	And preencho o nome 'PROFESSOR TESTE AUTOMATIZADO'
	And seleciono o sexo 'M'
	And insiro o nascimento '07/07/1990'
	And seleciono a naturalidade 'BR'
	And seleciono uma formacao 'Ensino Superior (Completo)'
	And informo o CPF '<GERAR_CPF>'
	And seleciono o estado civil 'casado'
	And preencho o CEP '74313380'
	And informo o atributo adicional obrigatorio
	When clico em 'gravar'
	Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
	#Then removo o operador


@Academico
@Professor
@CadastrarProfessorSemFormacao
Scenario: Cadastro de novo professor sem informar a Formacao
	Given que desejo cadastrar um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE JADER'
	And clico em 'Cadastrar'
	And preencho o nome 'PROFESSOR TESTE AUTOMATIZADO'
	And seleciono o sexo 'F'
	And insiro o nascimento '07/07/1990'
	And seleciono a naturalidade 'BR'
	And informo o CPF '<GERAR_CPF>'
	And seleciono o estado civil 'casado'
	And preencho o CEP '74313380'
	And informo o atributo adicional obrigatorio
	When clico em 'gravar'
	Then o botao 'Trabalho' é destacado em vermelho
	#Then removo o operador

@Academico
@Professor
@ValidarBotaoCancelarCadastro
Scenario: Cancelar a operação de cadastro
	Given que desejo cadastrar um professor no sistema
	And acesso o sistema utilizando o operador 'TESTE JADER'
	And clico em 'Cadastrar'
	And clico em 'Cancelar'
	When clico em 'Sim'
	Then é apresentada a tela de aterrizagem
	#Then removo o operador

@Academico
@Professor
@ValidarAnexoCadastroProfessor
Scenario: Anexar arquivo na professor
	Given que preciso adiconar uma anexo a uma professor
	And acesso o sistema utilizando o operador 'TESTE JADER'
	And pesquiso pelo professor 'PROFESSOR TESTE JADER'
	And clico em 'Ver anexos'
	And adiciono um anexo do tipo 'pdf'
	And informo o titulo do anexo 'Anexo do tipo PDF'
	When clico em 'Adicionar'
	Then ele é apresentado na grid de anexos
	#Then removo o operador

