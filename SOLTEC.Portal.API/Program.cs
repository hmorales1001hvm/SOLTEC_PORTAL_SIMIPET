using Microsoft.Extensions.Options;
using SOLTEC.Portal.API;
using SOLTEC.Portal.Business.Administracion;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
ConfigHelper.Configuration = builder.Configuration;

var app = builder.Build();

// 2️⃣ Usar CORS antes de MapControllers
app.UseCors("PermitirTodo");

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();

