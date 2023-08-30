using Ardalis.GuardClauses;
using AlohaMaui.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AlohaMaui.Core.Repositories;
using AlohaMaui.Core.Providers;
using AlohaMaui.Core.Entities;

internal class Program
{
    const string AllowedOriginsPolicyName = "AllowedOrigins";

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;
        ConfigureServices(builder.Services, configuration);
        ConfigureAuth(builder.Services, configuration);

        var app = builder.Build();
        Configure(app, app.Environment);

        app.MapControllers();

        app.Run();
    }

    private static void ConfigureAuth(IServiceCollection services, ConfigurationManager configuration)
    {
        var jwtSecret = configuration["Auth:Jwt:Secret"];
        var jwtIssuer = configuration["Auth:Jwt:Issuer"];
        var jwtAudience = configuration["Auth:Jwt:Audience"];

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddCookie(x =>
        {
            x.Cookie.Name = "X-Access-Token";

        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });
    }

    private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
#if DEBUG // CORS is handled by Azure in production
        services.AddCors(options =>
        {
            var allowedHosts = configuration["Cors:AllowedOrigins"];
            Guard.Against.NullOrEmpty(allowedHosts, nameof(allowedHosts));
            options.AddPolicy(name: AllowedOriginsPolicyName, builder =>
            {
                builder.WithOrigins(allowedHosts.Split(';'))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
#endif

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CommunityEvent).Assembly));

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddLazyCache();
        services.AddAutoMapper(typeof(MapperProfile).Assembly);

        AddDataServices(services, configuration);
    }

    private static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //app.UseAuthentication();
        app.UseCors(AllowedOriginsPolicyName);

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
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

        services.AddSingleton<IPledgeEventRepository>(new PledgeEventRepository(
            new CosmosContainerProvider("events", services.BuildServiceProvider().GetRequiredService<ICosmosDatabaseProvider>()))
        );

        services.AddSingleton<IOrgEventRepository>(new OrgEventRepository(
            new CosmosContainerProvider("events", services.BuildServiceProvider().GetRequiredService<ICosmosDatabaseProvider>()))
        );

        var jwtSecret = configuration["Auth:Jwt:Secret"];
        var jwtIssuer = configuration["Auth:Jwt:Issuer"];
        var jwtAudience = configuration["Auth:Jwt:Audience"];
        services.AddSingleton<IJwtTokenGenerator>(new JwtTokenGenerator(jwtSecret, jwtIssuer, jwtAudience));

        services.AddSingleton<IPasswordHasher>(new PasswordHasher(configuration["Auth:PasswordSalt"]));

        var googleClientId = configuration["Auth:Google:ClientId"];
        Guard.Against.NullOrEmpty(googleClientId, nameof(googleClientId));
        services.AddSingleton<IGoogleAuthValidator>(new GoogleAuthValidator(googleClientId));

        var blobConnectionString = configuration["Azure:Blob:ConnectionString"];
        Guard.Against.NullOrEmpty(blobConnectionString, nameof(blobConnectionString));
        services.AddSingleton<IBlobServiceClientProvider>(new BlobServiceClientProvider(blobConnectionString));
    }
}