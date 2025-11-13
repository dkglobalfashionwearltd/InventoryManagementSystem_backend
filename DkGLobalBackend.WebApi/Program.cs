using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services;
using DkGLobalBackend.WebApi.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== Add Services =====


// Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Inventory Management API",
        Description = "API to manage assets",
        Contact = new OpenApiContact
        {
            Name = "Cookies Software Solution Ltd.",
            Url = new Uri("https://cookiessoftwaresolution.com")
        }
    });

    // JWT Bearer support in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// DbContext
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("LiveConnectionString"),
        new MySqlServerVersion(new Version(9, 4, 0))
    )
);

// Scoped services
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IDbInitializerService, DbInitializerService>();
builder.Services.AddScoped<IChecker, Checker>();
builder.Services.AddScoped<IStockService, StockMethodService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<InventoryDbContext>()
    .AddDefaultTokenProviders();

// ===== JWT Authentication =====

var key = builder.Configuration.GetValue<string>("TokenSetting:SecretKey");
var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
    ValidateIssuer = false,
    ValidateAudience = false,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = tokenValidationParams;
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Read token from the HTTP-only cookie
            var accessToken = context.HttpContext.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// ===== CORS =====
var allowedOrigin = "https://localhost:5173";
var allowedLiveOrigin = "https://inventory-management-system-lyart.vercel.app";
var allowedLiveApiOrigin = "https://inventory.cookiesoftwareltd.com:4200";
var allowedLiveApiOrigin2 = "https://api.cookiesoftwareltd.com";

builder.Services.AddCors(options =>
{
    
    options.AddPolicy("AllowCors", policy =>
    {
        policy.WithOrigins(allowedOrigin,allowedLiveOrigin, allowedLiveApiOrigin, allowedLiveApiOrigin2)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


// ===== Build App =====
var app = builder.Build();

// Configure the HTTP request pipeline. ===== Middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowCors");

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ===== Health Checks =====
app.MapHealthChecks("/health");
app.MapControllers();


var conStr = builder.Configuration.GetConnectionString("LiveConnectionString");
if (!await ChecksDbConnection(app, conStr))
{
    // stop app completely
    return;
}
await SeedDatabaseAsync(app);

app.Run();


// ===== Helper Functions =====
async Task<bool> ChecksDbConnection(WebApplication app, string connectionString)
{
    using var scope = app.Services.CreateScope();
    var dbChecker = scope.ServiceProvider.GetRequiredService<IChecker>();
    var isConnected = await dbChecker.IsDatabaseConnectedAsync(connectionString);
    Console.WriteLine(isConnected
        ? "✅ Database is connected!"
        : "❌ Database connection failed. App is shutting down...");
    return isConnected;
}
async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializerService>();
    await dbInitializer.InitializeAsync();
}
