# Usuarios API .NET

Exemplo do mesmo CRUD de usuarios da pasta `python`, agora implementado em .NET 10 com ASP.NET Core Minimal API, Swagger, persistencia em memoria, testes automatizados e arquitetura Ports and Adapters.

## Tecnologias

- .NET 10
- ASP.NET Core Minimal API
- Swagger UI
- xUnit

## Pre-requisitos

- Windows com PowerShell
- .NET SDK 10 instalado

Para validar a instalacao:

```powershell
dotnet --list-sdks
```

## Arquitetura

Estrutura da solucao:

```text
src/
|-- Usuarios.Api
|-- Usuarios.Application
|-- Usuarios.Domain
`-- Usuarios.Adapters

tests/
`-- Usuarios.Tests
```

Responsabilidades:

- `Usuarios.Domain`: entidade, excecoes e porta de repositorio
- `Usuarios.Application`: servico de aplicacao e comando de entrada
- `Usuarios.Adapters`: implementacao em memoria do repositorio
- `Usuarios.Api`: endpoints Minimal API, Swagger e composicao da aplicacao
- `Usuarios.Tests`: testes de servico e de API

## Estilo da API

A camada HTTP da versao .NET agora usa Minimal API em vez de controllers MVC.

Isso significa que:

- os endpoints sao mapeados diretamente no startup da aplicacao
- o contrato HTTP continua o mesmo da versao anterior
- a estrutura continua separada por camadas, sem misturar regra de negocio com a borda HTTP

Arquivos principais da borda HTTP:

- `src/Usuarios.Api/Program.cs`
- `src/Usuarios.Api/Endpoints/HealthEndpoints.cs`
- `src/Usuarios.Api/Endpoints/UserEndpoints.cs`

## Modelo de dominio

- `id: int`
- `nome: string`
- `dtNascimento: date`
- `status: bool`
- `telefones: string[]`

## Como rodar

Na pasta `dotnet`:

### 1. Restaurar dependencias

```powershell
dotnet restore
```

### 2. Rodar a API

```powershell
dotnet run --project .\src\Usuarios.Api\Usuarios.Api.csproj
```

Por padrao, usando o `launchSettings.json` atual, a API sobe em:

- API: `http://localhost:5266`
- Swagger UI: `http://localhost:5266/swagger`

Se a porta mudar, o `dotnet run` vai mostrar a URL correta no terminal.

### 3. Build de validacao

```powershell
dotnet build .\UsuariosApi.slnx
```

## Como testar

```powershell
dotnet test .\UsuariosApi.slnx --collect:"XPlat Code Coverage"
```

Resultado esperado no estado atual do projeto:

- testes de servico
- testes de API
- 9 testes passando

## Endpoints

- `GET /health/live`
- `GET /health/ready`
- `POST /usuarios`
- `GET /usuarios`
- `GET /usuarios/{usuarioId}`
- `PUT /usuarios/{usuarioId}`
- `DELETE /usuarios/{usuarioId}`

## Exemplo de payload

```json
{
	"id": 1,
	"nome": "Carlos",
	"dtNascimento": "1992-03-14",
	"status": true,
	"telefones": [
		"11911112222",
		"1122223333"
	]
}
```

## CI

O workflow fica em `.github/workflows/ci.yml` e executa:

- restore
- build
- testes

## Observacoes

- A persistencia e totalmente em memoria.
- Ao reiniciar a aplicacao, os dados sao perdidos.
- O endpoint `/health/ready` retorna `503` se o servico principal nao estiver registrado.

## Comandos uteis

```powershell
dotnet restore
dotnet build .\UsuariosApi.slnx
dotnet test .\UsuariosApi.slnx
dotnet run --project .\src\Usuarios.Api\Usuarios.Api.csproj
```
