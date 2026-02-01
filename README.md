# Desafio Umbler

Esta é uma aplicação web que recebe um domínio e mostra suas informações de DNS.

Este é um exemplo real de sistema que utilizamos na Umbler.

Ex: Consultar os dados de registro do dominio `umbler.com`

**Retorno:**
- Name servers (ns254.umbler.com)
- IP do registro A (177.55.66.99)
- Empresa que está hospedado (Umbler)

Essas informações são descobertas através de consultas nos servidores DNS e de WHOIS.

*Obs: WHOIS (pronuncia-se "ruís") é um protocolo específico para consultar informações de contato e DNS de domínios na internet.*

Nesta aplicação, os dados obtidos são salvos em um banco de dados, evitando uma segunda consulta desnecessaria, caso seu TTL ainda não tenha expirado.

*Obs: O TTL é um valor em um registro DNS que determina o número de segundos antes que alterações subsequentes no registro sejam efetuadas. Ou seja, usamos este valor para determinar quando uma informação está velha e deve ser renovada.*

## Tecnologias Utilizadas

### Backend
- C# (.NET 6.0)
- ASP.NET Core
- MySQL
- Entity Framework Core
- Blazor Server

### Frontend
- Blazor Server Components
- CSS3 com animações e gradientes
- Design responsivo

### Testes
- MSTest
- Moq (para mocking)
- Entity Framework InMemory Database

## Pré-requisitos

Para rodar o projeto você vai precisar instalar:

