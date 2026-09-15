using KrisanthiumCurrency.Data;
using KrisanthiumCurrency.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC (Razor Views) + API Controllers
builder.Services.AddControllersWithViews();

// ----- Swagger / OpenAPI -----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----- Database (MySQL via Pomelo) -----
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ----- External exchange rate API client -----
builder.Services.AddHttpClient<IExchangeRateProvider, ExchangeRateApiProvider>();

// ----- Mock ERP client (calls our own /api/mock-erp/exchange-rate endpoint) -----
builder.Services.AddHttpClient<IMockErpClient, MockErpClient>((sp, client) => {
    var baseUrl = builder.Configuration["AppBaseUrl"] ?? "http://localhost:61095";
    client.BaseAddress = new Uri(baseUrl);
});

// ----- Application service -----
builder.Services.AddScoped<ExchangeRateAppService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Swagger UI diaktifkan agar dokumentasi API mudah diakses saat demo/testing.
app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency ERP Sync API v1");
    options.RoutePrefix = "swagger"; // akses via /swagger
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// MVC views (Exchange Rate page, History page)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// API controllers (attribute routed: /api/...)
app.MapControllers();

app.Run();
