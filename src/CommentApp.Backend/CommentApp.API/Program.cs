using CommentApp.API;
using CommentApp.Application;
using CommentApp.Infrastructure;
using CommentApp.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseApiServices();

if (app.Environment.IsDevelopment())
    await app.InitialiseDatabaseAsync();

app.Run();