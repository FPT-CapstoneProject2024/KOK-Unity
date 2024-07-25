using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace KOK.Assets._Scripts
{
    public class RPCMessage : NetworkBehaviour
    {
        public TMP_Text messageText;
        private string username;

        private void OnEnable()
        {
            messageText = GameObject.Find("ChatContent").GetComponent<TMP_Text>();
            StartCoroutine(SetUsername());
        }

        // Gimme the user name instead
        IEnumerator SetUsername()
        {
            yield return new WaitForSeconds(1f);
            username = GetComponent<PlayerNetworkBehavior>().PlayerName.ToString();
            if (username.IsNullOrEmpty())
            {
                StartCoroutine(SetUsername());
            }
        }

        public void CallMessageOnly1Player(string message)
        {
            messageText.text += $"{message}\n";
        }

        
        public void CallMessageRPC(string message)
        {
            RPC_SendMessage(message);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SendMessage(string message, RpcInfo rpcInfo = default)
        {
            messageText.text += $"{message}\n";
        }
    }
}
