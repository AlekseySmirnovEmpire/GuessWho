using Core.Models.Images;
using Core.Server.Database.Files;
using Core.Server.Repositories;

namespace Core.Server.Services;

public class FileDataService(IFileDataRepository repository, IUserRepository userRepository)
{
    private const string FilePrefix = "GuessWho_";

    private readonly Dictionary<string, string> _typePrefixes = new()
    {
        ["jpeg"] = "Image_",
        ["png"] = "Image_",
        ["gif"] = "Image_"
    };

    public Guid UploadImage(ImageDataModel? image, long userId)
    {
        if (image?.Data == null) throw new ArgumentNullException(nameof(image));

        var user = userRepository.Find(u => u.Id == userId);
        if (user == null) throw new ArgumentNullException(nameof(userId));

        var type = image.Type.Split('/').Last();
        var file = new FileData(
            $"{FilePrefix}{_typePrefixes[type]}{user.NickName}",
            image.Type,
            image.Data,
            type,
            user);
        if (user.FileId != null)
        {
            repository.Delete(f => f.Id == user.FileId);
        }

        repository.Add(file);

        return file.Id;
    }

    public FileData? FindFile(Guid fileId) => repository.Find(f => f.Id == fileId);
}