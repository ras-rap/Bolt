using _scripts._multiplayer._controller;
using _scripts._multiplayer._controller._game;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Bolt.Commands
{
    public struct VoteKickCommand : ICommand
    {
        public string Name => "votekick";

        public string Description => "Starts a vote kick.";

        public string[] Parameters => ["playerName"];

        public int PermissionLevel => 0;

        public static float kickThreashold = 0.75f;
        public static int kickDelayMs = 60000;
        public static PlayerInfo currentVote = null;
        public static List<ulong> voters = new List<ulong>();

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length == 0)
            {
                if (currentVote == null)
                    return "<color=red>No ongoing vote kick.";

                if (voters.Contains(playerInfo.CSteamID))
                    return "<color=red>You have already voted.";

                voters.Add(playerInfo.CSteamID);
                return "Your vote has gone through.";
            }
            if (args.Length > 1)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            if (currentVote != null)
                return string.Join("\n", [
                    $"<color=red>Vote in progress.",
                    "<color=red>Type command without the argument to vote yes."
                    ]);

                PlayerInfo votePlayerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
            if (votePlayerInfo == null)
                return $"<color=red>Could not find a player: <b>{args[0]}</b>.";

            if (PluginConfig.PlayerPermissions[votePlayerInfo.CSteamID] > 100)
                return "<color=red>You cannot start vote kick on staff.";

            currentVote = votePlayerInfo;
            voters.Add(votePlayerInfo.CSteamID);
            StartVoteKick(votePlayerInfo);
            return "";
        }

        public async Task StartVoteKick(PlayerInfo votePlayerInfo)
        {
            Plugin.SendMessageToAllPlayers(string.Join("\n", [
                $"Vote kick for {votePlayerInfo.PlayerName} has started.",
                $"To vote yes type \"<b>{CommandHandler.Prefix}votekick</b>\"."
                ]));

            await Task.Delay(kickDelayMs);

            GameControllerServer gameController = Plugin.FindObjectOfType<GameControllerServer>();
            if (gameController == null)
            {
                Plugin.LoggerInstance.LogError("Could not find the servers GameController.");
                return;
            }

            if ((float)gameController.CurrentPlayers / voters.Count >= kickThreashold)
            {
                if (!Plugin.GetPlayers().Contains(votePlayerInfo))
                {
                    Plugin.SendMessageToAllPlayers($"<b>{votePlayerInfo.PlayerName}</b> has already left.");
                    return;
                }
                gameController.DisconnectPlayer(votePlayerInfo.PlayerID);
                Plugin.SendMessageToAllPlayers($"<b>{votePlayerInfo.PlayerName}</b> has been kicked.");
            }
            else
            {
                Plugin.SendMessageToAllPlayers(string.Join("\n", [
                    $"<b>{votePlayerInfo.PlayerName}'s</b> vote did not result in a kick.",
                    $"The result came to <b>{voters.Count}/{Mathf.CeilToInt(kickThreashold * gameController.CurrentPlayers)}</b>."
                    ]));
            }

            votePlayerInfo = null;
            voters = new();
        }
    }
}
