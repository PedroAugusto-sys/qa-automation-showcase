Feature: Editar Area de Conhecimento Disciplinas

@Academico
@AreaConhecimentoDisciplinas
@EditarAreaConhecimentoCadastrando
Scenario: Editar Area de Conhecimento
Given que desejo editar uma area de conhecimento
And acesso o sistema utilizando o operador 'OPERADOR QA'
And pesquiso pela area de conhecimento 'area de conhecimento teste qa'
And clico no botao 'Icone Editar'
When clico em 'Editar' para confirmar a operacao
Then e apresentada a mensagem 'Registro editado com sucesso!'
Then removo o operador