using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder.Property(x => x.UserId);
        builder.Property(x => x.FileUrl);
        builder.Property(x => x.Id);
        builder.Property(x => x.FileName);
        builder.Property(x => x.IsSharing).HasDefaultValue("false");
        
        builder.HasKey(d => d.Id);
        builder
            .HasOne(u => u.User)
            .WithMany(d => d.Documents)
            .HasForeignKey(k => k.UserId);
    }
}