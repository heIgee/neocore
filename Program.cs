using Neocore.Components;
using Neo4j.Driver;
using Neocore.Repositories;
using Blazored.LocalStorage;
using Neocore.Auth;
using Neocore.Repositories.Abstract;

namespace Neocore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSingleton(
            GraphDatabase.Driver("neo4j://localhost:7687", AuthTokens.Basic("neo4j", "neo4j"))
        );

        builder.Services
            .AddScoped<IContractRepository, ContractRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IEmployeeRepository, EmployeeRepository>()
            .AddScoped<IItemRepository, ItemRepository>()
            .AddScoped<IRepairRepository, RepairRepository>()
            .AddScoped<ISaleRepository, SaleRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IVendorRepository, VendorRepository>()
            .AddScoped<IAuthService, AuthService>()
            .AddBlazoredLocalStorage()
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
