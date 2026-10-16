Feature: Excluir Atributo Adicional

@Academico
@AtributoAdicional
@ExcluirAtributoAdicionalCadastrando
Scenario: Excluir atributo adicional cadastrando
Given que desejo excluir um atributo adicional 
And seleciono o operador 'OPERADOR QA'
And pesquiso pelo atributo adicional 'Atributo teste Excluir'
And clico em 'Remover'
When clico em 'Sim'
Then e apresentada a mensagem 'Item excluído com sucesso.'
Then removo o operador

# Cenário para refatorar no futuro quanod a ficha do aluno estiver pronta
@Academico
@AtributoAdicional
@ExcluirAtributoAdicionalQuePossuiResposta
Scenario: Excluir atributo adicional que possui resposta
Given que desejo excluir um atributo adicional 
And seleciono o operador 'OPERADOR QA'
And pesquiso pelo atributo adicional com resposta 'Atributo teste respondido'
And clico em 'Remover'
When clico em 'Sim'
Then e apresentada a mensagem 'Codigo: Atributo adicional está em uso.'
Then removo o operador