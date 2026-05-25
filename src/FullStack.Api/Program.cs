using Asp.Versioning;
using FluentValidation;
using FullStack.Api.Behaviours;
using FullStack.Api.Endpoints;
using FullStack.Api.Infrastructure;
using FullStack.Api.Middleware;
using FullStack.Api.Serialization;
using FullStack.Api.Swagger;
using FullStack.Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog — skip UseSerilog in Testing environment to avoid "logger already frozen" in integration tests
    if (!builder.Environment.IsEnvironment("Testing"))
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console());
    }

    // System.Text.Json Source Generation
    builder.Services.ConfigureHttpJsonOptions(o =>
        o.SerializerOptions.TypeInfoResolverChain.Insert(0, ApiJsonContext.Default));

    builder.Services.AddControllers()
        .AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, ApiJsonContext.Default);
        });

    builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(err => new { Field = e.Key, Message = err.ErrorMessage }))
                .ToList();

            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "Validation Error",
                Detail = "One or more validation errors occurred.",
                Status = StatusCodes.Status422UnprocessableEntity,
                Instance = context.HttpContext.Request.Path
            };
            problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            problemDetails.Extensions["errors"] = errors;

            return new Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });

    // API Versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"));
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    // Swagger / OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "FullStack API",
            Version = "v1",
            Description = "Product catalog API with CQRS pattern"
        });
        options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "FullStack API",
            Version = "v2",
            Description = "Product catalog API v2"
        });

        options.DocumentFilter<ExcludeFromClientGenerationFilter>();

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    // EF Core InMemory
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("FullStackDb"));

    // Repository & UoW
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // MediatR + FluentValidation
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssemblyContaining<Program>());
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

    // Health checks
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }

    // OpenAPI spec export
    if (args.Contains("--export-openapi"))
    {
        using var scope = app.Services.CreateScope();
        var swaggerProvider = scope.ServiceProvider.GetRequiredService<Swashbuckle.AspNetCore.Swagger.ISwaggerProvider>();
        var doc = swaggerProvider.GetSwagger("v1");
        var outputPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "openapi", "openapi.json");
        var dir = Path.GetDirectoryName(outputPath)!;
        Directory.CreateDirectory(dir);

        var json = SerializeOpenApi(doc);
        File.WriteAllText(outputPath, json);
        Log.Information("OpenAPI spec exported to {Path}", outputPath);
        return;
    }

    // Middleware pipeline
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.UseSerilogRequestLogging();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FullStack API v1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "FullStack API v2");
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();

    app.MapControllers();
    app.MapTaxonomyEndpoints();
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static string SerializeOpenApi(Microsoft.OpenApi.Models.OpenApiDocument document)
{
    using var textWriter = new StringWriter();
    var jsonWriter = new Microsoft.OpenApi.Writers.OpenApiJsonWriter(textWriter);
    document.SerializeAsV3(jsonWriter);
    return textWriter.ToString();
}

/// <summary>
/// Partial Program class for test access (WebApplicationFactory).
/// </summary>
public partial class Program { }
