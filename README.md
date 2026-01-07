# Maná Food - Microsserviço de Usuário

## Estrutura de Pastas

```
mana-food-microsservico-usuario/
├── Core/
│   ├── ManaFood.Application/      # Camada de aplicação (casos de uso, serviços, validações, configurações)
│   │   ├── Configurations/
│   │   ├── Constantes/   
│   │   ├── Dtos/     
│   │   ├── Interfaces/     
│   │   ├── Mappings/     
│   │   ├── Services/     
│   │   ├── Shared/      
│   │   ├── UseCases/
│   │   └── Utils/
│   └── ManaFood.Domain/           # Camada de domínio (entidades, enums)
│       ├── Entidades/        
│       └── Enums/
├── Infrastructure/
│   └── ManaFood.Infrastructure/   # Infraestrutura (acesso a dados, repositórios, contexto do banco)
│       ├── Auth/
│       ├── Configurations/
│       └── Database/
│           └── Repositories/
├── k8s/                           # Infraestrutura Kubernetes utilizando IAC
├── Presentation/
│   └── ManaFood.WebAPI/           # Camada de apresentação (controllers, configuração da API)
│       ├── Controllers/
│       ├── Filters/
│       ├── Middlewares/
│       ├── Properties/
│       ├── appsettings.json
│       └── appsettings.Development.json
└── Test/
│   └── ManaFood.UnitTest/
├── README.md
├── .gitignore
├── Dockerfile
├── LICENSE
└── ManaFood.sln
```

## Descrição dos Principais Diretórios

