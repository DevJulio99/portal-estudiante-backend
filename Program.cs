using APIPostulaEnrolamiento.Funciones;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using MyPortalStudent.Domain.Ifunciones;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddHostedService<Worker>();

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("validarConsumo", configuracion =>
    {        
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

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
var app = builder.Build();

app.UseCors("validarConsumo");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
