using _scripts._multiplayer._controller;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct InfoCommand : ICommand
    {
        public string Name => "info";

        public string Description => "Tells you about the sever.";

        public string[] Parameters => [];

        public int PermissionLevel => 0;

        public string Run(PlayerInfo playerInfo, string[] args)
        {

            return string.Join("\n", [
                "This server is running Bolt",
                $"To get a list of the commands available type {CommandHandler.Prefix}help",
                $"Bolt version: {Plugin.PLUGIN_VERSION}",
                "Bolt developers: Ras_rap, SimPleased"
                ]);
            
        }
    }
}
