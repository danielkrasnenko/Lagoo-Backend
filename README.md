# Lagoo Backend
Server part for Lagoo project.
Client part you can found here: https://github.com/danielkrasnenko/Lagoo-Frontend

## Project stack
* PostgreSQL
* .NET 8
* ASP.NET Core
* C# 12

## Note
You shouldn't create a database in Postgres. The database will be created automatically.

## Getting started

- Create `appsettings.Development.json` file and paste it into `Lagoo.Api` project. Then add the following content into it.
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=lagoo_db;User Id=postgres;Password=postgres;Include Error Detail=True;"
  },
  "JwtAuth": {
    "Secret": "<GENERATED-SECRET>"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
```

## How to launch the app
1. Generate Secret for JWT
2. Spin up infrastructure by executing **run.infra.cmd**
3. Now you can run the app itself.