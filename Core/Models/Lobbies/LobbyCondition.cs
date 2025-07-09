using System.ComponentModel;

namespace Core.Models.Lobbies;

public enum LobbyCondition
{
    [Description("с войс чатом")] WithMicrophone = 1,

    [Description("с рейтингом")] Rating
}