using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using DkGLobalBackend.WebApi.Services;
using DkGLobalBackend.WebApi.Services.IServices;
using DkGLobalBackend.WebApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("LiveConnectionString"),
    new MySqlServerVersion(new Version("9.4.0"))
    ));

builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IDbInitializerService, DbInitializerService>();
builder.Services.AddScoped<IChecker, Checker>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<InventoryDbContext>()
    .AddDefaultTokenProviders();
// Add services
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5276); // HTTP
    options.ListenLocalhost(7189, listenOptions => listenOptions.UseHttps());
});

// Add AutoMapper
//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example: \"Bearer 1234asdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement(){
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Inventory Management v1",
        Description = "Api to manage asset",
        TermsOfService = new Uri("https://cookiessoftwaresolution.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Cookies Software Solution Ltd.",
            Url = new Uri("https://cookiessoftwaresolution.com")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://cookiessoftwaresolution.com/license")
        }
    });
   
});

var key = builder.Configuration.GetValue<string>("TokenSetting:SecretKey") ?? "";

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,

    };
});

// Add Cors.
builder.Services.AddCors(options =>
{
    
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

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


async Task<bool> ChecksDbConnection(WebApplication app, string connectionString)
{
    using var scope = app.Services.CreateScope();
    var dbChecker = scope.ServiceProvider.GetRequiredService<IChecker>();
    bool isConnected =  await dbChecker.IsDatabaseConnectedAsync(connectionString);
    if (!isConnected)
    {
        Console.WriteLine("❌ Database connection failed. app is shutting down...");
        return false;
        //throw new ApplicationException("Database connection failed.");
    }

    Console.WriteLine("✅ Database is connected!");
    return true;
}
async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializerService>();
    await dbInitializer.InitializeAsync();
}
