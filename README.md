# About
My custom implementation of API to store files, based on API keys to operate with files

## Stack
Platform: .NET 9.0
ORM: EntityFramework Core
Auth*: JWT/API Key
DB: PostgreSQL
Logging: Serilog

## Features
- **Simple Auth***: simple implementation of authentication/authorization based on username and passwords
- **API keys with permissions**: supports creating/revoking multiple API keys with setup permissions to READ, WRITE and DELETE
- **Operate files**: upload, read and delete files

## In-Progress/Planned
- **Update API key**: update API key configuration, by editing permissions, name or etc.
- **Share file access**: supports to share user files with new list of permissions

## To Run
To run project you need to modify `appsettings.json`:
1. setup connection string to PostgreSQL
2. Setup JWT SecurityKey

Then apply migrations, with command:
```
dotnet ef database update -p Persistence -s Presentation
```