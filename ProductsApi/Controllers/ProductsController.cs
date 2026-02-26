using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ProductsApi.Infraestructure.Persistence;
using ProductsApi.Models;

namespace ProductsApi.Controllers
{
    /// <summary>
    /// Controlador REST para gestionar operaciones CRUD de productos.
    /// Proporciona endpoints para obtener, crear, actualizar y eliminar productos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository store) : ControllerBase
    {
        /// <summary>
        /// Obtiene todos los productos disponibles.
        /// </summary>
        /// <returns>Una colección de todos los productos.</returns>
        [HttpGet]
        public IActionResult GetAll() => Ok(store.GetAll());

        /// <summary>
        /// Obtiene un producto específico por su identificador.
        /// Implementa caching mediante el encabezado 'If-Modified-Since'.
        /// </summary>
        /// <param name="id">Identificador único del producto.</param>
        /// <returns>El producto solicitado o 304 (No Modificado) si está en caché.</returns>
        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id)
        {
            var result = store.GetById(id);

            if (!result.IsSuccess) return MapError(result);

            // Lógica de Caché optimizada con Result
            var product = result.Value!;
            if (Request.Headers.TryGetValue(HeaderNames.IfModifiedSince, out var ifModified))
            {
                if (DateTime.TryParse(ifModified, out var date) && date >= product.LastUpdated.AddTicks(-(product.LastUpdated.Ticks % TimeSpan.TicksPerSecond)))
                    return StatusCode(304);
            }

            Response.Headers[HeaderNames.LastModified] = product.LastUpdated.ToString("R");
            return Ok(product);
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="product">Datos del producto a crear.</param>
        /// <param name="ct">Token de cancelación para la operación asincrónica.</param>
        /// <returns>El producto creado con su identificador, o un error si la operación falla.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product, CancellationToken ct)
        {
            var result = await store.Add(product, ct);
            return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = product.Id }, product)
                : MapError(result);
        }

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        /// <param name="id">Identificador único del producto a actualizar.</param>
        /// <param name="product">Datos actualizados del producto.</param>
        /// <param name="ct">Token de cancelación para la operación asincrónica.</param>
        /// <returns>No devuelve contenido (204) si la actualización es exitosa.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Product product, CancellationToken ct)
        {
            if (id != product.Id) return BadRequest("ID de URL no coincide con ID de cuerpo");

            var result = await store.Update(product, ct);
            if (!result.IsSuccess) return MapError(result);

            return NoContent();
        }

        /// <summary>
        /// Elimina un producto existente.
        /// </summary>
        /// <param name="id">Identificador único del producto a eliminar.</param>
        /// <returns>No devuelve contenido (204) si la eliminación es exitosa.</returns>
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var result = store.Delete(id);
            return result.IsSuccess ? NoContent() : MapError(result);
        }

        /// <summary>
        /// Devuelve los métodos HTTP permitidos para este recurso.
        /// </summary>
        /// <returns>Encabezado 'Allow' con los métodos soportados.</returns>
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Append("Allow", "GET, POST, PUT, DELETE, OPTIONS, HEAD");
            return Ok();
        }

        /// <summary>
        /// Realiza una solicitud HEAD para obtener metadatos de un producto sin descargar su contenido.
        /// </summary>
        /// <param name="id">Identificador único del producto.</param>
        /// <returns>Encabezados del producto sin cuerpo de respuesta.</returns>
        [HttpHead("{id:int}")]
        public async Task<IActionResult> Head(Guid id)
        {
            var result = store.GetById(id);
            if (result is null) return NotFound();

            Response.Headers[HeaderNames.LastModified] = result.Value!.LastUpdated.ToString("R");
            return Ok(); // HEAD no devuelve cuerpo, solo headers
        }

        /// <summary>
        /// Mapea un resultado de error a una respuesta HTTP apropiada.
        /// </summary>
        /// <typeparam name="T">Tipo genérico del resultado.</typeparam>
        /// <param name="result">Resultado que contiene el error a mapear.</param>
        /// <returns>Una respuesta HTTP con el código de estado y mensaje de error correspondiente.</returns>
        private IActionResult MapError<T>(Result<T> result) => result.StatusCode switch
        {
            404 => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
