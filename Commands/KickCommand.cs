using _scripts._multiplayer._controller;
using _scripts._multiplayer._controller._game;

namespace Bolt.Commands
{
    public struct KickCommand : ICommand
    {
        public string Name => "kick";

        public string Description => "Kicks a player off of the server.";

        public string[] Parameters => ["playerName"];

        public int PermissionLevel => PluginConfig.Ranks["Mod"];

        public string Run(PlayerInfo playerInfo, string[] args)
        {
            if (args.Length == 0)
            {
                CommandHandler.RunCommand(playerInfo, "help", [Name]);
                return "";
            }

            PlayerInfo kickPlayerInfo = Plugin.GetPlayers().Find(player => player.PlayerName == args[0]);
            if (kickPlayerInfo == null)
                return $"<color=red>Could not find a player: <b>{args[0]}</b>.";

            if (PluginConfig.PlayerPermissions[kickPlayerInfo.CSteamID] > PluginConfig.PlayerPermissions[playerInfo.CSteamID])
                return "<color=red>You cannot kick this player.";

            GameControllerServer gameController = Plugin.FindObjectOfType<GameControllerServer>();
            if (gameController == null)
                return $"<color=red>Could not find the servers GameController.";

            gameController.DisconnectPlayer(kickPlayerInfo.PlayerID);
            return $"<b>{kickPlayerInfo.PlayerName}</b> has been kicked.";
        }
    }
}
