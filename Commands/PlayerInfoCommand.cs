using _scripts._multiplayer._controller;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct PlayerInfoCommand : ICommand
    {
        public string Name => "playerinfo";

        public string Description => "Gives information about yourself.";

        public string[] Parameters => [];

        public int PermissionLevel => 0;

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            int rankLevel = PluginConfig.PlayerPermissions[playerInfo.CSteamID];
            return string.Join("\n", [
                $"Player name: {playerInfo.PlayerName}",
                $"Player ID: {playerInfo.PlayerID}",
                $"Player rank: {PluginConfig.GetRankForPlayer(playerInfo.CSteamID)}",
                $"Player rank level: {rankLevel}"
                ]);
        }
    }
}
