using CommentApp.Infrastructure.Data.Interceptors;
using CommentApp.Infrastructure.Services.CaptchaService;
using CommentApp.Infrastructure.Services.HtmlSanitizerService;
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

        services.AddSingleton<CaptchaService>();
        services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();

        return services;
    }
}
