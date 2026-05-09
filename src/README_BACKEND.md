# 📚 NorthwindTradersApp Backend - Documentación Completa

## 🎯 Resumen Ejecutivo

**NorthwindTradersApp** es una aplicación empresarial completa construida con **.NET 10** y **SQL Server** que gestiona el sistema de órdenes, productos y clientes de una empresa de comercio (Northwind Traders).

### **Características Principales**

✅ **Clean Architecture** - 4 capas bien definidas  
✅ **RESTful API** - 15 endpoints con documentación Swagger  
✅ **Base de Datos Relacional** - 11 tablas con relaciones complejas  
✅ **Operaciones CRUD** - Completas para Órdenes, Productos y Clientes  
✅ **Búsqueda Avanzada** - Filtrado y paginación en todos los endpoints  
✅ **Async/Await** - Todas las operaciones son asincrónicas  
✅ **Error Handling** - Manejo robusto de excepciones  

---

## 📑 Documentación Disponible

### **1. 🏗️ [ARCHITECTURE.md](ARCHITECTURE.md)** - Arquitectura del Sistema

Comprende la estructura general del proyecto:

- Descripción de las 4 capas (API, Application, Domain, Infrastructure)
- Diagrama de flujo de datos
- Patrones de diseño implementados
- Tecnologías utilizadas
- Referencias rápidas a archivos clave

**Para:** Entender cómo está organizado el proyecto y cómo interactúan las capas.

---

### **2. 🔌 [API_ENDPOINTS.md](API_ENDPOINTS.md)** - Documentación de API

Documentación completa de todos los endpoints REST:

| Recurso | Endpoints |
|---------|-----------|
| **Orders** | GET, POST, PUT, DELETE + Búsqueda |
| **Products** | GET, POST, PUT, DELETE + Búsqueda |
| **Customers** | GET, POST, PUT, DELETE + Búsqueda |
| **Health** | GET /health |

**Cada endpoint incluye:**
- Descripción
- Query parameters
- Request body examples
- Response examples
- Códigos de estado

**Para:** Consumir la API desde frontend o herramientas externas.

---

### **3. 🗄️ [DATABASE_SCHEMA.md](DATABASE_SCHEMA.md)** - Esquema de Base de Datos

Descripción detallada de todas las tablas y relaciones:

- **11 Tablas** (Customers, Products, Orders, OrderDetails, etc.)
- **Columnas** con tipos de datos
- **Claves Primarias y Foráneas**
- **Índices** para optimización
- **Relaciones Many-to-Many** (EmployeeTerritories)
- **Cascada de eliminaciones** automáticas

**Incluye:**
- Diagrama de entidades (ER)
- Ejemplos de datos
- Queries SQL comunes

**Para:** Entender la estructura de BD y escribir queries.

---

### **4. ⚙️ [SETUP_AND_INSTALLATION.md](SETUP_AND_INSTALLATION.md)** - Instalación & Configuración

Guía paso a paso para configurar el ambiente:

**Requisitos:**
- .NET 10 SDK
- SQL Server 2019+ o LocalDB
- Visual Studio 2022 (recomendado)

**Pasos:**
1. Clonar repositorio
2. Configurar base de datos (LocalDB o SQL Server)
3. Configurar connection string
4. Restaurar dependencias
5. Compilar solución
6. Ejecutar API

**Incluye:**
- Instrucciones para Windows, macOS y Linux
- Troubleshooting de instalación
- Verificación de conexión BD
- Estructura de archivos

**Para:** Configurar el ambiente de desarrollo desde cero.

---

### **5. 👨‍💻 [DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md)** - Guía de Desarrollo

Guía completa para desarrolladores:

**Secciones:**
- Estructura de capas
- Patrones de código
- Agregar nuevas características (ejemplo completo)
- Service Layer - Best practices
- Repository Layer - Patrones comunes
- API Endpoints - Convenciones
- Database Queries - Tips de LINQ
- Error Handling - Excepciones personalizadas
- Testing - Unit test patterns
- SOLID Principles

**Ejemplos de código:**
- Cómo implementar un nuevo endpoint
- Transacciones y bulk operations
- Paginación y búsqueda
- Manejo de excepciones

**Para:** Escribir código siguiendo los estándares del proyecto.

---

### **6. 🔧 [TROUBLESHOOTING.md](TROUBLESHOOTING.md)** - Resolución de Problemas

Guía de troubleshooting para errores comunes:

**Categorías:**
- Errores de conexión a BD
- Errores de compilación
- Runtime errors
- API errors (404, 405, 500, etc.)
- Problemas de rendimiento
- Excepciones comunes

**Cada problema incluye:**
- Síntomas
- Soluciones paso a paso
- Ejemplos de código
- Tips de debugging

**Para:** Resolver problemas rápidamente sin frustraciones.

---

## 🚀 Inicio Rápido

