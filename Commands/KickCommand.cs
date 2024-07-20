using _scripts._multiplayer._controller;
using _scripts._multiplayer._controller._game;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct KickCommand : ICommand
    {
        public string Name => "kick";

        public string Description => "Kicks a player off of the server.";

        public string[] Parameters => ["playerName"];

        public int PermissionLevel => PluginConfig.Ranks["Mod"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            PlayerInfo kickPlayerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
            if (playerInfo == null)
                return $"Could not find a player: {args[0]}";

            if (PluginConfig.PlayerPermissions[kickPlayerInfo.CSteamID] > PluginConfig.PlayerPermissions[playerInfo.CSteamID])
                return "You cannot kick this player.";

            GameControllerServer gameController = Plugin.FindObjectOfType<GameControllerServer>();
            gameController.DisconnectPlayer((byte)playerInfo.PlayerID);

            return $"{kickPlayerInfo.PlayerName} has been kicked.";
        }
    }
}
