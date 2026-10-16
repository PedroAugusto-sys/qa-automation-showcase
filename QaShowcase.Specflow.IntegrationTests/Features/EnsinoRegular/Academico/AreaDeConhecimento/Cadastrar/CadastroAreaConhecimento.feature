Feature: Area de Conhecimento Disciplinas

@Academico
@AreaConhecimentoDisciplinas
@CadastrarAreaConhecimento
Scenario: Cadastrar Area de Conhecimento
Given que desejo cadastrar uma area de conhecimento
And acesso o sistema utilizando o operador 'OPERADOR QA'
And informo a descricao 'area de conhecimento teste qa'
When clico em 'Cadastrar'
Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
Then removo o operador