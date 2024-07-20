using _scripts._multiplayer._controller;
using _scripts._multiplayer._controller._game;
using _scripts._multiplayer._data_objects._to_server;
using Chat;
using IslandsNS;
using ScriptableObjectsVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bolt
{
    public class CommandHandler
    {
        public static string Prefix = "!";
        private static ChatManagerServer chatManager = UnityEngine.Object.FindObjectOfType<ChatManagerServer>();
        private static Command[] Commands = [
            new Command(
                name: "help",
                run: (id, args) =>
                {
                    if (args.Length == 1)
                    {
                        string commandName = args[0].ToLower();
                        Command command = Commands.FirstOrDefault(command => command.Name == commandName);

                        if (command == null)
                        {
                            // TODO
                            return;
                        }

                        chatManager.SendChatMessageToPlayer(id, $"{command.Name}: {command.GetParameters()}");
                        chatManager.SendChatMessageToPlayer(id, $"Description: {command.Description}");

                        return;
                    }
                    else if (args.Length > 1)
                    {
                        // TODO
                    }

                    chatManager.SendChatMessageToPlayer(id, "Here are all the commands on this server:");
                    ulong playerName = Plugin.GetPlayerInfo(id).CSteamID;

                    foreach (Command command in Commands)
                    {

                        if (PluginConfig.PlayerPermissions[playerName] >= command.PermissionLevel)
                            chatManager.SendChatMessageToPlayer(id, $"{command.Name}: {command.GetParameters()}");
                    }
                },
                description: "Helps you with the commands.",
                parameters: ["commandName"]
            ), new Command(
                name: "info",
                run: (id, args) =>
                {
                    chatManager.SendChatMessageToPlayer(id, "This server is running Bolt");
                    chatManager.SendChatMessageToPlayer(id, $"Bolt version: {Plugin.PLUGIN_VERSION}");
                    chatManager.SendChatMessageToPlayer(id, $"To get a list of the commands available type {CommandHandler.Prefix}help");
                    chatManager.SendChatMessageToPlayer(id, "Bolt developers: Ras_rap, SimPleased");
                },
                description: "Tells you about the sever."
            ), new Command(
                name: "playerinfo",
                run: (id, args) =>
                {
                    PlayerInfo playerInfo = Plugin.GetPlayerInfo(id);
                    if (playerInfo != null)
                    {
                        chatManager.SendChatMessageToPlayer(id, "Player name: " + playerInfo.PlayerName);
                        chatManager.SendChatMessageToPlayer(id, "Player ID: " + playerInfo.PlayerID);
                        chatManager.SendChatMessageToPlayer(id, "Player rank: " + PluginConfig.GetRankForPlayer(playerInfo.CSteamID));
                        // Get the rank level
                        int rankLevel = PluginConfig.PlayerPermissions[playerInfo.CSteamID];
                        chatManager.SendChatMessageToPlayer(id, "Player rank level: " + rankLevel);
                    }
                    else
                    {
                        chatManager.SendChatMessageToPlayer(id, "Player not found.");
                    }
                },
                description: "Gives info about you."
            ), new Command(
                name: "players",
                run: (id, args) =>
                {
                    List<PlayerInfo> playerInfos = Plugin.GetPlayers();
                    if (playerInfos != null)
                    {
                        foreach (PlayerInfo playerInfo in playerInfos)
                        {
                            chatManager.SendChatMessageToPlayer(id, "Player name: " + playerInfo.PlayerName + " ID: " + playerInfo.PlayerID);
                        }
                    }
                    else
                    {
                        chatManager.SendChatMessageToPlayer(id, "No players found.");
                    }
                },
                description: "Gives you a list of all players on the server."
            ), new Command(
                name: "clear",
                run: (id, args) =>
                {
                    for (int i = 0; i < 20; i++)
                    {
                        chatManager.SendChatMessageToPlayer(id, " ");
                    }
                },
                description: "Clears the chat for you."
            ), new Command(
                name: "kick",
                run: (id, args) =>
                {
                    PlayerInfo playerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
                    if (playerInfo == null)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find a player: {args[0]}");
                        return;
                    }
                    GameControllerServer gameController = Plugin.FindObjectOfType<GameControllerServer>();

                    if (PluginConfig.PlayerPermissions[playerInfo.CSteamID] > PluginConfig.PlayerPermissions[Plugin.GetPlayerInfo(id).CSteamID])
                    {
                        chatManager.SendChatMessageToPlayer(id, "You cannot kick this player.");
                        return;
                    }
                    
                    gameController.DisconnectPlayer((byte)playerInfo.PlayerID);
                },
                description: "Kicks an annoying player.",
                parameters: ["player"],
                permissionLevel: PluginConfig.Ranks["Mod"]
            ), new Command(
                name: "prefix",
                run: (id, args) =>
                {
                    if (args.Length != 1)   
                        return;

                    Prefix = args[0];

                    chatManager.SendChatMessageToPlayer(id, $"Prefix changed to: {Prefix}");
                },
                description: "Changes the prefix of the command",
                parameters: ["prefix"],
                permissionLevel: PluginConfig.Ranks["Owner"]
            ), new Command(
                name: "rank",
                run : (id, args) =>
                {
                    if (args.Length != 2)
                    {
                        // TODO
                        return;
                    }

                    PlayerInfo playerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);

                    if (playerInfo == null)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find a player: {args[0]}");
                        return;
                    }

                    if (!PluginConfig.Ranks.ContainsKey(args[1]))
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find rank: {args[1]}");
                        return;
                    }

                    chatManager.SendChatMessageToPlayer(id, $"Gave the rank {args[1]} to {args[0]}");
                    if (PluginConfig.PlayerPermissions.ContainsKey(playerInfo.CSteamID))
                    {
                        PluginConfig.PlayerPermissions[playerInfo.CSteamID] = PluginConfig.Ranks[args[1]];
                    }
                    else
                    {
                        PluginConfig.PlayerPermissions.Add(playerInfo.CSteamID, PluginConfig.Ranks[args[1]]);
                    }
                },
                description: "Sets the rank of a player.",
                parameters: ["playerName", "rankName"],
                permissionLevel: PluginConfig.Ranks["Owner"]
            ), new Command(
                name: "setservername",
                run: (byte id, string[] args) => {
                    if (args.Length < 1)
                    {
                        return;
                    }

                    string newName = string.Join(" ", args);
                    chatManager.SendChatMessageToPlayer(id, $"Set servers name to: {newName}");
                    Plugin.serverConfig.SteamServerNameVariable.Value = newName;
                },
                description: "Sets the server name.",
                parameters: ["serverName"],
                permissionLevel: PluginConfig.Ranks["Owner"]
            ), new Command(
                name: "setmaxplayers",
                run: (byte id, string[] args) => {
                    if (args.Length != 1)
                    {
                        return;
                    }

                    if (int.TryParse(args[0], out int max))
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Set servers max player count to: {max}");
                        Plugin.serverConfig.VarMaxPlayers.Value = max;
                        return;
                    }

                    chatManager.SendChatMessageToPlayer(id, $"The input {{{args[0]}}} was not a number.");
                },
                description: "Sets the maximum amount of players.",
                parameters: ["maxPlayers"],
                permissionLevel: PluginConfig.Ranks["Admin"]
            ), new Command(
                name: "setpassword",
                run: (byte id, string[] args) => {
                    if (args.Length > 0)
                    {
                        string newPassword = string.Join(" ", args);
                        chatManager.SendChatMessageToPlayer(id, $"Set the servers password to: {newPassword}");
                        GameControllerServer.Instance.Password = newPassword;
                    }
                    else
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Removed servers password.");
                        GameControllerServer.Instance.Password = "";
                    }
                },
                description: "Sets the server password.",
                parameters: ["password"],
                permissionLevel: PluginConfig.Ranks["Owner"]
            ), new Command(
                name: "mute",
                run: (byte id, string[] args) => {
                    if (args.Length != 4)
                    {
                        return;
                    }

                    PlayerInfo playerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
                    if (playerInfo == null)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find player: {args[0]}");
                        return;
                    }

                    DateTime expirationDate = DateTime.UtcNow;

                    if (byte.TryParse(args[1], out byte months))
                        expirationDate.AddMonths(months);

                    if (byte.TryParse(args[1], out byte days))
                        expirationDate.AddMonths(days);

                    if (byte.TryParse(args[1], out byte hours))
                        expirationDate.AddMonths(hours);

                    chatManager.SendChatMessageToPlayer(id, $"You have muted {playerInfo.PlayerName}.");
                    chatManager.SendChatMessageToPlayer(id, $"The expiration date for their mute is {String.Format("{0:f}", expirationDate)}.");

                    PluginConfig.MutedPlayers.Add(playerInfo.CSteamID, expirationDate);
                },
                description: "Mutes a player in the server.",
                parameters: ["playerName", "months", "days", "hours"],
                permissionLevel: PluginConfig.Ranks["Mod"]
            ), new Command(
                name: "unmute",
                run: (byte id, string[] args) => {
                    if (args.Length != 1)
                    {
                        return;
                    }

                    PlayerInfo playerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
                    if (playerInfo == null)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find player: {args[0]}");
                        return;
                    }

                    chatManager.SendChatMessageToPlayer(id, $"You have unmuted {playerInfo.PlayerName}.");

                    PluginConfig.MutedPlayers.Remove(playerInfo.CSteamID);
                },
                description: "Unmutes a player in the server.",
                parameters: ["playerName"],
                permissionLevel: PluginConfig.Ranks["Mod"]
            ), new Command(
                name: "ban",
                run: (byte id, string[] args) => {
                    if (args.Length != 1)
                    {
                        return;
                    }

                    PlayerInfo playerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
                    if (playerInfo == null)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find player: {args[0]}");
                        return;
                    }

                    chatManager.SendChatMessageToPlayer(id, $"You have banned {playerInfo.PlayerName}.");

                    PluginConfig.MutedPlayers.Remove(playerInfo.CSteamID);
                },
                description: "Bans a player from joining the server.",
                parameters: ["playerName"],
                permissionLevel: PluginConfig.Ranks["Mod"]
            ), new Command(
                name: "unban",
                run: (byte id, string[] args) => {
                    if (args.Length != 1)
                    {
                        return;
                    }

                    PlayerInfo playerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
                    if (playerInfo == null)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find player: {args[0]}");
                        return;
                    }

                    chatManager.SendChatMessageToPlayer(id, $"You have unbanned {playerInfo.PlayerName}.");

                    PluginConfig.BannedPlayers.Remove(playerInfo.CSteamID);
                },
                description: "Unbans a player in the server.",
                parameters: ["playerName"],
                permissionLevel: PluginConfig.Ranks["Mod"]
            ), new Command(
                name: "setmap",
                run: (byte id, string[] args) => {
                    if (args.Length != 1)
                    {
                        return;
                    }

                    bool notFound = true;
                    int mapID = 0;
                    foreach (string mapName in Enum.GetNames(typeof(ServerMaps)))
                    {
                        if (args[0].ToUpper() == mapName) {
                            mapID = (int)Enum.Parse(typeof(ServerMaps), mapName);
                            notFound = false;
                            break;
                        }
                    }

                    if (notFound)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find map {args[0]}.");
                        return;
                    }

                    IslandConfig newMap = IslandConfig.GetIslandConfigByUniqueID(mapID);
                    if (newMap == null)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find map {args[0]}.");
                        return;
                    }

                    IslandsSwitcher mapSwitcher = UnityEngine.Object.FindObjectOfType<IslandsSwitcher>();
                    if (mapSwitcher == null)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find the map switcher.");
                        return;
                    }

                    mapSwitcher.SwitchToIsland(newMap);
                    chatManager.SendChatMessageToPlayer(id, $"Map changed to {args[0].ToUpper()}.");
                },
                description: $"Changes the servers map.\nCurrent maps are:\n{string.Join("\n", Enum.GetNames(typeof(ServerMaps)))}",
                parameters: ["mapName"],
                permissionLevel: PluginConfig.Ranks["Owner"]
            ), new Command(
                name: "msg",
                run: (byte id, string[] args) => {
                    if (args.Length < 2)
                    {
                        return;
                    }

                    PlayerInfo playerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
                    if (playerInfo == null)
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find player: {args[0]}");
                        return;
                    }
                    string message = string.Join(" ", args.Skip(1));

                    chatManager.SendChatMessageToPlayer(id, $"To {playerInfo.PlayerName}: {message}");
                    chatManager.SendChatMessageToPlayer(playerInfo.PlayerID, $"From {Plugin.GetPlayerInfo(id).PlayerName}: {message}");
                },
                description: "Messages a player.",
                parameters: ["playerName", "message"]
            )
        ];

        public static void AddCommand(Command command)
            => Commands.Append(command);

        public static bool RunCommand(PlayerInfo playerInfo, string commandName, string[] args)
        {
            foreach (Command command in Commands)
            {
                if (command.Name == commandName)
                {
                    if (PluginConfig.PlayerPermissions[playerInfo.CSteamID] < command.PermissionLevel)
                    {
                        chatManager.SendChatMessageToPlayer(playerInfo.PlayerID, $"You do not have enough privileges to run {command.Name}.");
                        return false;
                    }
                    command.Run(playerInfo.PlayerID, args);
                    return true;
                }
            }

            chatManager.SendChatMessageToPlayer(playerInfo.PlayerID, $"Could not find the command: {{{commandName}}}");
            return false;
        }

        public static bool RunCommand(ChatToServerMsg messagePacket)
        {
            if (!messagePacket.Message.StartsWith(Prefix))
                return false;

            string[] args = messagePacket.Message.Substring(Prefix.Length).Split(' ');

            return RunCommand(Plugin.GetPlayerInfo(messagePacket.PlayerID), args[0].ToLower(), args.Skip(1).ToArray());
        }
    }
}