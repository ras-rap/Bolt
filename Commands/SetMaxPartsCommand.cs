using _scripts._multiplayer._controller;
using HarmonyLib;
using SappConsoles;
using SappUnityUtils.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct SetMaxPartsCommand : ICommand
    {
        public string Name => "setmaxparts";

        public string Description => "Sets the maximum amount of parts.";

        public string[] Parameters => ["maxParts"];

        public int PermissionLevel => PluginConfig.Ranks["Owner"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            if (int.TryParse(args[0], out int maxParts))
            {
                Traverse.Create(ScriptableObjectSingleton<ServerSettingsConfiguration>.Instance).Field("defaultMaxAmountOfParts").SetValue(maxParts);
                return $"Set servers max player count to: <b>{maxParts}</b>";
            }

            return $"<color=red>The input <b>{args[0]}</b> was not a number.";
        }
    }
}
