using Comitructor.Application.Extension;
using Comitructor.Infrastructure.Extension;
using Comitructor.WebApi.Filters;
using Comitructor.WebApi.Middlewares;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Iniciando el Gestor de Solicitudes Internas...");

    // Add services to the container.
    builder.Services.AddInjectionInfrastructure(configuration);
    builder.Services.AddInjectionIApplication(configuration);

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ApiResponseFilter>();
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Comitructor API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.UseSerilogRequestLogging();

    app.Run();

}
catch (HostAbortedException)
{
    Log.Information("Usando herramientas de EF tools: Add-Migration / Update-Database");
}
catch (Exception ex)
{
    Log.Fatal(ex, "El host termino inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}