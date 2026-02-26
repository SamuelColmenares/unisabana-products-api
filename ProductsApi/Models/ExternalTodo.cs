namespace ProductsApi.Models;

/// <summary>
/// Representa una tarea externa devuelta por la API JSONPlaceholder.
/// Este modelo mapea la estructura de datos de la API externa a nuestra aplicación.
/// </summary>
/// <param name="Id">Identificador único de la tarea en JSONPlaceholder.</param>
/// <param name="Title">Título o descripción de la tarea.</param>
/// <param name="Completed">Indica si la tarea ha sido completada.</param>
public record ExternalTodo(int Id, string Title, bool Completed);
