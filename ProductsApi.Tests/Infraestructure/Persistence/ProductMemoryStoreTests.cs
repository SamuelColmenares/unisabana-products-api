using Moq;
using ProductsApi.Infraestructure;
using ProductsApi.Infraestructure.Persistence;
using ProductsApi.Models;
using Xunit;

namespace ProductsApi.Tests.Infraestructure.Persistence;

/// <summary>
/// Suite de pruebas unitarias para ProductMemoryStore.
/// Valida las operaciones CRUD del almacén en memoria de productos.
/// </summary>
public class ProductMemoryStoreTests
{
    private readonly Mock<IProductAcl> _mockAcl;
    private readonly ProductMemoryStore _store;
    private readonly Product _testProduct;

    public ProductMemoryStoreTests()
    {
        _mockAcl = new Mock<IProductAcl>();
        _store = new ProductMemoryStore(_mockAcl.Object);
        
        _testProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Producto de Prueba",
            IsActive = true,
            LastUpdated = DateTime.UtcNow
        };
    }

    #region GetAll Tests

    /// <summary>
    /// Verifica que GetAll devuelve una colección vacía cuando no hay productos.
    /// </summary>
    [Fact]
    public void GetAll_ShouldReturnEmptyCollection_WhenNoProductsExist()
    {
        // Arrange
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());
        var store = new ProductMemoryStore(_mockAcl.Object);

        // Act
        var result = store.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifica que GetAll devuelve todos los productos cargados inicialmente.
    /// </summary>
    [Fact]
    public void GetAll_ShouldReturnAllLoadedProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = Guid.NewGuid(), Name = "Producto 1", IsActive = true, LastUpdated = DateTime.UtcNow },
            new Product { Id = Guid.NewGuid(), Name = "Producto 2", IsActive = false, LastUpdated = DateTime.UtcNow }
        };
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        var store = new ProductMemoryStore(_mockAcl.Object);

        // Act
        var result = store.GetAll().ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Producto 1");
        Assert.Contains(result, p => p.Name == "Producto 2");
    }

    #endregion

    #region GetById Tests

    /// <summary>
    /// Verifica que GetById devuelve un resultado exitoso cuando el producto existe.
    /// </summary>
    [Fact]
    public void GetById_ShouldReturnSuccess_WhenProductExists()
    {
        // Arrange
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { _testProduct });
        var store = new ProductMemoryStore(_mockAcl.Object);

        // Act
        var result = store.GetById(_testProduct.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_testProduct.Id, result.Value.Id);
        Assert.Equal(_testProduct.Name, result.Value.Name);
    }

    /// <summary>
    /// Verifica que GetById devuelve un error 404 cuando el producto no existe.
    /// </summary>
    [Fact]
    public void GetById_ShouldReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _store.GetById(nonExistentId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("Producto no encontrado", result.Error);
        Assert.Equal(404, result.StatusCode);
    }

    /// <summary>
    /// Verifica que GetById devuelve el producto correcto entre múltiples productos.
    /// </summary>
    [Fact]
    public void GetById_ShouldReturnCorrectProduct_WhenMultipleProductsExist()
    {
        // Arrange
        var product1 = new Product { Id = Guid.NewGuid(), Name = "Producto 1", IsActive = true, LastUpdated = DateTime.UtcNow };
        var product2 = new Product { Id = Guid.NewGuid(), Name = "Producto 2", IsActive = false, LastUpdated = DateTime.UtcNow };
        var products = new List<Product> { product1, product2 };
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        var store = new ProductMemoryStore(_mockAcl.Object);

        // Act
        var result = store.GetById(product2.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(product2.Id, result.Value!.Id);
        Assert.Equal(product2.Name, result.Value.Name);
    }

    #endregion

    #region Add Tests

    /// <summary>
    /// Verifica que Add agrega exitosamente un nuevo producto.
    /// </summary>
    [Fact]
    public async Task Add_ShouldAddProduct_WhenProductIsNew()
    {
        // Arrange
        var newProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Nuevo Producto",
            IsActive = true,
            LastUpdated = DateTime.UtcNow
        };
        _mockAcl.Setup(x => x.PushUpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _store.Add(newProduct);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        var retrievedProduct = _store.GetById(newProduct.Id);
        Assert.True(retrievedProduct.IsSuccess);
        Assert.Equal(newProduct.Id, retrievedProduct.Value!.Id);
        _mockAcl.Verify(x => x.PushUpdateAsync(newProduct, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifica que Add falla cuando el producto ya existe.
    /// </summary>
    [Fact]
    public async Task Add_ShouldReturnFailure_WhenProductAlreadyExists()
    {
        // Arrange
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { _testProduct });
        var store = new ProductMemoryStore(_mockAcl.Object);

        // Act
        var result = await store.Add(_testProduct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Producto con el mismo ID ya existe", result.Error);
        Assert.Equal(400, result.StatusCode);
    }

    /// <summary>
    /// Verifica que Add falla cuando la sincronización con ACL falla.
    /// </summary>
    [Fact]
    public async Task Add_ShouldReturnFailure_WhenAclSyncFails()
    {
        // Arrange
        var newProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Nuevo Producto",
            IsActive = true,
            LastUpdated = DateTime.UtcNow
        };
        _mockAcl.Setup(x => x.PushUpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _store.Add(newProduct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Error al agregar en ACL", result.Error);
        Assert.Equal(500, result.StatusCode);
        var retrievedProduct = _store.GetById(newProduct.Id);
        Assert.False(retrievedProduct.IsSuccess);
    }

    /// <summary>
    /// Verifica que Add puede agregar múltiples productos diferentes.
    /// </summary>
    [Fact]
    public async Task Add_ShouldAddMultipleProducts_WhenProductsAreDifferent()
    {
        // Arrange
        var product1 = new Product { Id = Guid.NewGuid(), Name = "Producto 1", IsActive = true, LastUpdated = DateTime.UtcNow };
        var product2 = new Product { Id = Guid.NewGuid(), Name = "Producto 2", IsActive = false, LastUpdated = DateTime.UtcNow };
        _mockAcl.Setup(x => x.PushUpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result1 = await _store.Add(product1);
        var result2 = await _store.Add(product2);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        var allProducts = _store.GetAll().ToList();
        Assert.Equal(2, allProducts.Count);
    }

    #endregion

    #region Update Tests

    /// <summary>
    /// Verifica que Update actualiza exitosamente un producto existente.
    /// </summary>
    [Fact]
    public async Task Update_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { _testProduct });
        var store = new ProductMemoryStore(_mockAcl.Object);
        
        var updatedProduct = new Product
        {
            Id = _testProduct.Id,
            Name = "Producto Actualizado",
            IsActive = false,
            LastUpdated = DateTime.UtcNow
        };
        _mockAcl.Setup(x => x.PushUpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await store.Update(updatedProduct);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Producto Actualizado", result.Value!.Name);
        Assert.False(result.Value.IsActive);
        var retrievedProduct = store.GetById(_testProduct.Id);
        Assert.Equal("Producto Actualizado", retrievedProduct.Value!.Name);
    }

    /// <summary>
    /// Verifica que Update falla cuando el producto no existe.
    /// </summary>
    [Fact]
    public async Task Update_ShouldReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Producto Inexistente",
            IsActive = true,
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var result = await _store.Update(nonExistentProduct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("No se puede actualizar: Producto inexistente", result.Error);
        Assert.Equal(404, result.StatusCode);
    }

    /// <summary>
    /// Verifica que Update falla cuando la sincronización con ACL falla.
    /// </summary>
    [Fact]
    public async Task Update_ShouldReturnFailure_WhenAclSyncFails()
    {
        // Arrange
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { _testProduct });
        var store = new ProductMemoryStore(_mockAcl.Object);
        
        var updatedProduct = new Product
        {
            Id = _testProduct.Id,
            Name = "Producto Actualizado",
            IsActive = false,
            LastUpdated = DateTime.UtcNow
        };
        _mockAcl.Setup(x => x.PushUpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await store.Update(updatedProduct);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Error al actualizar en ACL", result.Error);
        Assert.Equal(500, result.StatusCode);
    }

    /// <summary>
    /// Verifica que Update actualiza la fecha LastUpdated del producto.
    /// </summary>
    [Fact]
    public async Task Update_ShouldUpdateLastModifiedDate()
    {
        // Arrange
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { _testProduct });
        var store = new ProductMemoryStore(_mockAcl.Object);
        
        var oldDate = _testProduct.LastUpdated;
        await Task.Delay(100); // Pequeña pausa para asegurar diferencia temporal
        
        var updatedProduct = new Product
        {
            Id = _testProduct.Id,
            Name = _testProduct.Name,
            IsActive = _testProduct.IsActive,
            LastUpdated = oldDate
        };
        _mockAcl.Setup(x => x.PushUpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await store.Update(updatedProduct);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.LastUpdated > oldDate);
    }

    #endregion

    #region Delete Tests

    /// <summary>
    /// Verifica que Delete elimina exitosamente un producto existente.
    /// </summary>
    [Fact]
    public void Delete_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { _testProduct });
        var store = new ProductMemoryStore(_mockAcl.Object);

        // Act
        var result = store.Delete(_testProduct.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        var retrievedProduct = store.GetById(_testProduct.Id);
        Assert.False(retrievedProduct.IsSuccess);
    }

    /// <summary>
    /// Verifica que Delete falla cuando el producto no existe.
    /// </summary>
    [Fact]
    public void Delete_ShouldReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _store.Delete(nonExistentId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("No se encontró el producto para eliminar", result.Error);
        Assert.Equal(404, result.StatusCode);
    }

    /// <summary>
    /// Verifica que Delete puede eliminar un producto de múltiples productos.
    /// </summary>
    [Fact]
    public void Delete_ShouldDeleteCorrectProduct_WhenMultipleProductsExist()
    {
        // Arrange
        var product1 = new Product { Id = Guid.NewGuid(), Name = "Producto 1", IsActive = true, LastUpdated = DateTime.UtcNow };
        var product2 = new Product { Id = Guid.NewGuid(), Name = "Producto 2", IsActive = false, LastUpdated = DateTime.UtcNow };
        var products = new List<Product> { product1, product2 };
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        var store = new ProductMemoryStore(_mockAcl.Object);

        // Act
        var result = store.Delete(product1.Id);

        // Assert
        Assert.True(result.IsSuccess);
        var allProducts = store.GetAll().ToList();
        Assert.Single(allProducts);
        Assert.Equal(product2.Id, allProducts[0].Id);
    }

    /// <summary>
    /// Verifica que Delete solo elimina el producto especificado.
    /// </summary>
    [Fact]
    public void Delete_ShouldNotDeleteOtherProducts()
    {
        // Arrange
        var product1 = new Product { Id = Guid.NewGuid(), Name = "Producto 1", IsActive = true, LastUpdated = DateTime.UtcNow };
        var product2 = new Product { Id = Guid.NewGuid(), Name = "Producto 2", IsActive = false, LastUpdated = DateTime.UtcNow };
        var products = new List<Product> { product1, product2 };
        _mockAcl.Setup(x => x.FetchInitialProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        var store = new ProductMemoryStore(_mockAcl.Object);

        // Act
        store.Delete(product1.Id);

        // Assert
        var result = store.GetById(product2.Id);
        Assert.True(result.IsSuccess);
        Assert.Equal(product2.Id, result.Value!.Id);
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// Verifica el flujo completo de operaciones CRUD.
    /// </summary>
    [Fact]
    public async Task CompleteFlow_ShouldExecuteCrudOperations()
    {
        // Arrange
        _mockAcl.Setup(x => x.PushUpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Producto Original",
            IsActive = true,
            LastUpdated = DateTime.UtcNow
        };

        // Act & Assert - CREATE
        var addResult = await _store.Add(product);
        Assert.True(addResult.IsSuccess);

        // Act & Assert - READ
        var getResult = _store.GetById(product.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("Producto Original", getResult.Value!.Name);

        // Act & Assert - UPDATE
        var updatedProduct = new Product
        {
            Id = product.Id,
            Name = "Producto Actualizado",
            IsActive = false,
            LastUpdated = DateTime.UtcNow
        };
        var updateResult = await _store.Update(updatedProduct);
        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Producto Actualizado", updateResult.Value!.Name);

        // Act & Assert - READ UPDATED
        var getUpdatedResult = _store.GetById(product.Id);
        Assert.True(getUpdatedResult.IsSuccess);
        Assert.Equal("Producto Actualizado", getUpdatedResult.Value!.Name);

        // Act & Assert - DELETE
        var deleteResult = _store.Delete(product.Id);
        Assert.True(deleteResult.IsSuccess);

        // Act & Assert - READ DELETED
        var getDeletedResult = _store.GetById(product.Id);
        Assert.False(getDeletedResult.IsSuccess);
    }

    #endregion
}
