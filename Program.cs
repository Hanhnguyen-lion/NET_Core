using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApi.Data;
using MyApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var APICors = "ApiCORS";
var services = builder.Services;
services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Preserve property names as defined in the C# models (disable camelCase naming)
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

services.AddOpenApi();
services.AddEndpointsApiExplorer();

services.AddCors(options =>
    options.AddPolicy(APICors, policy=>
    {
        policy.WithOrigins("*")
        .AllowAnyHeader()
        .AllowAnyMethod();
    }));

// services.Configure<IConfiguration>(builder.Configuration.GetSection("ConnectionStrings"));
// // services.Configure<DBSetting>(builder.Configuration.GetSection("DBSetting"));
// services.AddSingleton<DatabaseContext>();

services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgressConnectionDb")));
// builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreDBConnection")));
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreDBConnection")));
// Register the KeyRotationService as a hosted (background) service
// This service handles periodic rotation of signing keys to enhance security
services.AddHostedService<KeyRotationService>();
// Configure Authentication using JWT Bearer tokens
services.AddAuthentication(options =>
{
    // This indicates the authentication scheme that will be used by default when the app attempts to authenticate a user.
    // Which authentication handler to use for verifying who the user is by default.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // This indicates the authentication scheme that will be used by default when the app encounters an authentication challenge. 
    // Which authentication handler to use for responding to failed authentication or authorization attempts.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Define token validation parameters to ensure tokens are valid and trustworthy
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Ensure the token was issued by a trusted issuer
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // The expected issuer value from configuration
        ValidateAudience = false, // Disable audience validation (can be enabled as needed)
        ValidateLifetime = true, // Ensure the token has not expired
        ValidateIssuerSigningKey = true, // Ensure the token's signing key is valid
        // Define a custom IssuerSigningKeyResolver to dynamically retrieve signing keys from the JWKS endpoint
        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        {
            //Console.WriteLine($"Received Token: {token}");
            //Console.WriteLine($"Token Issuer: {securityToken.Issuer}");
            //Console.WriteLine($"Key ID: {kid}");
            //Console.WriteLine($"Validate Lifetime: {parameters.ValidateLifetime}");
            // Initialize an HttpClient instance for fetching the JWKS
            var httpClient = new HttpClient();
            // Synchronously fetch the JWKS (JSON Web Key Set) from the specified URL
            var jwks = httpClient.GetStringAsync($"{builder.Configuration["Jwt:Issuer"]}/.well-known/jwks.json").Result;
            // Parse the fetched JWKS into a JsonWebKeySet object
            var keys = new JsonWebKeySet(jwks);
            // Return the collection of JsonWebKey objects for token validation
            return keys.Keys;
        }
    };
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
}

app.UseCors(APICors);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
