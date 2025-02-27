﻿global using CommentApp.Domain.Models;
global using CommentApp.Domain.ValueObjects;
global using CommentApp.Domain.Abstractions;
global using CommentApp.Infrastructure.Data;
global using CommentApp.Application.Data;
global using CommentApp.Application.Services.HtmlSanitizerService;
global using CommentApp.Application.Services.CaptchaService;
global using CommentApp.Application.Services.FileService;
global using CommentApp.Application.Services.NotificationService;
global using CommentApp.Application.Services.CacheService;
global using CommentApp.Application.Services.RabbitMqService;
global using CommentApp.Shared.Exceptions;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.SignalR;
global using System.Reflection;
global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.Processing;
global using StackExchange.Redis;
global using RabbitMQ.Client;
global using File = CommentApp.Domain.Models.File;
