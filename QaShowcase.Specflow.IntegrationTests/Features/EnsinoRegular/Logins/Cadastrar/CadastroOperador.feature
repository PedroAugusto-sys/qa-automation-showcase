Feature: Cadastrar usuario

@Logins
@CadastroOperador
Scenario: Cadastrar usuario
Given que desejo cadastrar um operador
And clico em cadastrar
And preencho o campo nome 'PEDRO QA'
And preencho o campo login 'PEDRO QA'
And preencho o campo senha '<DEFAULT_PASSWORD>'
And habilito o usuario 'sim'
And seleciono a empresas visiveis para este operador 
And seleciono empresa padrao 
And seleciono as permissoes essenciais
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'