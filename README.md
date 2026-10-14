# FASHINO - Back-end

API e Worker do **FASHINO**, um app de moda que gera imagens com IA (provador virtual, criação de modelos, produto-para-modelo e rosto-para-modelo) usando a [Fashn AI](https://fashn.ai).

O usuário se autentica via Firebase, envia suas mídias (armazenadas no MinIO) e cria *jobs* de geração. A **API** registra o job e o **Worker** (Hangfire) envia para a Fashn AI, acompanha o status e salva o resultado. As assinaturas e planos são gerenciados pelo RevenueCat por meio de webhooks.

## Stack

- **.NET 8** / ASP.NET Core Web API (Clean Architecture: `Api`, `Application`, `Domain`, `Infrastructure`, `Shared`, `Worker`)
- **PostgreSQL 16** + Entity Framework Core
- **Hangfire** (jobs em segundo plano, storage em PostgreSQL)
- **MinIO** (armazenamento de arquivos compatível com S3)
- **Firebase Admin** (autenticação) + JWT
- **RevenueCat** (assinaturas)
- AutoMapper, Polly, Serilog, Swagger
- Docker / Docker Compose

## Como iniciar

**Pré-requisitos:** Docker, .NET 8 SDK e a ferramenta `dotnet-ef` (`dotnet tool install -g dotnet-ef`).

1. Crie o arquivo `Api/appsettings.json` (ele não é versionado), usando como base o `Api/appsettings.Development.json`, e adicione a chave de serviço do Firebase (`fashino-firebase-admin.json`).

2. Suba todo o ambiente:

   ```bash
   docker compose up -d --build
   ```

3. Aplique as migrations no banco:

   ```bash
   dotnet ef database update --project Infrastructure --startup-project Api
   ```

| Serviço         | URL                             |
| --------------- | ------------------------------- |
| API (Swagger)   | http://localhost:5000/swagger   |
| Hangfire        | http://localhost:5000/hangfire  |
| Worker          | http://localhost:5005           |
| MinIO Console   | http://localhost:9001 (admin / admin123) |
| pgAdmin         | http://localhost:8082 (admin@example.com / admin) |

Para rodar apenas a infraestrutura e executar a API/Worker pelo Visual Studio ou `dotnet run`, use:

```bash
docker compose up -d fashino.db minio fashino.pgadmin
```
