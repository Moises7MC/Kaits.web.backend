# Sistema de Gestión de Pedidos - Backend

API RESTful robusta para la gestión de pedidos, productos y clientes, desarrollada con ASP.NET Core aplicando arquitectura limpia y patrones modernos.

## 🚀 Tecnologías Utilizadas

### Core
- **ASP.NET Core 6+** - Framework web de alto rendimiento
- **C# 10+** - Lenguaje de programación moderno
- **Entity Framework Core** - ORM para acceso a datos
- **SQL Server Express / LocalDB** - Base de datos relacional

### Arquitectura y Patrones
- **CQRS Light** - Separación de comandos y consultas
- **MediatR** - Patrón Mediator para desacoplamiento
- **FluentValidation** - Validaciones de negocio elegantes
- **Clean Architecture** - Separación en capas

### Herramientas y Librerías
- **Swagger/OpenAPI** - Documentación interactiva de la API
- **xUnit** - Framework de testing (opcional)

## 📋 Prerequisitos

- .NET SDK 6.0 o superior
- SQL Server Express / LocalDB
- Visual Studio 2022 o VS Code con extensiones de C#
- SQL Server Management Studio (SSMS) - opcional

## 🏗️ Arquitectura del Proyecto

```
Kais.BACKENDEFC/ (Solución - 5 proyectos)
│
├── Kais.BACKENDEFC/                 # 🌐 Capa de Presentación (API)
│   ├── Controllers/
│   │   ├── PedidosController.cs
│   │   ├── ProductosController.cs
│   │   └── ClientesController.cs
│   ├── Program.cs                   # Punto de entrada y configuración
│   ├── appsettings.json            # Configuración de la aplicación
│   ├── Kais.BACKENDEFC.http        # Archivo de pruebas HTTP
│   └── Properties/
│
├── Kaits.Application/               # 📋 Capa de Aplicación (CQRS)
│   ├── Commands/
│   │   └── (Comandos para operaciones que modifican estado)
│   ├── Dtos/
│   │   └── (Data Transfer Objects)
│   ├── Handlers/
│   │   └── (Handlers de MediatR)
│   ├── Validators/
│   │   └── (Validadores con FluentValidation)
│   └── Exceptions/
│       └── (Excepciones personalizadas)
│
├── Kaits.Application.Tests/         # 🧪 Tests de Aplicación
│   └── CreatePedidoHandlerTests.cs
│
├── Kaits.DomainEFC/                 # 🎯 Capa de Dominio
│   └── Entities/
│       ├── Pedido.cs
│       ├── DetallePedido.cs
│       ├── Cliente.cs
│       └── Producto.cs
│
└── Kaits.Infrastructure/            # 🔧 Capa de Infraestructura
    ├── Persistence/
    │   └── KaitsDbContext.cs       # Contexto de EF Core
    ├── Mappings/
    │   └── (Configuraciones de entidades)
    ├── Migrations/
    │   └── (Migraciones de base de datos)
    └── Scripts/
        ├── seed_clientes.sql
        └── seed_productos.sql
```

## 📦 Paquetes NuGet Instalados

### Kais.BACKENDEFC (API)
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.x" />
  <PackageReference Include="Swashbuckle.AspNetCore" Version="6.x" />
</ItemGroup>
```

### Kaits.Application
```xml
<ItemGroup>
  <PackageReference Include="MediatR" Version="12.x" />
  <PackageReference Include="FluentValidation" Version="11.x" />
  <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.x" />
</ItemGroup>
```

### Kaits.Infrastructure
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.x" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.x" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.x" />
</ItemGroup>
```

### Kaits.Application.Tests
```xml
<ItemGroup>
  <PackageReference Include="xunit" Version="2.x" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.x" />
  <PackageReference Include="FluentAssertions" Version="6.x" />
  <PackageReference Include="Moq" Version="4.x" />
</ItemGroup>
```

## 🗄️ Modelo de Datos

