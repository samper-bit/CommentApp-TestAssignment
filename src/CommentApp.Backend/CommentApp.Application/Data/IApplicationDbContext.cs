﻿using CommentApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using File = CommentApp.Domain.Models.File;
namespace CommentApp.Application.Data;
public interface IApplicationDbContext
{
    DbSet<Comment> Comments { get; }
    DbSet<File> Files { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}