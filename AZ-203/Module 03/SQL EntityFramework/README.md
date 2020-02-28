###### EntityFramework Core 3.1
# Speaking SQL with EntityFramework and C#

# Introduction
Before being able to migrate or scaffold you'll have to install the dotnet ef command-line tool. It's not longer included in the .NET core SDK.

> dotnet tool install --global dotnet-ef --version 3.1.0

Add a migration to your entity framework

> dotnet ef  migrations add CreateDatabase

Update the database

> dotnet ef database update
