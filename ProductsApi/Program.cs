using ProductsApi.Infraestructure;
using ProductsApi.Infraestructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IProductAcl, JsonPlaceholderAcl>(c =>
{
    c.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

// El Store debe ser Singleton para mantener los datos en memoria entre peticiones
builder.Services.AddSingleton<IProductRepository, ProductMemoryStore>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapear endpoint raíz con HTML sencillo
app.MapGet("/", () => Results.Content(
    """
    <!DOCTYPE html>
    <html lang="es">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Products API</title>
        <style>
            * {
                margin: 0;
                padding: 0;
                box-sizing: border-box;
            }

            body {
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                min-height: 100vh;
                display: flex;
                justify-content: center;
                align-items: center;
            }

            .container {
                background: white;
                border-radius: 10px;
                box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
                padding: 40px;
                max-width: 600px;
                text-align: center;
            }

            h1 {
                color: #333;
                margin-bottom: 10px;
                font-size: 2.5em;
            }

            .subtitle {
                color: #666;
                margin-bottom: 30px;
                font-size: 1.1em;
                line-height: 1.5;
            }

            /* Estilo para hacer "Fundamentos DevOps" más pequeño */
            .devops-subtitle {
                font-size: 0.85em;
                color: #888;
                display: block;
                margin-top: 4px;
            }

            .links {
                display: flex;
                gap: 15px;
                justify-content: center;
                flex-wrap: wrap;
                margin-bottom: 30px;
            }

            .btn {
                display: inline-block;
                padding: 12px 24px;
                background-color: #667eea;
                color: white;
                text-decoration: none;
                border-radius: 5px;
                transition: background-color 0.3s ease;
                font-weight: 500;
            }

            .btn:hover {
                background-color: #764ba2;
            }

            .btn-secondary {
                background-color: #48bb78;
            }

            .btn-secondary:hover {
                background-color: #38a169;
            }

            .info {
                text-align: left;
                background-color: #f7fafc;
                padding: 20px;
                border-radius: 5px;
                border-left: 4px solid #667eea;
                margin-bottom: 25px;
            }

            .info h2 {
                color: #333;
                margin-bottom: 10px;
                font-size: 1.2em;
            }

            .info ul {
                list-style: none;
                color: #666;
            }

            .info li {
                padding: 5px 0;
            }

            .info li:before {
                content: "→ ";
                color: #667eea;
                font-weight: bold;
                margin-right: 8px;
            }

            /* Estilos para los créditos finales */
            .footer-credits {
                border-top: 1px solid #e2e8f0;
                padding-top: 15px;
                font-size: 0.85em;
                color: #718096;
                text-align: right;
            }
        </style>
    </head>
    <body>
        <div class="container">
            <h1>🚀 Products API</h1>
            <p class="subtitle">
                Bienvenido a la API de Productos
                <span class="devops-subtitle">Fundamentos DevOps</span>
            </p>

            <div class="links">
                <a href="/scalar" class="btn btn-secondary">📖 Documentación (Scalar)</a>
                <a href="/openapi/v1.json" class="btn">🔗 OpenAPI Schema</a>
            </div>

            <div class="info">
                <h2>Endpoints disponibles:</h2>
                <ul>
                    <li><strong>GET</strong> /api/products - Obtener todos los productos</li>
                    <li><strong>GET</strong> /api/products/{id} - Obtener producto por ID</li>
                    <li><strong>POST</strong> /api/products - Crear nuevo producto</li>
                    <li><strong>PUT</strong> /api/products/{id} - Actualizar producto</li>
                    <li><strong>DELETE</strong> /api/products/{id} - Eliminar producto</li>
                </ul>
            </div>

            <div class="footer-credits">
                <p><strong>Creado por:</strong> Samuel Colmenares</p>
                <p><strong>Fecha:</strong> 12 de junio de 2026</p>
            </div>
        </div>
    </body>
    </html>
    """,
    "text/html"));

app.MapControllers();

app.Run();
