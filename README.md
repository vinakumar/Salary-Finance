# FullStack.ApiClientGen

A production-grade full-stack solution demonstrating automated API client generation
using NSwag, .NET 8, and Angular 18.

## Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         FullStack.ApiClientGen                          │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌──────────────┐    ┌───────────────────┐    ┌──────────────────────┐ │
│  │ FullStack.UI │    │  FullStack.Api     │    │ FullStack.ApiClient  │ │
│  │ (Angular 18) │───▶│  (ASP.NET Core 8) │◀───│ (NSwag Generated)    │ │
│  │              │    │                    │    │                      │ │
│  │ • ng-openapi │    │ • Controllers      │    │ • Typed C# Client   │ │
│  │   -gen       │    │ • Minimal APIs     │    │ • Source Gen JSON    │ │
│  │ • TypeScript │    │ • MediatR CQRS     │    │ • Auto-generated    │ │
│  │   client     │    │ • FluentValidation │    │                      │ │
│  └──────────────┘    │ • Serilog          │    └──────────────────────┘ │
│                      │ • API Versioning   │                             │
│                      └─────────┬──────────┘                             │
│                                │                                        │
│                      ┌─────────▼──────────┐                             │
│                      │ FullStack.Domain    │                             │
│                      │                    │                             │
│                      │ • Entities         │                             │
│                      │ • Result<T> Monad  │                             │
│                      │ • Repository Iface │                             │
│                      │ • Value Objects    │                             │
│                      └────────────────────┘                             │
│                                                                         │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │ Tests                                                           │   │
│  │ • FullStack.Api.UnitTests (NUnit + Moq)                        │   │
│  │ • FullStack.Api.IntegTests (WebApplicationFactory + Playwright) │   │
│  └─────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────┘
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/) with npm
- [Angular CLI 18](https://angular.dev/) (`npm i -g @angular/cli@18`)
- [VS Code](https://code.visualstudio.com/) with recommended extensions

## Quick Start

```bash
# 1. Restore & build entire solution
dotnet build FullStack.ApiClientGen.sln

# 2. Run the API (starts at https://localhost:5001)
cd src/FullStack.Api
dotnet run

# 3. Run Angular UI (starts at http://localhost:4200)
cd src/FullStack.UI
npm install
npm start

# 4. Run all tests
dotnet test FullStack.ApiClientGen.sln
```

## VS Code Launch Configurations

| Configuration                     | Description                              |
|-----------------------------------|------------------------------------------|
| 🟢 API: FullStack.Api            | Launch API with debugger, opens Swagger  |
| 🔵 UI: Angular Dev Server        | Launch Angular dev server on port 4200   |
| 🧪 Tests: Backend Unit           | Run NUnit unit tests                     |
| 🧪 Tests: Backend Integration    | Run integration tests                    |
| 🚀 Full Stack (API + UI)         | Launch both API and Angular together     |

## Regenerate Clients

### C# Client (NSwag)

```bash
cd src/FullStack.ApiClient
dotnet nswag run nswag.json
```

### Angular TypeScript Client

```bash
cd src/FullStack.UI
npm run generate:api
```

## Design Decisions

### NSwag for Client Generation

NSwag was chosen over alternatives (AutoRest, Kiota) because:
- **Tight .NET integration** — runs as a dotnet tool, no Java/Node dependency at build time
- **Source-generation friendly** — produces clean C# that pairs well with System.Text.Json source generators
- **Dual output** — generates both C# and TypeScript clients from the same OpenAPI spec
- **Build-time generation** — integrates into MSBuild targets for CI/CD pipelines

### System.Text.Json Source Generation

Instead of runtime reflection-based serialization:
- **AOT-compatible** — supports Native AOT compilation for faster startup
- **Zero allocations** — compile-time generated serializers avoid boxing and reflection
- **Explicit type registration** — all serializable types are declared in `ApiJsonContext`, preventing accidental exposure
- **Faster cold start** — no JIT compilation of serializer code at runtime

### CQRS with MediatR

Command Query Responsibility Segregation:
- **Separation of concerns** — read and write operations have distinct models and handlers
- **Pipeline behaviours** — cross-cutting concerns (validation, logging, caching) are injected transparently
- **Testability** — each handler is a small, focused unit that can be tested in isolation
- **Scalability** — queries and commands can be independently optimized or scaled

### Result<T> Pattern

Instead of throwing exceptions for business logic flow:
- **Explicit error handling** — callers must handle failure cases; no hidden control flow
- **Typed errors** — `ResultErrorType` enum maps cleanly to HTTP status codes
- **Performance** — avoids exception overhead for expected failure paths (not found, validation)
- **Composability** — results can be chained and mapped functionally

## Project Structure

```
├── .vscode/                    # VS Code workspace configuration
├── docs/openapi/               # Generated OpenAPI spec (JSON)
├── src/
│   ├── FullStack.Api/          # ASP.NET Core 8 Web API
│   ├── FullStack.ApiClient/    # Auto-generated C# client (NSwag)
│   ├── FullStack.Domain/       # Domain entities, contracts, Result<T>
│   └── FullStack.UI/           # Angular 18 standalone app
├── tests/
│   ├── FullStack.Api.UnitTests/
│   └── FullStack.Api.IntegTests/
├── Directory.Build.props       # Shared MSBuild properties
├── .editorconfig               # Code style rules
└── FullStack.ApiClientGen.sln  # Solution file
```


