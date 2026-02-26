namespace ProductsApi.Models;

/// <summary>
/// Representa un resultado genérico de una operación que puede ser exitosa o fallida.
/// Este tipo encapsula tanto los datos de éxito como los detalles del error.
/// </summary>
/// <typeparam name="T">Tipo de datos que contiene el resultado exitoso.</typeparam>
/// <param name="Value">El valor devuelto en caso de éxito; null si la operación falló.</param>
/// <param name="IsSuccess">Indica si la operación fue exitosa.</param>
/// <param name="Error">Mensaje de error si la operación falló; cadena vacía si fue exitosa.</param>
/// <param name="StatusCode">Código de estado HTTP asociado al resultado (200 por defecto).</param>
public record Result<T>(T? Value, bool IsSuccess, string Error = "", int StatusCode = 200)
{
    /// <summary>
    /// Crea un resultado exitoso con el valor especificado.
    /// </summary>
    /// <param name="value">El valor de éxito a encapsular.</param>
    /// <returns>Un resultado exitoso con el valor proporcionado.</returns>
    public static Result<T> Success(T value) => new(value, true);

    /// <summary>
    /// Crea un resultado de error con el mensaje y código de estado especificados.
    /// </summary>
    /// <param name="error">Mensaje de error descriptivo.</param>
    /// <param name="statusCode">Código de estado HTTP del error (400 por defecto).</param>
    /// <returns>Un resultado fallido con el error y código de estado proporcionados.</returns>
    public static Result<T> Failure(string error, int statusCode = 400) => new(default, false, error, statusCode);
}