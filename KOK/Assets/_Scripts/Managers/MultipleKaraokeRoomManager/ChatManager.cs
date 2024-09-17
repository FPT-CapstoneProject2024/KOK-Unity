using Fusion;
using KOK.Assets._Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace KOK
{
    public class ChatManager : NetworkBehaviour
    {
        [SerializeField] TMP_InputField chatInputField;
        RPCMessage RPCMessage;
        NetworkRunner runner;
        [SerializeField] string playerName = "";
        [SerializeField] TMP_Text messageTMP;
        private static TMP_Text messageTMPP;
        bool allowEnter = false;

        public static ChatManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(SetUpChat());
            messageTMPP = messageTMP;

        }

        IEnumerator SetUpChat()
        {
            yield return new WaitForSeconds(2f);
            runner = NetworkRunner.Instances[0];
            RPCMessage = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<RPCMessage>();
            playerName = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().PlayerName.ToString();
            Debug.LogError(playerName + " | " + runner);
            if (playerName.IsNullOrEmpty() || runner == null)
            {
                StartCoroutine(SetUpChat());
            }
        }

        private void Update()
        {
            if (allowEnter)
            {
                if (!chatInputField.text.IsNullOrEmpty() && ((Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))))
                {
                    SendChat();
                    allowEnter = false;
                }
            }
            else
            {
                if (chatInputField.isFocused)
                {
                    allowEnter = true;
                }
            }

        }

        public void SendChat()
        {
            SendMessageAll(playerName, chatInputField.text.Trim());
            chatInputField.text = "";

        }

        public void SendMessageAll(string message)
        {
            //runner = NetworkRunner.Instances[0];
            if (runner.ActivePlayers.Count() > 1)
            {
                RPC_SendMessage(runner, $"{message}\n");
            }
            else
            {
                CallMessageOnly1Player($"{message}\n");
            }

        }

        public void SendMessageAll(string username, string message)
        {
            //runner = NetworkRunner.Instances[0];
            if (runner.ActivePlayers.Count() > 1)
            {
                RPC_SendMessage(runner, $"{username}: {message}\n");
            }
            else
            {
                CallMessageOnly1Player($"{username}: {message}\n");
            }

        }
        public void CallMessageOnly1Player(string message)
        {
            //Debug.LogError(message);
            messageTMP.text += $"{message}";
            runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().UpdateRoomLog($"[{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}] {message}");
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public static void RPC_SendMessage(NetworkRunner runner, string message)
        {
            messageTMPP.text += $"{message}";
            runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().UpdateRoomLog($"[{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}] {message}");
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
