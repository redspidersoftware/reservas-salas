# Etapa de build con .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY ReservasSalas/*.csproj ./ReservasSalas/
RUN dotnet restore ReservasSalas/ReservasSalas.csproj

# Copiar todo el código y compilar
COPY . .
WORKDIR /src/ReservasSalas
RUN dotnet publish -c Release -o /app/out

# Etapa de runtime más ligera
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ReservasSalas.dll"]
