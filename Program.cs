using System.Threading.RateLimiting;
using MyPortalStudent.Funciones;
using MyPortalStudent.Controllers;
using Microsoft.AspNetCore.Builder;
using Dapper;
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

// Configura Dapper para mapear automáticamente columnas con snake_case a propiedades en PascalCase
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.WebHost.ConfigureKestrel(serverOptions => {
    serverOptions.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "8080"));
});

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("validarConsumo", configuracion =>
    {        
        // Permitir cualquier origen en desarrollo
        // En producción, especificar los orígenes permitidos
        configuracion
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        
        // Nota: AllowAnyOrigin() no permite credenciales (cookies, headers de auth)
        // Si necesitas enviar credenciales, usa WithOrigins() en lugar de AllowAnyOrigin()
    });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConfiguration = builder.Configuration.GetValue<string>("Redis:ConnectionString")!;
    return ConnectionMultiplexer.Connect(redisConfiguration);
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

// Registrar HttpContextAccessor para acceder al HttpContext en servicios
builder.Services.AddHttpContextAccessor();

// Registrar servicios de tenant
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "API-Portal-Estudiante",
            Description = "Servicio con soporte Multitenant. El tenant se establece automáticamente desde el claim 'Codigo_Sede' del token JWT."
        }
        );
        
        // Configurar seguridad JWT Bearer para Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header usando el esquema Bearer. Ingresa 'Bearer' [espacio] y luego tu token JWT. Ejemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    }
);

builder.Services.AddScoped<IFuncionesApi, FuncionesCursos>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompetenciasGeneralesService, CompetenciasGeneralesService>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<INotasService, NotasService>();


var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// CORS debe ejecutarse ANTES del middleware de tenant
// para que los preflight requests (OPTIONS) puedan pasar
app.UseCors("validarConsumo");

// Middleware de tenant - debe ejecutarse después de CORS pero antes de los controladores
app.UseMiddleware<MyPortalStudent.Middleware.TenantMiddleware>();

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
