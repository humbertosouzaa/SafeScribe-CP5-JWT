using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SafeScribe.Api.Data;
using SafeScribe.Api.Middleware;
using SafeScribe.Api.Models;
using SafeScribe.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// DB InMemory
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("safescribe"));

// Serviços
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<ITokenBlacklistService, InMemoryTokenBlacklistService>();

builder.Services.AddControllers();

// JWT Auth
var jwtCfg = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtCfg["Key"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        // Validações principais do token:
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtCfg["Issuer"],
            ValidAudience = jwtCfg["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Seed (nome: admin / senha: 123456)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Users.Any())
    {
        db.Users.Add(new User {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = Role.Admin
        });
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();

// Middleware que bloqueia tokens em blacklist (após autenticação)
app.UseMiddleware<JwtBlacklistMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

/* ValidateIssuer/ValidateAudience: garante que o token foi emitido pelo Issuer e para a Audience esperada.

ValidateLifetime: recusa tokens expirados.

ValidateIssuerSigningKey: confere a assinatura com a IssuerSigningKey.

NameClaimType: ajuda a mapear qual claim usar como “nome” ao acessar via User.FindFirst. */