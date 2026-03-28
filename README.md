
![Logo](banner.jpg)
# SolarMetrics - API em .NET

**SolarMetrics** é uma API desenvolvida para monitoramento e análise de energia solar, fornecendo dados em tempo real sobre sensores, usuários e ocupação de sistemas. A aplicação permite integrar sensores IoT, gerar relatórios e fornecer dados para aplicativos móveis ou dashboards web. Nosso objetivo é fornecer uma solução confiável para monitoramento inteligente de energia solar, auxiliando residências e empresas a otimizarem o consumo e gerarem insights a partir dos dados coletados.

## Novidades (Sprint 3 — .NET)

- **Health checks** (`Microsoft.Extensions.Diagnostics.HealthChecks`): verificação do banco via EF Core e de um **serviço HTTP externo** configurável (`HealthChecks:ExternalUrl`). Endpoints documentados abaixo.
- **Logging estruturado** com **Serilog** (console + arquivo em `logs/`), níveis configuráveis e **correlação de requisições** via header `X-Correlation-Id`.
- **OpenTelemetry**: tracing e métricas da API (ASP.NET Core, HTTP client, runtime), com exportação para **console** (adequado para desenvolvimento e demonstração).
- **Autenticação JWT** nos endpoints de cliente; em ambientes não produtivos use `POST /auth/token` para obter um bearer e testar no Swagger.
- **Testes automatizados**: projeto **SolarMetrics.UnitTests** (xUnit + Moq, padrão AAA) e **SolarMetrics.IntegrationTests** (`WebApplicationFactory`, SQLite em memória, JWT nos cenários protegidos).

## Monitoramento e health checks

| Rota | Descrição |
|------|-----------|
| `GET /health` | Todos os checks registrados (banco + URL externa em ambientes que não sejam `Test`). |
| `GET /health/ready` | Apenas checks com tag `ready` (adequado a readiness em orquestradores). |
| `GET /health/live` | Liveness (processo vivo; sem checks adicionais). |

Em **ambiente de testes** (`Test`), o check de URL externa é omitido para estabilidade da suíte de integração.

### Como observar traces e métricas

1. Execute a API: `dotnet run --project SolarMetrics.API`.
2. Gere tráfego (Swagger ou `curl`).
3. Observe o **console**: spans e métricas exportados pelo OpenTelemetry (exportador Console).
4. Opcional: use [`dotnet-counters`](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-counters) contra o processo da API para inspecionar contadores expostos pelo runtime (ex.: `System.Runtime`).

### Correlação de logs

Envie ou deixe o servidor gerar `X-Correlation-Id`. O valor aparece nos logs Serilog e é devolvido na resposta HTTP.

## Testes

```bash
# Toda a solução
dotnet test

# Somente unitários
dotnet test SolarMetrics.UnitTests/SolarMetrics.UnitTests.csproj

# Somente integração
dotnet test SolarMetrics.IntegrationTests/SolarMetrics.IntegrationTests.csproj
```

- **Unitários**: `MetodoTestado_Cenario_ResultadoEsperado` em `ClienteUseCase` (camada de aplicação) e `EmailFormatoRegra` (domínio).
- **Integração**: compartilham uma `WebApplicationFactory` via **Collection Fixture** xUnit; antes de cada teste o banco SQLite em memória é limpo para isolar cenários. Inclui **401** sem token, **204/201/404/409** com JWT.

## Novidades anteriores da aplicação

- Implementação de uma **interface Web** moderna para monitoramento e gestão.  
- Integração da **WebAPI** com a interface web, permitindo interação em tempo real.  
- Visualização de dados de sensores, relatórios e estatísticas diretamente no navegador.  

## Requisitos Funcionais

- Cadastro e gerenciamento de clientes e usuários.  
- Integração com sensores IoT para captura de dados de energia e ocupação.  
- Geração de relatórios detalhados sobre consumo, geração e ocupação.  
- Consulta de dados históricos e em tempo real.  
- Atualização e sincronização de informações entre backend e aplicativos móveis.

## Requisitos Não Funcionais

