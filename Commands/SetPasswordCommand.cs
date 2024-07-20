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
    public struct SetPasswordCommand : ICommand
    {
        public string Name => "setpassword";

        public string Description => "Sets the servers password.";

        public string[] Parameters => ["password"];

        public int PermissionLevel => PluginConfig.Ranks["Owner"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length > 0)
            {
                string newPassword = string.Join(" ", args);
                GameControllerServer.Instance.Password = newPassword;
                return $"Set the servers password to: {newPassword}";
            }

            GameControllerServer.Instance.Password = "";
            return $"Removed servers password.";
        }
    }
}
