using _scripts._multiplayer._controller;
using _scripts._multiplayer._data_objects._to_server;
using Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Bolt
{
    public class CommandHandler
    {
        public static string Prefix = "!";
        private static ChatManagerServer chatManager = UnityEngine.Object.FindObjectOfType<ChatManagerServer>();
        private static Command[] Commands = {
            new Command(
                name: "help",
                run: (id, args) => 
                {
                    if (args.Length == 1)
                    {
                        string commandName = args[0].ToLower();
                        Command? command = Commands.FirstOrDefault(command => command.Name == commandName);

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
                    string playerName = Plugin.GetPlayerInfo(id)?.PlayerName;

                    foreach (Command command in Commands)
                    {
                        
                        if (config.PlayerPermissions[playerName] >= command.PermissionLevel)
                            chatManager.SendChatMessageToPlayer(id, $"{command.Name}: {command.GetParameters()}");
                    }
                },
                description: "Helps you with the commands.",
                parameters: new[] { "commandName" }
            ), new Command(
                name: "info",
                run: (id, args) =>
                {
                    chatManager.SendChatMessageToPlayer(id, "Bolt");
                    chatManager.SendChatMessageToPlayer(id, "Bolt version: " + PluginInfo.PLUGIN_VERSION);
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
                        chatManager.SendChatMessageToPlayer(id, "Player rank: " + config.GetRankForPlayer(playerInfo.PlayerName));
                        // Get the rank level
                        int rankLevel = config.PlayerPermissions[playerInfo.PlayerName];
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
                    chatManager.SendChatMessageToPlayer(id, "WIP");
                },
                description: "Kicks an annoying player.",
                parameters: new[]{ "player" },
                permissionLevel: config.Ranks["Mod"]
            ), new Command(
                name: "prefix",
                run: (id, args) =>
                {
                    if (args.Length != 1)
                        return;

                    Prefix = args[1];
                },
                description: "Changes the prefix of the command",
                parameters: new[]{ "prefix" },
                permissionLevel: config.Ranks["Owner"]
            ), new Command(
                name: "rank",
                run : (id, args) =>
                {
                    if (args.Length != 2)
                    {
                        // TODO
                    }

                    if (Plugin.GetPlayers().Any(player => player.PlayerName == args[0]))
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find a player: {args[0]}");
                        return;
                    }

                    if (config.Ranks.Any(rank => rank.Key == args[1]))
                    {
                        chatManager.SendChatMessageToPlayer(id, $"Could not find rank: {args[1]}");
                    }
                },
                description: "Sets the rank of a player.",
                parameters: new[]{ "playerName", "rankName" }
            )
        };

        public static void AddCommand(Command command)
            => Commands.Append(command);

        public static bool RunCommand(ChatToServerMsg messagePacket)
        {
            if (!messagePacket.Message.StartsWith(Prefix))
                return false;

            string[] args = messagePacket.Message.Substring(Prefix.Length).Split(' ');
            string commandName = args[0].ToLower();
            string playerName = Plugin.GetPlayerInfo(messagePacket.PlayerID)?.PlayerName;

            foreach (Command command in Commands)
            {
                if (command.Name == commandName && config.PlayerPermissions[playerName] >= command.PermissionLevel)
                {
                    command.Run(messagePacket.PlayerID, args.Skip(1).ToArray());
                    return true;
                }
            }

            chatManager.SendChatMessageToPlayer(messagePacket.PlayerID, $"Could not find the command: {{{commandName}}}");
            return false;
        }
    }
}