### Base de Datos: `Kaits`

#### Tabla: `Pedidos`
| Campo          | Tipo          | Descripción                    |
|----------------|---------------|--------------------------------|
| Id             | int (PK)      | Identificador único            |
| FechaOrden     | datetime2     | Fecha y hora del pedido        |
| ClienteCodigo  | varchar(50)   | Código del cliente (FK)        |
| PrecioTotal    | decimal(18,2) | Total calculado del pedido     |

#### Tabla: `DetallePedidos`
| Campo               | Tipo          | Descripción                    |
|---------------------|---------------|--------------------------------|
| Id                  | int (PK)      | Identificador único            |
| PedidoId            | int (FK)      | Referencia al pedido           |
| ProductoCodigo      | varchar(50)   | Código del producto            |
| ProductoDescripcion | varchar(200)  | Descripción del producto       |
| Cantidad            | int           | Cantidad solicitada            |
| PrecioUnitario      | decimal(18,2) | Precio unitario al momento     |
| Subtotal            | decimal(18,2) | Cantidad × Precio              |

#### Tabla: `Clientes`
| Campo  | Tipo        | Descripción              |
|--------|-------------|--------------------------|
| Id     | int (PK)    | Identificador único      |
| Codigo | varchar(50) | Código único del cliente |
| Nombre | varchar(200)| Nombre completo          |
| DNI    | varchar(8)  | Documento de identidad   |

#### Tabla: `Productos`
| Campo          | Tipo          | Descripción              |
|----------------|---------------|--------------------------|
| Id             | int (PK)      | Identificador único      |
| Codigo         | varchar(50)   | Código único del producto|
| Descripcion    | varchar(200)  | Descripción del producto |
| PrecioUnitario | decimal(18,2) | Precio actual            |

## 🔧 Configuración Inicial

### 1. Clonar el repositorio

### 2. Restaurar paquetes NuGet

```bash
# Desde la raíz de la solución
dotnet restore
```

### 3. Configurar cadena de conexión

Actualiza `appsettings.json` en el proyecto **Kais.BACKENDEFC**:
```json
{
  "ConnectionStrings": {
    "KaitsDatabase": "Server=(localdb)\\mssqllocaldb;Database=Kaits;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### 4. Ejecutar migraciones

```bash
# Desde la carpeta de la solución
dotnet ef database update --project Kaits.Infrastructure --startup-project Kais.BACKENDEFC

# O si prefieres usar Package Manager Console en Visual Studio
# (Asegúrate de seleccionar Kaits.Infrastructure como Default project)
Update-Database
```

### 5. Ejecutar scripts SQL de datos iniciales

Navega a `Kaits.Infrastructure/Scripts/` y ejecuta:

**seed_clientes.sql**
```sql
USE Kaits;
GO

INSERT INTO Clientes (Codigo, Nombre, DNI) VALUES
('C001', 'Juan Pérez', '12345678'),
('C002', 'María López', '87654321'),
('C003', 'Carlos Díaz', '56781234'),
('C004', 'Lucía Torres', '43218765');
```

**seed_productos.sql**
```sql
USE Kaits;
GO

INSERT INTO Productos (Codigo, Descripcion, PrecioUnitario) VALUES
('P001', 'Laptop Dell Inspiron 15', 2500.00),
('P002', 'Mouse Logitech Inalámbrico', 85.50),
('P003', 'Teclado Mecánico RGB', 350.00),
('P004', 'Monitor LG 24 pulgadas', 890.00),
('P005', 'Webcam HD 1080p', 180.00);
```

### 6. Ejecutar la aplicación

**Opción 1: Desde Visual Studio**
- Establece **Kais.BACKENDEFC** como proyecto de inicio
- Presiona **F5**

**Opción 2: Desde línea de comandos**
```bash
cd Kais.BACKENDEFC
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:7192`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:7192/swagger`

## 🔌 Endpoints de la API

### Pedidos

