using CommentApp.Infrastructure.Data.Interceptors;
using CommentApp.Infrastructure.Services.CaptchaService;
using CommentApp.Infrastructure.Services.FileService;
using CommentApp.Infrastructure.Services.HtmlSanitizerService;
using CommentApp.Infrastructure.Services.NotificationService;
using CommentApp.Infrastructure.Services.RedisCacheService;
using CommentApp.Infrastructure.Services.SignalRService;
using Microsoft.Extensions.Configuration;

namespace CommentApp.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.AddInterceptors(new AuditableEntityInterceptor());
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<SignalRService>();

        services.AddSingleton<CaptchaService>();
        services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var options = ConfigurationOptions.Parse("localhost:6379");
            options.ConnectTimeout = 5000;
            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });

        return services;
    }
}
