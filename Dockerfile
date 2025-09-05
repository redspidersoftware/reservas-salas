# Imagen oficial de .NET 8 SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.sln .
COPY ReservasSalas/*.csproj ./ReservasSalas/
RUN dotnet restore

# Copiar el resto del código
COPY . .

# Publicar en modo Release
RUN dotnet publish -c Release -o out

# Imagen ligera de runtime para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/ReservasSalas/out .

# Arrancar la app
ENTRYPOINT ["dotnet", "ReservasSalas.dll"]
