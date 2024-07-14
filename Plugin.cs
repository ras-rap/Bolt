using System;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using _scripts._multiplayer._data_objects._to_server;
using BepInEx;
using Chat;
using HarmonyLib;
using UnityEngine;
using _scripts._multiplayer._controller;
using _scripts._multiplayer._data_objects._from_server;
using _scripts._multiplayer._controller._game;

namespace Bolt

{
    
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static BepInEx.Logging.ManualLogSource LoggerInstance;
        public static bool isClient;
        public static Dictionary<int, PlayerInfo> ConnectedPlayers = new Dictionary<int, PlayerInfo>();

        private void Awake()
        {
            config.Initialize(Config);
            // Plugin startup logic
            LoggerInstance = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            // Call IsClient method
            isClient = CallIsClientMethod();
            Logger.LogInfo($"IsClient method returned: {isClient}");

            // Harmony patching
            var harmony = new Harmony("com.ras.Bolt");
            harmony.PatchAll();
        }

        public static bool CallIsClientMethod()
        {
            try
            {
                // Get the type of the class
                Type gameType = Type.GetType("_scripts._multiplayer._controller._game");

                if (gameType == null)
                {
                    LoggerInstance.LogError("_scripts._multiplayer._controller._game type not found.");
                    return false;
                }

                // Get the MethodInfo for IsClient method
                MethodInfo isClientMethod = gameType.GetMethod("IsClient", BindingFlags.Public | BindingFlags.Instance);

                if (isClientMethod == null)
                {
                    LoggerInstance.LogError("IsClient method not found.");
                    return false;
                }

                // Create an instance of the class
                object gameInstance = Activator.CreateInstance(gameType);

                // Invoke the IsClient method
                bool result = (bool)isClientMethod.Invoke(gameInstance, null);
                return result;
            }
            catch (Exception ex)
            {
                LoggerInstance.LogError($"Error invoking IsClient method: {ex.Message}");
                return false;
            }
        }

        public static PlayerInfo GetPlayerInfo(int playerID)
        {
            GameController gameController = FindObjectOfType<GameController>();

            if (gameController != null)
            {
                // Call GetPlayerInfo method on the found GameController instance
                PlayerInfo playerInfo = Traverse.Create(gameController).Method("GetPlayerInfo", (byte)playerID).GetValue() as PlayerInfo;

                if (playerInfo != null)
                {
                    return playerInfo;
                }
                else
                {
                    LoggerInstance.LogError($"Player info not found for ID: {playerID}");
                }
            }
            else
            {
                LoggerInstance.LogError("GameController instance not found.");
            }
            return null;
        }

        public static List<PlayerInfo> GetPlayers()
        {
            GameController gameController = FindObjectOfType<GameController>();

            if (gameController != null)
            {
                // Access PlayerInfos field
                List<PlayerInfo> playerInfos = gameController.PlayerInfos;

                if (playerInfos != null && playerInfos.Count > 0)
                {
                    return playerInfos;
                }
                else
                {
                    Debug.LogError("PlayerInfos list is null or empty.");
                }
            }
            else
            {
                Debug.LogError("GameController instance not found.");
            }

            return null;
        }

        [HarmonyPatch(typeof(ChatManagerServer), "ReceiveChatToServerMsg")]
        public static class ChatManagerServerPatch
        {
            static bool Prefix(ChatToServerMsg chatToServerMsg)
            {
                // Check if the patch should run
                if (Plugin.isClient)
                {
                    Plugin.LoggerInstance.LogInfo("Skipping patch.");
                    return true; // Returning true allows the original method to run
                }

                Plugin.LoggerInstance.LogInfo("Skipping original method.");
                return false; // Returning false skips the execution of the original method.
            }

            static void Postfix(ChatToServerMsg chatToServerMsg)
            {
                if (Plugin.isClient)
                {
                    Plugin.LoggerInstance.LogInfo("Skipping patch.");
                    return; // Skip the patch if the value is false
                }

                Plugin.LoggerInstance.LogInfo("Patching method.");
                string message = chatToServerMsg.Message;
                var chatManager = UnityEngine.Object.FindObjectOfType<ChatManagerServer>();

                if (chatManager == null)
                {
                    Plugin.LoggerInstance.LogError("ChatManagerServer instance not found.");
                    return;
                }

                if (message.StartsWith(CommandHandler.Prefix))
                    CommandHandler.RunCommand(chatToServerMsg);
                else
                {
                    // Send to all players using reflection
                    Plugin.LoggerInstance.LogInfo("Message sent: " + message);

                    MethodInfo sendChatMessageToAllPlayersMethod = typeof(ChatManagerServer).GetMethod("sendChatMessageToAllPlayers", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (sendChatMessageToAllPlayersMethod != null)
                    {
                        sendChatMessageToAllPlayersMethod.Invoke(chatManager, new object[] { chatToServerMsg.PlayerID, message });
                    }
                    else
                    {
                        Plugin.LoggerInstance.LogError("sendChatMessageToAllPlayers method not found.");
                    }
                }
            }
        }
        [HarmonyPatch(typeof(GameControllerServer))]
        [HarmonyPatch("sendPlayerList")]
        public static class PlayerListPatch
        {
            static bool Prefix()
            {
                return false; // Prevent the original method from running
            }

            static void Postfix(GameControllerServer __instance)
            {
                PlayerList playerList = new PlayerList();
                playerList.PlayerIDs = new byte[__instance.PlayerInfos.Count];
                playerList.PlayerNames = new string[__instance.PlayerInfos.Count];
                playerList.CSteamIDs = new ulong[__instance.PlayerInfos.Count];
                playerList.Pings = new float[__instance.PlayerInfos.Count];

                for (int i = 0; i < __instance.PlayerInfos.Count; i++)
                {
                    playerList.PlayerIDs[i] = __instance.PlayerInfos[i].PlayerID;
                    if (config.GetRankForPlayer(__instance.PlayerInfos[i].PlayerName) != null)
                    {
                        playerList.PlayerNames[i] = config.GetRankForPlayer(__instance.PlayerInfos[i].PlayerName) + ": " + __instance.PlayerInfos[i].PlayerName;
                    }
                    else
                    {
                        playerList.PlayerNames[i] = __instance.PlayerInfos[i].PlayerName;
                    }
                        
                    playerList.CSteamIDs[i] = __instance.PlayerInfos[i].CSteamID;
                    playerList.Pings[i] = __instance.PlayerInfos[i].Ping;
                }

                // Assuming SendAll is a method in GameControllerServer
                __instance.SendAll(true, playerList);
            }
        }
    }
}