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
                "\n<b>This server is running <color=#5764f0>Bolt\n",
                $"To get a list of the commands available type <b>{CommandHandler.Prefix}help</b>.",
                $"<b>Bolt version</b>: {Plugin.PLUGIN_VERSION}",
                "<b>Bolt developers</b>: Ras_rap, SimPleased"
                ]);
            
        }
    }
}
