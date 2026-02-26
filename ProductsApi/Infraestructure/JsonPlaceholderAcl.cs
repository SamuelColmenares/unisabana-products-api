using ProductsApi.Models;

namespace ProductsApi.Infraestructure;

/// <summary>
/// Implementación de la capa de acceso a API externa (ACL) para JSONPlaceholder.
/// Sincroniza productos con la API JSONPlaceholder, convirtiéndolos desde el formato externo.
/// </summary>
public class JsonPlaceholderAcl(HttpClient httpClient) : IProductAcl
{
    /// <summary>
    /// Obtiene tareas iniciales desde JSONPlaceholder y las convierte al modelo de producto.
    /// </summary>
    /// <param name="ct">Token de cancelación para la operación asincrónica.</param>
    /// <returns>Colección de productos convertidos desde tareas de JSONPlaceholder.</returns>
    public async Task<IEnumerable<Product>> FetchInitialProductsAsync(CancellationToken ct)
    {
        var external = await httpClient.GetFromJsonAsync<List<ExternalTodo>>("todos?_limit=10", ct);
        return external?.Select(e => new Product
        {
            Id = Guid.NewGuid(),
            Name = e.Title,
            IsActive = e.Completed,
            LastUpdated = DateTime.UtcNow
        }) ?? Enumerable.Empty<Product>();
    }

    /// <summary>
    /// Envía una actualización de producto a JSONPlaceholder mediante POST.
    /// </summary>
    /// <param name="product">El producto a enviar a la API externa.</param>
    /// <param name="ct">Token de cancelación para la operación asincrónica.</param>
    /// <returns>true si la solicitud fue exitosa; false en caso contrario.</returns>
    public async Task<bool> PushUpdateAsync(Product product, CancellationToken ct)
    {
        // Simulamos el envío a JSONPlaceholder
        var response = await httpClient.PostAsJsonAsync("todos", product, ct);
        return response.IsSuccessStatusCode;
    }
}
