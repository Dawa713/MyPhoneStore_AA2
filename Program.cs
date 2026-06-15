using api_clase.Controllers;
using api_clase.Services;
using api_clase.Mappings;
using api_clase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configurar Entity Framework Core con MySQL/MariaDB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                     ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar los servicios (Inyección de Dependencias)
// Scoped: Una instancia por cada request HTTP
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IPhoneRepository, PhoneRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<JwtService>();

// Registrar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configurar autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

// CORS: permite peticiones desde el frontend Vue
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173",  // desarrollo Vite
                  "http://localhost:3000",
                  "http://localhost:80",    // Docker frontend
                  "http://localhost"        // Docker frontend sin puerto
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Aplicar migraciones automáticamente al arrancar (necesario en Docker)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Middleware: inyecta el esquema Bearer JWT en el swagger.json generado.
// Usa System.Text.Json puro para evitar incompatibilidades con Microsoft.OpenApi.Models v2.
app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/swagger/v1/swagger.json"))
    {
        await next(context);
        return;
    }

    var originalBody = context.Response.Body;
    using var buffer = new MemoryStream();
    context.Response.Body = buffer;

    await next(context);

    buffer.Position = 0;
    var originalJson = await new StreamReader(buffer).ReadToEndAsync();
    var modified = InjectJwtSecurityScheme(originalJson);
    var modifiedBytes = Encoding.UTF8.GetBytes(modified);

    context.Response.Body = originalBody;
    context.Response.Headers.Remove("Content-Length");
    context.Response.ContentLength = modifiedBytes.Length;
    await context.Response.Body.WriteAsync(modifiedBytes);
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowVueFrontend");

app.UseAuthentication(); // Primero Authentication, luego Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();

// Inyecta securitySchemes Bearer en el JSON de Swagger (sin tipos Microsoft.OpenApi.Models)
static string InjectJwtSecurityScheme(string json)
{
    using var doc = System.Text.Json.JsonDocument.Parse(json);
    var root = doc.RootElement;
    bool hasComponents = root.TryGetProperty("components", out _);

    using var ms = new MemoryStream();
    using var w = new System.Text.Json.Utf8JsonWriter(ms);
    w.WriteStartObject();

    foreach (var prop in root.EnumerateObject())
    {
        if (prop.Name == "components")
        {
            w.WritePropertyName("components");
            w.WriteStartObject();
            foreach (var cp in prop.Value.EnumerateObject())
                cp.WriteTo(w);
            WriteBearerScheme(w);
            w.WriteEndObject();
        }
        else if (prop.Name == "paths")
        {
            if (!hasComponents)
            {
                w.WritePropertyName("components");
                w.WriteStartObject();
                WriteBearerScheme(w);
                w.WriteEndObject();

                w.WritePropertyName("security");
                w.WriteStartArray();
                w.WriteStartObject();
                w.WritePropertyName("Bearer");
                w.WriteStartArray();
                w.WriteEndArray();
                w.WriteEndObject();
                w.WriteEndArray();
            }
            prop.WriteTo(w);
        }
        else
        {
            prop.WriteTo(w);
        }
    }

    w.WriteEndObject();
    w.Flush();
    return Encoding.UTF8.GetString(ms.ToArray());
}

static void WriteBearerScheme(System.Text.Json.Utf8JsonWriter w)
{
    w.WritePropertyName("securitySchemes");
    w.WriteStartObject();
    w.WritePropertyName("Bearer");
    w.WriteStartObject();
    w.WriteString("type", "http");
    w.WriteString("scheme", "bearer");
    w.WriteString("bearerFormat", "JWT");
    w.WriteString("description", "Token JWT. Obtenerlo en POST /api/auth/login");
    w.WriteEndObject();
    w.WriteEndObject();
}
