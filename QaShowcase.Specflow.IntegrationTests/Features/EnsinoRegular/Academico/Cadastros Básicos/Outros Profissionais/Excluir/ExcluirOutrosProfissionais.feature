
Feature: : Cadastrar Profissional

@Academico
@OutrosProfissionais
@ExcluirOutrosProfissionaisSemVinculosCadastrando
Scenario: Excluir um profissional


Given que desejo excluir um profissional
And acesso o sistema utilizando o operador 'oooooo'
And pesquiso pelo profissional 'nome_profissional'
And clico em 'Excluir'
When confirmo a operacao
Then e apresentada a mensagem 'Item excluído com sucesso.'
Then removo o operador


@Academico
@OutrosProfissionais
@ExcluirOutrosProfissionaisComVinculosCadastrando
Scenario: Excluir um profissional Vinculado
Given que desejo excluir um profissional
And acesso o sistema utilizando o operador 'oooooo'
And pesquiso pelo profissional 'JADER TESTING LANCAMENTO CAIXA'
And clico em 'Excluir'
When confirmo a operacao
Then e apresentada a mensagem 'Lançamentos: Não é possível excluir profissional, pois o mesmo possui registros em caixa.'
Then removo o operador
