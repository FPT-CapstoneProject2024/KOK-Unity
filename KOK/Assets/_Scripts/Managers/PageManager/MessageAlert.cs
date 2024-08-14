using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace KOK
{
    public class MessageAlert : MonoBehaviour
    {
        [SerializeField] GameObject messagePanel;
        [SerializeField] TMP_Text messageText;
        [SerializeField] Color neutralColor;
        [SerializeField] Color successColor;
        [SerializeField] Color errorColor;
        public void Alert(string message)
        {
            messagePanel.SetActive(true);
            messageText.color = neutralColor;
            messageText.text = message;
        }

        public void Alert(string message, bool isSuccess)
        {
            messagePanel.SetActive(true);
            string hex;
            if (isSuccess)
            {
                hex = successColor.ToHexString().Substring(0, 6);
            }
            else
            {
                hex = errorColor.ToHexString().Substring(0, 6);
            }
            messageText.text = $"<#{hex}>{message}</color>";
        }

        public void Close()
        {
            messagePanel.SetActive(false);
        }
    }
}
