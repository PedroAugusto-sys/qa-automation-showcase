Feature: Editar Atributos Adicionais

@Academico
@AtributoAdicional
@EditarAtributoAdicionalCadastrando
Scenario: Editar atributo adicional cadastrando
Given que desejo editar um atributo adicional 
And seleciono o operador 'OPERADOR QA'
And pesquiso pelo atributo adicional 'Atributo teste Editar'
And clico em 'Editar'
And altero a descricao para 'Atributo editado!'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

# Cenário para refatorar no futuro quanod a ficha do aluno estiver pronta
@Academico
@AtributoAdicional
@EditarAtributoAdicionalQuePossuiResposta
Scenario: Editar atributo adicional que possui resposta 
Given que desejo editar um atributo que possui resposta
And seleciono o operador 'OPERADOR QA'
And pesquiso pelo atributo adicional com resposta 'Atributo teste com resposta'
And clico em 'Editar'
And altero a descricao para 'Atributo teste com resposta Editado'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador