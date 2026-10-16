Feature: Pesquisar Atributo Adicional

@Academico
@AtributoAdicional
@PesquisarAtributoAdicionalCadastrando
Scenario: Pesquisar atributo adicional cadastrando
Given que desejo pesquisar um atributo adicional 
And seleciono o operador 'OPERADOR QA'
When pesquiso pelo atributo adicional 'Atributo teste Pesquisar'
Then e apresentado o atributo adicinal pesquisado
Then removo o operador