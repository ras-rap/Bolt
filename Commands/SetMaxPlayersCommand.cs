﻿using _scripts._multiplayer._controller;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct SetMaxPlayersCommand : ICommand
    {
        public string Name => "setmaxplayers";

        public string Description => "Sets the maximum amount of players that can join the server.";

        public string[] Parameters => ["maxPlayers"];

        public int PermissionLevel => PluginConfig.Ranks["Owner"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            if (int.TryParse(args[0], out int maxPlayers))
            {
                Plugin.serverConfig.VarMaxPlayers.Value = maxPlayers;
                return $"Set servers max player count to: <b>{maxPlayers}</b>";
            }

            return $"<color=red>The input <b>{args[0]}</b> was not a number.";
        }
    }
}