- .NET Core SDK 6.0 ou superior (https://www.microsoft.com/net/download)
- Um editor de código, recomendamos o Visual Studio ou Visual Studio Code (https://code.visualstudio.com/)
- Um banco de dados MySQL (você pode rodar localmente ou criar um site PHP gratuitamente no app da Umbler https://app.umbler.com/ que oferece o banco MySQL adicionalmente)

## Como Executar

### 1. Configurar o Banco de Dados

Edite o arquivo `appsettings.json` com suas credenciais do MySQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=desafio_umbler;Uid=seu_usuario;Pwd=sua_senha"
  }
}
```

### 2. Executar as Migrations

```bash
dotnet tool update --global dotnet-ef
dotnet ef database update
```

### 3. Executar o Projeto

```bash
dotnet run
```

Ou clique em "play" no editor do Visual Studio Code.

O projeto estará disponível em `https://localhost:5001` ou `http://localhost:5000`.

## Objetivos do Desafio

O projeto original já estava funcional, mas havia vários pontos de melhoria identificados. Abaixo estão todos os requisitos e o status de implementação:

---

# Modificações Realizadas

## ✅ Frontend

### 1. Dados Formatados e Legíveis ✅ **IMPLEMENTADO**
**Requisito Original:** Os dados retornados não estavam formatados e precisavam ser apresentados de forma legível.

**Solução Implementada:**
- Criado componente Blazor `DomainSearch.razor` com interface moderna e profissional
- Cards com design limpo e organizado
- Seções separadas para informações DNS e WHOIS
- Tabelas estilizadas com labels e valores bem definidos
- Container scrollável para dados WHOIS extensos
- Cores e tipografia alinhadas ao design da Umbler
- Animações suaves de entrada (fade-in)
- Layout responsivo para dispositivos móveis

### 2. Validação no Frontend ✅ **IMPLEMENTADO**
**Requisito Original:** Não havia validação no frontend permitindo requisições inválidas (ex: domínio sem extensão).

**Solução Implementada:**
- Implementada classe `DomainValidator` no componente Blazor
- Validação em tempo real antes de enviar requisição ao servidor
- Mensagens de erro claras e amigáveis
- Validação também no evento de tecla Enter
- Feedback visual imediato para o usuário

### 3. Framework Moderno (Blazor) ✅ **IMPLEMENTADO**
**Requisito Original:** Estava sendo utilizado "vanilla-js" apesar do webpack configurado. O ideal seria usar ReactJs ou Blazor.

**Solução Implementada:**
- **Migração completa de JavaScript vanilla para Blazor Server**
- Componente `DomainSearch.razor` substitui completamente o código JavaScript anterior
- Integração com ASP.NET Core MVC
- Comunicação com API através de serviço HTTP client
- Estado reativo e atualização automática da UI
- Melhor integração entre frontend e backend

**Arquivos Criados:**
- `Components/DomainSearch.razor` - Componente principal de busca
- `Components/_Imports.razor` - Imports globais para componentes
- `Services/DomainApiService.cs` - Serviço HTTP client para comunicação com API

---

## ✅ Backend

### 1. Validação no Backend ✅ **IMPLEMENTADO**
**Requisito Original:** Não havia validação no backend permitindo requisições inválidas, causando exceptions (erro 500).

**Solução Implementada:**
- Validação implementada no `DomainController` usando `DomainValidator.IsValid()`
- Retorno de `BadRequest (400)` para domínios inválidos em vez de erro 500
- Validação também no `DomainService` como camada adicional de segurança
- Mensagens de erro padronizadas e informativas
- Tratamento adequado de exceções com códigos HTTP corretos

### 2. Arquitetura em Camadas ✅ **IMPLEMENTADO**
**Requisito Original:** A complexidade ciclomática do controller estava muito alta.

**Solução Implementada:**
- **Controller simplificado:** `DomainController` agora apenas coordena a requisição
- **Camada de Serviço:** Toda lógica de negócio movida para `DomainService`
- **Separação de responsabilidades:**
  - Controller: Validação básica e retorno HTTP
  - Service: Lógica de negócio, cache, consultas externas
  - Validators: Validação de domínios
  - DTOs: Transferência de dados
- **Injeção de dependência:** Uso de interfaces para facilitar testes e manutenção

**Estrutura:**
```
Controllers/
  └── DomainController.cs (simplificado)
Services/
  ├── IDomainService.cs
  ├── DomainService.cs (lógica de negócio)
  ├── IDomainApiService.cs
  └── DomainApiService.cs (HTTP client)
Validators/
  └── DomainValidator.cs
Dtos/
  └── DomainDto.cs
```

### 3. Uso de DTO (Data Transfer Object) ✅ **IMPLEMENTADO**
**Requisito Original:** O controller retornava a entidade `Domain` diretamente, expondo propriedades desnecessárias como `Id`, `Ttl` e `UpdatedAt`.

**Solução Implementada:**
- Criado `DomainDto` contendo apenas as propriedades necessárias para o cliente
- Mapeamento de `Domain` para `DomainDto` no `DomainService`
- Controller retorna apenas o DTO, não a entidade
- Propriedades expostas: `Name`, `Ip`, `HostedAt`, `NameServers`, `WhoIs`
- Propriedades ocultas: `Id`, `Ttl`, `UpdatedAt`

---

## ✅ Testes

### 1. Cobertura de Testes e Mocking ✅ **IMPLEMENTADO**
**Requisito Original:** Cobertura de testes muito baixa e impossibilidade de testar o controller por falta de mocking.

**Solução Implementada:**
- **Testes do Controller:** `DomainController` totalmente testável com mocks
- **Testes do Service:** `DomainService` testado com mocks de `IWhoisClient` e `IDnsClient`
- **Testes do API Service:** Criados testes para `DomainApiService`
- **Uso de Moq:** Todas as dependências externas são mockadas
- **InMemory Database:** Banco de dados mockado para testes isolados

**Testes Implementados:**
- ✅ `DomainController_Get_InvalidDomain_ReturnsBadRequest`
- ✅ `DomainController_Get_ValidDomain_ReturnsOk`
- ✅ `DomainService_GetDomainAsync_ReturnsCachedDomain_WhenTtlNotExpired`
- ✅ `DomainService_GetDomainAsync_QueriesExternalServices_WhenTtlExpired`
- ✅ `DomainService_GetDomainAsync_QueriesExternalServices_WhenDomainNotInDatabase`
- ✅ `Domain_Moking_WhoisClient`
- ✅ `Domain_Moking_DnsClient`
- ✅ `DomainApiService_GetDomainAsync_ValidDomain_ReturnsDomainDto`
- ✅ `DomainApiService_GetDomainAsync_InvalidDomain_ThrowsDomainValidationException`
- ✅ `DomainApiService_GetDomainAsync_NotFound_ThrowsDomainNotFoundException`
- ✅ `DomainApiService_GetDomainAsync_ServerError_ThrowsDomainServiceException`

### 2. Mocking de Whois e DNS ✅ **IMPLEMENTADO**
**Requisito Original:** Banco de dados já estava mockado, mas consultas Whois e DNS não.

**Solução Implementada:**
- Criadas interfaces `IWhoisClient` e `IDnsClient`
- Implementações wrapper: `WhoisClientWrapper` e `DnsClientWrapper`
- Todos os testes usam mocks dessas interfaces
- Verificação de chamadas usando `Moq.Verify()`
- Testes isolados sem dependências externas reais

---

## 🎨 Melhorias de Design e UX

### Design Moderno Alinhado à Umbler
- **Gradiente roxo/azul** no header seguindo identidade visual da Umbler
- **Cards com sombras** e bordas arredondadas
- **Animações suaves** de entrada e hover
- **Cores consistentes:** Verde para ações, roxo/azul para headers
- **Tipografia:** Uso da fonte Lato já carregada

### Responsividade
- Layout adaptável para mobile
- Botões e inputs em largura total em telas pequenas
- Informações empilhadas verticalmente quando necessário
- Espaçamentos ajustados por breakpoint

### Experiência do Usuário
- **Loading states:** Spinner animado durante consultas
- **Feedback visual:** Mensagens de erro estilizadas
- **Validação em tempo real:** Feedback imediato
- **Transições suaves:** Animações CSS para melhor percepção
- **Scrollbar customizada:** Para melhor visualização do WHOIS

---

## 📁 Estrutura do Projeto

```
Desafio.Umbler/
├── Components/
│   ├── _Imports.razor          # Imports globais Blazor
│   └── DomainSearch.razor      # Componente principal de busca
├── Controllers/
│   ├── DomainController.cs     # API Controller (simplificado)
│   └── HomeController.cs        # Controller MVC
├── Services/
│   ├── IDomainService.cs       # Interface do serviço de domínio
│   ├── DomainService.cs        # Lógica de negócio
│   ├── IDomainApiService.cs    # Interface do serviço HTTP
│   ├── DomainApiService.cs     # Serviço HTTP client
│   ├── IWhoisClient.cs         # Interface Whois
│   ├── WhoisClientWrapper.cs   # Wrapper Whois
│   ├── IDnsClient.cs           # Interface DNS
│   └── DnsClientWrapper.cs     # Wrapper DNS
├── Dtos/
│   └── DomainDto.cs            # Data Transfer Object
├── Models/
│   ├── Domain.cs               # Entidade de domínio
│   └── DatabaseContext.cs      # Contexto EF Core
├── Validators/
│   └── DomainValidator.cs     # Validação de domínios
├── Views/
│   ├── Home/
│   │   └── Index.cshtml        # View principal (usa Blazor)
│   └── Shared/
│       └── _Layout.cshtml      # Layout com rodapé dinâmico
└── wwwroot/
    └── css/
        └── site.css            # Estilos customizados

Desafio.Umbler.Test/
└── ControllersTests.cs         # Testes unitários completos
    └── DomainApiServiceTests.cs # Testes do serviço HTTP
```

---

## 🔍 Análise de Requisitos Atendidos

### Resumo de Atendimento

| Requisito | Status | Detalhes |
|-----------|--------|----------|
| **Frontend - Dados formatados** | ✅ | Componente Blazor com cards e tabelas estilizadas |
| **Frontend - Validação** | ✅ | Validação em tempo real no componente |
| **Frontend - Framework moderno** | ✅ | Migração completa para Blazor Server |
| **Backend - Validação** | ✅ | Validação no controller e service |
| **Backend - Arquitetura em camadas** | ✅ | Controller simplificado, lógica no service |
| **Backend - DTO** | ✅ | DomainDto implementado e usado |
| **Testes - Cobertura** | ✅ | 11 testes unitários implementados |
| **Testes - Mocking** | ✅ | Whois e DNS totalmente mockados |

**Total:** 8/8 requisitos atendidos (100%)

---

## 🚀 Como Executar os Testes

```bash
cd src/Desafio.Umbler.Test
dotnet test
```

---

## 📝 Notas Técnicas

### Blazor Server vs Blazor WebAssembly
- Escolhido **Blazor Server** para melhor integração com ASP.NET Core MVC existente
- Comunicação em tempo real via SignalR
- Menor tamanho de download para o cliente
- Melhor performance inicial

### Cache e TTL
- Sistema de cache implementado usando TTL dos registros DNS
- Domínios consultados são armazenados no banco
- Consultas externas apenas quando TTL expira
- Redução significativa de chamadas externas

### Tratamento de Erros
- Exceções customizadas: `DomainNotFoundException`, `DomainValidationException`, `DomainServiceException`
- Códigos HTTP apropriados: 400 (Bad Request), 404 (Not Found), 500 (Internal Server Error)
- Mensagens de erro claras e informativas
