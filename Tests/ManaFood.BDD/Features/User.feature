# language: pt-BR

Funcionalidade: Gerenciamento de Usuários
  Como um sistema de gerenciamento de usuários
  Eu quero criar, buscar, atualizar e deletar usuários
  Para que os clientes possam se cadastrar e gerenciar suas contas

Cenário: Criar um usuário válido
  Dado que eu tenho os dados de um novo usuário válido
  | Campo     | Valor                              |
  | Email     | teste.usuario@example.com          |
  | Nome      | João da Silva                      |
  | CPF       | 11144477735                        |
  | Senha     | SenhaForte@123                     |
  | DataNasc  | 1990-05-15                         |
  | TipoUser  | 1                                  |
  Quando eu solicito a criação do usuário
  Então o sistema deve criar o usuário com sucesso
  E o usuário deve ter um ID gerado
  E a senha deve estar criptografada
  E o email deve ser "teste.usuario@example.com"

Cenário: Não permitir criar usuário com email duplicado
  Dado que já existe um usuário com email "usuario.existente@example.com"
  E eu tento criar um novo usuário com os dados
  | Campo     | Valor                              |
  | Email     | usuario.existente@example.com      |
  | Nome      | Maria Santos                       |
  | CPF       | 52998224725                        |
  | Senha     | OutraSenha@456                     |
  | DataNasc  | 1995-08-20                         |
  | TipoUser  | 1                                  |
  Quando eu solicito a criação do usuário
  Então o sistema deve retornar um erro de email duplicado
  E a mensagem deve conter "já está vinculado a um usuário"

Cenário: Não permitir criar usuário com CPF duplicado
  Dado que já existe um usuário com CPF "52998224725"
  E eu tento criar um novo usuário com os dados
  | Campo     | Valor                              |
  | Email     | novo.usuario@example.com           |
  | Nome      | Pedro Oliveira                     |
  | CPF       | 52998224725                        |
  | Senha     | MinhaSenha@789                     |
  | DataNasc  | 1988-12-10                         |
  | TipoUser  | 2                                  |
  Quando eu solicito a criação do usuário
  Então o sistema deve retornar um erro de CPF duplicado
  E a mensagem deve conter "já está vinculado a um usuário"

Cenário: Buscar usuário por ID
  Dado que existe um usuário com ID "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  Quando eu busco o usuário por ID
  Então o sistema deve retornar os dados do usuário
  E o nome do usuário deve ser "João da Silva"
  E o email do usuário deve ser "joao.silva@example.com"

Cenário: Buscar usuário por email
  Dado que existe um usuário com email "maria.santos@example.com"
  Quando eu busco o usuário por email
  Então o sistema deve retornar os dados do usuário
  E o nome do usuário deve ser "Maria Santos"

Cenário: Buscar usuário por CPF
  Dado que existe um usuário com CPF "55566677788"
  Quando eu busco o usuário por CPF
  Então o sistema deve retornar os dados do usuário
  E o nome do usuário deve ser "Carlos Pereira"

Cenário: Listar todos os usuários
  Dado que existem 5 usuários cadastrados no sistema
  Quando eu solicito a listagem de todos os usuários
  Então o sistema deve retornar 5 usuários
  E todos devem ter ID, nome e email válidos

Cenário: Atualizar dados de um usuário
  Dado que existe um usuário com ID "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  E eu atualizo o nome para "João da Silva Atualizado"
  Quando eu solicito a atualização do usuário
  Então o sistema deve atualizar o usuário com sucesso
  E o nome do usuário deve ser "João da Silva Atualizado"

Cenário: Deletar um usuário
  Dado que existe um usuário com ID "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  Quando eu solicito a exclusão do usuário
  Então o sistema deve deletar o usuário com sucesso

Cenário: Validar CPF inválido
  Dado que eu tento criar um usuário com CPF inválido "12345678901"
  Quando eu solicito a criação do usuário
  Então o sistema deve retornar um erro de validação
  E a mensagem deve indicar CPF inválido

Cenário: Validar email inválido
  Dado que eu tento criar um usuário com email inválido "emailinvalido"
  Quando eu solicito a criação do usuário
  Então o sistema deve retornar um erro de validação
  E a mensagem deve indicar email inválido

Cenário: Validar senha fraca
  Dado que eu tento criar um usuário com senha fraca "12"
  Quando eu solicito a criação do usuário
  Então o sistema deve retornar um erro de validação
  E a mensagem deve indicar senha fraca

Cenário: Validar data de nascimento futura
  Dado que eu tento criar um usuário com data de nascimento futura "2030-01-01"
  Quando eu solicito a criação do usuário
  Então o sistema deve retornar um erro de validação
  E a mensagem deve indicar data de nascimento inválida

Cenário: Validar usuário menor de idade
  Dado que eu tento criar um usuário com data de nascimento "2020-01-01"
  Quando eu solicito a criação do usuário
  Então o sistema deve retornar um erro de validação
  E a mensagem deve indicar que o usuário deve ser maior de idade