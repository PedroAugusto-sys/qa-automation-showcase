
Feature: Editar Profissional





@Academico
@OutrosProfissionais
@EditarNomeOutrosProfissionaisCadastrando
Scenario: Editar nome do profissional

Given que desejo alterar o nome de um profissional que nao seja um professor
And acesso o sistema utilizando o operador 'teste qa a a a'
And pesquiso pelo profissional 'TESTE FEATURE EDITAR NOME OUTROS PROFISSIONAIS CADASTRANDO'
And clico em 'Editar'
And altero o nome do profissional para 'NOME ALTERADOOOOO'
When clico em 'Gravar' 
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador





@Academico
@OutrosProfissionais
@AlterarSituacaoOutrosProfissionaisCadastrando
Scenario: Alterar situacao do profissional
Given que desejo alterar a situacao de um profissional
And acesso o sistema utilizando o operador 'teste qa a a a'
And pesquiso pelo profissional 'TESTE FEATURE ALTERAR SITUACAO'
And clico em 'Editar'
And altero a situacao para 'Inativo'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

