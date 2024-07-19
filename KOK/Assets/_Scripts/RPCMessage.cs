using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK.Assets._Scripts
{
    public class RPCMessage : NetworkBehaviour
    {
        public TMP_Text messageText;
        public TMP_InputField messageInput;
        public TMP_InputField usernameInput;
        private NetworkObject netobject;
        private string username;

        // Gimme the user name instead
        public void SetUsername()
        {
            username = usernameInput.text;
        }

        public void CallMessageRPC()
        {
            string message = messageInput.text;
            RPC_SendMessage(username, message);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SendMessage(string username, string message, RpcInfo rpcInfo = default)
        {
            messageText.text += $"{username}: {message}\n";
        }
    }
}
