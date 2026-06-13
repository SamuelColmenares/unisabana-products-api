using ProductsApi.Models;
using Xunit;

namespace ProductsApi.Tests.Models;

/// <summary>
/// Suite de pruebas unitarias para Product.
/// Valida la clase modelo de productos.
/// </summary>
public class ProductTests
{
    /// <summary>
    /// Verifica que Product se puede crear con los parámetros especificados.
    /// </summary>
    [Fact]
    public void Product_ShouldCreateWithParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test Product";
        var isActive = true;
        var lastUpdated = DateTime.UtcNow;

        // Act
        var product = new Product(id, name, isActive, lastUpdated);

        // Assert
        Assert.Equal(id, product.Id);
        Assert.Equal(name, product.Name);
        Assert.True(product.IsActive);
        Assert.Equal(lastUpdated, product.LastUpdated);
    }

    /// <summary>
    /// Verifica que el constructor sin parámetros genera un producto con valores por defecto.
    /// </summary>
    [Fact]
    public void Product_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var product = new Product();

        // Assert
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Empty(product.Name);
        Assert.False(product.IsActive);
        Assert.True(product.LastUpdated > DateTime.MinValue);
    }

    /// <summary>
    /// Verifica que se pueden actualizar las propiedades del producto.
    /// </summary>
    [Fact]
    public void Product_PropertiesShouldBeUpdatable()
    {
        // Arrange
        var product = new Product();
        var newName = "Updated Product";
        var newIsActive = true;

        // Act
        product.Name = newName;
        product.IsActive = newIsActive;

        // Assert
        Assert.Equal(newName, product.Name);
        Assert.True(product.IsActive);
    }

    /// <summary>
    /// Verifica que dos productos con el mismo ID se pueden diferenciar.
    /// </summary>
    [Fact]
    public void Product_ShouldMaintainUniqueIds()
    {
        // Arrange & Act
        var product1 = new Product();
        var product2 = new Product();

        // Assert
        Assert.NotEqual(product1.Id, product2.Id);
    }

    /// <summary>
    /// Verifica que Product puede inicializarse con valores específicos.
    /// </summary>
    [Theory]
    [InlineData("Active Product", true)]
    [InlineData("Inactive Product", false)]
    [InlineData("", false)]
    public void Product_ShouldInitializeWithVariousStates(string name, bool isActive)
    {
        // Arrange
        var id = Guid.NewGuid();
        var lastUpdated = DateTime.UtcNow;

        // Act
        var product = new Product(id, name, isActive, lastUpdated);

        // Assert
        Assert.Equal(name, product.Name);
        Assert.Equal(isActive, product.IsActive);
    }

    /// <summary>
    /// Verifica que LastUpdated se actualiza correctamente.
    /// </summary>
    [Fact]
    public void Product_LastUpdated_ShouldBeUpdatable()
    {
        // Arrange
        var product = new Product();
        var originalDate = product.LastUpdated;

        // Act
        var newDate = DateTime.UtcNow.AddDays(1);
        product.LastUpdated = newDate;

        // Assert
        Assert.NotEqual(originalDate, product.LastUpdated);
        Assert.Equal(newDate, product.LastUpdated);
    }
}
