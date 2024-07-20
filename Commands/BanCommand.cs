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
    public struct BanCommand : ICommand
    {
        public string Name => "ban";

        public string Description => "Bans a player from joining the server.";

        public string[] Parameters => ["playerName"];

        public int PermissionLevel => PluginConfig.Ranks["Mod"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            PlayerInfo banPlayerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
            if (banPlayerInfo == null)
            {
                return $"Could not find player: {args[0]}";
            }
            PluginConfig.MutedPlayers.Remove(banPlayerInfo.CSteamID);
            return $"You have banned {banPlayerInfo.PlayerName}.";
        }
    }
}
