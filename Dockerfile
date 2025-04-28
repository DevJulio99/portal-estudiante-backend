# Etapa 1: Construcción de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0-slim AS build
WORKDIR /app

# Instalar dependencias necesarias para SkiaSharp
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    libx11-6 \
    && rm -rf /var/lib/apt/lists/*

# Copiar los archivos del proyecto y restaurar dependencias
COPY ["MyPortalStudent.csproj", "."]
RUN dotnet restore "MyPortalStudent.csproj"

# Copiar todo el código fuente y compilar en modo Release
COPY . . 
RUN dotnet build "MyPortalStudent.csproj" -c Release -o /app/build

# Publicar la aplicación para Linux x64 y con la opción --self-contained
RUN dotnet publish "MyPortalStudent.csproj" -c Release -r linux-x64 --self-contained true -o /app/publish

# Etapa 2: Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0-slim AS final
WORKDIR /app

# Instalar las dependencias necesarias para SkiaSharp
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    libx11-6 \
    && rm -rf /var/lib/apt/lists/*

# Copiar la aplicación compilada desde la etapa de construcción
COPY --from=build /app/publish .

# Establecer la entrada del servicio Worker
ENTRYPOINT ["dotnet", "MyPortalStudent.dll"]
