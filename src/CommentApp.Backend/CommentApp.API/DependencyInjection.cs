using CommentApp.Shared.Exceptions.Handler;

namespace CommentApp.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddCarter();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }
    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.UseCors("AllowAllOrigins");

        app.MapCarter();

        app.UseExceptionHandler(_ => { });

        return app;
    }
}