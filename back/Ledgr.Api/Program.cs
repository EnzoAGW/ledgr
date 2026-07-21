using System.Text;
using Ledgr.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Ledgr.Application.Auth.Commands.Login;
using Ledgr.Infrastructure;
using Ledgr.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
        opt.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddOpenApi();

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p =>
        p.WithOrigins("http://localhost:4200", "http://localhost")
         .AllowAnyHeader()
         .AllowAnyMethod()));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.MapInboundClaims = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer   = false,
            ValidateAudience = false,
        };
    });

builder.Services.AddAuthorization();

// ── App ───────────────────────────────────────────────────────────────────────
var app = builder.Build();

// Migrate + Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LedgrDbContext>();
    await db.Database.MigrateAsync();
    await LedgrDbSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
