Feature: Excluir Usuario

@Logins
@ExcluirUsuario
@ExcluirUsuarioCadastrado
Scenario: Excluir usuario cadastrado
Given que desejo excluir usuario
And pesquiso pelo 'OPERADOR QA1'
And clico em Remover
When seleciono a opcao 'Sim'
Then e apresentada a mensagem de exclusao 'Item excluído com sucesso.' 

@Logins
@ExcluirUsuario
@ExcluirUsuarioCadastrando
Scenario: Excluir usuario
Given que desejo excluir usuario
And seleciono o operador
And clico em Remover
When seleciono a opcao 'Sim'
Then e apresentada a mensagem de exclusao 'Item excluído com sucesso.'