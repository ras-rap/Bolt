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
    public struct UnbanCommand : ICommand
    {
        public string Name => "unban";

        public string Description => "Unbans a player in the server.";

        public string[] Parameters => ["playerName"];

        public int PermissionLevel => PluginConfig.Ranks["Mod"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            PlayerInfo unbanPlayerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
            if (unbanPlayerInfo == null)
            {
                return $"Could not find player: {args[0]}";
            }
            PluginConfig.BannedPlayers.Remove(unbanPlayerInfo.CSteamID);
            return $"You have unbanned {unbanPlayerInfo.PlayerName}.";
        }
    }
}
