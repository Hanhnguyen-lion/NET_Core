using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyApi.Services;

var builder = WebApplication.CreateBuilder(args);

var APICors = "ApiCORS";
var services = builder.Services;
services.AddControllers()
                .AddJsonOptions(options =>
                {
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

// services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgressConnectionDb")));
// builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreDBConnection")));
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreDBConnection")));

// services.Configure<UsersContextDb>(builder.Configuration.GetSection("ConnectionStrings"));

services.AddHostedService<KeyRotationService>();

services.AddAuthentication(options =>{
    options.DefaultAuthenticateScheme = 
    options.DefaultChallengeScheme = 
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(options =>
{
    // Define token validation parameters to ensure tokens are valid and trustworthy
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        {
            var httpClient = new HttpClient();
            var jwks = httpClient.GetStringAsync(builder.Configuration["Jwt:JWSK"]).Result;
            return new JsonWebKeySet(jwks).Keys;
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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
