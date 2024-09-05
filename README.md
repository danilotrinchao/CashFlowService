# CashierService
# Projeto Cashier Service

Este projeto é um serviço de fluxo de caixa que permite registrar e consolidar transações financeiras diárias.

## Funcionalidades

- Registro de transações financeiras (crédito e débito).
- Consolidação de saldo diário com base nas transações registradas.
- Sistema de cache para otimização de consultas de saldos diários.

## Tecnologias Utilizadas

- **.NET 7**
- **C#**
- **PostgreSQL** (Banco de Dados)
- **Dapper** (ORM para acesso ao banco de dados)
- **MediatR** (Mediador para lidar com comandos e queries)
- **Microsoft.Extensions.Caching.Memory** (Cache em memória)

## Pré-requisitos

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [PostgreSQL](https://www.postgresql.org/download/) (se rodar o banco de dados localmente)

## Como Rodar a Aplicação

### 1. Clonando o Repositório

Clone este repositório para sua máquina local:

```bash
git clone https://github.com/seu-usuario/seu-repositorio.git
cd seu-repositorio


### Principais Pontos:
1. **Banco de dados na nuvem**: A aplicação está configurada para usar um banco na nuvem(heroku) por padrão, sem necessidade de ajustes locais.
2. **Rodar localmente** Realizar criação do banco de dados conforme as intruções a seguir:
- Execute o comando CREATE DATABASE cashier_servic;
- Ao concluir a criação do banco com exito, dentro da camada de Infra acesse a pasta Script e execute-o para criar a tabela necessária para o funcionamento do projeto.
- Ajuste a connectionstring em appsettings.json na camada Presentation com os dados de seu banco local.

