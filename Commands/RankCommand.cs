using _scripts._multiplayer._controller;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct RankCommand : ICommand
    {
        public string Name => "rank";

        public string Description => "Gives a player a rank.";

        public string[] Parameters => ["playerName", "rankName"];

        public int PermissionLevel => PluginConfig.Ranks["Admin"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 2)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return null;
            }

            PlayerInfo rankPlayerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);

            if (playerInfo == null)
                return $"<color=red>Could not find a player: <b>{args[0]}</b>.";

            if (!PluginConfig.Ranks.TryGetValue(args[1], out int permissionLevel))
                return $"<color=red>Could not find rank: <b>{args[1]}</b>.";


            if (PluginConfig.PlayerPermissions[playerInfo.CSteamID] <= permissionLevel)
                return "<color=red>You dont have enough permissions to give out this rank.";

            if (PluginConfig.PlayerPermissions.TryGetValue(rankPlayerInfo.CSteamID, out int oldPermissions))
            {
                if (PluginConfig.PlayerPermissions[playerInfo.CSteamID] <= oldPermissions)
                    return $"<color=red>You do not have permissions to change the rank of <b>{args[0]}</b>.";

                PluginConfig.PlayerPermissions[rankPlayerInfo.CSteamID] = PluginConfig.Ranks[args[1]];
            }
            else
                PluginConfig.PlayerPermissions.Add(rankPlayerInfo.CSteamID, PluginConfig.Ranks[args[1]]);

            Plugin.chatManager.SendChatMessageToPlayer(rankPlayerInfo.PlayerID, $"You now have the rank <b>{args[1]}</b>.");
            return $"Gave the rank <b>{args[1]}</b> to <b>{args[0]}</b>.";
        }
    }
}
