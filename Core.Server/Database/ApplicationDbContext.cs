using Core.Database.Email;
using Core.Models.Users;
using Core.Server.Database.Email;
using Core.Server.Database.Files;
using Core.Server.Database.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Core.Server.Database;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<FileData> Files { get; set; }

    public DbSet<EmailSendingQueue> EmailSendingQueue { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(u => u.Id);
            user.HasIndex(u => u.Email).IsUnique();
            user.HasIndex(u => u.NickName).IsUnique();
            user.HasOne(u => u.File)
                .WithMany(f => f.Users)
                .HasForeignKey(u => u.FileId)
                .OnDelete(DeleteBehavior.SetNull);
            user.Property(u => u.Role)
                .HasConversion(new EnumToStringConverter<UserRole>())
                .HasMaxLength(15);
        });

        modelBuilder.Entity<EmailSendingQueue>(email =>
        {
            email.Property(e => e.Priority)
                .HasConversion(new EnumToStringConverter<EmailPriority>())
                .HasMaxLength(10);
        });

        modelBuilder.Entity<FileData>(file =>
        {
            file.HasKey(f => f.Id);
            file.HasIndex(f => f.Name).IsUnique();
        });
    }
}