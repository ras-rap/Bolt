using _scripts._multiplayer._controller;
using _scripts._multiplayer._data_objects._to_server;
using Bolt.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bolt
{
    public class CommandHandler
    {
        public static string Prefix = "!";
        public static Dictionary<string, ICommand> Commands { get; private set; } = new();

        public static bool AddCommand(ICommand command)
        {
            Plugin.LoggerInstance.LogInfo($"Trying to add command {command.Name}");
            if (!Commands.ContainsKey(command.Name.ToLower()))
            {
                Plugin.LoggerInstance.LogInfo($"Adding {command.Name}");
                Commands.Add(command.Name.ToLower(), command);
                return true;
            }

            return false;
        }

        public static bool RunCommand(PlayerInfo playerInfo, string commandName, string[] args = null)
        {
            if (Commands.TryGetValue(commandName, out ICommand command))
            {
                int playerPermission = 0;
                if (PluginConfig.PlayerPermissions.TryGetValue(playerInfo.CSteamID, out int foundPlayerPermission))
                    playerPermission = foundPlayerPermission;

                if (playerPermission < command.PermissionLevel)
                {
                    Plugin.chatManager.SendChatMessageToPlayer(playerInfo.PlayerID, $"You do not have enough privileges to run {command.Name}.");
                    return false;
                }
                Plugin.LoggerInstance.LogInfo($"{playerInfo.PlayerName} has run {command.Name}");
                Plugin.chatManager.SendChatMessageToPlayer(playerInfo.PlayerID, command.Run(playerInfo, args));
                return true;
            }

            Plugin.chatManager.SendChatMessageToPlayer(playerInfo.PlayerID, $"Could not find the command: {{{commandName}}}");
            return false;
        }

        public static bool RunCommand(ChatToServerMsg messagePacket)
        {
            if (!messagePacket.Message.StartsWith(Prefix))
                return false;

            string[] args = messagePacket.Message.Substring(Prefix.Length).Split(' ');

            PlayerInfo playerInfo = Plugin.GetPlayerInfo(messagePacket.PlayerID);

            if (playerInfo == null)
                return false;

            return RunCommand(playerInfo, args[0].ToLower(), args.Skip(1).ToArray());
        }

        public static void Initialize()
        {
            Plugin.LoggerInstance.LogInfo("Command Handler Initializing");

            AddCommand(new HelpCommand());
            AddCommand(new InfoCommand());
            AddCommand(new PlayerInfoCommand());
            AddCommand(new PlayersCommand());
            AddCommand(new MessageCommand());
            AddCommand(new ClearChatCommand());

            AddCommand(new PrefixCommand());
            AddCommand(new SetServerNameCommand());
            AddCommand(new SetPasswordCommand());
            AddCommand(new SetMaxPlayersCommand());
            AddCommand(new SetMapCommand());
            //AddCommand(new SetMaxPartsCommand());
            //AddCommand(new SetDamageFactorCommand());

            AddCommand(new VoteKickCommand());
            AddCommand(new KickCommand());
            AddCommand(new MuteCommand());
            AddCommand(new UnmuteCommand());
            // These are very broken so have been removed for now
            AddCommand(new BanCommand());
            //AddCommand(new UnbanCommand());

            Plugin.LoggerInstance.LogInfo("Finished initializing");
        }
    }
}