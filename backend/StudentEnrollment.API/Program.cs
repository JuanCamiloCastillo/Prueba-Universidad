using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StudentEnrollment.API.Middleware;
using StudentEnrollment.Application;
using StudentEnrollment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Capas de aplicación e infraestructura
builder.Services.AgregarAplicacion();
builder.Services.AgregarInfraestructura(builder.Configuration);

// Controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API de Inscripción Estudiantil", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Encabezado de autorización JWT usando el esquema Bearer.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Autenticación JWT
var configuracionJwt = builder.Configuration.GetSection("JwtSettings");
var claveSecreta = configuracionJwt["SecretKey"] ?? throw new InvalidOperationException("Se requiere JwtSettings:SecretKey.");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveSecreta)),
        ValidateIssuer = true,
        ValidIssuer = configuracionJwt["Issuer"],
        ValidateAudience = true,
        ValidAudience = configuracionJwt["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["AllowedOrigins"] ?? "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Limitación de tasa
builder.Services.AddRateLimiter(options =>
{
    var limitePermitido = builder.Configuration.GetValue<int?>("RateLimitSettings:PermitLimit") ?? 10;
    var segundosVentana = builder.Configuration.GetValue<int?>("RateLimitSettings:WindowSeconds") ?? 1;
    options.AddPolicy("PoliticaPorUsuario", context =>
    {
        var idUsuario = context.User?.FindFirst("studentId")?.Value ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonimo";
        return RateLimitPartition.GetFixedWindowLimiter(idUsuario, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = limitePermitido,
            Window = TimeSpan.FromSeconds(segundosVentana),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });
    options.RejectionStatusCode = 429;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PoliticaFrontend");
app.UseMiddleware<MiddlewareExcepciones>();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("PoliticaPorUsuario");

app.Run();
