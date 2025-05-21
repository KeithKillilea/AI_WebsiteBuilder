# AI_WebsiteBuilder

If you encounter a problem with running the program, it is likely due to the SQL Server database. Follow these 2 quick commands:

1. Open the NuGet Package Manager Console and enter this command: Add-Migration InitialCreate

2. Apply the Migration with this command: Update-Database

Now rebuild the project and it will work. 

## OpenAI Key Setup

To run this project locally, you must add your OpenAI API key, please run this command in your terminal window:

`dotnet user-secrets set "OpenAI:ApiKey" "sk-REPLACE_WITH_YOUR_KEY"`

[![.NET](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/dotnet.yml)
[![Label approved pull requests](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/approval-workflow.yml/badge.svg)](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/approval-workflow.yml)
[![Deploy to Azure Production](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/deploy-to-production.yml/badge.svg)](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/deploy-to-production.yml)
[![Deploy to Azure Staging](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/deploy-to-staging.yml/badge.svg)](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/deploy-to-staging.yml)
[![Super-Linter](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/linter.yml/badge.svg)](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/linter.yml)
[![CodeQL Analysis](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/KeithKillilea/AI_WebsiteBuilder/actions/workflows/codeql-analysis.yml)