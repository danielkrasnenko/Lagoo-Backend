# PhotoMarketplace BackEnd
Server part for Lagoo project.
Client part you can found here: https://github.com/Zilfatto/Lagoo-Frontend
## Project stack
* MS SQL Server
* .NET 6
* ASP.NET Core
* C# 10
## Note
You shouldn't create a database in MSSQL Server. A database will be created automatically.
## Setup
Install MSSQL Server.
Clone this repository to a folder.
For development mode create `appsettings.Development.json` in Lagoo.Api project with the following content:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Lagoo;User Id=<<YOUR_ACCOUNT_ID>>;Trusted_Connection=True;MultipleActiveResultSets=True;"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
```
You'll need to change SQL Server instance name if it defers with the one specified in all settings.
Optionally you can change the name of a database for this project in all settings

If you are a Windows user you can set 'Trusted_Connection=True' for bypassing password requirement.
Otherwise you'll need to add a password of your SQL Server instance to user secrets with the "MainDbPassword" key
How to do that navigate to https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0
Also you'll need to add a secret for JWT to user secrets with a "JwtAuth:Secret" key.

Finally, run this project.
And then run the frontend project.