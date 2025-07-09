using Core.Database.Email;
using Core.Models.Users;
using Core.Server.Database.Email;
using Core.Server.Database.Files;
using Core.Server.Database.GamePacks;
using Core.Server.Database.Lobbies;
using Core.Server.Database.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Core.Server.Database;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<FileData> Files { get; set; }

    public DbSet<Lobby> Lobbies { get; set; }

    public DbSet<GamePack> GamePacks { get; set; }

    public DbSet<EmailSendingQueue> EmailSendingQueue { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(u => u.Id);
            user.HasIndex(u => u.Email).IsUnique();
            user.HasIndex(u => u.NickName).IsUnique();
            user.HasOne(u => u.Image)
                .WithOne(f => f.User)
                .HasForeignKey<User>(u => u.ImageId)
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

        modelBuilder.Entity<Lobby>(lobby =>
        {
            lobby.HasKey(f => f.Id);
            lobby.HasOne(l => l.Host)
                .WithMany(u => u.HostedLobby)
                .HasForeignKey(l => l.HostId)
                .OnDelete(DeleteBehavior.Cascade);
            lobby.HasMany(l => l.Users)
                .WithOne(u => u.JoinedLobby)
                .HasForeignKey(l => l.JoinedLobbyId)
                .OnDelete(DeleteBehavior.SetNull);
            lobby.HasOne(l => l.GamePack)
                .WithMany(gp => gp.Lobbies)
                .HasForeignKey(l => l.GamePackId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GamePack>(gamePack =>
        {
            gamePack.HasKey(gp => gp.Id);
            gamePack.HasOne(gp => gp.File)
                .WithOne(f => f.GamePack)
                .HasForeignKey<GamePack>(gp => gp.FileId)
                .OnDelete(DeleteBehavior.Cascade);
            gamePack.HasOne(gp => gp.UserCreated)
                .WithMany(u => u.GamePacks)
                .HasForeignKey(gp => gp.UserCreatedId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}