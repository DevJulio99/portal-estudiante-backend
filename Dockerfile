# Etapa 1: Construcción de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

# Instalar dependencias necesarias para SkiaSharp en Alpine
RUN apk update && apk add --no-cache \
    fontconfig \
    freetype \
    libx11 \
    libxext \
    libxrender \
    libglvnd \
    atk \
    gdk-pixbuf \
    && rm -rf /var/cache/apk/*

# Copiar los archivos del proyecto y restaurar dependencias
COPY ["MyPortalStudent.csproj", "."]
RUN dotnet restore "MyPortalStudent.csproj"

# Copiar todo el código fuente y compilar en modo Release
COPY . . 
RUN dotnet build "MyPortalStudent.csproj" -c Release -o /app/build

# Publicar la aplicación
RUN dotnet publish "MyPortalStudent.csproj" -c Release -o /app/publish

# Etapa 2: Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Instalar dependencias necesarias para SkiaSharp en Alpine
RUN apk update && apk add --no-cache \
    fontconfig \
    freetype \
    libx11 \
    libxext \
    libxrender \
    libglvnd \
    atk \
    gdk-pixbuf \
    && rm -rf /var/cache/apk/*

# Copiar la aplicación compilada desde la etapa de construcción
COPY --from=build /app/publish . 

# Establecer la entrada del servicio Worker
ENTRYPOINT ["dotnet", "MyPortalStudent.dll"]