#### Crear Pedido
```http
POST /api/Pedidos
Content-Type: application/json

{
  "clienteCodigo": "C001",
  "items": [
    {
      "productoCodigo": "P001",
      "cantidad": 2
    },
    {
      "productoCodigo": "P003",
      "cantidad": 1
    }
  ]
}
```

#### Obtener Pedido por ID
```http
GET /api/Pedidos/{id}
```

#### Listar Todos los Pedidos
```http
GET /api/Pedidos
```

#### Actualizar Pedido
```http
PUT /api/Pedidos/{id}
Content-Type: application/json

{
  "pedidoId": 1,
  "clienteCodigo": "C002",
  "items": [
    {
      "productoCodigo": "P002",
      "cantidad": 5
    }
  ]
}
```

#### Eliminar Pedido
```http
DELETE /api/Pedidos/{id}
```

### Productos

#### Crear Producto
```http
POST /api/Productos
Content-Type: application/json

{
  "codigo": "P006",
  "descripcion": "Audífonos Bluetooth",
  "precioUnitario": 150.00
}
```

#### Listar Productos
```http
GET /api/Productos
```

#### Actualizar Producto
```http
PUT /api/Productos/{codigo}
Content-Type: application/json

{
  "codigo": "P006",
  "descripcion": "Audífonos Bluetooth Premium",
  "precioUnitario": 180.00
}
```

#### Eliminar Producto
```http
DELETE /api/Productos/{codigo}
```

### Clientes

#### Crear Cliente
```http
POST /api/Clientes
Content-Type: application/json

{
  "codigo": "C005",
  "nombre": "Ana García",
  "dni": "11223344"
}
```

#### Listar Clientes
```http
GET /api/Clientes
```

#### Actualizar Cliente
```http
PUT /api/Clientes/{id}
Content-Type: application/json

{
  "id": 1,
  "codigo": "C001",
  "nombre": "Juan Pérez Actualizado",
  "dni": "12345678"
}
```

#### Eliminar Cliente
```http
DELETE /api/Clientes/{id}
```

## ✅ Validaciones Implementadas

### Pedidos (CreatePedidoValidator)
- ✅ Cliente debe existir en la base de datos
- ✅ Al menos un producto en el pedido
- ✅ Cantidad de productos > 0
- ✅ Productos deben existir en la base de datos
- ✅ Cálculo automático de subtotales y total
- ✅ Transacción única para pedido y detalles

### Productos
- ✅ Código único (no duplicados)
- ✅ Descripción obligatoria
- ✅ Precio unitario > 0

### Clientes
- ✅ Código único (no duplicados)
- ✅ Nombre obligatorio
- ✅ DNI de 8 dígitos numéricos
- ✅ DNI único (no duplicados)

## 🎯 Características Destacadas

### Arquitectura Limpia (5 Proyectos)
- **Kais.BACKENDEFC**: Capa de presentación (Controllers, Program.cs)
- **Kaits.Application**: Lógica de aplicación (Commands, Handlers, Validators)
- **Kaits.DomainEFC**: Entidades de dominio
- **Kaits.Infrastructure**: Persistencia y EF Core
- **Kaits.Application.Tests**: Tests unitarios

### CQRS Light con MediatR
- **Comandos**: Operaciones que modifican estado (Create, Update, Delete)
- **Handlers**: Lógica de negocio encapsulada en `Kaits.Application/Handlers`
- **Pipeline**: Validación automática antes de ejecutar comandos
- **Desacoplamiento**: Controllers delegan a MediatR

### FluentValidation
- **Validadores centralizados** en `Kaits.Application/Validators`
- **Validaciones expresivas** y legibles
- **Mensajes personalizados** de error
- **Validaciones asíncronas** para consultas a BD

### Entity Framework Core
- **DbContext** en `Kaits.Infrastructure/Persistence`
- **Mappings** configurados en carpeta dedicada
- **Migraciones** versionadas
- **Transacciones** para operaciones críticas

