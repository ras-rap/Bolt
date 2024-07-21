using _scripts._multiplayer._controller;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct ClearChatCommand : ICommand
    {
        public string Name => "clear";

        public string Description => "Clears the chat for you.";

        public string[] Parameters => [];

        public int PermissionLevel => 0;

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            return string.Concat(Enumerable.Repeat(" \n ", 80));
        }
    }
}
