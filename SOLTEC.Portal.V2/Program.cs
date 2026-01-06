using Microsoft.AspNetCore.Authentication.Cookies;
using SOLTEC.Portal.V2;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------
// 🔐 Configurar autenticación por cookies
// -------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";          // Redirige si no ha iniciado sesión
        options.LogoutPath = "/Account/Logout";     // Ruta para cerrar sesión
        options.AccessDeniedPath = "/Account/Denied"; // Opcional
        options.ExpireTimeSpan = TimeSpan.FromMinutes(3000); // Duración del ticket de autenticación
        options.SlidingExpiration = true;            // Renueva cookie si hay actividad
    });

// -------------------------------------------
// 🧠 Agregar soporte de Session
// -------------------------------------------
builder.Services.AddDistributedMemoryCache(); // Requerido para usar Session en memoria

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(3000); // Expira igual que la cookie (puedes ajustar)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// -------------------------------------------
// MVC Controllers + Views
// -------------------------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddSingleton<ApiUrlResolver>();

var app = builder.Build();

// -------------------------------------------
// Configuración del pipeline de la app
// -------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🧩 Orden correcto de middlewares
app.UseSession();          
app.UseAuthentication();
app.UseAuthorization();

// 🔒 Middleware para verificar autenticación
app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    if (endpoint != null)
    {
        var allowAnonymous = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>();
        if (allowAnonymous != null)
        {
            await next();
            return;
        }
    }

    if (!context.User.Identity.IsAuthenticated)
    {
        context.Response.Redirect("/Home/Login");
        return;
    }

    await next();
});

// -------------------------------------------
// Rutas (por defecto, va al login)
// -------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}"
);

app.Run();
