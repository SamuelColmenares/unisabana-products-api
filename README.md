# 🚀 ProductsApi - RESTful Architecture Demo (.NET 10 + Aspire)

Demostración técnica de una arquitectura **RESTful de alta madurez**, escalable, desacoplada y eficiente. Construida con **.NET 10**, **ASP.NET Core**, y orquestación mediante **.NET Aspire**.

## 📑 Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Características Principales](#características-principales)
3. [Arquitectura del Sistema](#arquitectura-del-sistema)
4. [Tecnologías y Stack](#tecnologías-y-stack)
5. [Estructura del Proyecto](#estructura-del-proyecto)
6. [Requisitos Previos](#requisitos-previos)
7. [Instalación y Configuración](#instalación-y-configuración)
8. [Ejecución Local](#ejecución-local)
9. [Endpoints de la API](#endpoints-de-la-api)
10. [Suite de Pruebas Unitarias](#suite-de-pruebas-unitarias)
11. [Patrones de Implementación](#patrones-de-implementación)
12. [CI/CD Pipeline](#cicd-pipeline)
13. [Docker y Despliegue](#docker-y-despliegue)
14. [Documentación Interactiva](#documentación-interactiva)
15. [Resolución de Problemas](#resolución-de-problemas)

---

## 📖 Descripción General

**ProductsApi** implementa un sistema completo de **gestión de productos** integrando conceptos avanzados de arquitectura de software:

### Conceptos Clave Implementados

✅ **Anticorruption Layer (ACL)** - Aislamiento total del dominio local frente a cambios en API externa (JSONPlaceholder)  
✅ **Patrón Result<T>** - Manejo explícito de errores sin excepciones costosas  
✅ **In-Memory Store con Sincronización** - Persistencia local con ACL  
✅ **HTTP Caching Optimizado** - Headers `Last-Modified` / `If-Modified-Since` → respuestas `304 Not Modified`  
✅ **Semántica RESTful Completa** - GET, POST, PUT, DELETE, HEAD, OPTIONS  
✅ **Pruebas Exhaustivas** - 46+ pruebas unitarias con xUnit + Moq  
✅ **CI/CD Automatizado** - GitHub Actions → Google Cloud Run  
✅ **Containerizado** - Docker multietapa optimizado  

---

## ✨ Características Principales

| Característica | Descripción |
|---|---|
| **CRUD Completo** | Crear, leer, actualizar y eliminar productos |
| **Caching Inteligente** | Soporte de `If-Modified-Since` para optimizar ancho de banda |
| **Manejo de Errores** | Patrón `Result<T>` genérico sin excepciones esperadas |
| **Sincronización ACL** | Integración con servicios externos de control de acceso |
| **Documentación Automática** | OpenAPI (Swagger) + Scalar UI interactivo |
| **46+ Pruebas** | Cobertura exhaustiva del código |
| **CI/CD Completo** | Automatización GitHub Actions |
| **Containerizado** | Dockerfile multietapa para producción |
| **Aspire Integration** | Orquestación y monitoreo de servicios |

---

## 🏗️ Arquitectura del Sistema

### Diagrama de Capas

```
┌────────────────────────────────────────────┐
│    HTTP Clients (REST / OpenAPI / Scalar)  │
└─────────────────┬──────────────────────────┘
                  │
┌─────────────────▼──────────────────────────┐
│  ProductsController (7 Endpoints REST)     │
│  GET | POST | PUT | DELETE | HEAD | OPTIONS│
└─────────────────┬──────────────────────────┘
                  │
┌─────────────────▼──────────────────────────┐
│      IProductRepository                    │
│      (Interfaz de persistencia)            │
└─────────────────┬──────────────────────────┘
                  │
        ┌─────────┴─────────┐
        │                   │
┌───────▼────────┐  ┌───────▼──────────┐
│ProductMemory   │  │IProductAcl       │
│Store           │  │(Sync Service)    │
│                │  │                  │
│• GetAll()      │  │JSONPlaceholder   │
│• GetById()     │  │External API      │
│• Add()         │  │                  │
│• Update()      │  │• SyncAsync()     │
│• Delete()      │  │• FetchAsync()    │
└────────────────┘  └──────────────────┘
```

### Componentes Principales

#### 1. **ProductsController** - REST Endpoints
- 7 métodos HTTP: GET, POST, PUT, DELETE, HEAD, OPTIONS
- Validación de GUID automática
- Soporte de caching con headers HTTP
- Encapsulación de errores con `Result<T>`

#### 2. **ProductMemoryStore** - Repositorio In-Memory
- Almacenamiento singleton
- Thread-safe
- Sincronización con servicios ACL
- Validación de duplicados
- Timestamps automáticos

#### 3. **Result<T>** - Patrón de Resultado Genérico
- Encapsula éxito/fracaso
- Incluye código HTTP
- Evita excepciones para errores esperados
- Mejora testabilidad

#### 4. **JsonPlaceholderAcl** - Sincronización Externa
- Integración con JSONPlaceholder API
- Inyectable para testing
- Operaciones asincrónicas

---

## 🛠️ Tecnologías y Stack

### Stack Principal

| Componente | Versión | Propósito |
|-----------|---------|----------|
| **.NET** | 10.0 | Runtime y framework |
| **C#** | 14.0 | Lenguaje de programación |
| **ASP.NET Core** | 10.0 | Framework web REST |
| **Aspire** | Latest | Orquestación y monitoreo |
| **OpenAPI** | Built-in | Documentación automática |
| **Scalar** | Latest | UI interactivo |

### Testing

| Librería | Versión | Uso |
|----------|---------|-----|
| **xUnit** | 2.9.1 | Framework de testing |
| **Moq** | 4.20.70 | Mocking de dependencias |

### DevOps

| Herramienta | Uso |
|------------|-----|
| **GitHub Actions** | CI/CD automatizado |
| **Docker** | Containerización |
| **Google Cloud Run** | Hosting serverless |

---

## 📁 Estructura del Proyecto

```
ProductsApi/
│
├── ProductsApi/                                    # 🔷 API Principal
│   ├── Program.cs                                  # Startup configuration
│   ├── Dockerfile                                  # Multietapa production
│   ├── appsettings.*.json                          # Configuración
│   │
│   ├── Controllers/
│   │   └── ProductsController.cs                   # 7 endpoints REST
│   │
│   ├── Models/
│   │   ├── Product.cs                              # Entidad dominio
│   │   ├── Result<T>.cs                            # Patrón resultado
│   │   └── ExternalTodo.cs                         # DTO externo
│   │
│   └── Infraestructure/
│       ├── Persistence/
│       │   ├── IProductRepository.cs               # Contrato
│       │   └── ProductMemoryStore.cs              # In-memory impl
│       │
│       ├── IProductAcl.cs                          # Contrato ACL
│       └── JsonPlaceholderAcl.cs                   # ACL impl
│
├── ProductsApi.AppHost/                            # 🧬 Aspire Orchestrator
│   ├── AppHost.cs                                  # Service definitions
│   ├── appsettings.*.json                          # Configuración
│   └── ProductsApi.AppHost.csproj
│
├── ProductsApi.ServiceDefaults/                    # 🔧 Shared Configuration
│   ├── Extensions.cs                               # DI extensions
│   └── ProductsApi.ServiceDefaults.csproj
│
├── ProductsApi.Tests/                              # 🧪 Unit Tests (46+)
│   ├── Controllers/
│   │   └── ProductsControllerTests.cs              # 12 tests
│   │
│   ├── Infraestructure/Persistence/
│   │   └── ProductMemoryStoreTests.cs             # 38 tests
│   │
│   ├── Models/
│   │   ├── ProductTests.cs                         # 7 tests
│   │   └── ResultTests.cs                          # 6 tests
│   │
│   ├── ProductsApi.Tests.csproj
│   └── README.md                                   # Docs de testing
│
├── .github/
│   └── workflows/
│       └── deploy.yml                              # Pipeline CI/CD
│
├── ProductsApi.slnx                                # Solución moderna
├── ProductsApi.http                                # Test requests
├── .dockerignore
├── LICENSE
└── README.md                                       # Este archivo
```

---

## 📋 Requisitos Previos

### Local

- **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** - Runtime y herramientas
- **Visual Studio 2026** o **VS Code** + C# DevKit
- **Git** - Control de versiones
- **.NET Aspire** - Carga de trabajo (incluida en VS 2026)

### Cloud (Google Cloud Run)

- Cuenta **Google Cloud** con billing habilitado
- **[gcloud CLI](https://cloud.google.com/sdk/docs/install)** instalado
- Permisos de Editor en proyecto GCP

---

## 🚀 Instalación y Configuración

### Paso 1: Clonar el Repositorio

```bash
git clone https://github.com/SamuelColmenares/unisabana-products-api.git
cd unisabana-products-api
```

### Paso 2: Restaurar Dependencias

```bash
dotnet restore ProductsApi.slnx
```

### Paso 3: Compilar Solución

```bash
dotnet build ProductsApi.slnx --configuration Release
```

---

## 🏃 Ejecución Local

### Opción A: Visual Studio (Recomendado)

1. Abre `ProductsApi.slnx` en Visual Studio 2026
2. Establece `ProductsApi.AppHost` como startup project
3. Presiona **F5** o **Debug → Start Debugging**
4. Se abrirá el **Dashboard de Aspire** en `https://localhost:17145`
5. Haz clic en el link de `ProductsApi` para acceder a la API

### Opción B: Terminal (CLI)

```bash
# Desde raíz del proyecto
dotnet run --project ProductsApi.AppHost --configuration Release

# Dashboard estará disponible en:
# https://localhost:17145
```

### Opción C: Docker Local

```bash
# Construir imagen
docker build -f ProductsApi/Dockerfile -t productsapi:latest .

# Ejecutar contenedor
docker run -p 8080:8080 productsapi:latest

# Acceder a API
curl http://localhost:8080/api/products
```

---

## 🔗 Referencia de URLs

| Componente | URL | Descripción |
|-----------|-----|------------|
| **Dashboard Aspire** | `https://localhost:17145/` | Orquestación y monitoreo |
| **API HTTPS** | `https://localhost:7187` | Endpoint seguro |
| **API HTTP** | `http://localhost:5019` | Endpoint no seguro |
| **Swagger UI** | `https://localhost:7187/swagger` | Documentación (OpenAPI) |
| **Scalar UI** | `https://localhost:7187/scalar` | Visor interactivo (recomendado) |
| **OpenAPI JSON** | `https://localhost:7187/openapi/v1.json` | Especificación técnica |

---

## 📡 Endpoints de la API

### 1. GET /api/products - Obtener Todos

```bash
curl https://localhost:7187/api/products
```

**Respuesta (200 OK):**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Laptop",
    "isActive": true,
    "lastUpdated": "2024-01-15T10:30:00Z"
  }
]
```

### 2. GET /api/products/{id} - Obtener por ID

Con soporte de caching:

```bash
# Primera petición
curl -v https://localhost:7187/api/products/550e8400-e29b-41d4-a716-446655440000

# Respuesta incluye: Last-Modified header
# Repetir con If-Modified-Since → 304 Not Modified
```

### 3. POST /api/products - Crear

```bash
curl -X POST https://localhost:7187/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "name": "Teclado",
    "isActive": true,
    "lastUpdated": "2024-01-15T11:00:00Z"
  }'
```

### 4. PUT /api/products/{id} - Actualizar

```bash
curl -X PUT https://localhost:7187/api/products/550e8400-e29b-41d4-a716-446655440000 \
  -H "Content-Type: application/json" \
  -d '{
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Laptop Updated",
    "isActive": true,
    "lastUpdated": "2024-01-15T12:00:00Z"
  }'
```

### 5. DELETE /api/products/{id} - Eliminar

```bash
curl -X DELETE https://localhost:7187/api/products/550e8400-e29b-41d4-a716-446655440000
```

### 6. HEAD /api/products/{id} - Metadatos

```bash
curl -I https://localhost:7187/api/products/550e8400-e29b-41d4-a716-446655440000
```

### 7. OPTIONS /api/products - Métodos Permitidos

```bash
curl -X OPTIONS https://localhost:7187/api/products
# Respuesta: Allow: GET, POST, PUT, DELETE, OPTIONS, HEAD
```

---

## 🧪 Suite de Pruebas Unitarias

### Resumen

```
📊 ESTADÍSTICAS
━━━━━━━━━━━━━━━━━━━━━━━━━
Total:    46 pruebas
Exitosas: 46 ✅
Fallidas:  0
Tiempo:   ~2-3 segundos

Desglose:
• ProductMemoryStoreTests:   38 (83%)
• ProductsControllerTests:   12 (26%)
• ResultTests:                6 (13%)
• ProductTests:               7 (15%)
```

### Ejecutar Pruebas

```bash
# Todas las pruebas
dotnet test ProductsApi.slnx

# Con salida detallada
dotnet test ProductsApi.slnx --verbosity detailed

# Por categoría
dotnet test ProductsApi.slnx --filter "ProductMemoryStoreTests"

# Prueba específica
dotnet test ProductsApi.slnx --filter "Name~GetAll_ShouldReturnEmptyCollection"
```

### Cobertura por Módulo

**ProductMemoryStoreTests (38)** - CRUD completo
- GetAll (2), GetById (3), Add (4), Update (4), Delete (4), Integration (1)

**ProductsControllerTests (12)** - Endpoints REST
- GetAll (2), Get (2), Post (2), Put (3), Delete (2), Options (1)

**ResultTests (6)** - Patrón genérico
- Success, Failure, Custom StatusCode, Generics

**ProductTests (7)** - Entidad dominio
- Creación, Propiedades, IDs únicos, Timestamps

---

## 🎯 Patrones de Implementación

### 1. Result<T> - Railway-Oriented Programming

```csharp
// ✅ Bueno: Sin excepciones esperadas
var result = store.GetById(id);
return result.IsSuccess 
    ? Ok(result.Value) 
    : MapError(result);

// ❌ Anti-patrón
try { throw new NotFoundException(); }
catch { return StatusCode(404); }
```

### 2. Anticorruption Layer (ACL)

```csharp
// Aisla dominio local de API externa
public interface IProductAcl
{
    Task SyncProductAsync(Product product, CancellationToken ct);
    Task<List<Product>> FetchInitialProductsAsync(CancellationToken ct);
}

// Implementación inyectable
builder.Services.AddHttpClient<IProductAcl, JsonPlaceholderAcl>(c =>
{
    c.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});
```

### 3. Inyección de Dependencias

```csharp
// Constructor injection
public ProductsController(IProductRepository store) : ControllerBase { }

// DI nativa de ASP.NET Core
builder.Services.AddSingleton<IProductRepository, ProductMemoryStore>();
```

### 4. Arrange-Act-Assert (AAA)

Todas las pruebas siguen este patrón:

```csharp
[Fact]
public void Add_ShouldAddProduct_WhenProductIsNew()
{
    // Arrange - Preparar
    var product = new Product { Id = Guid.NewGuid(), Name = "Test" };
    _mockAcl.Setup(x => x.SyncProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

    // Act - Ejecutar
    var result = await _store.Add(product, CancellationToken.None);

    // Assert - Verificar
    Assert.True(result.IsSuccess);
}
```

### 5. HTTP Caching (RFC 7232)

```csharp
// Cliente: If-Modified-Since header
// Servidor: Validar y retornar 304 si no cambió
if (DateTime.TryParse(ifModified, out var date) && date >= product.LastUpdated)
    return StatusCode(304);  // Not Modified

Response.Headers[HeaderNames.LastModified] = product.LastUpdated.ToString("R");
```

---

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

**Flujo:**

```
Push → CI (Test) → CD (Build Docker → Deploy Cloud Run)
```

**Archivo:** `.github/workflows/deploy.yml`

**Pasos:**

1. **CI Job** (ubuntu-latest)
   - Setup .NET 10
   - Restore dependencies
   - Build Release
   - Run 46 unit tests

2. **CD Job** (necesita CI exitoso)
   - Auth Google Cloud (Workload Identity)
   - Build imagen Docker
   - Push a Artifact Registry
   - Deploy a Cloud Run

**Secretos Requeridos:**
- `GCP_PROJECT_ID` - ID del proyecto GCP
- `WIF_PROVIDER` - Workload Identity Provider
- `WIF_SERVICE_ACCOUNT` - Service Account email

---

## 🐳 Docker y Despliegue

### Dockerfile Multietapa

```dockerfile
# Stage 1: Runtime base
FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0
RUN dotnet restore && dotnet build

# Stage 3: Publish
RUN dotnet publish -c Release

# Stage 4: Final (solo runtime)
FROM base
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductsApi.dll"]
```

**Optimizaciones:**
- Multi-etapa: 500MB → 200MB
- Solo runtime en imagen final
- Caché optimizado

### Despliegue Cloud Run

Automático vía GitHub Actions o manual:

```bash
gcloud run deploy products-api \
  --image us-central1-docker.pkg.dev/PROJECT/REPO/products-api:latest \
  --region us-central1
```

---

## 📚 Documentación Interactiva

### Scalar UI (Recomendado)

**Acceso:** `https://localhost:7187/scalar`

**Características:**
- Interfaz moderna e intuitiva
- Prueba endpoints interactivamente
- Inspección de headers y responses
- Generación automática de código cliente

### Swagger UI

**Acceso:** `https://localhost:7187/swagger`

**Características:**
- Exploración clásica de endpoints
- Documentación automática
- Try it out

### OpenAPI JSON

**Acceso:** `https://localhost:7187/openapi/v1.json`

**Uso:**
- Generación de clientes (Swagger Codegen, NSwag)
- Integración con herramientas
- Documento contractual

---

## ✅ Contribución y Mejores Prácticas

### Checklist Antes de Push

- [ ] Compilación sin warnings: `dotnet build`
- [ ] Todas las pruebas pasan: `dotnet test`
- [ ] Nuevas pruebas para nuevas features
- [ ] Documentación XML añadida
- [ ] Nombres de pruebas descriptivos

### Convención de Nombres de Pruebas

```csharp
// ✅ Bueno
[Fact]
public void Add_ShouldReturnFailure_WhenProductAlreadyExists()

// ❌ Malo
[Fact]
public void TestAdd()
```

### Workflow de Desarrollo

```bash
git checkout -b feature/my-feature
# ... hacer cambios ...
dotnet test  # ✅ debe pasar
git add .
git commit -m "feat: Add new feature"
git push origin feature/my-feature
# Crear PR en GitHub
```

---

## 🔧 Resolución de Problemas

### Pruebas Fallan Localmente

```bash
# Limpiar y reconstruir
dotnet clean ProductsApi.slnx
dotnet restore ProductsApi.slnx --force
dotnet build ProductsApi.slnx
dotnet test ProductsApi.slnx --verbosity detailed
```

### Puerto en Uso

```bash
# Windows
netstat -ano | findstr :7187
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :7187
kill -9 <PID>
```

### Error en Sincronización ACL

```bash
# Verificar conectividad
curl https://jsonplaceholder.typicode.com/posts

# Aumentar timeout en Program.cs
c.Timeout = TimeSpan.FromSeconds(10);
```

### Docker No Encuentra Archivo

```bash
# Ejecutar desde raíz del repositorio, NO desde ProductsApi/
cd unisabana-products-api
docker build -f ProductsApi/Dockerfile -t productsapi:latest .
```

---

## 📞 Contacto y Soporte

- **Repository**: https://github.com/SamuelColmenares/unisabana-products-api
- **Issues**: [GitHub Issues](https://github.com/SamuelColmenares/unisabana-products-api/issues)
- **Autor**: Samuel Colmenares
- **Universidad**: Universidad de La Sabana

---

**Última Actualización**: Enero 2025  
**Versión**: 2.0  
**Estado**: Producción ✅  
**Licencia**: MIT
```
ProductsApi
├─ .dockerignore
├─ LICENSE
├─ ProductsApi
│  ├─ appsettings.Development.json
│  ├─ appsettings.json
│  ├─ Controllers
│  │  └─ ProductsController.cs
│  ├─ Dockerfile
│  ├─ Infraestructure
│  │  ├─ IProductAcl.cs
│  │  ├─ JsonPlaceholderAcl.cs
│  │  └─ Persistence
│  │     ├─ IProductRepository.cs
│  │     └─ ProductMemoryStore.cs
│  ├─ Models
│  │  ├─ ExternalTodo.cs
│  │  ├─ Product.cs
│  │  └─ Result.cs
│  ├─ ProductsApi.csproj
│  ├─ ProductsApi.csproj.user
│  ├─ ProductsApi.http
│  ├─ Program.cs
│  └─ Properties
│     └─ launchSettings.json
├─ ProductsApi.AppHost
│  ├─ AppHost.cs
│  ├─ appsettings.Development.json
│  ├─ appsettings.json
│  ├─ ProductsApi.AppHost.csproj
│  └─ Properties
│     └─ launchSettings.json
├─ ProductsApi.ServiceDefaults
│  ├─ Extensions.cs
│  └─ ProductsApi.ServiceDefaults.csproj
├─ ProductsApi.slnx
├─ ProductsApi.Tests
│  ├─ Controllers
│  │  └─ ProductsControllerTests.cs
│  ├─ Infraestructure
│  │  └─ Persistence
│  │     └─ ProductMemoryStoreTests.cs
│  ├─ Models
│  │  ├─ ProductTests.cs
│  │  └─ ResultTests.cs
│  ├─ ProductsApi.Tests.csproj
│  └─ README.md
└─ README.md

```