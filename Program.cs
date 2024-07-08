using Neocore.Components;
using Neo4j.Driver;
using Neocore.Repositories;

namespace Neocore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddSingleton(
            GraphDatabase.Driver("neo4j://localhost:7687", AuthTokens.Basic("neo4j", "neo4j"))
        );

        //builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services
            .AddScoped<ItemRepository>()
            .AddScoped<VendorRepository>()
        ;

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
