# Etapa 1: Construcción de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar los archivos del proyecto y restaurar dependencias
COPY ["MyPortalStudent/MyPortalStudent.csproj", "MyPortalStudent/"]
RUN dotnet restore "MyPortalStudent/MyPortalStudent.csproj"

# Copiar todo el código fuente y compilar en modo Release
COPY . .
WORKDIR "/app/MyPortalStudent"
RUN dotnet build "MyPortalStudent.csproj" -c Release -o /app/build

# Publicar la aplicación
RUN dotnet publish "MyPortalStudent.csproj" -c Release -o /app/publish

# Etapa 2: Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar la aplicación compilada desde la etapa de construcción
COPY --from=build /app/publish .

# Establecer la entrada del servicio Worker
ENTRYPOINT ["dotnet", "MyPortalStudent.dll"]
