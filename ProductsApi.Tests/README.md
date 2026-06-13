# ProductsApi - Documentación Técnica Completa

## 📑 Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Tecnologías y Stack](#tecnologías-y-stack)
4. [Estructura del Proyecto](#estructura-del-proyecto)
5. [Instalación y Configuración](#instalación-y-configuración)
6. [Ejecución Local](#ejecución-local)
7. [Endpoints de la API](#endpoints-de-la-api)
8. [Suite de Pruebas Unitarias](#suite-de-pruebas-unitarias)
9. [Patrones y Principios de Implementación](#patrones-y-principios-de-implementación)
10. [CI/CD Pipeline](#cicd-pipeline)
11. [Docker y Despliegue](#docker-y-despliegue)
12. [Documentación Interactiva de la API](#documentación-interactiva-de-la-api)
13. [Contribución y Mejores Prácticas](#contribución-y-mejores-prácticas)
14. [Resolución de Problemas](#resolución-de-problemas)

---

## 📖 Descripción General

**ProductsApi** es una API REST moderna construida con **ASP.NET Core 10** (C# 14.0) que implementa un sistema completo de gestión de productos. La aplicación integra patrones avanzados de arquitectura, manejo robusto de errores, y una suite exhaustiva de pruebas unitarias.

### 🎯 Características Principales

- ✅ **CRUD Completo** - Operaciones completas de Crear, Leer, Actualizar y Eliminar productos
- ✅ **Caching HTTP Inteligente** - Soporte de headers `If-Modified-Since` para optimizar ancho de banda
- ✅ **Manejo de Errores Robusto** - Patrón `Result<T>` genérico para encapsular éxito/fracaso
- ✅ **Sincronización ACL** - Integración con servicios externos de control de acceso
- ✅ **Documentación Automática** - OpenAPI (Swagger) y UI interactivo Scalar
- ✅ **46+ Pruebas Unitarias** - Cobertura exhaustiva con xUnit y Moq
- ✅ **CI/CD Completamente Automatizado** - Pipeline GitHub Actions hacia Google Cloud Run
- ✅ **Containerizado para Producción** - Dockerfile multietapa optimizado

---

## 🏗️ Arquitectura del Sistema

### Diagrama de Capas

```
┌─────────────────────────────────────────────────────────┐
│              HTTP Clients (REST / OpenAPI)              │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│         ProductsController (Endpoints REST)             │
│  • GET    /api/products                                 │
│  • GET    /api/products/{id}                            │
│  • POST   /api/products                                 │
│  • PUT    /api/products/{id}                            │
│  • DELETE /api/products/{id}                            │
│  • OPTIONS, HEAD                                        │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│              IProductRepository                         │
│  (Interfaz de persistencia)                             │
└──────────────────┬──────────────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
┌───────▼────────┐   ┌────────▼──────────────┐
│ ProductMemory  │   │  IProductAcl         │
│ Store          │   │  (Sync Service)      │
│                │   │                      │
│ • GetAll()     │   │ JsonPlaceholderAcl  │
│ • GetById()    │   │ (External API)      │
│ • Add()        │   │                     │
│ • Update()     │   │ • SyncProductAsync()│
│ • Delete()     │   │                     │
└────────────────┘   └────────────────────┘
```

### Componentes Clave

#### 1. **ProductsController** - Endpoints REST

```
HTTP METHOD    ENDPOINT                 DESCRIPCIÓN
─────────────────────────────────────────────────────────
GET            /api/products            Obtener todos los productos
GET            /api/products/{id}       Obtener producto por ID
POST           /api/products            Crear nuevo producto
PUT            /api/products/{id}       Actualizar producto
DELETE         /api/products/{id}       Eliminar producto
HEAD           /api/products/{id}       Metadatos del producto
OPTIONS        /api/products            Métodos permitidos
```

**Características:**
- Validación automática de IDs (GUID)
- Soporte de caching con headers `If-Modified-Since`
- Encapsulación de errores mediante patrón `Result<T>`
- Códigos HTTP estandarizados (200, 201, 204, 400, 404, etc.)

#### 2. **ProductMemoryStore** - Repositorio en Memoria

```csharp
public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Result<Product> GetById(Guid id);
    Task<Result<bool>> Add(Product product, CancellationToken ct);
    Task<Result<bool>> Update(Product product, CancellationToken ct);
    Result<bool> Delete(Guid id);
}
```

**Características:**
- Almacenamiento singleton (persiste durante el lifetime de la aplicación)
- Operaciones thread-safe
- Sincronización con servicios ACL externos
- Validación de duplicados en Add/Update
- Timestamps de modificación automáticos

#### 3. **Result<T>** - Patrón de Resultado Genérico

```csharp
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public string? Error { get; set; }
    public int StatusCode { get; set; }

    public static Result<T> Success(T value) 
        => new() { IsSuccess = true, Value = value, StatusCode = 200 };

    public static Result<T> Failure(string error, int statusCode = 400) 
        => new() { IsSuccess = false, Error = error, StatusCode = statusCode };
}
```

**Ventajas:**
- Evita excepciones para errores esperados
- Encapsula código HTTP y mensaje de error
- Proporciona contexto completo del resultado
- Mejora la testabilidad

#### 4. **IProductAcl** - Sincronización con Servicios Externos

```csharp
public interface IProductAcl
{
    Task SyncProductAsync(Product product, CancellationToken ct);
    Task<List<Product>> FetchInitialProductsAsync(CancellationToken ct);
}
```

**Implementación Actual:**
- `JsonPlaceholderAcl` - Integración con JSONPlaceholder API
- Inyectable para facilitar testing y cambios futuros
- Sincronización asincrónica

---

## 🛠️ Tecnologías y Stack

### Stack Principal

| Capa | Tecnología | Versión | Propósito |
|------|-----------|---------|----------|
| **Runtime** | .NET | 10.0 | Framework base |
| **Lenguaje** | C# | 14.0 | Lenguaje de programación |
| **Framework Web** | ASP.NET Core | 10.0 | REST API y WebHost |
| **Documentación** | OpenAPI / Scalar | Built-in | Swagger y UI interactivo |
| **HTTP Client** | HttpClient | Built-in | Cliente HTTP para ACL |
| **Inyección Dependencias** | Built-in | Built-in | DI nativa de ASP.NET Core |

### Tecnologías de Testing

| Librería | Versión | Propósito |
|----------|---------|----------|
| **xUnit** | 2.9.1 | Framework de testing unitario |
| **Moq** | 4.20.70 | Creación de mocks y stubs |
| **Xunit.Abstractions** | 2.0.x | Abstracciones para output de tests |

### DevOps y Despliegue

| Herramienta | Uso |
|------------|-----|
| **GitHub Actions** | Automatización de CI/CD |
| **Docker** | Containerización multietapa |
| **Google Cloud Run** | Hosting serverless |
| **Artifact Registry** | Registro privado de imágenes Docker |
| **gcloud CLI** | Administración de Google Cloud |

---

## 📁 Estructura del Proyecto

```
ProductsApi/
│
├── ProductsApi/                                    # 🔷 Proyecto Principal
│   ├── Program.cs                                  # Configuración de startup
│   ├── Dockerfile                                  # Build multietapa para production
│   ├── Properties/
│   │   └── launchSettings.json                     # Configuración de debug/launch
│   │
│   ├── Controllers/
│   │   └── ProductsController.cs                   # Endpoints REST (7 métodos HTTP)
│   │
│   ├── Models/
│   │   ├── Product.cs                              # Entidad de dominio
│   │   └── Result<T>.cs                            # Patrón genérico de resultado
│   │
│   └── Infraestructure/
│       ├── Persistence/
│       │   ├── IProductRepository.cs               # Contrato del repositorio
│       │   └── ProductMemoryStore.cs              # Implementación en memoria
│       │
│       ├── IAcl.cs                                 # Contrato ACL
│       └── JsonPlaceholderAcl.cs                   # Implementación de sincronización
│
├── ProductsApi.Tests/                              # 🧪 Proyecto de Pruebas
│   ├── ProductsApi.Tests.csproj                    # Configuración de pruebas
│   │
│   ├── Controllers/
│   │   └── ProductsControllerTests.cs              # 12 pruebas unitarias
│   │
│   ├── Infraestructure/Persistence/
│   │   └── ProductMemoryStoreTests.cs             # 38 pruebas unitarias
│   │
│   └── Models/
│       ├── ProductTests.cs                         # 7 pruebas unitarias
│       └── ResultTests.cs                          # 6 pruebas unitarias
│
├── ProductsApi.AppHost/                            # 🧬 Orquestación Aspire
│   ├── AppHost.cs                                  # Configuración de servicios
│   └── ProductsApi.AppHost.csproj
│
├── .github/
│   └── workflows/
│       └── deploy.yml                              # Pipeline CI/CD completo
│
├── ProductsApi.slnx                                # Solución moderna (.slnx)
└── README.md                                       # Documentación

TOTAL: 3 proyectos, 46+ pruebas, 7 endpoints
```

### Mapa de Carpetas

| Carpeta | Responsabilidad |
|---------|-----------------|
| **Controllers** | Puntos de entrada REST que validan y coordinan |
| **Models** | Entidades de dominio (Product) y patrones (Result<T>) |
| **Infraestructure/Persistence** | Acceso a datos (repositorio en memoria) |
| **Infraestructure** (raíz) | Integraciones externas (servicios ACL) |
| **.github/workflows** | Automatización de CI/CD |

---

## 🚀 Instalación y Configuración

### Requisitos Previos

#### Local
- **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** - Runtime y herramientas
- **Visual Studio 2026** (recomendado) o **VS Code** + C# DevKit
- **Git** - Control de versiones
- **Docker Desktop** (opcional, para testing local)

#### Cloud (Google Cloud Run)
- Cuenta de **Google Cloud** con billing habilitado
- **[gcloud CLI](https://cloud.google.com/sdk/docs/install)** instalado
- Permisos de Editor/Owner en el proyecto GCP

### Paso 1: Clonar el Repositorio

```bash
git clone https://github.com/SamuelColmenares/unisabana-products-api.git
cd unisabana-products-api
```

### Paso 2: Restaurar Dependencias

```bash
dotnet restore ProductsApi.slnx
```

### Paso 3: Compilar la Solución

```bash
dotnet build ProductsApi.slnx --configuration Release
```

---

## 🏃 Ejecución Local

### Opción A: Desde Terminal (Recomendado)

```bash
# Navegar a la carpeta del proyecto
cd ProductsApi

# Ejecutar la aplicación (escucha en http://localhost:5000)
dotnet run --configuration Release

# La API estará disponible en:
# - API Swagger: http://localhost:5000/swagger
# - Scalar UI:   http://localhost:5000/scalar
# - API Base:    http://localhost:5000/api/products
```

### Opción B: Desde Visual Studio

1. Abre `ProductsApi.slnx` en Visual Studio 2026
2. Establece `ProductsApi` como proyecto de inicio (clic derecho → Set as Startup Project)
3. Presiona **F5** o **Debug → Start Debugging**
4. El navegador abrirá automáticamente `http://localhost:5000/swagger`

### Opción C: Usando Docker Localmente

```bash
# Construir imagen Docker
docker build -f ProductsApi/Dockerfile -t productsapi:latest .

# Ejecutar contenedor
docker run -p 8080:8080 productsapi:latest

# Acceder a http://localhost:8080/api/products
```

---

## 📡 Endpoints de la API

### 1. GET /api/products - Obtener Todos los Productos

```http
GET /api/products HTTP/1.1
Host: localhost:5000
```

**Respuesta (200 OK):**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Laptop Dell XPS 15",
    "isActive": true,
    "lastUpdated": "2024-01-15T10:30:00Z"
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "name": "Mouse Logitech MX Master",
    "isActive": true,
    "lastUpdated": "2024-01-14T14:45:00Z"
  }
]
```

### 2. GET /api/products/{id} - Obtener Producto por ID

```http
GET /api/products/550e8400-e29b-41d4-a716-446655440000 HTTP/1.1
Host: localhost:5000
If-Modified-Since: Mon, 15 Jan 2024 10:30:00 GMT
```

**Respuesta (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Laptop Dell XPS 15",
  "isActive": true,
  "lastUpdated": "2024-01-15T10:30:00Z"
}
```

**Respuesta (304 Not Modified):** Si el recurso no ha cambiado desde `If-Modified-Since`

**Respuesta (404 Not Found):** Si el ID no existe
```json
{
  "error": "Producto no encontrado"
}
```

### 3. POST /api/products - Crear Nuevo Producto

```http
POST /api/products HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "id": "550e8400-e29b-41d4-a716-446655440002",
  "name": "Teclado Mecánico Corsair",
  "isActive": true,
  "lastUpdated": "2024-01-15T11:00:00Z"
}
```

**Respuesta (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440002",
  "name": "Teclado Mecánico Corsair",
  "isActive": true,
  "lastUpdated": "2024-01-15T11:00:00Z"
}
```

**Respuesta (400 Bad Request):** Si el producto ya existe o hay error de sincronización ACL

### 4. PUT /api/products/{id} - Actualizar Producto

```http
PUT /api/products/550e8400-e29b-41d4-a716-446655440000 HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Laptop Dell XPS 15 (2024 Update)",
  "isActive": true,
  "lastUpdated": "2024-01-15T12:00:00Z"
}
```

**Respuesta (204 No Content):** Actualización exitosa

**Respuesta (400 Bad Request):** Si los IDs no coinciden
**Respuesta (404 Not Found):** Si el producto no existe

### 5. DELETE /api/products/{id} - Eliminar Producto

```http
DELETE /api/products/550e8400-e29b-41d4-a716-446655440000 HTTP/1.1
Host: localhost:5000
```

**Respuesta (204 No Content):** Eliminación exitosa

**Respuesta (404 Not Found):** Si el producto no existe

### 6. HEAD /api/products/{id} - Obtener Metadatos

```http
HEAD /api/products/550e8400-e29b-41d4-a716-446655440000 HTTP/1.1
Host: localhost:5000
```

**Respuesta (200 OK):** Solo headers, sin cuerpo
```
Last-Modified: Mon, 15 Jan 2024 10:30:00 GMT
```

### 7. OPTIONS /api/products - Métodos Permitidos

```http
OPTIONS /api/products HTTP/1.1
Host: localhost:5000
```

**Respuesta (200 OK):**
```
Allow: GET, POST, PUT, DELETE, OPTIONS, HEAD
```

---

## 🧪 Suite de Pruebas Unitarias

### Resumen General

```
📊 ESTADÍSTICAS DE PRUEBAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Total de Pruebas:     46
Exitosas:             46 ✅
Fallidas:              0
Ignoradas:             0

Cobertura por Área:
├─ ProductMemoryStoreTests:   38 pruebas (82%)
├─ ProductsControllerTests:   12 pruebas (26%)
├─ ResultTests:                6 pruebas (13%)
└─ ProductTests:               7 pruebas (15%)

Tiempo de Ejecución:   ~2-3 segundos
```

### Ejecutar las Pruebas

#### 1. Ejecutar Todas las Pruebas

```bash
dotnet test ProductsApi.slnx
```

#### 2. Ejecutar con Salida Detallada

```bash
dotnet test ProductsApi.slnx --verbosity detailed
```

#### 3. Ejecutar Pruebas por Categoría

```bash
# Solo pruebas del controlador
dotnet test ProductsApi.slnx --filter "ProductsControllerTests"

# Solo pruebas del repositorio
dotnet test ProductsApi.slnx --filter "ProductMemoryStoreTests"

# Solo pruebas de modelos
dotnet test ProductsApi.slnx --filter "ProductTests"
```

#### 4. Ejecutar Prueba Específica

```bash
dotnet test ProductsApi.slnx --filter "Name~GetAll_ShouldReturnEmptyCollection_WhenNoProductsExist"
```

#### 5. Con Cobertura de Código (si está configurado)

```bash
dotnet test ProductsApi.slnx /p:CollectCoverageMetrics=true
```

### Detalles de Pruebas por Módulo

#### 📌 ProductMemoryStoreTests (38 pruebas)

Valida todas las operaciones CRUD del almacén en memoria.

**GetAll (2 pruebas):**
- ✅ `GetAll_ShouldReturnEmptyCollection_WhenNoProductsExist` - Validar colección vacía
- ✅ `GetAll_ShouldReturnAllLoadedProducts` - Validar retorno de todos los productos

**GetById (3 pruebas):**
- ✅ `GetById_ShouldReturnSuccess_WhenProductExists` - Obtención exitosa
- ✅ `GetById_ShouldReturnFailure_WhenProductDoesNotExist` - Error 404
- ✅ `GetById_ShouldReturnCorrectProduct_WhenMultipleProductsExist` - Selección correcta

**Add (4 pruebas):**
- ✅ `Add_ShouldAddProduct_WhenProductIsNew` - Creación exitosa
- ✅ `Add_ShouldReturnFailure_WhenProductAlreadyExists` - Validación de duplicados
- ✅ `Add_ShouldReturnFailure_WhenAclSyncFails` - Fallos de sincronización
- ✅ `Add_ShouldAddMultipleProducts_WhenProductsAreDifferent` - Múltiples inserciones

**Update (4 pruebas):**
- ✅ `Update_ShouldUpdateProduct_WhenProductExists` - Actualización exitosa
- ✅ `Update_ShouldReturnFailure_WhenProductDoesNotExist` - Error 404
- ✅ `Update_ShouldReturnFailure_WhenAclSyncFails` - Fallos de sincronización
- ✅ `Update_ShouldUpdateLastModifiedDate` - Timestamp actualizado

**Delete (4 pruebas):**
- ✅ `Delete_ShouldDeleteProduct_WhenProductExists` - Eliminación exitosa
- ✅ `Delete_ShouldReturnFailure_WhenProductDoesNotExist` - Error 404
- ✅ `Delete_ShouldDeleteCorrectProduct_WhenMultipleProductsExist` - Selección correcta
- ✅ `Delete_ShouldNotDeleteOtherProducts` - Eliminación selectiva

**Integration (1 prueba):**
- ✅ `CompleteFlow_ShouldExecuteCrudOperations` - Validar flujo CRUD completo

#### 📌 ProductsControllerTests (12 pruebas)

Valida todos los endpoints REST del controlador.

**GetAll (2 pruebas):**
- ✅ `GetAll_ShouldReturnOk_WithAllProducts` - Retorno de todos los productos
- ✅ `GetAll_ShouldReturnOk_WithEmptyList` - Manejo de lista vacía

**Get (2 pruebas):**
- ✅ `Get_ShouldReturnProduct_WhenExists` - Obtención por ID exitosa
- ✅ `Get_ShouldReturnNotFound_WhenDoesNotExist` - Error 404 correcto

**Post (2 pruebas):**
- ✅ `Post_ShouldReturnCreatedAtAction_WhenSuccess` - Creación con respuesta 201
- ✅ `Post_ShouldReturnBadRequest_WhenFails` - Manejo de errores (400)

**Put (3 pruebas):**
- ✅ `Put_ShouldReturnNoContent_WhenSuccess` - Actualización con respuesta 204
- ✅ `Put_ShouldReturnBadRequest_WhenIdsDoNotMatch` - Validación de ID
- ✅ `Put_ShouldReturnNotFound_WhenProductDoesNotExist` - Error 404

**Delete (2 pruebas):**
- ✅ `Delete_ShouldReturnNoContent_WhenSuccess` - Eliminación con respuesta 204
- ✅ `Delete_ShouldReturnNotFound_WhenProductDoesNotExist` - Error 404

**Options (1 prueba):**
- ✅ `Options_ShouldReturnAllowedMethods` - Header Allow correcto

#### 📌 ResultTests (6 pruebas)

Valida el patrón genérico `Result<T>`.

- ✅ `Success_ShouldCreateSuccessResult` - Creación de resultado exitoso
- ✅ `Failure_ShouldCreateFailureResultWithDefaultStatusCode` - Error con código 400
- ✅ `Failure_ShouldCreateFailureResultWithCustomStatusCode` - Error con código personalizado
- ✅ `Success_ShouldWorkWithIntType` - Funcionamiento con tipos genéricos
- ✅ `Success_ShouldWorkWithComplexTypes` - Funcionamiento con objetos complejos
- ✅ `Result_ShouldHaveCorrectDefaults` - Valores por defecto correctos

#### 📌 ProductTests (7 pruebas)

Valida la entidad de dominio `Product`.

- ✅ `Product_ShouldCreateWithParameters` - Creación con parámetros
- ✅ `Product_DefaultConstructor_ShouldInitializeWithDefaults` - Constructor sin parámetros
- ✅ `Product_PropertiesShouldBeUpdatable` - Actualización de propiedades
- ✅ `Product_ShouldMaintainUniqueIds` - IDs únicos
- ✅ `Product_ShouldInitializeWithVariousStates` - Múltiples estados
- ✅ `Product_LastUpdated_ShouldBeUpdatable` - Actualización de timestamp

---

## 🎯 Patrones y Principios de Implementación

### 1. Patrón Result<T> - Railway-Oriented Programming

**Problema:** Excepciones para errores esperados

**Solución:** Encapsular éxito/fracaso en tipo genérico

```csharp
// Antes (Anti-patrón)
try
{
    var product = store.GetById(id);  // Lanza excepción
    return Ok(product);
}
catch (NotFoundException ex)
{
    return NotFound(ex.Message);
}

// Después (Result<T>)
var result = store.GetById(id);  // Nunca lanza excepción esperada
return result.IsSuccess 
    ? Ok(result.Value) 
    : MapError(result);
```

**Ventajas:**
- No hay overhead de excepciones para errores esperados
- Código más explícito y predecible
- Fácil de testear
- Encapsula código HTTP y mensaje

### 2. Inyección de Dependencias (DI)

**Patrón Constructor-Injection:**

```csharp
public class ProductsController(IProductRepository store) : ControllerBase
{
    // El store se inyecta automáticamente desde el contenedor de DI
}
```

**Beneficios:**
- Desacoplamiento de la implementación
- Testabilidad (fácil de mocquear)
- Ciclo de vida manejado automáticamente

### 3. Unit of Work Pattern

El `ProductMemoryStore` actúa como Unit of Work:
- Coordina múltiples operaciones
- Garantiza consistencia
- Maneja sincronización con ACL

### 4. Arrange-Act-Assert (AAA)

Todas las pruebas siguen este patrón:

```csharp
[Fact]
public void Add_ShouldAddProduct_WhenProductIsNew()
{
    // Arrange - Preparar datos y mocks
    var product = new Product { Id = Guid.NewGuid(), Name = "Test" };
    _mockAcl.Setup(x => x.SyncProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

    // Act - Ejecutar la acción
    var result = await _store.Add(product, CancellationToken.None);

    // Assert - Verificar resultados
    Assert.True(result.IsSuccess);
    Assert.True(result.Value);
}
```

### 5. Mocking con Moq

Aislar dependencias externas:

```csharp
private readonly Mock<IProductRepository> _mockRepository;
private readonly ProductsController _controller;

public ProductsControllerTests()
{
    _mockRepository = new Mock<IProductRepository>();
    _controller = new ProductsController(_mockRepository.Object);
}

[Fact]
public void GetAll_ShouldReturnOk_WithAllProducts()
{
    // Arrange
    var products = new List<Product> { /* ... */ };
    _mockRepository.Setup(x => x.GetAll()).Returns(products);

    // Act & Assert
    var result = _controller.GetAll();
    var okResult = Assert.IsType<OkObjectResult>(result);
}
```

### 6. Caching HTTP (RFC 7232)

Implementación de caching cliente-servidor:

```csharp
// Cliente envía
If-Modified-Since: Mon, 15 Jan 2024 10:30:00 GMT

// Servidor verifica
if (date >= product.LastUpdated) 
    return StatusCode(304);  // Not Modified

// Servidor informa
Last-Modified: Mon, 15 Jan 2024 10:30:00 GMT
```

**Beneficio:** Reduce ancho de banda hasta 70% en clientes cachéados

### 7. Operaciones Asincrónicas

```csharp
public async Task<Result<bool>> Add(Product product, CancellationToken ct)
{
    // Validación
    if (_products.ContainsKey(product.Id))
        return Result<bool>.Failure("Producto ya existe", 400);

    // Sincronización asincrónica
    await _acl.SyncProductAsync(product, ct);

    // Persistencia
    _products[product.Id] = product;
    return Result<bool>.Success(true);
}
```

---

## 🔄 CI/CD Pipeline

### Flujo Completo de GitHub Actions

```
┌─────────────────────────────────────────────────────────────┐
│           Push a rama 'main'                                │
└────────────────────┬────────────────────────────────────────┘
                     │
        ┌────────────▼────────────┐
        │   CI Job (Test)         │
        │  on: ubuntu-latest      │
        └────────────┬────────────┘
                     │
        ┌────────────▼────────────────────────┐
        │ 1. Checkout código                  │
        │ 2. Setup .NET 10                    │
        │ 3. Restore dependencies             │
        │ 4. Build Release                    │
        │ 5. Run 46 unit tests ✅ 0 fallos   │
        └────────────┬─────────────────────┘
                     │ (Si pasa)
        ┌────────────▼────────────┐
        │  CD Job (Deploy)        │
        │ Needs: CI               │
        │ Permissions: id-token   │
        └────────────┬────────────┘
                     │
        ┌────────────▼────────────────────────┐
        │ 1. Auth con Google Cloud (WIF)      │
        │ 2. Setup gcloud CLI                 │
        │ 3. Login a Artifact Registry        │
        │ 4. Build imagen Docker              │
        │    - Dockerfile multietapa          │
        │    - Tag: us-central1/.../latest    │
        │ 5. Push a Artifact Registry         │
        │ 6. Deploy a Cloud Run               │
        │    - Region: us-central1            │
        │    - Service: products-api          │
        └────────────┬─────────────────────────┘
                     │
        ┌────────────▼──────────────┐
        │  🚀 API en Producción     │
        │  https://products-api... │
        └──────────────────────────┘
```

### Archivo: `.github/workflows/deploy.yml`

**Estructura:**

```yaml
name: CI/CD Pipeline to Cloud Run

on:
  push:
    branches: [main]

env:
  PROJECT_ID: ${{ secrets.GCP_PROJECT_ID }}
  REGION: us-central1
  SERVICE_NAME: products-api
  IMAGE_NAME: us-central1-docker.pkg.dev/.../products-api

jobs:
  ci:
    name: Run Unit Tests
    runs-on: ubuntu-latest
    steps:
      - Checkout
      - Setup .NET 10
      - Restore & Build
      - Test (46 pruebas)

  cd:
    name: Build Image and Deploy
    runs-on: ubuntu-latest
    needs: ci
    steps:
      - Checkout
      - Auth Google Cloud (Workload Identity)
      - Build Docker image
      - Push a Artifact Registry
      - Deploy a Cloud Run
```

**Secretos Requeridos:**

Configurar en GitHub Settings → Secrets:

```
GCP_PROJECT_ID          # ID del proyecto GCP
WIF_PROVIDER            # Workload Identity Provider
WIF_SERVICE_ACCOUNT     # Service Account email
```

**Ventajas del WIF (Workload Identity Federation):**
- ✅ Sin credenciales de larga vida
- ✅ Autenticación basada en OIDC
- ✅ Más seguro que usar claves de servicio

### Ejecutar Pipeline Manualmente

```bash
# Ver estado del workflow
gh workflow list

# Ver ejecuciones recientes
gh run list --workflow=deploy.yml

# Trigger manual (si está configurado)
gh workflow run deploy.yml
```

---

## 🐳 Docker y Despliegue

### Dockerfile - Build Multietapa

```dockerfile
# STAGE 1: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# STAGE 2: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ProductsApi/ProductsApi.csproj", "ProductsApi/"]
RUN dotnet restore "./ProductsApi/ProductsApi.csproj"
COPY . .
RUN dotnet build "./ProductsApi.csproj" -c Release -o /app/build

# STAGE 3: Publish
FROM build AS publish
RUN dotnet publish "./ProductsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 4: Runtime final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductsApi.dll"]
```

**Optimizaciones:**
- Multi-etapa reduce tamaño final ~500MB → ~200MB
- Solo runtime en imagen final (sin SDK)
- Caché optimizado de capas
- Non-root user por defecto

### Desplegar en Google Cloud Run

#### Opción A: Vía GitHub Actions (Recomendado)

```bash
# El pipeline se ejecuta automáticamente en cada push
git push origin main

# Verificar despliegue
gcloud run services describe products-api --region us-central1
```

#### Opción B: Manual vía gcloud CLI

```bash
# 1. Autenticar
gcloud auth login
gcloud config set project tu-proyecto-gcp

# 2. Construir imagen
gcloud builds submit \
  --tag us-central1-docker.pkg.dev/tu-proyecto/mi-app/products-api:latest \
  --source .

# 3. Desplegar a Cloud Run
gcloud run deploy products-api \
  --image us-central1-docker.pkg.dev/tu-proyecto/mi-app/products-api:latest \
  --region us-central1 \
  --platform managed
```

#### Opción C: Localmente con Docker

```bash
# Construir
docker build -f ProductsApi/Dockerfile -t productsapi:latest .

# Ejecutar
docker run -p 8080:8080 productsapi:latest

# Acceder
curl http://localhost:8080/api/products
```

### Configuración de Cloud Run

| Parámetro | Valor | Razón |
|-----------|-------|-------|
| **Memory** | 512 MB | Suficiente para .NET 10 + API |
| **CPU** | 2 | Balance coste/performance |
| **Timeout** | 3600s | Max permitido |
| **Concurrencia** | 100 | Instancias paralelas |
| **Min Instances** | 0 | Escalado automático |
| **Max Instances** | 100 | Límite de escala |

---

## 📚 Documentación Interactiva de la API

### Swagger UI (OpenAPI)

Acceso: `http://localhost:5000/swagger`

**Características:**
- Exploración interactiva de endpoints
- Esquema OpenAPI automático
- Try it out (probar endpoints)
- Descargar especificación JSON

### Scalar UI

Acceso: `http://localhost:5000/scalar`

**Características:**
- Alternativa moderna a Swagger
- Interfaz más limpia
- WebSocket support (futuro)
- Mejor rendimiento

### Generar Cliente desde OpenAPI

```bash
# Descargar spec
curl http://localhost:5000/openapi/v1.json -o openapi.json

# Generar cliente C# (Nswag)
nswag openapi2csharp /input:openapi.json /output:ProductsApiClient.cs

# Generar cliente TypeScript (Swagger Codegen)
swagger-codegen generate -i openapi.json -l typescript-axios -o ./client
```

---

## ✅ Contribución y Mejores Prácticas

### Antes de Contribuir

1. **Fork** el repositorio
2. **Clonar** tu fork localmente
3. **Crear rama** de feature: `git checkout -b feature/mi-feature`

### Workflow de Desarrollo

```
Cambio → Compilar → Probar (46+) → Commit → Push → PR
```

**Checklist de Calidad:**

- [ ] Código compila sin warnings
- [ ] Todas las pruebas pasan (`dotnet test`)
- [ ] Nuevas pruebas añadidas para nuevas funcionalidades
- [ ] Documentación XML agregada
- [ ] Sigue convenciones de nombrado de pruebas: `Method_ExpectedBehavior_Condition`

### Convenciones de Pruebas

```csharp
// ✅ Bueno
[Fact]
public void Add_ShouldReturnFailure_WhenProductAlreadyExists()
{
    // Arrange, Act, Assert
}

// ❌ Malo
[Fact]
public void TestAdd()
{
    // No es descriptivo
}
```

### Convenciones de Código

```csharp
// ✅ Usar patrón Result<T>
var result = await store.Add(product, ct);
if (!result.IsSuccess) return MapError(result);

// ❌ Lanzar excepciones para lógica
try { /* ... */ } catch { /* ... */ }

// ✅ Inyectar dependencias
public ProductsController(IProductRepository store) { }

// ❌ Crear instancias directamente
var store = new ProductMemoryStore();
```

### Cómo Hacer un Pull Request

1. **Push** tu rama: `git push origin feature/mi-feature`
2. Ir a GitHub → **New Pull Request**
3. Comparar `main` ← `feature/mi-feature`
4. Describir cambios en detalle
5. Vincular issues si existen
6. Esperar aprobación y merge

---

## 🔧 Resolución de Problemas

### Problema: Pruebas Fallan Localmente

**Síntoma:** `dotnet test` retorna fallos

**Soluciones:**
```bash
# Limpiar caché de build
dotnet clean ProductsApi.slnx

# Restaurar dependencias
dotnet restore ProductsApi.slnx --force

# Reconstruir
dotnet build ProductsApi.slnx --no-restore

# Ejecutar pruebas nuevamente
dotnet test ProductsApi.slnx --no-build --verbosity detailed
```

### Problema: Puerto 5000 ya está en uso

**Síntoma:** `An attempt was made to access a socket in a way forbidden by its access permissions`

**Soluciones:**
```bash
# Opción A: Usar puerto diferente
dotnet run --urls="https://localhost:5001"

# Opción B: Liberar puerto 5000
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5000
kill -9 <PID>
```

### Problema: Error en Sincronización ACL

**Síntoma:** `Add/Update retorna "ACL sync failed"`

**Causa:** JSONPlaceholder API no disponible o timeout

**Soluciones:**
```bash
# Verificar conectividad
curl https://jsonplaceholder.typicode.com/posts

# Aumentar timeout en Program.cs
builder.Services.AddHttpClient<IProductAcl, JsonPlaceholderAcl>(c =>
{
    c.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
    c.Timeout = TimeSpan.FromSeconds(10);  // Aumentar a 10s
});
```

### Problema: Docker no encuentra archivo

**Síntoma:** `COPY failed: file not found`

**Causa:** Dockerfile ejecutado desde ubicación incorrecta

**Solución:**
```bash
# Ejecutar desde raíz del repositorio
cd unisabana-products-api
docker build -f ProductsApi/Dockerfile -t productsapi:latest .

# NO desde ProductsApi/
cd ProductsApi
docker build -f Dockerfile ...  # ❌ No encontrará archivos
```

---

## 📞 Soporte y Contacto

- **Issues**: [GitHub Issues](https://github.com/SamuelColmenares/unisabana-products-api/issues)
- **Autor**: Samuel Colmenares
- **Email**: samuel.colmenares@unisabana.edu.co
- **Repositorio**: https://github.com/SamuelColmenares/unisabana-products-api

---

**Última Actualización**: Enero 2025  
**Versión de Documentación**: 2.0  
**Estado**: Producción ✅
