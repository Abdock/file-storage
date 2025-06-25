using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Constants;
using Persistence.Entities;

namespace Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Username)
            .HasMaxLength(MaxLengthConstraints.Username);
        builder.Property(e => e.PasswordHash)
            .HasMaxLength(MaxLengthConstraints.PasswordHash);
        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}