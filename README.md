# AI_WebsiteBuilder

If you encounter a problem with running the program, it is likely due to the SQL Server database. Follow these 2 quick commands:

1. Open the NuGet Package Manager Console and enter this command: Add-Migration InitialCreate

2. Apply the Migration with this command: Update-Database

Now rebuild the project and it will work. 

[![.NET](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/dotnet.yml)