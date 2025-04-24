using System.Threading.RateLimiting;
using APIPostulaEnrolamiento.Funciones;
using JwtLoginService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using MyPortalStudent.Domain.Ifunciones;
using MyPortalStudent.Domain.IServices;
using MyPortalStudent.Services;
using MyPortalStudent.Utils;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddHostedService<Worker>();
// Configurar el puerto para Railway
builder.WebHost.ConfigureKestrel(serverOptions => {
    serverOptions.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "8080"));
});

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("validarConsumo", configuracion =>
    {        
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var host = Environment.GetEnvironmentVariable("REDISHOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("REDISPORT") ?? "6379";
    var password = Environment.GetEnvironmentVariable("REDISPASSWORD");

    var configurationOptions = new ConfigurationOptions
    {
        EndPoints = { $"{host}:{port}" },
        Password = password,
        Ssl = true,
        AbortOnConnectFail = false
    };

    return ConnectionMultiplexer.Connect(configurationOptions);
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var path = context.Request.Path.ToString().ToLower();

        var key = $"{ip}:{path}";

        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 3,
            Window = TimeSpan.FromSeconds(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    options.RejectionStatusCode = 429;
});

builder.Services.AddSingleton<IRedisDB, RedisDB>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "API-Portal-Estudiante",
            Description = "Servicio."
        }
        );
    }
);

builder.Services.AddScoped<IFuncionesApi, FuncionesCursos>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompetenciasGeneralesService, CompetenciasGeneralesService>();


var app = builder.Build();

app.UseCors("validarConsumo");
app.UseRateLimiter();

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Solo en producción
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
