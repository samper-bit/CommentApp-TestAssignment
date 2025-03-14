﻿using Microsoft.AspNetCore.Builder;

namespace CommentApp.Infrastructure.Data.Extensions;
public static class DatabaseExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.MigrateAsync().GetAwaiter().GetResult();

        await SeedAsync(context);
    }

    private static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.Comments.AnyAsync())
        {
            await context.Comments.AddRangeAsync(InitialData.CommentsWithAttachedFiles);
            await context.SaveChangesAsync();
        }
    }
}