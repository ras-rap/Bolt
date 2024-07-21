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
    public struct SetDamageFactorCommand : ICommand
    {
        public string Name => "setdamagefactor";

        public string Description => "Sets the damage factor of cars.";

        public string[] Parameters => ["damageFactor"];

        public int PermissionLevel => PluginConfig.Ranks["Owner"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            if (float.TryParse(args[0], out float damageFactor))
            {
                Traverse.Create(ScriptableObjectSingleton<ServerSettingsConfiguration>.Instance).Field("defaultDamageFactor").SetValue(damageFactor);
                return $"Set servers max player count to: <b>{damageFactor}</b>";
            }

            return $"<color=red>The input <b>{args[0]}</b> was not a number.";
        }
    }
}
