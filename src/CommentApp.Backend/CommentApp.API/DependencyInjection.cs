using CommentApp.Application.SignalR;
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
                builder.WithOrigins("http://localhost:6061", "http://localhost:5051")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddSignalR();

        return services;
    }
    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.UseRouting();

        app.UseCors("AllowAllOrigins");

        app.MapHub<CommentHub>("/chatHub");

        app.MapCarter();

        app.UseExceptionHandler(_ => { });

        return app;
    }
}