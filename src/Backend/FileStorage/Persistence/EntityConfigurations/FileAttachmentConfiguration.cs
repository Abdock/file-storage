using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Constants;
using Persistence.Entities;

namespace Persistence.EntityConfigurations;

public sealed class FileAttachmentConfiguration : IEntityTypeConfiguration<FileAttachment>
{
    public void Configure(EntityTypeBuilder<FileAttachment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(e => e.Name)
            .HasMaxLength(MaxLengthConstraints.FileName);
        builder.Property(e => e.CheckSum)
            .HasMaxLength(MaxLengthConstraints.CheckSum);
        builder.HasOne(e => e.CreatorApiKey)
            .WithMany()
            .HasForeignKey(e => e.CreatorApiKeyId);
        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}