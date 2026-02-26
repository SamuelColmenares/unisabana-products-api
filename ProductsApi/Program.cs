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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
