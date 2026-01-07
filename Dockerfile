# Etapa 1 -> Base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Etapa 2 -> Build
FROM mcr.microsoft.com/dotnet/sdk:9.0.301 AS build
WORKDIR /src

COPY ["Core/ManaFood.Domain/ManaFood.Domain.csproj", "Core/ManaFood.Domain/"]
COPY ["Core/ManaFood.Application/ManaFood.Application.csproj", "Core/ManaFood.Application/"]
COPY ["Infrastructure/ManaFood.Infrastructure/ManaFood.Infrastructure.csproj", "Infrastructure/ManaFood.Infrastructure/"]
COPY ["Presentation/ManaFood.WebAPI/ManaFood.WebAPI.csproj", "Presentation/ManaFood.WebAPI/"]

WORKDIR /src/Presentation/ManaFood.WebAPI
RUN dotnet restore

WORKDIR /src
COPY ["Core/", "Core/"]
COPY ["Infrastructure/", "Infrastructure/"]
COPY ["Presentation/", "Presentation/"]

WORKDIR /src/Presentation/ManaFood.WebAPI
RUN dotnet publish -c Release -o /app/publish

# Etapa 3 -> Final
FROM base AS final

# Criar usuário não-root
RUN groupadd -r appuser && useradd -r -g appuser appuser

WORKDIR /app
COPY --from=build /app/publish .

RUN chown -R appuser:appuser /app
USER appuser

ENTRYPOINT ["dotnet", "ManaFood.WebAPI.dll"]