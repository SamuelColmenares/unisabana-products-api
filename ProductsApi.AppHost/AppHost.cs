var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ProductsApi>("productsapi");

builder.Build().Run();
