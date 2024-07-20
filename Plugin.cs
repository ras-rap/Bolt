using System.Collections.Generic;
using System.Net;
using _scripts._multiplayer._data_objects._to_server;
using BepInEx;
using Chat;
using HarmonyLib;
using UnityEngine;
using _scripts._multiplayer._controller;
using _scripts._multiplayer._data_objects._from_server;
using _scripts._multiplayer._controller._game;
using SappNetwork;
using SappUnityUtils.ScriptableObjects;
using System;
using System.Linq;

namespace Bolt

{
    [BepInPlugin(Plugin.PLUGIN_GUID, Plugin.PLUGIN_NAME, Plugin.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.ras.Bolt";
        public const string PLUGIN_NAME = "Bolt";
        public const string PLUGIN_VERSION = "1.0.0.0";

        public static NetworkConfiguration serverConfig = ScriptableObjectSingleton<NetworkConfiguration>.Instance;

        internal static BepInEx.Logging.ManualLogSource LoggerInstance;
        public static Dictionary<int, PlayerInfo> ConnectedPlayers = new Dictionary<int, PlayerInfo>();

        private void Awake()
        {
            global::PluginConfig.Initialize(Config);
            // Plugin startup logic
            LoggerInstance = Logger;
            Logger.LogInfo($"Plugin {Plugin.PLUGIN_GUID} is loaded!");

            // Harmony patching
            var harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();
        }

        public static PlayerInfo GetPlayerInfo(int playerID)
        {
            GameController gameController = FindObjectOfType<GameController>();

            if (gameController != null)
            {
                PlayerInfo playerInfo = gameController.GetPlayerInfo((byte)playerID);

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
            static bool Prefix()
            {
                return false; // Returning false skips the execution of the original method.
            }

            static void Postfix(ChatManagerServer __instance, ChatToServerMsg chatToServerMsg)
            {
                string message = chatToServerMsg.Message;

                if (message.StartsWith(CommandHandler.Prefix))
                    CommandHandler.RunCommand(chatToServerMsg);
                else
                {
                    PlayerInfo playerInfo = Plugin.GetPlayerInfo(chatToServerMsg.PlayerID);
                    if (PluginConfig.MutedPlayers.TryGetValue(playerInfo.CSteamID, out DateTime expirationDate))
                    {
                        Plugin.LoggerInstance.LogInfo($"Muted player {{{playerInfo.PlayerName}}} tried to send a message.");
                        if (expirationDate < DateTime.UtcNow)
                        {
                            Plugin.LoggerInstance.LogInfo($"{playerInfo.PlayerName}'s mute has expired.");

                            PluginConfig.MutedPlayers.Remove(playerInfo.CSteamID);
                        }
                        else
                        {
                            Plugin.LoggerInstance.LogInfo($"{playerInfo.PlayerName}'s message has been blocked due to being muted.");
                            return;
                        }
                    }
                    // Send to all players using reflection
                    Plugin.LoggerInstance.LogInfo("Message sent: " + message);

                    Traverse.Create(__instance).Method("sendChatMessageToAllPlayers", [chatToServerMsg.PlayerID, message]).GetValue();
                }
            }
        }

        [HarmonyPatch(typeof(GameControllerServer))]
        class GameControllerServerPatch
        {
            [HarmonyPatch("playerConnected")]
            [HarmonyPrefix]
            static bool playerConnectedPatch(GameControllerServer __instance, ConnectMsg connectMsg, IPAddress ip, int port)
            {
                var traverse = Traverse.Create(__instance);

                PlayerInfo playerInfo = new PlayerInfo
                {
                    PlayerID = traverse.Method("getNextFreePlayerID").GetValue<byte>(),
                    IPAddress = ip,
                    Port = port,
                    PlayerName = connectMsg.Playername,
                    CSteamID = connectMsg.CSteamID
                };

                if (connectMsg.ForcedSelfID != 255 && traverse.Method("isIDFree", connectMsg.ForcedSelfID).GetValue<bool>())
                {
                    Debug.Log("[Server] Client is forcing his own ID to be: " + connectMsg.ForcedSelfID.ToString());
                    playerInfo.PlayerID = connectMsg.ForcedSelfID;
                }
                else
                {
                    traverse.Field("_lastAssignedPlayerID").SetValue(playerInfo.PlayerID);
                }

                if (traverse.Method("isServerAlreadyFull").GetValue<bool>())
                {
                    playerInfo.PlayerID = byte.MaxValue;
                }

                ConnectAnswer connectAnswer = new ConnectAnswer();
                connectAnswer.PlayerID = playerInfo.PlayerID;

                if (playerInfo.PlayerID == 255)
                {
                    Debug.Log("Server is full. Rejecting new connection");
                    connectAnswer.ConnectSuccessfull = false;
                }
                else if (PluginConfig.BannedPlayers.Any(playerCSteamID => playerCSteamID == playerInfo.CSteamID))
                {
                    Plugin.LoggerInstance.LogInfo($"Banned player {{{playerInfo.PlayerName}}} tried to connect.");
                    connectAnswer.ConnectSuccessfull = false;
                }
                else
                {
                    Debug.Log("[Server] Player connected with id: " + playerInfo.PlayerID.ToString());
                    connectAnswer.ConnectSuccessfull = true;
                    traverse.Property("PlayerInfos").GetValue<System.Collections.Generic.List<PlayerInfo>>().Add(playerInfo);
                    traverse.Method("triggerPlayerConnectedEvent", playerInfo).GetValue();
                    traverse.Method("sendPlayerListInTime", 0.1f).GetValue();
                }

                traverse.Method("Send", true, playerInfo, connectAnswer).GetValue();
                
                // TODO display info command on join

                return false;
            }
        }

        [HarmonyPatch(typeof(HostManager))]
        class HostManagerPatch
        {
            [HarmonyPatch("setPlayerIsHost")]
            [HarmonyPrefix]
            static bool NoHostPatch(HostManager __instance)
            {
                Traverse.Create(__instance).Field("_cSteamIDHost").SetValue(ulong.MaxValue);
                return false;
            }
        }
    }
}