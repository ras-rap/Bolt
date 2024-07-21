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
    public struct MuteCommand : ICommand
    {
        public string Name => "mute";

        public string Description => "Mutes a player in the server.";

        public string[] Parameters => ["playerName", "days", "hours"];

        public int PermissionLevel => PluginConfig.Ranks["Mod"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 4)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            PlayerInfo mutePlayerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
            if (mutePlayerInfo == null)
            {
                return $"<color=red>Could not find player: {args[0]}";
            }

            DateTime expirationDate = DateTime.UtcNow;

            if (ushort.TryParse(args[1], out ushort days))
                expirationDate.AddMonths(days);

            if (byte.TryParse(args[1], out byte hours))
                expirationDate.AddMonths(hours);

            PluginConfig.MutedPlayers.Add(mutePlayerInfo.CSteamID, expirationDate);
            return string.Join("\n", [
                $"You have muted <b>{mutePlayerInfo.PlayerName}</b>.",
                $"The expiration date for their mute is <b>{String.Format("{0:f}", expirationDate)}</b>."
                ]);
        }
    }
}
