FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Ledgr.sln .
COPY Ledgr.Domain/Ledgr.Domain.csproj           Ledgr.Domain/
COPY Ledgr.Application/Ledgr.Application.csproj Ledgr.Application/
COPY Ledgr.Infrastructure/Ledgr.Infrastructure.csproj Ledgr.Infrastructure/
COPY Ledgr.Api/Ledgr.Api.csproj                 Ledgr.Api/
COPY Ledgr.Tests/Ledgr.Tests.csproj             Ledgr.Tests/

RUN dotnet restore

COPY . .
RUN dotnet publish Ledgr.Api/Ledgr.Api.csproj \
    --no-restore -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Ledgr.Api.dll"]
