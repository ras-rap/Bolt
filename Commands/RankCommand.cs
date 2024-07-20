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
                return $"Could not find a player: {args[0]}";

            if (!PluginConfig.Ranks.TryGetValue(args[1], out int permissionLevel))
                return $"Could not find rank: {args[1]}";


            if (PluginConfig.PlayerPermissions[playerInfo.CSteamID] <= permissionLevel)
                return "You dont have enough permissions to give out this rank.";

            if (PluginConfig.PlayerPermissions.ContainsKey(playerInfo.CSteamID))
                PluginConfig.PlayerPermissions[rankPlayerInfo.CSteamID] = PluginConfig.Ranks[args[1]];
            else
                PluginConfig.PlayerPermissions.Add(rankPlayerInfo.CSteamID, PluginConfig.Ranks[args[1]]);


            return string.Join("\n", [
                $"Gave the rank {args[1]} to {args[0]}.",
                $"You now have the rank {args[1]}."
                ]);
        }
    }
}
