using _scripts._multiplayer._controller;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct PrefixCommand : ICommand
    {
        public string Name => "prefix";

        public string Description => "Changes the servers command prefix.";

        public string[] Parameters => ["prefix"];

        public int PermissionLevel => PluginConfig.Ranks["Owner"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            CommandHandler.Prefix = args[0];

            return $"Prefix changed to: <b>{args[0]}</b>";
        }
    }
}
