using ProductsApi.Models;
using Xunit;

namespace ProductsApi.Tests.Models;

/// <summary>
/// Suite de pruebas unitarias para Result{T}.
/// Valida el patrón de resultado genérico para manejo de errores.
/// </summary>
public class ResultTests
{
    /// <summary>
    /// Verifica que Success crea un resultado exitoso.
    /// </summary>
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        // Arrange & Act
        var value = "Test Value";
        var result = Result<string>.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
        Assert.Empty(result.Error);
        Assert.Equal(200, result.StatusCode);
    }

    /// <summary>
    /// Verifica que Failure crea un resultado de error con código 400 por defecto.
    /// </summary>
    [Fact]
    public void Failure_ShouldCreateFailureResultWithDefaultStatusCode()
    {
        // Arrange
        var errorMessage = "Test Error";

        // Act
        var result = Result<string>.Failure(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal(errorMessage, result.Error);
        Assert.Equal(400, result.StatusCode);
    }

    /// <summary>
    /// Verifica que Failure crea un resultado de error con código personalizado.
    /// </summary>
    [Fact]
    public void Failure_ShouldCreateFailureResultWithCustomStatusCode()
    {
        // Arrange
        var errorMessage = "Not Found";
        var statusCode = 404;

        // Act
        var result = Result<string>.Failure(errorMessage, statusCode);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal(errorMessage, result.Error);
        Assert.Equal(statusCode, result.StatusCode);
    }

    /// <summary>
    /// Verifica que Success funciona con diferentes tipos genéricos.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999)]
    public void Success_ShouldWorkWithIntType(int value)
    {
        // Act
        var result = Result<int>.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    /// <summary>
    /// Verifica que Success funciona con objetos complejos.
    /// </summary>
    [Fact]
    public void Success_ShouldWorkWithComplexTypes()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            IsActive = true,
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var result = Result<Product>.Success(product);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(product.Id, result.Value.Id);
        Assert.Equal(product.Name, result.Value.Name);
    }

    /// <summary>
    /// Verifica que los valores por defecto del resultado son correctos.
    /// </summary>
    [Fact]
    public void Result_ShouldHaveCorrectDefaults()
    {
        // Act
        var result = new Result<string>(null, false);

        // Assert
        Assert.Null(result.Value);
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Error);
        Assert.Equal(200, result.StatusCode);
    }
}
