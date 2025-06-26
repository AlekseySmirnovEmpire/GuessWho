using System.ComponentModel.DataAnnotations;
using Core.Server.Database.Users;

namespace Core.Server.Database.Files;

public class FileData
{
    [Key] public Guid Id { get; init; }

    [Required]
    [MaxLength(100), MinLength(10)]
    public string Name { get; init; }

    [Required]
    [MaxLength(100), MinLength(5)]
    public string ContentType { get; init; }

    [Required]
    [MaxLength(100), MinLength(3)]
    public string Type { get; init; }

    [Required] public byte[] Data { get; init; }

    public ICollection<User> Users { get; init; }

    [Obsolete(message: "ONLY EF", error: true)]
    public FileData()
    {
    }

    public FileData(string name, string contentType, byte[] data, string type, User? user = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        ContentType = contentType;
        Data = data;
        Type = type;
        if (user == null) return;

        Users ??= new List<User>();
        Users.Add(user);
    }
}