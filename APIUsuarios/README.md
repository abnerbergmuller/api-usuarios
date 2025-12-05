# API de Gerenciamento de Usuários

## Descrição
Esta API fornece operações CRUD para gerenciamento de usuários de uma plataforma digital. O objetivo é demonstrar o uso de padrões de projeto (Repository, Service e DTO Pattern), separação de camadas inspirada em Clean Architecture e persistência com Entity Framework Core + SQLite.

A solução foi construída com .NET 8/10, ASP.NET Core Web API (Controllers), validações via Data Annotations (com suporte a FluentValidation opcional) e documentação interativa via Swagger. O banco de dados utiliza abordagem Code-First com Migrations.

## Tecnologias Utilizadas
- .NET 8.0+ (compatível com .NET 10 target da solução)
- ASP.NET Core Web API
- Entity Framework Core 8
- SQLite
- Swagger (Swashbuckle)
- Data Annotations (e compatível com FluentValidation)

## Padrões de Projeto Implementados
- Repository Pattern (abstração da persistência)
- DTO Pattern (separação entre entidade e contratos de I/O)
- Dependency Injection (nativa do ASP.NET Core)

## Como Executar o Projeto

### Pré-requisitos
- .NET SDK 8.0 ou superior

### Passos
1. Clone o repositório
   ```bash
   git clone <URL_DO_SEU_REPOSITORIO>
   cd api-usuarios-as-<seu-nome>/APIUsuarios
   ```
2. (Opcional) Restaure as dependências
   ```bash
   dotnet restore
   ```
3. Criar e aplicar as migrations (Code First)
   ```bash
   dotnet ef migrations add Initial
   dotnet ef database update
   ```
4. Executar a aplicação
   ```bash
   dotnet run
   ```
5. Acessar o Swagger UI
   - HTTPS: https://localhost:7218/swagger
   - HTTP:  http://localhost:5201/swagger

> Observação: Em `Properties/launchSettings.json`, os perfis já definem `ASPNETCORE_ENVIRONMENT=Development`, e o Swagger é habilitado para Development por padrão em `Program.cs`.

## Endpoints

Base URL local (exemplos): `https://localhost:7218` ou `http://localhost:5201`

- GET `/usuarios` — Lista todos os usuários (inclui inativos)
- GET `/usuarios/{id}` — Busca usuário por ID
- POST `/usuarios` — Cria novo usuário
- PUT `/usuarios/{id}` — Atualiza um usuário existente
- DELETE `/usuarios/{id}` — Soft delete (marca `Ativo=false`)

### Exemplos (curl)

Listar todos:
```bash
curl -k https://localhost:7218/usuarios
```

Buscar por ID:
```bash
curl -k https://localhost:7218/usuarios/1
```

Criar usuário:
```bash
curl -k -X POST https://localhost:7218/usuarios \
  -H "Content-Type: application/json" \
  -d '{
        "nome": "Maria Souza",
        "email": "maria.souza@example.com",
        "senha": "segredo123"
      }'
```

Atualizar usuário:
```bash
curl -k -X PUT https://localhost:7218/usuarios/1 \
  -H "Content-Type: application/json" \
  -d '{
        "nome": "Maria S. Souza",
        "email": "maria.souza@example.com",
        "ativo": true
      }'
```

Remover (soft delete):
```bash
curl -k -X DELETE https://localhost:7218/usuarios/1
```

### Exemplos de Payload

POST `/usuarios` (request):
```json
{
  "nome": "João Silva",
  "email": "joao.silva@example.com",
  "senha": "minhaSenha123"
}
```

POST `/usuarios` (response 201):
```json
{
  "id": 1,
  "nome": "João Silva",
  "email": "joao.silva@example.com",
  "ativo": true
}
```

PUT `/usuarios/{id}` (request):
```json
{
  "nome": "João P. Silva",
  "email": "joao.silva@example.com",
  "ativo": true
}
```

Erros esperados:
- 400 Bad Request — Validação falhou (Data Annotations/ModelState)
- 404 Not Found — Usuário não encontrado
- 409 Conflict — Email já cadastrado
- 500 Internal Server Error — Erro inesperado

## Estrutura do Projeto
```
APIUsuarios/
├── Application/
│   ├── DTOs/
│   │   ├── CreateUsuarioDto.cs
│   │   ├── ResponseUsuarioDto.cs
│   │   └── UpdateUsuarioDto.cs
│   └── Interfaces/
│       └── IUsuarioRepository.cs
├── Controllers/
│   └── UsuariosController.cs
├── Domain/
│   └── Entities/
│       └── Usuario.cs
├── Infrastructure/
│   ├── Persistence/
│   │   └── AppDbContext.cs
│   └── Repositories/
│       └── UsuarioRepository.cs
├── Migrations/ (geradas via EF)
├── Program.cs
├── appsettings.json
└── APIUsuarios.csproj
```

## Notas de Implementação
- Soft delete implementado no repositório (define `Ativo=false`).
- Normalização de e-mail (`trim().toLowerInvariant()`) garantida no repositório.
- Índice único para `Email` e validações por Data Annotations.
- O `SaveChangesAsync` é controlado pelo controlador (ou camada de serviço, caso adicionada).

## Autor
- Nome: Abner Bergmüller
- Disciplina: Desenvolvimento Back End - Turma 5N
- Curso: Análise e Desenvolvimento de Sistemas, ULBRA

## Collection do Postman
Exporte sua collection de testes como `APIUsuarios.postman_collection.json` e adicione na raiz do repositório.