### **1. Instalación (5 minutos)**

```bash
# Clonar repositorio
git clone <URL>
cd NorthwindTradersApp

# Configurar BD (si usas LocalDB)
sqllocaldb start mssqllocaldb

# Restaurar dependencias
dotnet restore

# Compilar
dotnet build
```

### **2. Ejecutar API**

```bash
# Navegar a la carpeta API
cd src/NorthwindTradersApp.Api

# Ejecutar
dotnet run

# API disponible en http://localhost:5000
# Swagger UI en http://localhost:5000/swagger/index.html
```

### **3. Probar Endpoints**

```bash
# Opción 1: Usar Swagger UI (interfaz gráfica)
# Abrir http://localhost:5000/swagger/index.html

# Opción 2: Usar curl
curl http://localhost:5000/api/products

# Opción 3: Usar VS Code REST Client
# Ver archivo NorthwindTradersApp.Api.http
```

---

## 📊 Arquitectura Visual

```
┌─────────────────────────────────────────────┐
│         Frontend (Quasar Vue.js)            │
│      (En carpeta frontend/)                 │
└────────────────────┬────────────────────────┘
                     │ HTTP/REST
                     ▼
┌─────────────────────────────────────────────┐
│  NorthwindTradersApp.Api (Program.cs)       │
│  - Endpoints REST (/api/*)                  │
│  - Swagger Documentation                    │
│  - CORS Configuration                       │
└────────────────────┬────────────────────────┘
                     │ Depende de
                     ▼
┌─────────────────────────────────────────────┐
│ NorthwindTradersApp.Application             │
│  - OrdersService                            │
│  - ProductsService                          │
│  - CustomersService                         │
│  - Lógica de Negocio                        │
└────────────────────┬────────────────────────┘
                     │ Implementa Interfaces de
                     ▼
┌─────────────────────────────────────────────┐
│ NorthwindTradersApp.Domain                  │
│  - Entidades (Order, Product, Customer)     │
│  - DTOs (OrderDto, ProductDto, etc)         │
│  - Interfaces (IOrdersRepository, etc)      │
└────────────────────┬────────────────────────┘
                     │ Implementa
                     ▼
┌─────────────────────────────────────────────┐
│ NorthwindTradersApp.Infrastructure          │
│  - NorthwindDbContext (Entity Framework)    │
│  - OrdersRepository                         │
│  - ProductsRepository                       │
│  - CustomersRepository                      │
└────────────────────┬────────────────────────┘
                     │ Accede a
                     ▼
                  ┌──────────┐
                  │SQL Server│
                  │ Northwind│
                  │   BD     │
                  └──────────┘
```

---

## 📈 Estadísticas del Proyecto

| Métrica | Valor |
|---------|-------|
| **Tablas de BD** | 11 |
| **Entidades .NET** | 11 |
| **DTOs** | 4 |
| **Servicios** | 3 |
| **Repositorios** | 3 |
| **Endpoints REST** | 15 |
| **Métodos Asincronicos** | 30+ |
| **Líneas de Código** | ~3,000 |

---

## 🔑 Conceptos Clave

### **1. Clean Architecture**

El proyecto divide responsabilidades en 4 capas:

- **API**: Interfaz HTTP
- **Application**: Lógica de negocio
- **Domain**: Modelos y contratos
- **Infrastructure**: Acceso a datos

**Ventaja:** Fácil de testear, mantener y escalar.

---

### **2. Repository Pattern**

Cada entidad principal tiene un repositorio:

- `IOrdersRepository` → Acceso a datos de Órdenes
- `IProductsRepository` → Acceso a datos de Productos
- `ICustomersRepository` → Acceso a datos de Clientes

**Ventaja:** Abstracción del acceso a datos, fácil de cambiar BD.

---

### **3. Dependency Injection (DI)**

Todos los servicios se inyectan en lugar de instanciarse:

```csharp
public ProductService(IProductsRepository repository)
{
    _repository = repository;
}
```

**Ventaja:** Fácil de testear con mocks.

---

### **4. DTOs (Data Transfer Objects)**

Las entidades de BD no se exponen directamente:

- BD: `Product` (tabla con 8 columnas)
- API: `ProductDto` (incluyendo `CategoryName`)

**Ventaja:** Decoplamiento entre BD y API.

---

### **5. Async/Await**

Todas las operaciones I/O son asincrónicas:

```csharp
public async Task<ProductDto> GetProductAsync(int id)
{
    return await _repository.GetByIdAsync(id);
}
```

**Ventaja:** Mejor rendimiento y escalabilidad.

---

## 📂 Estructura de Carpetas

