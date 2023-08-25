using Ardalis.GuardClauses;
using AlohaMaui.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AlohaMaui.Core.Repositories;
using AlohaMaui.Core.Providers;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

#if DEBUG
        builder.Services.AddCors(options =>
        {
            var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"];
            Guard.Against.NullOrEmpty(allowedOrigins, nameof(allowedOrigins));
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(allowedOrigins.Split(";"))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
#endif
        AddDataServices(builder.Services, builder.Configuration);
        AddAuth(builder.Services, builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseCors();
        app.MapControllers();
        app.Run();
    }

    private static void AddAuth(IServiceCollection services, ConfigurationManager configuration)
    {
        var jwtSecret = configuration["Auth:JwtSecret"];
        Guard.Against.NullOrEmpty(jwtSecret, nameof(jwtSecret));

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddCookie(x =>
        {
            x.Cookie.Name = "X-Access-Token";

        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            x.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["X-Access-Token"];
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });
    }

    private static void AddDataServices(IServiceCollection services, ConfigurationManager configuration)
    {
        var cosmosAccount = configuration["CosmosDb:Account"];
        var cosmosKey = configuration["CosmosDb:Key"];
        var comsosDatabase = configuration["CosmosDb:Database"];

        Guard.Against.NullOrEmpty(cosmosAccount, nameof(cosmosAccount));
        Guard.Against.NullOrEmpty(cosmosKey, nameof(cosmosKey));
        services.AddSingleton<ICosmosClientProvider>(new CosmosClientProvider(cosmosAccount, cosmosKey));

        Guard.Against.NullOrEmpty(comsosDatabase, nameof(comsosDatabase));
        services.AddSingleton<ICosmosDatabaseProvider>(new CosmosDatabaseProvider(comsosDatabase, services.BuildServiceProvider().GetRequiredService<ICosmosClientProvider>()));

        services.AddSingleton<ICommunityEventRepository>(new CommunityEventRepository(
            new CosmosContainerProvider("events", services.BuildServiceProvider().GetRequiredService<ICosmosDatabaseProvider>()))
        );

        services.AddSingleton<IUserRepository>(new UserRepository(
            new CosmosContainerProvider("users", services.BuildServiceProvider().GetRequiredService<ICosmosDatabaseProvider>()))
        );

        var jwtSecret = configuration["Auth:JwtSecret"];
        Guard.Against.NullOrEmpty(jwtSecret, nameof(jwtSecret));
        services.AddSingleton<IJwtTokenGenerator>(new JwtTokenGenerator(jwtSecret));

        var googleClientId = configuration["Auth:Google:ClientId"];
        Guard.Against.NullOrEmpty(googleClientId, nameof(googleClientId));
        services.AddSingleton<IGoogleAuthValidator>(new GoogleAuthValidator(googleClientId));

        var blobConnectionString = configuration["Azure:Blob:ConnectionString"];
        Guard.Against.NullOrEmpty(blobConnectionString, nameof(blobConnectionString));
        services.AddSingleton<IBlobServiceClientProvider>(new BlobServiceClientProvider(blobConnectionString));
    }
}