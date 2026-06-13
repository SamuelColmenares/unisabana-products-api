# ProductsApi.Tests - Suite de Pruebas Unitarias

## Descripción

`ProductsApi.Tests` es un proyecto de pruebas unitarias para la API de Productos, utilizando **xUnit** como framework de testing y **Moq** para mocking de dependencias.

## Estructura del Proyecto

```
ProductsApi.Tests/
├── Controllers/
│   └── ProductsControllerTests.cs      # Pruebas del controlador REST
├── Infraestructure/
│   └── Persistence/
│       └── ProductMemoryStoreTests.cs  # Pruebas del repositorio en memoria
├── Models/
│   ├── ProductTests.cs                 # Pruebas del modelo Product
│   └── ResultTests.cs                  # Pruebas del patrón Result<T>
└── ProductsApi.Tests.csproj            # Configuración del proyecto
```

## Tecnologías

- **Framework**: xUnit v2.9.1
- **Mocking**: Moq v4.20.70
- **SDK**: .NET 10.0
- **Lenguaje**: C# 14.0

## Ejecutar las Pruebas

### Ejecutar todas las pruebas
```bash
dotnet test ProductsApi.Tests/ProductsApi.Tests.csproj
```

### Ejecutar con salida detallada
```bash
dotnet test ProductsApi.Tests/ProductsApi.Tests.csproj --verbosity detailed
```

### Ejecutar pruebas específicas
```bash
dotnet test ProductsApi.Tests/ProductsApi.Tests.csproj --filter "ProductMemoryStoreTests"
```

## Cobertura de Pruebas

### ProductMemoryStoreTests (38 pruebas)

Suite completa de pruebas para el repositorio en memoria de productos.

#### GetAll
- ✅ `GetAll_ShouldReturnEmptyCollection_WhenNoProductsExist` - Valida que devuelve colección vacía
- ✅ `GetAll_ShouldReturnAllLoadedProducts` - Valida que devuelve todos los productos cargados

#### GetById
- ✅ `GetById_ShouldReturnSuccess_WhenProductExists` - Valida obtención exitosa
- ✅ `GetById_ShouldReturnFailure_WhenProductDoesNotExist` - Valida error 404
- ✅ `GetById_ShouldReturnCorrectProduct_WhenMultipleProductsExist` - Valida selección correcta

#### Add
- ✅ `Add_ShouldAddProduct_WhenProductIsNew` - Valida creación exitosa
- ✅ `Add_ShouldReturnFailure_WhenProductAlreadyExists` - Valida duplicados
- ✅ `Add_ShouldReturnFailure_WhenAclSyncFails` - Valida fallos de sincronización
- ✅ `Add_ShouldAddMultipleProducts_WhenProductsAreDifferent` - Valida múltiples productos

#### Update
- ✅ `Update_ShouldUpdateProduct_WhenProductExists` - Valida actualización exitosa
- ✅ `Update_ShouldReturnFailure_WhenProductDoesNotExist` - Valida error 404
- ✅ `Update_ShouldReturnFailure_WhenAclSyncFails` - Valida fallos de sincronización
- ✅ `Update_ShouldUpdateLastModifiedDate` - Valida actualización de timestamp

#### Delete
- ✅ `Delete_ShouldDeleteProduct_WhenProductExists` - Valida eliminación exitosa
- ✅ `Delete_ShouldReturnFailure_WhenProductDoesNotExist` - Valida error 404
- ✅ `Delete_ShouldDeleteCorrectProduct_WhenMultipleProductsExist` - Valida selección correcta
- ✅ `Delete_ShouldNotDeleteOtherProducts` - Valida eliminación selectiva

#### Integration
- ✅ `CompleteFlow_ShouldExecuteCrudOperations` - Valida flujo CRUD completo

### ResultTests (6 pruebas)

Pruebas del patrón genérico `Result<T>` para manejo de errores.

- ✅ `Success_ShouldCreateSuccessResult` - Valida creación de resultado exitoso
- ✅ `Failure_ShouldCreateFailureResultWithDefaultStatusCode` - Valida error con código 400
- ✅ `Failure_ShouldCreateFailureResultWithCustomStatusCode` - Valida error con código personalizado
- ✅ `Success_ShouldWorkWithIntType` - Valida con tipos genéricos
- ✅ `Success_ShouldWorkWithComplexTypes` - Valida con objetos complejos
- ✅ `Result_ShouldHaveCorrectDefaults` - Valida valores por defecto

