using archerly.api.endpoints;
using archerly.core.hunts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
namespace archerly.api.extensions;

public static class WebAppBuilderExtensions
{
    public static IApplicationBuilder UseRoutes(this WebApplication self)
    {
        // Activate Login
        self.MapLoginEndpoints();
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
        var url = builder.Configuration["Supabase:Url"]
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
            var validIssuer = builder.Configuration["Supabase:ValidIssuer"]
            ?? throw new InvalidOperationException("Supabase:ValidIssuer missing");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = validIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret)
            ),

                // 👇 important: map `sub` correctly
                NameClaimType = ClaimTypes.NameIdentifier
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

}