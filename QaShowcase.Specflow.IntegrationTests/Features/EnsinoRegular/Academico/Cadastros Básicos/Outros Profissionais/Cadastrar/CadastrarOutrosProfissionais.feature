Feature: Cadastrar Profissional


@Academico
@OutrosProfissionais
@CadastrarOutrosProfissionaisObrigatorio

Scenario: Cadastrar outros profissionais somente com dados obrigatorios

Given que desejo cadastrar um profissional que nao seja um professor
And acesso o sistema utilizando o operador 'teste qa a a a'
And informo o nome '<nome_profissional>'
And informo a data de nascimento
And informo a naturalidade
And seleciono o sexo
And informo a nacionalidade
And seleciono a etnia
And seleciono o tipo de profissional 'tipo_profissional'
And informo o endereco
And preencho os atributos adicionais obrigatorios
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador