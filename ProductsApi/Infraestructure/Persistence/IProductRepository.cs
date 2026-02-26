using ProductsApi.Models;

namespace ProductsApi.Infraestructure.Persistence;

/// <summary>
/// Define el contrato para el repositorio de productos.
/// Especifica las operaciones de persistencia (CRUD) para productos.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Obtiene todos los productos disponibles en el repositorio.
    /// </summary>
    /// <returns>Colección de todos los productos.</returns>
    IEnumerable<Product> GetAll();

    /// <summary>
    /// Obtiene un producto específico por su identificador.
    /// </summary>
    /// <param name="id">Identificador único del producto a recuperar.</param>
    /// <returns>Resultado que contiene el producto si existe; error 404 si no se encuentra.</returns>
    Result<Product> GetById(Guid id);

    /// <summary>
    /// Añade de forma asincrónica un nuevo producto al repositorio.
    /// </summary>
    /// <param name="product">El producto a añadir.</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
    /// <returns>Resultado indicando el éxito o error de la operación.</returns>
    Task<Result<bool>> Add(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza de forma asincrónica un producto existente en el repositorio.
    /// </summary>
    /// <param name="product">El producto con los datos actualizados.</param>
    /// <param name="ct">Token de cancelación para la operación asincrónica.</param>
    /// <returns>Resultado que contiene el producto actualizado o un error.</returns>
    Task<Result<Product>> Update(Product product, CancellationToken ct = default);

    /// <summary>
    /// Elimina un producto existente del repositorio.
    /// </summary>
    /// <param name="id">Identificador único del producto a eliminar.</param>
    /// <returns>Resultado indicando el éxito o error de la operación de eliminación.</returns>
    Result<bool> Delete(Guid id);
}
