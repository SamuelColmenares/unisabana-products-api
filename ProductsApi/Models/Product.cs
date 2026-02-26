namespace ProductsApi.Models;

/// <summary>
/// Representa un producto en el dominio de la aplicación.
/// Este modelo encapsula la información de un producto con su identificador único,
/// nombre, estado de actividad y fecha de última actualización.
/// </summary>
public class Product(Guid id, string name, bool isActive, DateTime lastUpdated)
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    public Guid Id { get; set; } = id;

    /// <summary>
    /// Nombre o descripción del producto.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Indica si el producto está activo en el sistema.
    /// </summary>
    public bool IsActive { get; set; } = isActive;

    /// <summary>
    /// Fecha y hora de la última actualización del producto.
    /// </summary>
    public DateTime LastUpdated { get; set; } = lastUpdated;

    /// <summary>
    /// Inicializa una nueva instancia de la clase Product con valores por defecto.
    /// </summary>
    public Product() : this(Guid.NewGuid(), "", false, DateTime.UtcNow) { }
}
