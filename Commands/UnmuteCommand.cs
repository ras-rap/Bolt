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
    public struct UnmuteCommand : ICommand
    {
        public string Name => "unmute";

        public string Description => "Unmutes a player in the server.";

        public string[] Parameters => ["playerName"];

        public int PermissionLevel => PluginConfig.Ranks["Mod"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            PlayerInfo unmutePlayerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
            if (unmutePlayerInfo == null)
            {
                return $"<color:red>Could not find player: {args[0]}";
            }
            PluginConfig.MutedPlayers.Remove(unmutePlayerInfo.CSteamID);
            return $"You have unmuted <b>{unmutePlayerInfo.PlayerName}</b>.";
        }
    }
}
