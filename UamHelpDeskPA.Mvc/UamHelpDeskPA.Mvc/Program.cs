// Builder principal de la aplicación MVC.
using UamHelpDeskPA.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Registramos soporte MVC con controladores y vistas.
builder.Services.AddControllersWithViews();

// HttpClient
builder.Services.AddHttpClient();

//  NECESARIO PARA LEER COOKIES
builder.Services.AddHttpContextAccessor();

//  CLIENTE CENTRALIZADO
builder.Services.AddScoped<ApiClientService>();

// Registramos HttpClient para consumir el API desde el servidor MVC.
builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Construimos la aplicación con todos los servicios ya registrados.
var app = builder.Build();

// Si NO estamos en desarrollo, activamos manejo de errores y HSTS.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Fuerza redirección a HTTPS.
app.UseHttpsRedirection();

// Activa enrutamiento.
app.UseRouting();
app.UseSession();

app.UseAuthentication();
// Activa middleware de autorización (queda listo por si se agrega seguridad del lado MVC).
app.UseAuthorization();

// Mapea archivos estáticos (css/js/img) para .NET 10.
app.MapStaticAssets();

// Ruta por defecto de la app MVC: Maintenance/Index.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Inicia la aplicación y la deja escuchando peticiones HTTP.
app.Run();