- **Desempenho:** O sistema deve processar e disponibilizar dados em tempo real com latência mínima.  
- **Segurança:** Proteção de dados sensíveis, autenticação e autorização de usuários.  
- **Disponibilidade:** Sistema deve possuir alta disponibilidade (mínimo 99,5%) para acesso contínuo.  
- **Escalabilidade:** Capacidade de suportar aumento no número de sensores e usuários sem perda de performance.  
- **Manutenibilidade:** Código e arquitetura organizados para facilitar atualizações e correções.  
- **Compatibilidade:** Suporte a múltiplos dispositivos móveis e integração com diferentes sensores IoT.  
- **Confiabilidade:** Garantir integridade dos dados capturados e armazenados, evitando perdas ou inconsistências.  

## Problemas que a aplicação resolve

- Falta de visibilidade sobre a geração e consumo de energia solar.  
- Dificuldade em monitorar ocupação e eficiência de sistemas.  
- Necessidade de relatórios detalhados para tomada de decisão.  
- Integração limitada entre diferentes sistemas de IoT e aplicações móveis.

## Sobre o time

- **Édipo Borges de Carvalho RM:567164**: Responsável pelo banco de dados e Compliance QA.  
- **Carlos Clementino RM:561187**: Responsável pelo desenvolvimento da API em .NET e Java Spring Boot, infraestrutura e práticas de DevOps, e pela integração com dispositivos IoT.  
- **Eder Silva RM:559647**: Responsável pela criação do APP mobile.

## Como rodar a aplicação

### Pré-requisitos

- .NET 8 SDK ou superior (repositório direcionado a `net8.0`; veja `global.json`)  
- IDE recomendada: **Rider** ou **Visual Studio**  
- Oracle Database (para execução completa contra o banco de curso; testes de integração usam SQLite em memória)

### Configuração sensível

Não commite senhas. Use **User Secrets** na API:

```bash
cd SolarMetrics.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:OracleDb" "sua_connection_string"
dotnet user-secrets set "Jwt:Key" "sua_chave_forte_com_pelo_menos_32_caracteres"
```

O arquivo `appsettings.json` contém apenas **placeholders** de exemplo.

### Passos para executar

1. Clone o repositório:  
```bash
git clone https://github.com/ARC-ceo/SolarMetrics-Dotnet.git
```

2. Abra a solução `SolarMetrics.sln`.

3. Execute a API:  
```bash
dotnet run --project SolarMetrics.API
```

4. A API costuma responder em `http://localhost:5090` (veja `launchSettings.json`).

### Testando a API

1. Obtenha um token (ambiente não produtivo): `POST /auth/token` (sem corpo).
2. No Swagger, use **Authorize** e informe `Bearer {access_token}`.
3. Documentação interativa: raiz da API em desenvolvimento (`/` → Swagger UI).

## Diagramas

### Modelo Físico

![Arquitetura](MER.png)

## Apresentação

Assista ao vídeo explicando a proposta tecnológica, o público-alvo e os problemas que a aplicação resolve:  
[Apresentação SolarMetrics](https://youtu.be/Fcza8JBvsyw)

## Endpoints da API

A API foi documentada com **Swagger / OpenAPI**, oferecendo exemplos completos de requisição e resposta.

### Endpoints principais

| Método | Endpoint       | Descrição                                    |
|--------|----------------|---------------------------------------------|
| POST   | /auth/token    | Obter JWT (indisponível em Produção)        |
| GET    | /Cliente       | Listar todos clientes cadastrados           |
| PUT    | /Cliente       | Atualizar cadastro do cliente               |
| POST   | /Cliente       | Criar cadastro do cliente                   |
| GET    | /Cliente/{id}  | Buscar cadastro do cliente                  |
| DELETE | /Cliente/{id}  | Deletar cadastro do cliente                 |
| GET    | /health*       | Health / ready / live                       |

> Para todos os endpoints de negócio, exemplos detalhados estão no **Swagger UI**.

## Tecnologias utilizadas

- .NET 8 / C#  
- ASP.NET Core Web API  
- Entity Framework Core  
- Oracle Database  
- Swagger / OpenAPI  
- Serilog, OpenTelemetry, xUnit, Moq  

---

**SolarMetrics** — Sua energia. Seu controle ☀️
