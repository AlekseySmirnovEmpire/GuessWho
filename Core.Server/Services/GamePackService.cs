using Core.Models.GamePacks;
using Core.Models.Users;
using Core.Server.Database.GamePacks;
using Core.Server.Database.Users;
using Core.Server.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Server.Services;

public class GamePackService(IGamePackRepository repository, FileDataService fileDataService)
{
    public GamePackDtoModel Create(GamePackCreateDtoModel model, User user)
    {
        if (!user.CheckAccess(UserRole.Moderator)) throw new UnauthorizedAccessException();

        var packFile = fileDataService.CreateGamePack(model);
        var gamePack = repository.Add(new GamePack(model.DisplayName, user.Id, packFile.Id, model.Description));
        if (gamePack == null) throw new Exception("Не удалось создать пак");

        return new GamePackDtoModel
        {
            DisplayName = gamePack.DisplayName,
            Description = gamePack.Description,
            CreatedAt = gamePack.CreatedAt,
            Id = gamePack.Id,
            UserCreatedId = user.Id,
            UserCreated = new UserModel
            {
                Id = user.Id,
                NickName = user.NickName,
                ImageId = user.ImageId
            },
            IsActive = gamePack.IsActive
        };
    }

    public List<GamePackDtoModel> FindAll(bool needActivity = false)
    {
        return repository
            .FindList(
                gp => !needActivity || gp.IsActive,
                includes: query => query.Include(gp => gp.UserCreated))
            .OrderByDescending(gp => gp.CreatedAt)
            .Select(gp => new GamePackDtoModel
            {
                Id = gp.Id,
                DisplayName = gp.DisplayName,
                Description = gp.Description,
                CreatedAt = gp.CreatedAt,
                IsActive = gp.IsActive,
                UserCreatedId = gp.UserCreated.Id,
                UserCreated = new UserModel
                {
                    Id = gp.UserCreated.Id,
                    NickName = gp.UserCreated.NickName,
                    ImageId = gp.UserCreated.ImageId
                }
            })
            .ToList();
    }

    public GamePackDtoModel Find(long id)
    {
        var model = repository.Find(
            gp => gp.Id == id && gp.IsActive,
            includes: query => query.Include(gp => gp.UserCreated));
        if (model == null) throw new ArgumentNullException(nameof(id));

        return new GamePackDtoModel
        {
            DisplayName = model.DisplayName,
            Description = model.Description,
            CreatedAt = model.CreatedAt,
            IsActive = model.IsActive,
            UserCreatedId = model.UserCreatedId,
            UserCreated = new UserModel
            {
                Id = model.UserCreated.Id,
                NickName = model.UserCreated.NickName,
                ImageId = model.UserCreated.ImageId
            }
        };
    }

    public void ChangeGamePackActivity(User user, long id, bool active)
    {
        if (!user.CheckAccess(UserRole.Moderator)) throw new UnauthorizedAccessException();

        repository.Update(gp => gp.Id == id, sp => sp.SetProperty(gp => gp.IsActive, active));
    }

    public void Remove(User user, long id)
    {
        if (!user.CheckAccess(UserRole.Moderator)) throw new UnauthorizedAccessException();

        repository.Delete(gp => gp.Id == id);
    }
}