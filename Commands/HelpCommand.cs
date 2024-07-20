using _scripts._multiplayer._controller;
using SappConsoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bolt.Commands
{
    public struct HelpCommand : ICommand
    {
        public string Name => "help";

        public string Description => "Helps you with the commands.";

        public string[] Parameters => ["pageNumber/commandName"];

        public int PermissionLevel => 0;

        private const int commandsPerPage = 6;

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length == 1)
            {
                if (int.TryParse(args[0], out int pageNumber))
                    return GetHelpPage(playerInfo, pageNumber);
                else if (CommandHandler.Commands.TryGetValue(args[0].ToLower(), out ICommand command))
                    return string.Join("\n", [
                        $"{command.Name}: {GetParameters(command)}",
                        $"Description: {command.Description}"
                        ]);
                else return $"Could not find command: {args[0]}";
            }
            else if (args.Length > 1)
            {
                CommandHandler.RunCommand(playerInfo, Name, [Name]);
                return "";
            }

            return GetHelpPage(playerInfo, 1);
        }

        public string GetHelpPage(PlayerInfo playerInfo, int pageNumber)
        {
            List<string> commands = CommandHandler.Commands.Values
                .Where(command =>
                    PluginConfig.PlayerPermissions.TryGetValue(playerInfo.CSteamID, out int perm) && perm >= command.PermissionLevel)
                .Select(command => $"{command.Name}: {GetParameters(command)}")
                .ToList();

            int totalPages = (int)Math.Ceiling((double)commands.Count / commandsPerPage);

            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            return $"Server Commands Page {pageNumber}/{totalPages}:\n" +
                string.Join("\n", commands
                    .Skip((pageNumber - 1) * commandsPerPage)
                    .Take(commandsPerPage));
        }

        private static string GetParameters(ICommand command)
            => command.Parameters.Length > 0 ? string.Join(" ", command.Parameters.Select(s => $"{{{s}}}")) : "No parameters";
    }
}
