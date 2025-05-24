# Sistema de Mapeamento Inteligente de Pátio Mottu

<div align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-purple" alt=".NET 8.0">
  <img src="https://img.shields.io/badge/ASP.NET_Core-8.0-blue" alt="ASP.NET Core 8.0">
  <img src="https://img.shields.io/badge/Entity_Framework-8.0-green" alt="Entity Framework 8.0">
  <img src="https://img.shields.io/badge/Oracle-Database-red" alt="Oracle Database">
  <img src="https://img.shields.io/badge/Swagger-OpenAPI-green" alt="Swagger OpenAPI">
</div>

## Grupo:
#### Daniel da Silva Barros | RM 556152
#### Luccas de Alencar Rufino | RM 558253
<br>

## 📝 Descrição do Projeto

A Mottu, líder em aluguel de motocicletas para entregadores de aplicativo, enfrenta desafios na localização manual de motos em seus pátios, causando perdas de frotas e retrabalhos operacionais. Este projeto implementa um Sistema de Mapeamento Inteligente de Pátio que integra:

**Visão Computacional:** captura e reconhecimento automático de motocicletas (placa, modelo).

**API REST:** camadas ASP.NET Core para gerenciar entidades críticas: Moto, Câmera, Usuário, Cargo, Registro e Reconhecimento.

**Auditoria:** Log de Alterações para rastrear modificações de dados.

**Documentação:** interface Swagger para explorar os endpoints interativamente.

## ⭐ Principais Benefícios

- Localização em segundos em vez de minutos
- Redução de perdas e erros de inventário
- Aumento de até 40% na produtividade operacional
- Escalabilidade para mais de 100 filiais
- Rastreabilidade e auditoria completa

## ⚙️ Tecnologias e Dependências

- **.NET 8.0** - Framework principal
- **ASP.NET Core 8.0** - API Web
- **Entity Framework Core 8.0** - ORM para acesso a dados
- **Oracle Database** - Banco de dados principal
- **Swagger/OpenAPI** - Documentação da API
- **C#** - Linguagem de programação

### Pacotes NuGet Utilizados:
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Oracle.EntityFrameworkCore" Version="8.21.121" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
```

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas com:

- **Controllers:** Endpoints REST da API
- **Services:** Lógica de negócios e processamento
- **Repositories:** Interação com o banco de dados Oracle
- **Models:** Entidades e objetos de transferência de dados (DTOs)
- **Configuration:** Configurações do ASP.NET Core e Entity Framework
- **Middleware:** Tratamento centralizado de exceções

## 📝 Entidades Principais

- **Moto:** Informações sobre as motocicletas (placa, modelo, status)
- **Câmera:** Dispositivos de captura instalados no pátio
- **Usuário:** Funcionários/operadores do sistema
- **Cargo:** Funções e permissões de usuários
- **Registro:** Histórico de movimentações de motos
- **Reconhecimento:** Detecções realizadas pelo sistema de visão computacional
- **LogAlterações:** Detecções de quando algo é atualizado manualmente

---

## 🚀 Como Executar

### ⚠️ **IMPORTANTE: Configuração do Banco de Dados**

Antes de executar o projeto, você deve configurar suas credenciais do Oracle no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=seuUsuario;Password=suaSenha;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));"
  }
}
```

**Substitua `seuUsuario` e `suaSenha` pelas suas credenciais reais do Oracle.**

### Executando o Projeto

**1. Clone o repositório:**
```bash
git clone [URL_DO_REPOSITORIO]
cd [NOME_DA_PASTA_DO_PROJETO]
```

**2. Inicialização completa (recomendado):**
```bash
dotnet restore
dotnet clean
dotnet build
dotnet ef database drop --force
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

**3. Ou execute diretamente no JetBrains Rider:**
- Abra o projeto no Rider
- Configure as credenciais do banco no `appsettings.json`
- Execute o projeto normalmente

**4. Acessar a API:**
- Base URL: `http://localhost:5000/index.html`

Também pode dar um ctrl + clique na url do terminal

