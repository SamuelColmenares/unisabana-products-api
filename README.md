# 🚀 RESTful Architecture Demo (.NET 10 + Aspire)

Este proyecto es una demostración técnica de una arquitectura **RESTful de alta madurez**, diseñada para ser escalable, desacoplada y eficiente. Utiliza **.NET 10** y **.NET Aspire** para la orquestación y monitoreo de servicios.

## 📌 Descripción del Proyecto
El ejemplo implementa un CRUD completo de productos, integrando conceptos avanzados de arquitectura de software discutidos para la exposición:

* **Anticorruption Layer (ACL):** Aislamiento total del dominio local frente a cambios en la API externa (JSONPlaceholder). La lógica de negocio no se contamina con modelos externos.
* **Patrón Result:** Manejo de flujo de control y errores de forma explícita sin el uso costoso de excepciones, mejorando el rendimiento y la semántica de las respuestas.
* **In-Memory Store con Sincronización:** Persistencia local en memoria que se inicializa a través de la ACL y mantiene un estado funcional durante la ejecución.
* **Optimización de Protocolo (HTTP Caching):** Implementación de encabezados `Last-Modified` y validación condicional `If-Modified-Since` para soportar respuestas `304 Not Modified`.
* **Semántica Completa y Verbos:** Soporte para `GET`, `POST`, `PUT`, `DELETE`, `PATCH`, `HEAD` y `OPTIONS`.

---

## 🛠️ Cómo Ejecutar el Proyecto

### 1. Desde la Interfaz Gráfica (Visual Studio / Rider)
1. Abre la solución en tu IDE favorito.
2. Asegúrate de tener instalado el **SDK de .NET 10** y la carga de trabajo de **.NET Aspire**.
3. Selecciona el proyecto **`ProductsApi.AppHost`** como proyecto de inicio.
4. Haz clic en el botón **Ejecutar** (usando el perfil de `https`).
5. Se abrirá automáticamente el Dashboard de .NET Aspire en el puerto `17145`.



### 2. Desde la Línea de Comandos (CLI)
Navega hasta la carpeta raíz del proyecto o específicamente a la carpeta del `AppHost` y ejecuta:

```bash
dotnet run --project ProductsApi.AppHost
```
---
## 🧪 Pruebas con Scalar UI
Para esta demostración, hemos sustituido Swagger por **Scalar**, un visor de documentación de API moderno e interactivo que facilita las pruebas de los verbos HTTP y la inspección de headers.

1. Abre el navegador en: `https://localhost:7187/scalar`
2. **Prueba de Verbos:**
    * **GET**: Realiza una petición y observa el header `Last-Modified`. Si repites la petición sin cambios, el servidor responderá con un `304`.
    * **OPTIONS**: Úsalo para inspeccionar qué métodos permite el recurso en los headers de respuesta.
    * **HEAD**: Verifica los metadatos (headers) sin descargar el cuerpo de la respuesta.
    * **CRUD (POST/PUT/DELETE)**: Opera sobre la memoria local y observa cómo la ACL intenta sincronizar los cambios.



---

## 📄 Contrato OpenAPI (JSON)
El corazón de nuestra documentación es el archivo **OpenAPI JSON**. Este es un esquema técnico estandarizado que describe cada endpoint, modelo y regla de nuestra API. Es la "fuente de la verdad" que Scalar interpreta para generar la interfaz de usuario.

* **Ruta del JSON:** `https://localhost:7187/openapi/v1.json`

> **Definición corta:** El JSON de OpenAPI es un documento de metadatos que define el contrato de comunicación de la API, permitiendo la interoperabilidad y la generación automática de clientes.

---

## 🔗 Referencia de URLs Rápidas

| Componente | URL |
| :--- | :--- |
| **Dashboard de Aspire** | [https://localhost:17145/](https://localhost:17145/) |
| **API Endpoint (HTTPS)** | [https://localhost:7187](https://localhost:7187) |
| **API Endpoint (HTTP)** | [http://localhost:5019](http://localhost:5019) |
| **Scalar UI (Visor de pruebas)** | [https://localhost:7187/scalar](https://localhost:7187/scalar) |

---
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