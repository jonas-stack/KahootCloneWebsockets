# Brug en officiel .NET runtime som base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Brug en officiel .NET SDK til build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopier projektfiler
COPY server/Api/Api.csproj server/Api/
COPY server/DataAccess/DataAccess.csproj server/DataAccess/
WORKDIR /src/server/Api
RUN dotnet restore

# Kopier hele koden
COPY . .

# Byg API-projektet
RUN dotnet build "Api.csproj" -c Release -o /app/build

# Publicer API-projektet
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

# Kør API'en
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