```
src/
├── NorthwindTradersApp.Api/
│   ├── Program.cs                          # Configuración API
│   ├── appsettings.json                    # Conexión BD
│   ├── NorthwindTradersApp.Api.http        # Test requests
│   └── Properties/
│       └── launchSettings.json
│
├── NorthwindTradersApp.Application/
│   ├── Services/
│   │   ├── OrdersService.cs
│   │   ├── ProductService.cs
│   │   └── CustomersService.cs
│   └── DependencyInjection.cs
│
├── NorthwindTradersApp.Domain/
│   ├── Contracts/                          # DTOs
│   │   ├── OrderDto.cs
│   │   ├── ProductDto.cs
│   │   ├── CustomerDto.cs
│   │   └── EmployeeDto.cs
│   └── NorthwindDbEntities/                # Entidades
│       ├── Order.cs
│       ├── Product.cs
│       ├── Customer.cs
│       └── ... (8 más)
│
└── NorthwindTradersApp.Infrastructure/
    ├── Persistence/
    │   └── NorthwindDbContext.cs           # DbContext EF
    ├── Repositories/
    │   ├── OrdersRepository.cs
    │   ├── ProductsRepository.cs
    │   └── CustomersRepository.cs
    └── DependencyInjection.cs
```

---

## 🎓 Flujo de Aprendizaje Recomendado

**Para Nuevos Desarrolladores:**

1. ✅ Leer [ARCHITECTURE.md](ARCHITECTURE.md) (15 min)
2. ✅ Ejecutar según [SETUP_AND_INSTALLATION.md](SETUP_AND_INSTALLATION.md) (20 min)
3. ✅ Probar endpoints en [API_ENDPOINTS.md](API_ENDPOINTS.md) (10 min)
4. ✅ Revisar [DATABASE_SCHEMA.md](DATABASE_SCHEMA.md) (20 min)
5. ✅ Leer [DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md) (30 min)
6. ✅ Agregar un nuevo endpoint de prueba

**Total: ~1.5 horas para familiarizarse**

---

## 🔗 Enlaces Rápidos

| Documento | Propósito |
|-----------|-----------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | Entender la estructura |
| [API_ENDPOINTS.md](API_ENDPOINTS.md) | Usar la API |
| [DATABASE_SCHEMA.md](DATABASE_SCHEMA.md) | Entender la BD |
| [SETUP_AND_INSTALLATION.md](SETUP_AND_INSTALLATION.md) | Configurar ambiente |
| [DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md) | Desarrollar nuevas features |
| [TROUBLESHOOTING.md](TROUBLESHOOTING.md) | Resolver problemas |

---

## 🆘 Soporte

Si encuentras problemas:

1. **Primero**, revisa [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
2. **Luego**, busca en documentación relevante
3. **Si nada funciona**, verifica los logs en VS Code Output

---

## 📋 Stack Tecnológico

| Componente | Tecnología | Versión |
|-----------|-----------|---------|
| Framework | .NET | 10.0 |
| ORM | Entity Framework Core | 10.x |
| BD | SQL Server | 2019+ |
| API Documentation | Swagger/OpenAPI | 6.x |
| Language | C# | 12+ |

---

## ✨ Características por Recurso

### **Orders (Pedidos)**

- ✅ Obtener todos con paginación
- ✅ Buscar por ID o cliente
- ✅ Crear con detalles de línea
- ✅ Actualizar orden completa
- ✅ Eliminar con cascada automática

### **Products (Productos)**

- ✅ Obtener todos con categorías
- ✅ Buscar por nombre
- ✅ Crear nuevo producto
- ✅ Actualizar información
- ✅ Eliminar producto

### **Customers (Clientes)**

- ✅ Obtener todos paginados
- ✅ Buscar por empresa o contacto
- ✅ Crear nuevo cliente
- ✅ Actualizar datos
- ✅ Eliminar cliente

---

## 📝 Notas Importantes

1. **LocalDB vs SQL Server**: El proyecto está configurado para LocalDB por defecto, pero soporta SQL Server completo.

2. **Connection String**: Cambiar en `appsettings.json` según tu ambiente.

3. **Swagger UI**: Excelente para testing manual de endpoints.

4. **Logs**: Revisar Output window para debugging.

5. **CORS**: Configurado para desarrollo local, actualizar para producción.

---

## 🎉 ¡Listo!

Tienes acceso a documentación completa de tu backend. 

**Próximos pasos:**
- [ ] Leer ARCHITECTURE.md
- [ ] Ejecutar según SETUP_AND_INSTALLATION.md
- [ ] Probar endpoints en Swagger UI
- [ ] Revisar DEVELOPMENT_GUIDE.md para agregar features
- [ ] Compartir con tu equipo

---

## 📞 Contacto & Contribución

Para contribuir a la documentación:
1. Actualizar archivos .md correspondientes
2. Seguir formato y convenciones
3. Agregar ejemplos cuando sea posible
4. Mantener índices actualizados

---

**Última actualización:** Mayo 2024  
**Versión del Proyecto:** 1.0.0  
**Estado:** ✅ Producción Ready