### ProductTests (7 pruebas)

Pruebas del modelo `Product`.

- ✅ `Product_ShouldCreateWithParameters` - Valida creación con parámetros
- ✅ `Product_DefaultConstructor_ShouldInitializeWithDefaults` - Valida constructor sin parámetros
- ✅ `Product_PropertiesShouldBeUpdatable` - Valida actualización de propiedades
- ✅ `Product_ShouldMaintainUniqueIds` - Valida IDs únicos
- ✅ `Product_ShouldInitializeWithVariousStates` - Valida múltiples estados
- ✅ `Product_LastUpdated_ShouldBeUpdatable` - Valida actualización de timestamp

### ProductsControllerTests (12 pruebas)

Pruebas de los endpoints REST del controlador.

#### GetAll
- ✅ `GetAll_ShouldReturnOk_WithAllProducts` - Valida devolución de todos los productos
- ✅ `GetAll_ShouldReturnOk_WithEmptyList` - Valida lista vacía

#### Get
- ✅ `Get_ShouldReturnProduct_WhenExists` - Valida obtención por ID
- ✅ `Get_ShouldReturnNotFound_WhenDoesNotExist` - Valida error 404

#### Post
- ✅ `Post_ShouldReturnCreatedAtAction_WhenSuccess` - Valida creación exitosa
- ✅ `Post_ShouldReturnBadRequest_WhenFails` - Valida manejo de errores

#### Put
- ✅ `Put_ShouldReturnNoContent_WhenSuccess` - Valida actualización exitosa
- ✅ `Put_ShouldReturnBadRequest_WhenIdsDoNotMatch` - Valida validación de IDs
- ✅ `Put_ShouldReturnNotFound_WhenProductDoesNotExist` - Valida error 404

#### Delete
- ✅ `Delete_ShouldReturnNoContent_WhenSuccess` - Valida eliminación exitosa
- ✅ `Delete_ShouldReturnNotFound_WhenProductDoesNotExist` - Valida error 404

#### Options
- ✅ `Options_ShouldReturnAllowedMethods` - Valida métodos permitidos

## Resultado de Pruebas

```
Test summary: total: 46, failed: 0, succeeded: 46, skipped: 0
```

## Patrones Utilizados

### Arrange-Act-Assert (AAA)
Todas las pruebas siguen el patrón AAA para claridad y consistencia:

```csharp
[Fact]
public void SampleTest()
{
    // Arrange - Preparar datos y mocks
    var product = new Product { Id = Guid.NewGuid(), Name = "Test" };
    
    // Act - Ejecutar la acción
    var result = store.GetById(product.Id);
    
    // Assert - Verificar resultados
    Assert.True(result.IsSuccess);
}
```

### Mocking con Moq
Se utilizan mocks para aislar las unidades bajo prueba:

```csharp
_mockRepository.Setup(x => x.Add(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(Result<bool>.Success(true));
```

## Mejores Prácticas

1. ✅ **Pruebas independientes**: Cada prueba es independiente y puede ejecutarse en cualquier orden
2. ✅ **Mocks claros**: Se utilizan mocks explícitos con Moq
3. ✅ **Nombres descriptivos**: Los nombres de las pruebas describen qué se prueba y qué se espera
4. ✅ **Casos límite**: Se incluyen pruebas para casos de error y situaciones excepcionales
5. ✅ **Documentación XML**: Cada prueba incluye comentarios XML explicativos
6. ✅ **Datos de prueba**: Se utilizan datos representativos y variados

## Contribuir

Al agregar nuevas funcionalidades:

1. ✅ Escribir pruebas primero (TDD preferible)
2. ✅ Seguir el patrón AAA
3. ✅ Nombrar pruebas descriptivamente
4. ✅ Agregar documentación XML
5. ✅ Asegurar que todas las pruebas pasen antes de hacer commit

## Integración Continua

Estas pruebas están configuradas para ejecutarse en el pipeline de CI/CD. Ver `.github/workflows/` para más detalles.
