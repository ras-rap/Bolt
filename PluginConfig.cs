using BepInEx.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;

public static class PluginConfigParser
{
    public static string Serialize<T>(List<T> list)
        => string.Join(",", list.Select(item => Convert.ToString(item, System.Globalization.CultureInfo.InvariantCulture)));

    public static List<T> Deserialize<T>(string str)
        => str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                  .Select(item => (T)Convert.ChangeType(item.Trim(), typeof(T), System.Globalization.CultureInfo.InvariantCulture))
                  .ToList();

    public static string Serialize<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        => string.Join(", ", dictionary.Select(kv => $"{kv.Key}: {kv.Value}"));

    public static Dictionary<TKey, TValue> Deserialize<TKey, TValue>(string str)
        => str.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
              .Select(part => part.Split(new[] { ": " }, 2, StringSplitOptions.RemoveEmptyEntries))
              .ToDictionary(
                  split => (TKey)Convert.ChangeType(split[0], typeof(TKey)),
                  split => (TValue)Convert.ChangeType(split[1], typeof(TValue))
              );
}

public static class PluginConfig
{
    public static ConfigEntry<string> PlayerPermissionsConfig;
    public static ConfigEntry<string> RanksConfig;
    public static ConfigEntry<string> BannedPlayersConfig;
    public static ConfigEntry<string> MutedPlayersConfig;

    public static Dictionary<ulong, int> PlayerPermissions = new();
    public static List<ulong> BannedPlayers = new();
    public static Dictionary<ulong, DateTime> MutedPlayers = new();
    public static Dictionary<string, int> Ranks = new();

    public static void Initialize(ConfigFile config)
    {
        PlayerPermissionsConfig = config.Bind("Permissions", "PlayerPermissions",
            PluginConfigParser.Serialize(new Dictionary<ulong, int> { { 76561198119046479, int.MaxValue }, { 76561199734402057, int.MaxValue } }),
            "The players with ranks and their corresponding permissions");

        RanksConfig = config.Bind("Permissions", "Ranks",
            PluginConfigParser.Serialize(new Dictionary<string, int> { { "Developer", int.MaxValue }, { "Owner", int.MaxValue - 1 }, { "Admin", 1000 }, { "Mod", 500 } }),
            "The ranks and their permissions");

        BannedPlayersConfig = config.Bind("Permissions", "BannedPlayers",
            PluginConfigParser.Serialize(new List<ulong> { }),
            "The banned players");

        MutedPlayersConfig = config.Bind("Permissions", "MutedPlayers",
            PluginConfigParser.Serialize(new Dictionary<ulong, string> { }),
            "The muted players and their mute expiration dates");

        // Deserialize the configs into the dictionaries
        if (!string.IsNullOrEmpty(PlayerPermissionsConfig.Value))
        {
            PlayerPermissions = PluginConfigParser.Deserialize<ulong, int>(PlayerPermissionsConfig.Value);
        }

        if (!string.IsNullOrEmpty(RanksConfig.Value))
        {
            Ranks = PluginConfigParser.Deserialize<string, int>(RanksConfig.Value);
        }

        if (!string.IsNullOrEmpty(BannedPlayersConfig.Value))
        {
            BannedPlayers = PluginConfigParser.Deserialize<ulong>(BannedPlayersConfig.Value);
        }

        if (!string.IsNullOrEmpty(MutedPlayersConfig.Value))
        {
            MutedPlayers = PluginConfigParser.Deserialize<ulong, DateTime>(MutedPlayersConfig.Value);
        }
    }


    public static void Save()
    {
        // Add your custom serialization logic here if needed
        PlayerPermissionsConfig.Value = PluginConfigParser.Serialize(PlayerPermissions);
        RanksConfig.Value = PluginConfigParser.Serialize(Ranks);
        BannedPlayersConfig.Value = PluginConfigParser.Serialize(BannedPlayers);
        MutedPlayersConfig.Value = PluginConfigParser.Serialize(MutedPlayers);
    }

    public static string GetRankForPlayer(ulong CSteamID) // Returns null when CSteamID is not found in PlayerPermissions
        => PlayerPermissions.TryGetValue(CSteamID, out int playerPermission) ? Ranks.FirstOrDefault(r => r.Value == playerPermission).Key : null;
}