## 🔐 Configuración de CORS

Para permitir peticiones desde el frontend en desarrollo, en `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors("AllowReactApp");
```

## 📝 Scripts Útiles

### Entity Framework Core

```bash
# Agregar nueva migración
dotnet ef migrations add NombreDeLaMigracion --project Kaits.Infrastructure --startup-project Kais.BACKENDEFC

# Actualizar base de datos
dotnet ef database update --project Kaits.Infrastructure --startup-project Kais.BACKENDEFC

# Ver SQL que se ejecutará
dotnet ef migrations script --project Kaits.Infrastructure --startup-project Kais.BACKENDEFC

# Eliminar última migración (si no se aplicó)
dotnet ef migrations remove --project Kaits.Infrastructure --startup-project Kais.BACKENDEFC

# Eliminar base de datos
dotnet ef database drop --project Kaits.Infrastructure --startup-project Kais.BACKENDEFC
```

### Build y Testing

```bash
# Compilar toda la solución
dotnet build

# Ejecutar todos los tests
dotnet test

# Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar solo tests de un proyecto
dotnet test Kaits.Application.Tests
```

## 🧪 Testing

El proyecto incluye **Kaits.Application.Tests** con:

### Tests Unitarios
- ✅ Tests de handlers (CreatePedidoHandlerTests.cs)
- ✅ Tests de validadores con FluentValidation.TestHelper
- ✅ Mocks con Moq para dependencias
- ✅ Assertions con FluentAssertions

**Ejemplo de ejecución:**
```bash
dotnet test Kaits.Application.Tests
```

## 🚨 Manejo de Errores

### Excepciones Controladas
- **ValidationException** (FluentValidation) → 400 Bad Request
- **KeyNotFoundException** → 404 Not Found
- **InvalidOperationException** → 409 Conflict
- **Exception genérica** → 500 Internal Server Error

### Respuestas de Error Consistentes
```json
{
  "message": "Descripción del error",
  "errors": ["Error 1", "Error 2"]  // Solo en validaciones
}
```

## 📊 Logging

El proyecto incluye logging en los controllers:

```csharp
_logger.LogInformation("Pedido {PedidoId} creado exitosamente", pedidoId);
_logger.LogError(ex, "Error al crear pedido");
_logger.LogWarning("Cliente {Codigo} no encontrado", codigo);
```

## 🔒 Buenas Prácticas Implementadas

- ✅ **Separación en 5 proyectos** para mejor organización
- ✅ **Inyección de dependencias** en toda la aplicación
- ✅ **Async/Await** para operaciones I/O
- ✅ **DTOs** para transferencia de datos
- ✅ **Validaciones en servidor** obligatorias
- ✅ **Transacciones** para consistencia de datos
- ✅ **Logging** estructurado
- ✅ **Tests unitarios** incluidos
- ✅ **Código limpio** siguiendo convenciones de C#

## 🐛 Solución de Problemas

### Error al conectar a la base de datos
**Síntoma**: "Cannot open database" o "Login failed"

**Solución**: 
1. Verifica que SQL Server esté corriendo
2. Actualiza la cadena de conexión en `appsettings.json`
3. Ejecuta las migraciones desde el proyecto correcto

### Error al ejecutar migraciones
**Síntoma**: "No DbContext was found"

**Solución**: 
Asegúrate de especificar ambos proyectos:
```bash
dotnet ef database update --project Kaits.Infrastructure --startup-project Kais.BACKENDEFC
```

### Error de compilación entre proyectos
**Síntoma**: Referencias no encontradas

**Solución**: 
```bash
dotnet clean
dotnet restore
dotnet build
```

### Tests no se ejecutan
**Síntoma**: "No test is available"

**Solución**: 
Reconstruye el proyecto de tests:
```bash
dotnet build Kaits.Application.Tests
dotnet test Kaits.Application.Tests
```

## 📚 Recursos Adicionales

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [xUnit](https://xunit.net/)

---