- **Core/ManaFood.Application/**: Implementa os casos de uso da aplicação, validações, comportamentos compartilhados e configurações específicas da camada de aplicação.
- **Core/ManaFood.Domain/**: Contém as entidades de domínio, interfaces e regras de negócio puras, sem dependências externas.
- **Infrastructure/ManaFood.Infrastructure/**: Responsável pela implementação da infraestrutura, como acesso ao DynamoDB, repositórios e configurações relacionadas à persistência de dados NoSQL.
- **Presentation/ManaFood.WebAPI/**: Camada de apresentação, onde ficam os controllers da API e Webhook, configurações do ASP.NET Core, arquivos de configuração (appsettings) e propriedades do projeto.

## Explicação do Docker

### O que é o Dockerfile?

O `Dockerfile` define como a imagem da aplicação será construída. No caso deste projeto, ele:

- Usa uma imagem base do ASP.NET para rodar a aplicação.
- Usa uma imagem do SDK do .NET para compilar e publicar o projeto.
- Copia os arquivos publicados para a imagem final.
- Define o comando de inicialização da API.

---
## Como executar o projeto

### 1. Clonando o repositório

```sh
git clone https://github.com/mana-food/mana-food-microsservico-usuario.git
cd mana-food-microsservico-usuario
```

### 2. Executando localmente

Certifique-se de ter o [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) instalado.

#### Configuração do AWS DynamoDB Local (Opcional para desenvolvimento)

Para rodar localmente com DynamoDB:
```sh
docker run -p 8000:8000 amazon/dynamodb-local -jar DynamoDBLocal.jar -sharedDb -inMemory
```

#### Executar a aplicação

```sh
dotnet run --project Presentation/ManaFood.WebAPI/ManaFood.WebAPI.csproj
```

A API estará disponível em: [http://localhost:5000](http://localhost:5000) ou [https://localhost:5001](https://localhost:5001)

### 3. Executando os containers

#### Como rodar os containers com Docker

1. **Pré-requisitos**  
   Certifique-se de ter o [Docker](https://www.docker.com/get-started).

2. **Clone o repositório (caso ainda não tenha feito):**
   ```sh
   git clone https://github.com/mana-food/mana-food-microsservico-usuario.git
   cd mana-food-microsservico-usuario
   ```

3. **Construa e execute com Docker**

   **Primeiro, inicie o DynamoDB Local:**
   ```sh
   docker run -d -p 8000:8000 --name dynamodb-local amazon/dynamodb-local -jar DynamoDBLocal.jar -sharedDb -inMemory
   ```

   **Depois, construa e execute a API:**
   ```sh
   # Build da imagem
   docker build -t manafood-api:latest .

   # Execute o container
   docker run -d -p 8080:8080 \
   -e AWS_ACCESS_KEY_ID=dummy \
   -e AWS_SECRET_ACCESS_KEY=dummy \
   -e AWS_REGION=us-east-1 \
   -e AWS_SERVICE_URL=http://host.docker.internal:8000 \
   --name manafood-api \
   manafood-api:latest
   ```

4. **Acesse a aplicação:**
   - API/Swagger: http://localhost:8080
   - DynamoDB Local: localhost:8000


#### Como rodar os containers com Kubernetes

1. **Pré-requisitos**
   - [Kubernetes](https://kubernetes.io/)
   - [Minikube](https://minikube.sigs.k8s.io/docs/start/)
   - [kubectl](https://kubernetes.io/docs/tasks/tools/)

2. **Inicie o Minikube**
   ```sh
   minikube start
   ```

3. **Construa a imagem Docker localmente**
   ```sh
   # Configure o ambiente Docker do Minikube
   eval $(minikube docker-env)
   
   # Navegue até o diretório do Dockerfile
   cd Presentation/ManaFood.WebAPI
   
   # Construa a imagem
   docker build -t manafood-api:latest .
   
   # Volte para a raiz do projeto
   cd ../..
   ```

4. **Aplique os manifestos Kubernetes**
   
   Execute os comandos na sequência:
   ```sh
   cd k8s
   
   # ConfigMaps e Secrets
   kubectl apply -f api-configmap.yaml
   kubectl apply -f api-secret.yaml
   
   # Banco de Dados
   kubectl apply -f db-service.yaml
   kubectl apply -f db-deployment.yaml
   
   # API
   kubectl apply -f api-service.yaml
   kubectl apply -f api-deployment.yaml
   
   # Autoscaling
   kubectl apply -f api-hpa.yaml
   ```

   **Recursos criados:**
   - **ConfigMaps**: Variáveis de ambiente não sensíveis
   - **Secrets**: Credenciais e dados sensíveis
   - **DB Service**: ClusterIP para comunicação interna com o banco
   - **DB Deployment**: Pod do DynamoDB Local em memória
   - **API Service**: LoadBalancer para acesso externo à API
   - **API Deployment**: Pods da aplicação com múltiplas réplicas
   - **API HPA**: Auto-scaling horizontal baseado em CPU/Memória

5. **Verifique o status dos pods**
   ```sh
   kubectl get pods
   kubectl get services
   kubectl get deployments
   ```

6. **Acesse a aplicação**
   ```sh
   # Obtenha a URL do serviço
   minikube service api-service --url
   ```

7. **Monitore via Dashboard**
   ```sh
   minikube dashboard
   ```
   
   Interface de gerenciamento visual:
   
   ![Minikube Dashboard](Assets/Minikube_ManaFood.png)

8. **Para limpar os recursos**
   ```sh
   kubectl delete -f .
   cd ..
   minikube stop
   ```

---
### 4. Trabalhando com DynamoDB

#### Estrutura de Tabelas

O projeto utiliza AWS DynamoDB como banco de dados NoSQL. As tabelas são criadas automaticamente pela aplicação usando atributos do AWS SDK.

#### Configuração Local

Para desenvolvimento local, use DynamoDB Local:
```sh
docker run -p 8000:8000 amazon/dynamodb-local -jar DynamoDBLocal.jar -sharedDb -inMemory
```
---
### 5. Explicação da Autenticação e Autorização

#### Visão Geral

A aplicação utiliza autenticação baseada em **JWT (JSON Web Token)** para garantir que apenas usuários autenticados possam acessar endpoints protegidos. A autorização é feita por meio de **roles**, permitindo restringir o acesso conforme o tipo de usuário.

#### Autenticação 🔐

**Processo de Login:**

1. O usuário envia credenciais (CPF/email e senha) para o endpoint `/api/auth/login`
2. O sistema valida as credenciais no banco de dados
3. Se válido, gera um token JWT contendo:
   - ID do usuário
   - Tipo de usuário (role)
   - Data de expiração
4. O token é retornado ao cliente

**Utilizando o Token:**

Em todas as requisições protegidas, inclua o token no header:

```
Authorization: Bearer {seu_token_jwt}
```

**Fluxo de Validação:**

O middleware `JwtAuthenticationMiddleware`:
- Intercepta todas as requisições
- Extrai o token do header Authorization
- Valida a assinatura e expiração do token
- Extrai as claims (ID do usuário, role)
- Define o usuário autenticado no contexto (`HttpContext.Items["User"]`)

#### Autorização 👤

**Uso do Atributo CustomAuthorize:**

Para proteger endpoints específicos:

```csharp
// Permite apenas usuários autenticados
[CustomAuthorize]
public IActionResult MinhaAction() { ... }

// Permite apenas Admin
[CustomAuthorize(UserType.Admin)]
public IActionResult ActionAdmin() { ... }

// Permite Admin ou Manager
[CustomAuthorize(UserType.Admin, UserType.Manager)]
public IActionResult ActionMultiRole() { ... }
```

**Tipos de Usuário Disponíveis:**

```csharp
public enum UserType
{
    Admin = 0,
    Customer = 1,
    Kitchen = 2,
    Operator = 3,
    Manager = 4
}
```

**Códigos de Resposta:**

- `200 OK`: Autenticado e autorizado
- `401 Unauthorized`: Token inválido ou ausente
- `403 Forbidden`: Autenticado mas sem permissão (role inadequada)

#### Configuração de Segurança

**Chave Secreta JWT:**

Configurada via variável de ambiente `JWT_SECRET_KEY` ou no appsettings.json:

```json
{
  "Jwt": {
    "SecretKey": "sua-chave-secreta-aqui-minimo-32-caracteres",
    "Issuer": "ManaFood",
    "Audience": "ManaFoodUsers",
    "ExpirationMinutes": 60
  }
}
```

**⚠️ Importante:** Nunca commite a chave secreta no repositório. Use variáveis de ambiente em produção.

---

### 6. Ordem de execução das APIs

#### Fluxo Básico de Uso

1. **Criar Usuário**
   - Endpoint: `POST /api/user`
   - Não requer autenticação
   - Body: Nome, CPF, Email, Senha, Tipo de Usuário

2. **Realizar Login**
   - Endpoint: `POST /api/auth/login`
   - Body: CPF ou Email + Senha
   - Retorna: Token JWT

3. **Autorizar no Swagger**
   - Clique no botão "Authorize" 🔒
   - Digite: `Bearer {token_recebido}`
   - Clique em "Authorize"

4. **Acessar Endpoints Protegidos**
   - Agora você pode acessar endpoints que requerem autenticação
   - O token será enviado automaticamente

#### Endpoints Disponíveis

**Autenticação (Público):**
- `POST /api/auth/login` - Realizar login

**Usuários:**
- `POST /api/user` - Criar usuário (público)
- `GET /api/user` - Listar usuários (requer autenticação)
- `GET /api/user/{id}` - Buscar usuário por ID (requer autenticação)
- `PUT /api/user/{id}` - Atualizar usuário (requer autenticação)
- `DELETE /api/user/{id}` - Deletar usuário (Admin apenas)

---

### 7. Documentação Complementar

#### Documentação Notion:
```
https://chartreuse-fountain-62d.notion.site/203ce57501598031b488df683ec4c8dd?v=203ce57501598002923d000c738029fd&source=copy_link
```

#### Documentação MIRO:
```
https://miro.com/app/board/uXjVIHWEfCI=/
```

#### Vídeo Explicativo (YouTube) | FIAP Pós Tech Challenge Fase 4:
```
https://www.youtube.com/watch?v=60IeDq_nK6I
```

---

## Tecnologias Utilizadas

- **.NET 9**: Framework principal
- **ASP.NET Core**: Web API
- **AWS DynamoDB**: Banco de dados NoSQL
- **Kubernetes**: Orquestração de containers
- **JWT**: Autenticação e autorização
- **Swagger/OpenAPI**: Documentação da API
- **AutoMapper**: Mapeamento objeto-objeto
- **FluentValidation**: Validação de dados

---

## Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

---

## Licença

Este projeto está sob a licença especificada no arquivo [LICENSE](LICENSE).
