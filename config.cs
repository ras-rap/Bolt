using BepInEx.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;

public static class CustomConfigParser
{
    public static string Serialize(Dictionary<string, int> dictionary)
    {
        return string.Join(", ", dictionary.Select(kv => $"{kv.Key}: {kv.Value}"));
    }

    public static Dictionary<string, int> Deserialize(string str)
    {
        return str.Split(new[] { ", " }, StringSplitOptions.None)
                  .Select(part => part.Split(new[] { ": " }, StringSplitOptions.None))
                  .ToDictionary(split => split[0], split => int.Parse(split[1]));
    }
}

public static class config
{
    public static ConfigEntry<string> PlayerPermissionsConfig;
    public static ConfigEntry<string> RanksConfig;

    public static Dictionary<string, int> PlayerPermissions = new Dictionary<string, int>();
    public static Dictionary<string, int> Ranks = new Dictionary<string, int>();

    public static void Initialize(ConfigFile config)
    {
        PlayerPermissionsConfig = config.Bind("Permissions", "PlayerPermissions", CustomConfigParser.Serialize(new Dictionary<string, int> { { "Ras_rap", int.MaxValue}, { "SimPleased", int.MaxValue } }), "The players with ranks and their corresponding permissions");
        RanksConfig = config.Bind("Permissions", "Ranks", CustomConfigParser.Serialize(new Dictionary<string, int> { { "Developer", int.MaxValue}, { "Owner", int.MaxValue - 1 }, { "Admin", 1000 }, { "Mod", 500 } }), "The ranks and their permissions");

        // Deserialize the configs into the dictionaries
        if (!string.IsNullOrEmpty(PlayerPermissionsConfig.Value))
        {
            // Add your custom deserialization logic here if needed
            PlayerPermissions = CustomConfigParser.Deserialize(PlayerPermissionsConfig.Value);
        }

        if (!string.IsNullOrEmpty(RanksConfig.Value))
        {
            Ranks = CustomConfigParser.Deserialize(RanksConfig.Value);
        }
    }

    public static void Save()
    {
        // Add your custom serialization logic here if needed
        // PlayerPermissionsConfig.Value = CustomConfigParser.Serialize(PlayerPermissions);
        RanksConfig.Value = CustomConfigParser.Serialize(Ranks);
    }

    public static string GetRankForPlayer(string playerName)
    {
        if (PlayerPermissions.TryGetValue(playerName, out int playerPermission))
        {
            foreach (var rank in Ranks)
            {
                if (rank.Value == playerPermission)
                {
                    return rank.Key;
                }
            }
        }

        return null; // Return null or a default value if no rank is found
    }
}
