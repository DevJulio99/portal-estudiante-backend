using APIPostulaEnrolamiento.Funciones;
using JwtLoginService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
    var redisConfiguration = builder.Configuration.GetValue<string>("Redis:ConnectionString")!;
    return ConnectionMultiplexer.Connect(redisConfiguration);
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
        // var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    }
);

builder.Services.AddScoped<IFuncionesApi, FuncionesCursos>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompetenciasGeneralesService, CompetenciasGeneralesService>();


var app = builder.Build();

app.UseCors("validarConsumo");

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Solo en producción
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
