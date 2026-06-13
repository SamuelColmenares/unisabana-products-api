using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductsApi.Controllers;
using ProductsApi.Infraestructure.Persistence;
using ProductsApi.Models;
using Xunit;

namespace ProductsApi.Tests.Controllers;

/// <summary>
/// Suite de pruebas unitarias para ProductsController.
/// Valida los endpoints REST de la API de productos.
/// </summary>
public class ProductsControllerTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _controller = new ProductsController(_mockRepository.Object);
    }

    #region GetAll Tests

    /// <summary>
    /// Verifica que GetAll devuelve OK con todos los productos.
    /// </summary>
    [Fact]
    public void GetAll_ShouldReturnOk_WithAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = Guid.NewGuid(), Name = "Product 1", IsActive = true, LastUpdated = DateTime.UtcNow },
            new Product { Id = Guid.NewGuid(), Name = "Product 2", IsActive = false, LastUpdated = DateTime.UtcNow }
        };
        _mockRepository.Setup(x => x.GetAll()).Returns(products);

        // Act
        var result = _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Equal(2, returnedProducts.Count);
    }

    /// <summary>
    /// Verifica que GetAll devuelve OK incluso con una lista vacía.
    /// </summary>
    [Fact]
    public void GetAll_ShouldReturnOk_WithEmptyList()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAll()).Returns(new List<Product>());

        // Act
        var result = _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Empty(returnedProducts);
    }

    #endregion

    #region Get Tests

    /// <summary>
    /// Verifica que Get devuelve un producto cuando existe.
    /// </summary>
    [Fact]
    public void Get_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Test Product", IsActive = true, LastUpdated = DateTime.UtcNow };
        _mockRepository.Setup(x => x.GetById(productId)).Returns(Result<Product>.Success(product));

        // Mock the Request and Response headers
        var mockHttpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
        var mockRequest = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
        var mockResponse = new Mock<Microsoft.AspNetCore.Http.HttpResponse>();
        var requestHeaders = new Microsoft.AspNetCore.Http.HeaderDictionary();
        var responseHeaders = new Microsoft.AspNetCore.Http.HeaderDictionary();

        mockRequest.Setup(r => r.Headers).Returns(requestHeaders);
        mockResponse.Setup(r => r.Headers).Returns(responseHeaders);
        mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
        mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);
        _controller.ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object };

        // Act
        var result = _controller.Get(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(productId, returnedProduct.Id);
    }

    /// <summary>
    /// Verifica que Get devuelve NotFound cuando el producto no existe.
    /// </summary>
    [Fact]
    public void Get_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockRepository.Setup(x => x.GetById(productId))
            .Returns(Result<Product>.Failure("Producto no encontrado", 404));

        // Act
        var result = _controller.Get(productId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    #endregion

    #region Post Tests

    /// <summary>
    /// Verifica que Post crea un producto exitosamente.
    /// </summary>
    [Fact]
    public async Task Post_ShouldReturnCreatedAtAction_WhenSuccess()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "New Product", IsActive = true, LastUpdated = DateTime.UtcNow };
        _mockRepository.Setup(x => x.Add(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.Post(product, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProductsController.Get), createdResult.ActionName);
        var routeId = createdResult.RouteValues?["id"];
        Assert.Equal(productId, routeId);
    }

    /// <summary>
    /// Verifica que Post devuelve BadRequest cuando falla la creación.
    /// </summary>
    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenFails()
    {
        // Arrange
        var product = new Product { Id = Guid.NewGuid(), Name = "Product", IsActive = true, LastUpdated = DateTime.UtcNow };
        _mockRepository.Setup(x => x.Add(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Product already exists", 400));

        // Act
        var result = await _controller.Post(product, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    #endregion

    #region Put Tests

    /// <summary>
    /// Verifica que Put actualiza un producto existente.
    /// </summary>
    [Fact]
    public async Task Put_ShouldReturnNoContent_WhenSuccess()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Updated Product", IsActive = false, LastUpdated = DateTime.UtcNow };
        _mockRepository.Setup(x => x.Update(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Product>.Success(product));

        // Act
        var result = await _controller.Put(productId, product, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Verifica que Put devuelve BadRequest cuando los IDs no coinciden.
    /// </summary>
    [Fact]
    public async Task Put_ShouldReturnBadRequest_WhenIdsDoNotMatch()
    {
        // Arrange
        var urlId = Guid.NewGuid();
        var bodyId = Guid.NewGuid();
        var product = new Product { Id = bodyId, Name = "Product", IsActive = true, LastUpdated = DateTime.UtcNow };

        // Act
        var result = await _controller.Put(urlId, product, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    /// <summary>
    /// Verifica que Put devuelve NotFound cuando el producto no existe.
    /// </summary>
    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Product", IsActive = true, LastUpdated = DateTime.UtcNow };
        _mockRepository.Setup(x => x.Update(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Product>.Failure("Product not found", 404));

        // Act
        var result = await _controller.Put(productId, product, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    #endregion

    #region Delete Tests

    /// <summary>
    /// Verifica que Delete elimina un producto exitosamente.
    /// </summary>
    [Fact]
    public void Delete_ShouldReturnNoContent_WhenSuccess()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockRepository.Setup(x => x.Delete(productId))
            .Returns(Result<bool>.Success(true));

        // Act
        var result = _controller.Delete(productId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Verifica que Delete devuelve NotFound cuando el producto no existe.
    /// </summary>
    [Fact]
    public void Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockRepository.Setup(x => x.Delete(productId))
            .Returns(Result<bool>.Failure("Product not found", 404));

        // Act
        var result = _controller.Delete(productId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    #endregion

    #region Options Tests

    /// <summary>
    /// Verifica que Options devuelve los métodos HTTP permitidos.
    /// </summary>
    [Fact]
    public void Options_ShouldReturnAllowedMethods()
    {
        // Arrange
        var mockHttpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
        var mockResponse = new Mock<Microsoft.AspNetCore.Http.HttpResponse>();
        var mockHeaders = new Microsoft.AspNetCore.Http.HeaderDictionary();
        mockResponse.Setup(r => r.Headers).Returns(mockHeaders);
        mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);
        _controller.ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object };

        // Act
        var result = _controller.Options();

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    #endregion
}
