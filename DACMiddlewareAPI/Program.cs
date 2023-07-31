using DACMiddlewareAPI.Context;
using DACMiddlewareAPI.Interfaces;
using DACMiddlewareAPI.Repositories;
using DACMiddlewareAPI.ServiceExtensions;
using DACMiddlewareAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:Default"];

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .MSSqlServer(
        connectionString: "Server=localhost;Database=LogDb;Integrated Security=SSPI;",
        sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents" })
    .CreateLogger();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 10000, rollOnFileSizeLimit: true)
    //.WriteTo.MSSqlServer(connectionString, "Logs", autoCreateSqlTable: true)
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("BankApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input a valid token to access this API"
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "BankApiBearerAuth"
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddDbContext<MiddlewareContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "AppIdAppKey";
    options.DefaultChallengeScheme = "AppIdAppKey";
})
    .AddClientAuthentication(options => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
})

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
