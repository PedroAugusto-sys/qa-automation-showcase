Feature: Excluir um tipo de Profissional

@Academico
@TiposProfissionais
@ExcluirTipoProfissionalSemUsoCadastrando
Scenario: Excluir um tipo de profissional

Given que desejo excluir um tipo de profissional que nao esta em uso
And acesso o sistema utilizando o operador 'teste qa jader'
And pesquiso pelo tipo de profissional 'teste qa jader'
And clico em 'Excluir'
When clico em 'Sim'
Then e apresentada a mensagem 'Item excluído com sucesso'
Then removo o operador



@Academico
@TiposProfissionais
@ExcluirTipoProfissionalEmUsoCadastrando
Scenario: Excluir um tipo de profissional em uso

Given que desejo excluir um tipo de profissional que esta em uso
And acesso o sistema utilizando o operador 'ABCDEF'
And pesquiso pelo tipo de profissional em uso 'ABCDEF'
And clico em 'Excluir'
When clico em 'Sim'
Then e apresentada a mensagem 'Tipo de profissional: Exclusão não permitida! Existe profissional cadastrado com esse tipo.'
Then removo o operador