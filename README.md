# EVALUACIÓN TÉCNICA NUXIBA

Prueba: **DESARROLLADOR JR**

Deadline: **1 día**

Nombre: Patricio Álvarez Hernández

---

# Requisitos previos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

- [.NET SDK 9](https://dotnet.microsoft.com/en-us/download)
- SQL Server
- SQL Server Management Studio / Azure Data Studio
- Visual Studio Code
- Docker
- Postman (opcional, para pruebas)

---

# 1. Levantar SQL Server

Este proyecto utiliza **SQL Server** como base de datos.

## Levantar contenedor con SQL Server

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd'    -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```

## Conectate con SQL Server Management o Azure Data Studio

Servidor: localhost, puerto 1433 - Usuario: sa - Contraseña: YourStrong!Passw0rd

# 2. Crear la base de datos con Entity Framework

Desde la carpeta del proyecto ejecutar los siguientes comandos:

```bash
dotnet restore
dotnet build
dotnet ef database update
```

# 3. Ejecutar la API

Una vez creada la base de datos, ejecutar:

```bash
dotnet run
```

# 4. Ejecutar API

Si todo sale correcto, la API quedara en una URL similar a:

```bash
http://localhost:5007
```

# 5. Probar API

## Postman

Se puede probar los endpoints desde postman con la URL

```bash
http://localhost:5007
```

# 6. Endpoints Disponibles

## Obtener todos los registros de login/logout

### GET /logins

Ejemplo:

```bash
http://localhost:5007/logins
```

## Crear un nuevo registro de login/logout

### POST /logins

Body de ejemplo:

```bash
{
"user_id": 1,
"extension": 1001,
"tipoMov": 1,
"fecha": "2026-03-27T08:00:00"
}
```

## Actualizar un registro existente

### PUT /logins/{id}

Ejemplo:

```bash
http://localhost:5007/logins/1
```

Body de ejemplo:

```bash
{
"id": 1,
"user_id": 1,
"extension": 1001,
"tipoMov": 0,
"fecha": "2026-03-27T18:00:00"
}
```

## Eliminar un registro

### DELETE /logins/{id}

Ejemplo:

```bash
http://localhost:5007/logins/1
```

# 7. Descargar el CSV generado

La API incluye un endpoint adicional para generar un archivo CSV con:

- Nombre de usuario (Login)
- Nombre completo
- Área
- Total de horas trabajadas

## Endpoint

```bash
GET /logins/export-csv
```

URL de ejemplo

```bash
http://localhost:5007/logins/export-csv
```

Al acceder a ese endpoint desde el navegador, se descargará automáticamente el archivo CSV generado.
