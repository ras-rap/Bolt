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
        public static Dictionary<int, PlayerInfo> ConnectedPlayers = new Dictionary<int, PlayerInfo>();

        private void Awake()
        {
            config.Initialize(Config);
            // Plugin startup logic
            LoggerInstance = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            // Harmony patching
            var harmony = new Harmony("com.ras.Bolt");
            harmony.PatchAll();
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
                return false; // Returning false skips the execution of the original method.
            }

            static void Postfix(ChatToServerMsg chatToServerMsg)
            {
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
                        sendChatMessageToAllPlayersMethod.Invoke(chatManager, [chatToServerMsg.PlayerID, message]);
                    }
                    else
                    {
                        Plugin.LoggerInstance.LogError("sendChatMessageToAllPlayers method not found.");
                    }
                }
            }
        }
    }
}