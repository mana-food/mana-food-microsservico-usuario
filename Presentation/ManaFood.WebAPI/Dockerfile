# Etapa 1 -> Base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Etapa 2 -> Build
FROM mcr.microsoft.com/dotnet/sdk:9.0.301 AS build
WORKDIR /src
COPY . .
WORKDIR /src/Presentation/ManaFood.WebAPI
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Etapa 3 -> Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ManaFood.WebAPI.dll"]