using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.Property(x => x.UserName);
        builder.Property(x => x.Login);
        builder.Property(x => x.PasswordHash);
        
        builder.HasKey(a => a.Id);
        builder.HasMany(d => d.Documents)
            .WithOne(d => d.User)
            .HasForeignKey(k => k.UserId);
    }
}
