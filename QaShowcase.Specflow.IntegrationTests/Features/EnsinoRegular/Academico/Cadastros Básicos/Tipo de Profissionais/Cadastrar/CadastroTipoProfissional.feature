Feature: Cadastrar um tipo de Profissional

@Academico
@TiposProfissionais
@CadastrarTipoProfissionais

Scenario: Cadastrar tipos de profissionais

Given que desejo cadastrar um tipo de profissional
And acesso o sistema utilizando o operador 'teste qa pedro a a'
And informo a descricao 'TIPO DE PROFISSIONAL'
When clico em 'Cadastrar'
Then e apresentada a mensagem 'Registro cadastrado com sucesso!'
Then removo o operador