FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 👇 Копируем решение и ВСЕ .csproj (включая тесты!)
COPY ["CurrencyService.sln", "./"]

# UserService
COPY ["src/UserService.UserService.Domain/UserService.Domain.csproj", "src/UserService.UserService.Domain/"]
COPY ["src/UserService.UserService.Application/UserService.Application.csproj", "src/UserService.UserService.Application/"]
COPY ["src/UserService.UserService.Infrastructure/UserService.Infrastructure.csproj", "src/UserService.UserService.Infrastructure/"]
COPY ["src/UserService.UserService.Api/UserService.Api.csproj", "src/UserService.UserService.Api/"]

# FinanceService
COPY ["src/FinanceService.FinanceService.Domain/FinanceService.Domain.csproj", "src/FinanceService.FinanceService.Domain/"]
COPY ["src/FinanceService.FinanceService.Application/FinanceService.Application.csproj", "src/FinanceService.FinanceService.Application/"]
COPY ["src/FinanceService.FinanceService.Infrastructure/FinanceService.Infrastructure.csproj", "src/FinanceService.FinanceService.Infrastructure/"]
COPY ["src/FinanceService.FinanceService.Api/FinanceService.Api.csproj", "src/FinanceService.FinanceService.Api/"]

COPY ["src/ApiGateway/ApiGateway.csproj", "src/ApiGateway/"]
COPY ["src/CurrencyScraper/CurrencyScraper.csproj", "src/CurrencyScraper/"]


COPY ["tests/UserService.Tests/UserService.Tests.csproj", "tests/UserService.Tests/"]
COPY ["tests/FinanceService.Tests/FinanceService.Tests.csproj", "tests/FinanceService.Tests/"]

RUN dotnet restore
COPY . .

WORKDIR "/src/src/UserService.UserService.Api"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "UserService.Api.dll"]