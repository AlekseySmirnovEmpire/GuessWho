using Core.Models.GamePacks;
using Core.Models.Images;
using Core.Server.Database.Files;
using Core.Server.Database.Users;
using Core.Server.Repositories;

namespace Core.Server.Services;

public class FileDataService(IFileDataRepository repository, IUserRepository userRepository)
{
    private const string FilePrefix = "GuessWho_";

    private readonly Dictionary<string, string> _typePrefixes = new()
    {
        ["jpeg"] = "Image_",
        ["png"] = "Image_",
        ["gif"] = "Image_",
        ["zip"] = "GamePack_",
        ["x-tar"] = "GamePack_",
        ["gzip"] = "GamePack_",
        ["vnd.rar"] = "GamePack_",
        ["x-7z-compressed"] = "GamePack_"
    };

    public Guid UploadImage(ImageDataModel? image, User user)
    {
        if (image?.Data == null) throw new ArgumentNullException(nameof(image));

        ArgumentNullException.ThrowIfNull(user);

        var type = image.Type.Split('/').Last();
        var file = new FileData(
            $"{FilePrefix}{_typePrefixes[type]}{user.NickName}",
            image.Type,
            image.Data,
            type);
        if (user.ImageId != null)
        {
            repository.Delete(f => f.Id == user.ImageId);
        }

        var added = repository.Add(file);
        if (added == null) throw new InvalidCastException();

        userRepository.Update(
            u => u.Id == user.Id, 
            sp => sp.SetProperty(u => u.ImageId, added.Id));

        return file.Id;
    }

    public FileData? FindFile(Guid fileId) => repository.Find(f => f.Id == fileId);

    public FileData CreateGamePack(GamePackCreateDtoModel model)
    {
        if (model.Data == null) throw new ArgumentNullException(nameof(model));

        var type = model.Type.Split('/').Last();
        var output = repository.Add(new FileData(
            $"{FilePrefix}{_typePrefixes[type]}{Guid.NewGuid()}",
            model.Type,
            model.Data,
            type));
        if (output == null) throw new ArgumentNullException(nameof(model));

        return output;
    }
}