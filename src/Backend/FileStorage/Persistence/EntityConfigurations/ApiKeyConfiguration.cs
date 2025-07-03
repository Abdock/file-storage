using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Constants;
using Persistence.Converters;
using Persistence.Entities;

namespace Persistence.EntityConfigurations;

public sealed class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Creator)
            .WithMany()
            .HasForeignKey(e => e.CreatorId);
        builder.Property(e => e.Name)
            .HasMaxLength(MaxLengthConstraints.ApiKeyName);
        builder.Property(e => e.Token)
            .HasMaxLength(MaxLengthConstraints.ApiKeyToken);
        builder.PrimitiveCollection(e => e.Permissions)
            .ElementType(typeBuilder =>
            {
                typeBuilder.HasConversion<PermissionConversion>();
            });
        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}