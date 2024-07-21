using _scripts._multiplayer._controller;
using _scripts._multiplayer._controller._game;
using IslandsNS;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.Commands
{
    public struct SetMapCommand : ICommand
    {
        public string Name => "setmap";

        public string Description => $"Changes the servers map.\nCurrent maps are:\n{string.Join("\n", Enum.GetNames(typeof(ServerMaps)))}";

        public string[] Parameters => ["mapName"];

        public int PermissionLevel => PluginConfig.Ranks["Owner"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length != 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            bool notFound = true;
            int mapID = 0;
            foreach (string mapName in Enum.GetNames(typeof(ServerMaps)))
            {
                if (args[0].ToUpper() == mapName)
                {
                    mapID = (int)Enum.Parse(typeof(ServerMaps), mapName);
                    notFound = false;
                    break;
                }
            }

            if (notFound)
            {
                return $"<color=red>Could not find map {args[0]}.";
            }

            IslandConfig newMap = IslandConfig.GetIslandConfigByUniqueID(mapID);
            if (newMap == null)
            {
                return $"<color=red>Could not find map {args[0]}.";
            }

            IslandsSwitcher mapSwitcher = UnityEngine.Object.FindObjectOfType<IslandsSwitcher>();
            if (mapSwitcher == null)
            {
                return $"<color=red>Could not find the map switcher.";
            }

            mapSwitcher.SwitchToIsland(newMap);
            return $"Map changed to <b>{args[0].ToUpper()}</b>.";
        }
    }
}
