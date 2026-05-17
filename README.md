# CRM System

A Customer Relationship Management (CRM) system built with a .NET 10 Minimal API and an Azure Function, backed by Azure Cosmos DB. This project demonstrates a decoupled architecture where the API handles data operations, and an Azure Function reacts to database changes to trigger asynchronous email notifications.

---

## Project Structure

* **`api/`**: An ASP.NET Core Minimal API handling all CRUD operations and search functionality for Customer and Sales Representative records.
* **`azfunc/`**: An Azure Functions worker project containing a Cosmos DB trigger that sends automated email notifications to Sales Representatives when their assigned customer records are created or updated.

---

## Features

* **RESTful Minimal API**: Create, Read, Update, Patch, and Delete customer records.
* **Text Search**: Search for customers by their name or their responsible sales representative's name (diacritic and case-insensitive).
* **Automated Database Seeding**: Automatically seeds initial data on first run when the Cosmos DB container is created.
* **Event-Driven Notifications**: A Cosmos DB Change Feed trigger automatically emails sales representatives about changes to their customers using MailKit/SMTP.

---

## Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* [Azure Cosmos DB Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator) (for local development)
* An SMTP Server account (e.g., SendGrid, Mailtrap) for testing email notifications.

---

## Configuration

### API (`api/appsettings.Development.json`)
Configure your Cosmos DB connection string and database details:
```json
{
  "CosmosDb": {
    "Endpoint": "https://localhost:8081/",
    "Key": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "DatabaseName": "CrmDatabase",
    "ContainerName": "Customers"
  }
}
```
_(Example with CosmosDB Emulator )_

### Azure Function (`azfunc/local.settings.json`)

Provide the connection string for Cosmos DB and your SMTP credentials:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDbConnection": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "DatabaseName": "CrmDatabase",
    "ContainerName": "Customers",
    "SmtpHost": "smtp.provider.com",
    "SmtpPort": "2525",
    "SmtpUsername": "your_username",
    "SmtpPassword": "your_password/app_token"
  }
}
```

---

## Running the Application

1. If using **Azure Cosmos DB Emulator**, ensure it is already running locally.
2. Start the API project:
```bash
[user@host ./api]$ dotnet run

```


*The API can be accessed and tested via the Scalar API reference at `http://localhost:5255/scalar/v1`.*
3. In a separate terminal, start the Azure Function:
```bash
[user@host ./azfunc]$ func start