## 📖 Documentação Swagger

Após iniciar a aplicação, acesse a documentação interativa:

**URL:** `http://localhost:5000/index.html`

Também pode dar um ctrl + clique na url do terminal

**Interativo:** explore e teste todos os endpoints disponíveis.

## 📜 Lista Completa de Endpoints REST

Todos os endpoints disponíveis na API:

| Entidades | Método | URL | Descrição |
|-----------|--------|-----|-----------|
| **Câmeras** | GET | `/api/cameras` | Listar câmeras (filtros: `?status=...&localizacao=...`, paginação) |
| | GET | `/api/cameras/{id}` | Buscar câmera por ID |
| | POST | `/api/cameras` | Criar nova câmera |
| | PUT | `/api/cameras/{id}` | Atualizar câmera existente |
| | DELETE | `/api/cameras/{id}` | Deletar câmera por ID |
| **Cargos** | GET | `/api/cargos` | Listar cargos (paginação/ordenação) |
| | GET | `/api/cargos/{id}` | Buscar cargo por ID |
| | POST | `/api/cargos` | Criar novo cargo |
| | PUT | `/api/cargos/{id}` | Atualizar cargo existente |
| | DELETE | `/api/cargos/{id}` | Deletar cargo por ID |
| | GET | `/api/cargos/nivel/{nivel}` | Buscar cargos por nível |
| | GET | `/api/cargos/search/{termo}` | Pesquisar cargos |
| **LogAlterações** | GET | `/api/logs` | Listar logs de alterações (filtros/data, paginação) |
| | GET | `/api/logs/{id}` | Buscar log de alteração por ID |
| | POST | `/api/logs` | Criar novo log de alteração |
| **Motos** | GET | `/api/motos` | Listar motos (filtros: `?marca=...&modelo=...`, paginação/ordenação) |
| | GET | `/api/motos/{id}` | Buscar moto por ID |
| | POST | `/api/motos` | Criar nova moto |
| | PUT | `/api/motos/{id}` | Atualizar moto existente |
| | DELETE | `/api/motos/{id}` | Deletar moto por ID |
| | GET | `/api/motos/placa/{placa}` | Buscar moto por placa |
| | GET | `/api/motos/presentes` | Listar motos presentes no pátio |
| **Reconhecimentos** | GET | `/api/reconhecimentos` | Listar reconhecimentos (filtros/data, paginação) |
| | GET | `/api/reconhecimentos/{id}` | Buscar reconhecimento por ID |
| | POST | `/api/reconhecimentos` | Criar novo reconhecimento automático |
| | PUT | `/api/reconhecimentos/{id}` | Atualizar reconhecimento existente |
| | DELETE | `/api/reconhecimentos/{id}` | Deletar reconhecimento por ID |
| **Registros** | GET | `/api/registros` | Listar registros de entrada/saída (filtros/data, paginação) |
| | GET | `/api/registros/{id}` | Buscar registro por ID |
| | POST | `/api/registros` | Criar novo registro manual ou automático |
| | PUT | `/api/registros/{id}` | Atualizar registro existente |
| | DELETE | `/api/registros/{id}` | Deletar registro por ID |
| **Usuários** | GET | `/api/usuarios` | Listar usuários (paginação/ordenação) |
| | GET | `/api/usuarios/{id}` | Buscar usuário por ID |
| | POST | `/api/usuarios` | Criar novo usuário |
| | PUT | `/api/usuarios/{id}` | Atualizar usuário existente |
| | DELETE | `/api/usuarios/{id}` | Deletar usuário por ID |

### Parâmetros de Consulta Comuns:
- `?page=0&size=10&sort=id,desc` para paginação/ordenação
- Filtros específicos disponíveis conforme cada endpoint


## 📋 Pré-requisitos

- .NET 8.0 SDK
- Acesso ao banco Oracle (oracle.fiap.com.br)
- Credenciais válidas do Oracle
- JetBrains Rider ou Visual Studio (opcional)

---

Desenvolvido com 💻 e ♥️ por Daniel Barros e Luccas Rufino.
