using ProductsApi.Models;

namespace ProductsApi.Infraestructure.Persistence;

/// <summary>
/// Implementación en memoria del repositorio de productos.
/// Almacena productos en una colección en memoria y se sincroniza con una API externa mediante ACL.
/// </summary>
public class ProductMemoryStore : IProductRepository
{
    /// <summary>
    /// Colección en memoria que almacena los productos.
    /// </summary>
    private readonly List<Product> _products = new();

    /// <summary>
    /// Referencia a la capa de acceso a API externa para sincronización.
    /// </summary>
    private readonly IProductAcl _acl;

    /// <summary>
    /// Inicializa una nueva instancia de ProductMemoryStore.
    /// Carga los productos iniciales desde la API externa mediante ACL.
    /// </summary>
    /// <param name="acl">Implementación de IProductAcl para sincronización externa.</param>
    public ProductMemoryStore(IProductAcl acl)
    {
        _acl = acl;
        // Carga inicial síncrona para simplificar la demo
        var initial = _acl.FetchInitialProductsAsync(default).GetAwaiter().GetResult();
        _products.AddRange(initial);
    }

    /// <summary>
    /// Obtiene todos los productos del almacén en memoria.
    /// </summary>
    /// <returns>Colección de todos los productos disponibles.</returns>
    public IEnumerable<Product> GetAll() => _products;

    /// <summary>
    /// Obtiene un producto específico por su identificador.
    /// </summary>
    /// <param name="id">Identificador único del producto.</param>
    /// <returns>Resultado con el producto si existe; error 404 si no se encuentra.</returns>
    public Result<Product> GetById(Guid id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return product != null
            ? Result<Product>.Success(product)
            : Result<Product>.Failure("Producto no encontrado", 404);
    }

    /// <summary>
    /// Añade un nuevo producto al almacén en memoria y lo sincroniza con la API externa.
    /// </summary>
    /// <param name="product">El producto a añadir.</param>
    /// <param name="cancellationToken">Token de cancelación para la operación asincrónica.</param>
    /// <returns>Resultado indicando el éxito de la operación o error si el producto ya existe.</returns>
    public async Task<Result<bool>> Add(Product product, CancellationToken cancellationToken = default)
    {
        var exists = _products.Any(p => p.Id == product.Id);
        if (exists) {
            return Result<bool>.Failure("Producto con el mismo ID ya existe", 400);
        }

        var result = await _acl.PushUpdateAsync(product, cancellationToken); // Notifica a la "API Externa"
        if (!result)
        {
            return Result<bool>.Failure("Error al agregar en ACL", 500);
        }

        _products.Add(product);
        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Actualiza un producto existente en el almacén en memoria y sincroniza con la API externa.
    /// </summary>
    /// <param name="product">El producto con los datos actualizados.</param>
    /// <param name="ct">Token de cancelación para la operación asincrónica.</param>
    /// <returns>Resultado con el producto actualizado o error si no se encuentra.</returns>
    public async Task<Result<Product>> Update(Product product, CancellationToken ct = default)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index == -1) return Result<Product>.Failure("No se puede actualizar: Producto inexistente", 404);

        product.LastUpdated = DateTime.UtcNow;
        
        // Sincronización con ACL 
        var res = await _acl.PushUpdateAsync(product, ct);
        if (!res)
        {
            return Result<Product>.Failure($"Error al actualizar en ACL", 500);
        }

        _products[index] = product;
        
        return Result<Product>.Success(product);
    }

    /// <summary>
    /// Elimina un producto del almacén en memoria.
    /// </summary>
    /// <param name="id">Identificador único del producto a eliminar.</param>
    /// <returns>Resultado indicando el éxito de la operación o error si el producto no existe.</returns>
    public Result<bool> Delete(Guid id)
    {
        var removed = _products.RemoveAll(p => p.Id == id);
        return removed > 0
            ? Result<bool>.Success(true)
            : Result<bool>.Failure("No se encontró el producto para eliminar", 404);
    }
}
