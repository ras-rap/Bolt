using _scripts._multiplayer._controller;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct PlayersCommand : ICommand
    {
        public string Name => "players";

        public string Description => "Gives a list of the players online.";

        public string[] Parameters => [];

        public int PermissionLevel => 0;

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            List<PlayerInfo> playerInfos = Plugin.GetPlayers();
            if (playerInfos != null)
            {
                return string.Join("\n", playerInfos.Select(playerInfo =>
                    $"Player: <b>{playerInfo.PlayerName}</b> ({playerInfo.PlayerID})"));
            }
            else
            {
                return "<color=red>No players found.";
            }
        }
    }
}
