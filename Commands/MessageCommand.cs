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
    public struct MessageCommand : ICommand
    {
        public string Name => "msg";

        public string Description => "Message a player secretly.";

        public string[] Parameters => ["player", "messaage"];

        public int PermissionLevel => 0;

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length < 2)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            if (PluginConfig.MutedPlayers.TryGetValue(playerInfo.CSteamID, out DateTime expirationDate))
            {
                if (expirationDate >= DateTime.UtcNow)
                    return "You are currently muted.";

                Plugin.LoggerInstance.LogInfo($"{playerInfo.PlayerName}'s mute has expired.");
                PluginConfig.MutedPlayers.Remove(playerInfo.CSteamID);
            }

            PlayerInfo recieverPlayerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
            if (recieverPlayerInfo == null)
            {
                return $"Could not find player: {args[0]}";
            }

            string message = string.Join(" ", args.Skip(1));

            Plugin.chatManager.SendChatMessageToPlayer(recieverPlayerInfo.PlayerID, $"From {playerInfo.PlayerName}: {message}");
            return $"To {recieverPlayerInfo.PlayerName}: {message}";
        }
    }
}
