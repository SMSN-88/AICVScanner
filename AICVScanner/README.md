# AI CV Scanner

Upload a CV, paste a job description, and get an AI-generated match score,
ATS score, missing skills and improvement suggestions.

**Live demo:** _(add the link after deploying)_

## Tech stack
- Blazor Web App (.NET 8, interactive server rendering)
- ASP.NET Core Identity (cookie authentication)
- Entity Framework Core + SQL Server
- OpenAI API (gpt-4o-mini)
- PdfPig for PDF text extraction

## Features
- Register / login
- Upload a CV (PDF or TXT)
- AI match score and ATS score
- Missing skills and keyword analysis
- Saved scan history

## Run locally
1. Install .NET 8 SDK and SQL Server Express
2. Clone the repo
3. Set the connection string in `appsettings.json`
4. Add your OpenAI key with user secrets:
   `dotnet user-secrets set "OpenAI:ApiKey" "your-key"`
5. Run `dotnet ef database update`, then `dotnet run`