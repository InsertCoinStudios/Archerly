using archerly.api.endpoints;
using archerly.core.hunts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using archerly.database.repos.interfaces;
using archerly.database.jsondb.repositories;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using archerly.database.jsondb;
namespace archerly.api.extensions;

public static class WebAppBuilderExtensions
{
    public static IApplicationBuilder UseRoutes(this WebApplication self)
    {
        // Activate Login
        self.MapLoginEndpoints();

        self.MapLogoutEndpoints();
        // Activate Register Endpoint
        self.MapRegisterEndpoints();
        // Activate Hunt
        self.MapHuntEndpoints();
        // Activate All Time Statistic
        self.MapAllTimeStatEndpoints();
        // Activate Course Endpoint
        self.MapCourseEndpoints();
        // Activate Animal Endpoints
        self.MapAnimalEndpoints();
        // Activate Image Endpoints
        self.MapImageEndpoints();

        return self;
    }

    public static WebApplicationBuilder AddSupabase(this WebApplicationBuilder builder)
    {
        builder.AddSupabaseAuth();
        builder.AddSupabaseClient();

        return builder;
    }

    private static WebApplicationBuilder AddSupabaseClient(this WebApplicationBuilder builder)
    {
        // Environmental Variable = SUPABASE__URL 
        var url = builder.Configuration["supabase:url"]
            ?? throw new InvalidOperationException("Supabase:Url missing");

        // Environmental Variable = SUPABASE__AnonKey
        var key = builder.Configuration["Supabase:AnonKey"]
            ?? throw new InvalidOperationException("Supabase:AnonKey missing");

        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        builder.Services.AddSingleton(_ =>
            new Supabase.Client(url, key, options)
        );

        return builder;
    }

    private static WebApplicationBuilder AddSupabaseAuth(this WebApplicationBuilder builder)
    {
        builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // Environmental Variable = SUPABASE__JWTSecret
            var secret = builder.Configuration["Supabase:JwtSecret"]
            ?? throw new InvalidOperationException("Supabase:JwtSecret missing");

            // Environmental Variable = SUPABASE__ValidIssuer
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // 👇 important: map `sub` correctly
                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = "role",

                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secret)
                ),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)

            };
        });
        return builder;
    }

    public static WebApplicationBuilder AddHuntManagerService(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(_ =>
            new HuntManager(true, 5)
        );
        return builder;
    }
    public static WebApplicationBuilder AddRepoServices(this WebApplicationBuilder builder)
    {
        var dbPath = Path.Combine(
            AppContext.BaseDirectory,
            "logs",
            "database.json"
        );
        builder.Services.AddSingleton(new JsonDatabaseStore(dbPath));
        builder.Services.AddSingleton<IUserRepository>(sp =>
        {
            var client = sp.GetRequiredService<JsonDatabaseStore>();
            return new UserRepository(client);
        }
        );

        // Animal repo
        builder.Services.AddSingleton<IAnimalRepository>(sp =>
        {
            var client = sp.GetRequiredService<JsonDatabaseStore>();
            return new AnimalRepository(client);
        });

        // Course repo
        builder.Services.AddSingleton<ICourseRepository>(sp =>
        {
            var client = sp.GetRequiredService<JsonDatabaseStore>();
            return new CourseRepository(client);
        });

        // CourseAnimal (cross-table) repo
        builder.Services.AddSingleton<ICourseAnimalRepository>(sp =>
        {
            var client = sp.GetRequiredService<JsonDatabaseStore>();
            return new CourseAnimalRepository(client);
        });

        // Shot repo
        builder.Services.AddSingleton<IShotRepository>(sp =>
        {
            var client = sp.GetRequiredService<JsonDatabaseStore>();
            return new ShotRepository(client);
        });

        // HydratedCourse repo (reuses existing repos)
        builder.Services.AddSingleton<IHydratedCourseRepository>(sp =>
        {
            var client = sp.GetRequiredService<JsonDatabaseStore>();
            return new HydratedCourseRepository(client);
        });
        return builder;
    }
}