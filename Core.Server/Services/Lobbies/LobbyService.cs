using Core.Models.Lobbies;
using Core.Models.Users;
using Core.Server.Database.Lobbies;
using Core.Server.Repositories;

namespace Core.Server.Services.Lobbies;

public class LobbyService(ILobbyRepository repository, IUserRepository repositoryUser)
{
    public LobbyDtoModel CreateLobby(LobbyDtoModel lobby)
    {
        var host = repositoryUser.Find(u => u.Id == lobby.HostId);
        if (host == null) throw new Exception("Не удалось создать лобби");

        var model = repository.Add(
            new Lobby(
                lobby.DisplayName,
                lobby.HostId,
                lobby.PlayersCount,
                lobby.RatingChange,
                lobby.GamePackId,
                lobby.Password,
                lobby.Conditions));
        if (model == null) throw new Exception("Не удалось создать лобби");

        return new LobbyDtoModel
        {
            DisplayName = model.DisplayName,
            HostId = model.HostId,
            PlayersCount = model.PlayersCount,
            RatingChange = model.RatingChange,
            GamePackId = model.GamePackId,
            Password = model.Password,
            Conditions = model.Conditions,
            Id = model.Id,
            CreatedAt = model.CreatedAt,
            Host = new UserModel
            {
                Id = host.Id,
                ImageId = host.ImageId,
                Rating = host.Rating,
                NickName = host.NickName,
                Role = host.Role
            }
        };
    }
}