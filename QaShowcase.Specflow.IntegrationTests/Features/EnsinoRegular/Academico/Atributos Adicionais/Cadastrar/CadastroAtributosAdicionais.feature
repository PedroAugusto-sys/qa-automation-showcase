Feature: Cadastrar Atributo Adicional

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Texto
Scenario: Cadastrar atributo adicional conceito aluno e tipo texto
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And insiro a descricao 'Atributo teste Aluno'
And seleciono a obrigatoriedade 'Não'
And seleciono a unicidade 'Não'
And seleciono o conceito 'Aluno'
And seleciono o tipo 'Texto'
And defino o tamanho maximo '450'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.FichaMedica.Tp.Texto
Scenario: Cadadastrar atributo adicional conceito Ficha Medica e tipo Texto
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And informo a descricao 'Atributo teste Ficha Medica'
And seleciono a obrigatoriedade 'Não'
And seleciono a unicidade 'Não'
And seleciono o conceito 'Ficha Médica'
And seleciono o tipo 'Texto'
And defino o tamanho maximo '450'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Responsavel.Tp.Texto
Scenario: Cadadastrar atributo adicional conceito Responsavel e tipo Texto
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And informo a descricao 'Atributo teste Responsavel'
And seleciono a obrigatoriedade 'Não'
And seleciono a unicidade 'Não'
And seleciono o conceito 'Responsável'
And seleciono o tipo 'Texto'
And defino o tamanho maximo '450'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Texto.Unico
Scenario: Cadadastrar atributo adicional conceito Aluno e tipo Texto Unico
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And informo a descricao 'Atributo teste Aluno Unico'
And seleciono a obrigatoriedade 'Não'
And seleciono a unicidade 'Sim'
And seleciono o conceito 'Aluno'
And seleciono o tipo 'Texto'
And defino o tamanho maximo '450'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Texto.Obrigatorio
Scenario: Cadadastrar atributo adicional conceito Aluno e tipo Texto Obrigatorio
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And informo a descricao 'Atributo teste Aluno Obrigatorio'
And seleciono a obrigatoriedade 'Sim'
And seleciono a unicidade 'Não'
And seleciono o conceito 'Aluno'
And seleciono o tipo 'Texto'
And defino o tamanho maximo '450'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Texto.Obrigatório.Unico
Scenario: Cadadastrar atributo adicional conceito Aluno e tipo Texto Obrigatorio Unico
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And informo a descricao 'Atributo teste Aluno Obrigatorio Unico'
And seleciono a obrigatoriedade 'Sim'
And seleciono a unicidade 'Sim'
And seleciono o conceito 'Aluno'
And seleciono o tipo 'Texto'
And defino o tamanho maximo '450'
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Lista
Scenario: Cadastrar atributo adicional conceito aluno e tipo lista
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And informo a descricao 'Atributo teste Aluno Lista'
And seleciono a obrigatoriedade 'Não'
And seleciono a unicidade 'Não'
And seleciono o conceito 'Aluno'
And seleciono o tipo 'Lista'
And adiciono '3' itens na lista
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Lista.Obrigatorio
Scenario: Cadastrar atributo adicional conceito aluno e tipo lista obrigatorio
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And informo a descricao 'Atributo teste Aluno Lista Obrigatorio'
And seleciono a obrigatoriedade 'Sim'
And seleciono a unicidade 'Não'
And seleciono o conceito 'Aluno'
And seleciono o tipo 'Lista'
And adiciono '3' itens na lista
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Lista.Unico
Scenario: Cadastrar atributo adicional conceito aluno e tipo lista unico
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And informo a descricao 'Atributo teste Aluno Lista Unico'
And seleciono a obrigatoriedade 'Não'
And seleciono a unicidade 'Sim'
And seleciono o conceito 'Aluno'
And seleciono o tipo 'Lista'
And adiciono '3' itens na lista
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador

@Academico 
@AtributoAdicional
@CadastrarAtributoAdicional.Conc.Aluno.Tp.Lista.Obrigatorio.Unico
Scenario: Cadastrar atributo adicional conceito aluno e tipo lista obrigatorio unico
Given que desejo cadastrar um atributo adicional
And seleciono o operador 'OPERADOR QA'
And clico em 'Cadastrar'
And informo a descricao 'Atributo teste Aluno Lista Obrigatorio Unico'
And seleciono a obrigatoriedade 'Sim'
And seleciono a unicidade 'Sim'
And seleciono o conceito 'Aluno'
And seleciono o tipo 'Lista'
And adiciono '3' itens na lista
When clico em 'Gravar'
Then e apresentada a mensagem 'Cadastro realizado com sucesso!'
Then removo o operador