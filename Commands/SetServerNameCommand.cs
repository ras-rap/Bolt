using _scripts._multiplayer._controller;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct SetServerNameCommand : ICommand
    {
        public string Name => "setservername";

        public string Description => "Sets the servers name.";

        public string[] Parameters => ["serverName"];

        public int PermissionLevel => PluginConfig.Ranks["Owner"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length < 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            string newName = string.Join(" ", args);
            Plugin.serverConfig.SteamServerNameVariable.Value = newName;
            return $"Set servers name to: {newName}";
        }
    }
}
