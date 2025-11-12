 VindiBank API

API REST desenvolvida em .NET 8 para gerenciamento de contas bancárias, permitindo criação, consulta, desativação e transferência de saldo entre contas.

 Visão Geral

A VindiBank API simula o fluxo operacional de um sistema bancário simplificado, com suporte a:

Criação de novas contas com saldo inicial padrão.

Consulta de contas por nome ou documento.

Desativação lógica de contas.

Transferência entre contas com controle de transação e verificação de saldo.

 Tecnologias Utilizadas

.NET 8 / C#

ASP.NET Core Web API

Dapper + Npgsql (acesso ao PostgreSQL)

PostgreSQL 12+

Dependency Injection

Transações e locks (SELECT ... FOR UPDATE)

Arquitetura em camadas (Controller → Service → Repository)

 Estrutura de Banco de Dados
CREATE SEQUENCE IF NOT EXISTS id_conta_vindi_seq START 1;

CREATE TABLE IF NOT EXISTS public.es_vindiclientes_table (
    id_conta_vindi_int BIGINT PRIMARY KEY DEFAULT nextval('id_conta_vindi_seq'),
    nome_cliente_str VARCHAR(999) NOT NULL,
    documento_str VARCHAR(20) NOT NULL,
    tipo_conta_str VARCHAR(50) NOT NULL,
    saldo_dec NUMERIC(18,2) NOT NULL DEFAULT 1000,
    data_abertura_conta_tstamp TIMESTAMP NOT NULL DEFAULT now(),
    status_conta_bit BIT(1) NOT NULL DEFAULT B'1',
    CONSTRAINT uq_documento UNIQUE (documento_str)
);


Cada conta possui saldo inicial padrão de 1000, definido tanto na tabela quanto na model.

 Endpoints Principais
Método	Rota	Descrição	Corpo da Requisição	Retorno
POST	/api/controller/CreateAccount	Cria nova conta	AccountVindi	200 / 400
GET	/api/controller/GetAccounts	Busca conta(s) por documento ou nome	Query params	AccountVindi
DELETE	/api/controller/AccountDeactivation	Desativa conta	DeleteAccount	200 / 404
PUT	/api/controller/AccountTransfer	Transfere saldo entre contas	TransferBetweenAccounts	200 / 400
🧾 Exemplo de Uso
Criar conta
POST /api/controller/CreateAccount
Content-Type: application/json

{
  "NomeCliente": "Maria Oliveira",
  "Documento": "12345678900",
  "TipoConta": "PF"
}

Transferência entre contas
PUT /api/controller/AccountTransfer
Content-Type: application/json

{
  "documentoSaida": "12345678900",
  "documentoEntrada": "98765432100",
  "saldoDocumentoSaida": 250
}

 Regras Importantes

Contas inativas não podem transferir ou receber valores.

Conta origem e destino não podem ser a mesma.

O valor transferido deve ser maior que zero e menor ou igual ao saldo disponível.

A transferência é atômica (uso de transação e rollback automático em erro).

A tabela possui constraint de unicidade por documento (uq_documento).

 Execução Local

Configure o PostgreSQL e crie o banco vindibank.

Ajuste a connection string no appsettings.Development.json:

"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=vindibank;Username=postgres;Password=postgres"
}


Execute:

dotnet build
dotnet run


API disponível em https://localhost:5001.

 Estrutura do Projeto
VindiBank/
 ├── Controllers/
 │    └── HomeController.cs
 ├── Services/
 │    └── FluxoAPIControll.cs
 ├── Repository/
 │    └── RequisicoesRepository.cs
 ├── Models/
 │    ├── AccountVindi.cs
 │    ├── TransferBetweenAccounts.cs
 │    └── DeleteAccount.cs
 ├── Program.cs
 └── appsettings.Development.json

 Observações Técnicas

FluxoAPIControll atua como roteador lógico entre o controller e os métodos do repositório.

Cada operação é registrada via enum MetodoAPI, garantindo padronização.

A validação de erro é centralizada: se o retorno contiver "Erro:", é lançada uma exceção.

O saldo padrão é configurado diretamente no modelo e na base de dados, garantindo consistência.

 Próximos Passos

Adicionar Swagger para documentação automática.

Implementar logs estruturados (Serilog / ELK).

Criar camada de testes unitários e integração.

Adicionar suporte a autenticação JWT.

📄 Licença

Projeto de estudo / demonstração técnica.
© 2025 — Todos os direitos reservados.
