using Core.Models.Lobbies;
using Core.Models.Users;
using Core.Server.Database.Lobbies;
using Core.Server.Database.Users;
using Core.Server.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Server.Services.Lobbies;

public class LobbyService(ILobbyRepository repository)
{
    public LobbyDtoModel CreateLobby(LobbyDtoModel lobby, User? host)
    {
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
            },
            Users = new List<UserModel>()
        };
    }

    public Task RemoveOld() =>
        Task.Run(() => repository.Delete(l => l.CreatedAt <= DateTime.Now.AddHours(3)));

    public LobbyDtoModel Find(Guid id)
    {
        var lobby = repository.Find(
            l => l.Id == id,
            includes: query => query
                .Include(l => l.Host)
                .Include(l => l.Users));
        if (lobby == null) throw new Exception("Не удалось найти лобби");

        return new LobbyDtoModel
        {
            DisplayName = lobby.DisplayName,
            HostId = lobby.HostId,
            PlayersCount = lobby.PlayersCount,
            RatingChange = lobby.RatingChange,
            GamePackId = lobby.GamePackId,
            Password = lobby.Password,
            Conditions = lobby.Conditions,
            Id = lobby.Id,
            CreatedAt = lobby.CreatedAt,
            Host = new UserModel
            {
                Id = lobby.HostId,
                ImageId = lobby.Host.ImageId,
                Rating = lobby.Host.Rating,
                NickName = lobby.Host.NickName,
                Role = lobby.Host.Role
            },
            Users = lobby.Users.Select(u => new UserModel
            {
                Id = u.Id,
                ImageId = u.ImageId,
                Rating = u.Rating,
                NickName = u.NickName,
                Role = u.Role
            }).ToList()
        };
    }

    public IEnumerable<LobbyDtoModel> FindAll() =>
        repository.FindAll(query => query
                .Include(l => l.Host)
                .Include(l => l.Users))
            .OrderByDescending(l => l.CreatedAt)
            .Select(lobby => new LobbyDtoModel
            {
                DisplayName = lobby.DisplayName,
                HostId = lobby.HostId,
                PlayersCount = lobby.PlayersCount,
                RatingChange = lobby.RatingChange,
                GamePackId = lobby.GamePackId,
                Password = lobby.Password,
                Conditions = lobby.Conditions,
                Id = lobby.Id,
                CreatedAt = lobby.CreatedAt,
                Host = new UserModel
                {
                    Id = lobby.HostId,
                    ImageId = lobby.Host.ImageId,
                    Rating = lobby.Host.Rating,
                    NickName = lobby.Host.NickName,
                    Role = lobby.Host.Role
                },
                Users = lobby.Users.Select(u => new UserModel
                {
                    Id = u.Id,
                    ImageId = u.ImageId,
                    Rating = u.Rating,
                    NickName = u.NickName,
                    Role = u.Role
                }).ToList()
            });

    public void CheckPassword(string password, Guid lobbyId)
    {
        var lobby = repository.Find(l => l.Id == lobbyId);
        if (lobby == null) throw new ArgumentNullException(nameof(lobbyId));

        if (string.IsNullOrEmpty(lobby.Password) || lobby.Password != password) 
            throw new ArgumentNullException(nameof(password));
    }
}