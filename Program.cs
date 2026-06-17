using System.Text;
using System.Text.Json.Serialization;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using alposim.Data;
using alposim.Helper;
using alposim.Interfaces;
using alposim.Models;
using alposim.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
DotEnv.Load();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//cloud
var CloudDbPassword = Environment.GetEnvironmentVariable("CLOUD_DB_PASSWORD");
var CloudDbHost = Environment.GetEnvironmentVariable("CLOUD_DB_HOST");
var CloudDbPort = Environment.GetEnvironmentVariable("CLOUD_DB_PORT");
var CloudDbName = Environment.GetEnvironmentVariable("CLOUD_DB_NAME");
var CloudDbUser = Environment.GetEnvironmentVariable("CLOUD_DB_USER");

//local
var LocalDbPassword = Environment.GetEnvironmentVariable("LOCAL_DB_PASSWORD");
var LocalDbHost = Environment.GetEnvironmentVariable("LOCAL_DB_HOST");
var LocalDbPort = Environment.GetEnvironmentVariable("LOCAL_DB_PORT");
var LocalDbName = Environment.GetEnvironmentVariable("LOCAL_DB_NAME");
var LocalDbUser = Environment.GetEnvironmentVariable("LOCAL_DB_USER");


var jwtkey = Environment.GetEnvironmentVariable("JWT_KEY");

var CloudConnectionString = $"Host={CloudDbHost};Database={CloudDbName};Username={CloudDbUser};Password={CloudDbPassword};SSL Mode=Require;Trust Server Certificate=true";
var LocalConnectionString = $"Host={LocalDbHost};Database={LocalDbName};Username={LocalDbUser};Password={LocalDbPassword};";



builder.Services.AddSingleton<DbContextFactory>(sp => new DbContextFactory(
    LocalConnectionString,
    CloudConnectionString
));

builder.Services.AddDbContext<LocalDbContext>(options =>
{
    options.UseNpgsql(LocalConnectionString);
});

builder.Services.AddDbContext<CloudDbContext>(options =>
{
    options.UseNpgsql(CloudConnectionString);
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ISyncRepository, SyncRepository>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    
    options.UseInlineDefinitionsForEnums();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here."
    });
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", doc)] = []
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtkey!)
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.FromMinutes(10)
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var localContext = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
    localContext.Database.Migrate();
    SeedData.Initialize(localContext);

    var cloudContext = scope.ServiceProvider.GetRequiredService<CloudDbContext>();
    cloudContext.Database.Migrate();
    SeedData.Initialize(cloudContext);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();