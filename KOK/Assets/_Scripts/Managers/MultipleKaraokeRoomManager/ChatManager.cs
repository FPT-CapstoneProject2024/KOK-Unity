using Fusion;
using KOK.Assets._Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace KOK
{
    public class ChatManager : MonoBehaviour
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
            yield return new WaitForSeconds(1f);
            runner = FindAnyObjectByType<NetworkRunner>();
            RPCMessage = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<RPCMessage>();
            playerName = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().PlayerName.ToString();
            if (playerName.IsNullOrEmpty())
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
                    SendMessageAll(playerName, chatInputField.text);
                    chatInputField.text = "";
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

        public void SendMessageAll(string message)
        {
            if (runner.ActivePlayers.Count() > 1)
            {
                RPC_SendMessage($"{message}");
            }
            else
            {
                CallMessageOnly1Player($"{message}");
            }
        }

        public void SendMessageAll(string username, string message)
        {
            if (runner.ActivePlayers.Count() > 1)
            {
                RPC_SendMessage($"{username}: {message}");
            }
            else
            {
                CallMessageOnly1Player($"{username}: {message}");
            }
        }
        public void CallMessageOnly1Player(string message)
        {
            messageTMP.text += $"{message}\n";
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public static void RPC_SendMessage(string message, RpcInfo rpcInfo = default)
        {
            messageTMPP.text += $"{message}\n";
        }
    }
}
