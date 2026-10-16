Feature: Editar um tipo de Profissional

@Academico
@TiposProfissionais
@EditarTipoProfissionalCadastrando
Scenario: Editar um tipo de profissional


Given que desejo editar um tipo de profissional
And acesso o sistema utilizando o operador 'testesqapedro'
And pesquiso pelo tipo de profissional 'TIPO DE PROFISSIONAL'
And clico em 'Editar'
When clico em 'Editar' para confirmar a operacao
Then e apresentada a mensagem 'Registro editado com sucesso!'
Then removo o operador