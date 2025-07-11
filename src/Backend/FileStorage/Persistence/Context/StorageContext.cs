﻿using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Context;

public sealed class StorageContext : DbContext
{
    public DbSet<ApiKey> ApiKeys { get; init; }
    public DbSet<FileAttachment> FileAttachments { get; init; }
    public DbSet<User> Users { get; init; }

    private StorageContext()
    {
    }

    public StorageContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StorageContext).Assembly);
    }
}