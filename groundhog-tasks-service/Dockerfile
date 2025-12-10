# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el código y construir
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

# Exponer puerto
EXPOSE 8080

# Variable de entorno para producción
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Comando para ejecutar la API
ENTRYPOINT ["dotnet", "groundhog-tasks-service.dll"]
