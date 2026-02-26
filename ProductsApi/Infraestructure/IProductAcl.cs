using ProductsApi.Models;

namespace ProductsApi.Infraestructure;

/// <summary>
/// Define el contrato para la capa de acceso a la API externa (ACL).
/// Especifica las operaciones de sincronización de datos con servicios externos.
/// </summary>
public interface IProductAcl
{
    /// <summary>
    /// Obtiene de forma asincrónica la lista inicial de productos desde una API externa.
    /// </summary>
    /// <param name="ct">Token de cancelación para la operación asincrónica.</param>
    /// <returns>Una colección de productos obtenidos de la API externa.</returns>
    Task<IEnumerable<Product>> FetchInitialProductsAsync(CancellationToken ct);

    /// <summary>
    /// Envía de forma asincrónica una actualización de producto a la API externa.
    /// </summary>
    /// <param name="product">El producto a sincronizar con la API externa.</param>
    /// <param name="ct">Token de cancelación para la operación asincrónica.</param>
    /// <returns>true si la operación fue exitosa; false en caso contrario.</returns>
    Task<bool> PushUpdateAsync(Product product, CancellationToken ct);
}
