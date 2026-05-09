# ⚙️ NorthwindTradersApp - Setup & Installation Guide

## 📋 Requisitos Previos

### **Sistema Operativo**
- Windows 10/11 o superior
- macOS 11+ o Linux (con .NET SDK)

### **Software Requerido**
- **.NET 10 SDK** (Descargar desde [dotnet.microsoft.com](https://dotnet.microsoft.com/download))
- **SQL Server 2019+** (Express, Standard o Developer Edition)
  - O: **SQL Server LocalDB** (incluido con Visual Studio)
- **Visual Studio 2022** (recomendado) o **VS Code**
- **Git** (para clonar el repositorio)

### **Verificar Instalación**

```powershell
# Verificar .NET
dotnet --version

# Verificar SQL Server (si está instalado)
sqlcmd -S . -Q "SELECT @@VERSION"
```

---

## 🔧 Instalación Paso a Paso

### **Paso 1: Clonar el Repositorio**

```bash
git clone https://github.com/tu-usuario/NorthwindTradersApp.git
cd NorthwindTradersApp
```

### **Paso 2: Configurar la Base de Datos**

#### **Opción A: SQL Server LocalDB (Recomendado para desarrollo)**

LocalDB es una versión ligera de SQL Server ideal para desarrollo.

**Windows:**
```powershell
# Crear una instancia LocalDB
sqllocaldb create MSSQLLocalDB

# Iniciar la instancia
sqllocaldb start MSSQLLocalDB

# Verificar estado
sqllocaldb info MSSQLLocalDB
```

**Restaurar Base de Datos Northwind:**

```sql
-- Descargar el script de Northwind desde Microsoft
-- https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/

-- Ejecutar el script desde SQL Management Studio o sqlcmd
sqlcmd -S (localdb)\mssqllocaldb -i instnwnd.sql
```

#### **Opción B: SQL Server Completo**

1. Instalar SQL Server 2019 o superior
2. Restaurar la base de datos Northwind:
   - Abrir SQL Server Management Studio
   - Descargar el backup de Northwind
   - Restore > From Device > Seleccionar archivo .bak

### **Paso 3: Configurar Connection String**

Editar `appsettings.json` en la carpeta `src/NorthwindTradersApp.Api`:

**Para LocalDB:**
```json
{
  "ConnectionStrings": {
    "NorthwindDatabase": "Server=(localdb)\\mssqllocaldb;Database=Northwind;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

**Para SQL Server Named Instance:**
```json
{
  "ConnectionStrings": {
    "NorthwindDatabase": "Server=YourServerName\\SQLEXPRESS;Database=Northwind;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**Para SQL Server con Autenticación SQL:**
```json
{
  "ConnectionStrings": {
    "NorthwindDatabase": "Server=localhost;Database=Northwind;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
  }
}
```

### **Paso 4: Restaurar Dependencias**

Navegar a la carpeta raíz del proyecto:

```bash
# Restaurar paquetes NuGet
dotnet restore

# O, desde Visual Studio
# Tools > NuGet Package Manager > Package Manager Console > Update-Package
```

### **Paso 5: Compilar la Solución**

```bash
# Compilar en Debug (por defecto)
dotnet build

# Compilar en Release
dotnet build --configuration Release

# O desde Visual Studio
# Build > Build Solution (Ctrl+Shift+B)
```

### **Paso 6: Ejecutar la API**

```bash
# Desde la carpeta del proyecto API
cd src/NorthwindTradersApp.Api

# Ejecutar
dotnet run

# La API estará disponible en:
# http://localhost:5000
# Swagger UI: http://localhost:5000/swagger/index.html
```

**Alternatively, usando Visual Studio:**
1. Click derecho en `NorthwindTradersApp.Api`
2. Set as Startup Project
3. Presionar F5 o Ctrl+F5

---

## 📦 Estructura de Proyectos

```
NorthwindTradersApp/
├── src/
│   ├── NorthwindTradersApp.Api/              # Capa de Presentación
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── NorthwindTradersApp.Api.csproj
│   │   └── Properties/
│   │       └── launchSettings.json
│   │
│   ├── NorthwindTradersApp.Application/      # Capa de Aplicación (Servicios)
│   │   ├── DependencyInjection.cs
│   │   ├── NorthwindTradersApp.Application.csproj
│   │   └── Services/
│   │       ├── OrdersService.cs
│   │       ├── ProductService.cs
│   │       └── CustomersService.cs
│   │
│   ├── NorthwindTradersApp.Domain/           # Capa de Dominio (Entidades)
│   │   ├── NorthwindTradersApp.Domain.csproj
│   │   ├── Contracts/
│   │   │   ├── Contracts.cs
│   │   │   ├── OrderDto.cs
│   │   │   ├── ProductDto.cs
│   │   │   ├── CustomerDto.cs
│   │   │   └── EmployeeDto.cs
│   │   └── NorthwindDbEntities/
│   │       ├── Order.cs
│   │       ├── OrderDetail.cs
│   │       ├── Product.cs
│   │       ├── Customer.cs
│   │       ├── Employee.cs
│   │       ├── Category.cs
│   │       ├── Supplier.cs
│   │       ├── Shipper.cs
│   │       ├── Region.cs
│   │       ├── Territory.cs
│   │       └── EmployeeTerritory.cs
│   │
│   └── NorthwindTradersApp.Infrastructure/   # Capa de Infraestructura (Datos)
│       ├── DependencyInjection.cs
│       ├── NorthwindTradersApp.Infrastructure.csproj
│       ├── Persistence/
│       │   └── NorthwindDbContext.cs
│       └── Repositories/
│           ├── OrdersRepository.cs
│           ├── ProductsRepository.cs
│           └── CustomersRepository.cs
│
├── frontend/                                 # Aplicación Frontend (Quasar)
│   └── quasar-project/
│       ├── package.json
│       ├── quasar.config.js
│       └── src/
│           ├── pages/
│           ├── components/
│           └── services/
│
├── NorthwindTradersApp.slnx                 # Archivo de solución
└── README.md
```

---

## 🗄️ Verifying Database Connection

Para verificar que la conexión a BD está funcionando:

**Opción 1: Mediante SQL Server Management Studio**
```
Server: (localdb)\mssqllocaldb
Database: Northwind
```

**Opción 2: Mediante código**
```csharp
// En Program.cs, verificar que DbContext se carga sin errores
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<NorthwindDbContext>();
await dbContext.Database.CanConnectAsync();
```

**Opción 3: Usar la API Health Check**
```bash
curl http://localhost:5000/health
```

---

## 🚀 Desarrollo Local

### **Ejecutar en Modo Development**

```bash
# Asegurar que está en modo Development
$env:ASPNETCORE_ENVIRONMENT = "Development"

# O en Linux/macOS
export ASPNETCORE_ENVIRONMENT=Development

# Ejecutar con hot reload
dotnet watch run
```

### **Swagger UI para Testing**

Una vez que la API está corriendo, abrir:
```
http://localhost:5000/swagger/index.html
```

Desde aquí puedes:
- Ver todos los endpoints disponibles
- Leer documentación automática
- Hacer requests de prueba directamente

### **HTTP Requests File**

Usar el archivo [NorthwindTradersApp.Api.http](../src/NorthwindTradersApp.Api/NorthwindTradersApp.Api.http) en VS Code:

1. Instalar la extensión **REST Client**
2. Abrir el archivo .http
3. Hacer click en "Send Request" sobre cada endpoint

---

## 🔗 NuGet Packages

Paquetes principales utilizados:

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| `Microsoft.EntityFrameworkCore` | 10.x | ORM |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.x | Proveedor SQL Server |
| `Swashbuckle.AspNetCore` | 6.x | Swagger/OpenAPI |
| `Microsoft.AspNetCore.App` | 10.x | Framework ASP.NET Core |

---

## ⚡ Troubleshooting

### **Error: "Cannot connect to database"**

```
Solución:
1. Verificar que SQL Server/LocalDB está corriendo:
   sqllocaldb start mssqllocaldb

2. Verificar connection string en appsettings.json

3. Verificar que la base de datos existe:
   SELECT * FROM sys.databases WHERE name = 'Northwind'
```

### **Error: "Column '<column>' does not exist"**

```
Solución:
1. Verificar que todas las migraciones están aplicadas
2. Regenerar scaffold:
   cd src/NorthwindTradersApp.Infrastructure
   dotnet ef dbcontext scaffold <connection_string> Microsoft.EntityFrameworkCore.SqlServer -f
```

### **Error: "Port 5000 is already in use"**

```bash
# Verificar qué está usando el puerto
netstat -ano | findstr :5000

# Usar un puerto diferente
dotnet run --urls="http://localhost:5001"
```

### **Error: "NuGet package not found"**

```bash
# Limpiar caché NuGet
dotnet nuget locals all --clear

# Restaurar de nuevo
dotnet restore
```

---

## 📝 Environment Files

### **appsettings.json** (Producción)
```json
{
  "ConnectionStrings": {
    "NorthwindDatabase": "Server=prod-server;Database=Northwind;User Id=sa;Password=***;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

### **appsettings.Development.json** (Desarrollo)
```json
{
  "ConnectionStrings": {
    "NorthwindDatabase": "Server=(localdb)\\mssqllocaldb;Database=Northwind;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

---

## 🧪 Ejecutar Tests (Si existen)

```bash
# Discover all test projects
dotnet test

# Run specific test project
dotnet test src/NorthwindTradersApp.Api.Tests/NorthwindTradersApp.Api.Tests.csproj

# With code coverage
dotnet test /p:CollectCoverage=true
```

---

## 📚 Próximos Pasos

1. ✅ API está corriendo en http://localhost:5000
2. 📖 Revisar [API_ENDPOINTS.md](API_ENDPOINTS.md) para ver todos los endpoints
3. 🏗️ Revisar [ARCHITECTURE.md](ARCHITECTURE.md) para entender la estructura
4. 💻 Revisar [DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md) para guía de desarrollo
5. 🔍 Revisar [TROUBLESHOOTING.md](TROUBLESHOOTING.md) para resolver problemas

---

## 🆘 Soporte

Si tienes problemas durante la instalación:

1. Revisar la sección [Troubleshooting](#troubleshooting)
2. Verificar que todos los requisitos previos estén instalados
3. Revisar los logs de VS Code/Visual Studio
4. Ejecutar `dotnet --info` para ver información del ambiente
