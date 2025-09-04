using Microsoft.EntityFrameworkCore;
using ReservasSalas.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
// 🔹 Agregamos servicios MVC (controladores y vistas Razor)
builder.Services.AddControllersWithViews();

builder.Services.AddSession();


// 🔹 Configuración de Entity Framework con Postgres
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// 🔹 Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

// 🔹 Rutas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 🔹 Rutas API (ej: /reservas)
app.MapControllers();

app.Run();
