using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class AlertManager : MonoBehaviour
    {
        [SerializeField] GameObject alertPanel;
        [SerializeField] TMP_Text alertText;
        [SerializeField] Color neutralColor;
        [SerializeField] Color successColor;
        [SerializeField] Color errorColor;
        [SerializeField] Button alertButton;
        [SerializeField] Button confirmButton;
        [SerializeField] Button cancelButton;

        public void Alert(string message)
        {
            alertPanel.SetActive(true);
            string hex = neutralColor.ToHexString().Substring(0, 6);
            alertText.text = $"<#{hex}>{message}</color>";
        }

        public void Alert(string message, bool isSuccess)
        {
            alertPanel.SetActive(true);
            alertText.text = "";
            string hex;
            if (isSuccess)
            {
                hex = successColor.ToHexString().Substring(0, 6);
            }
            else
            {
                hex = errorColor.ToHexString().Substring(0, 6);
            }
            alertText.text = $"<#{hex}>{message}</color>";
        }
        
        public void Alert(string message, string buttonText, bool isSuccess)
        {
            alertPanel.SetActive(true);
            alertText.text = "";
            string hex;
            if (isSuccess)
            {
                hex = successColor.ToHexString().Substring(0, 6);
            }
            else
            {
                hex = errorColor.ToHexString().Substring(0, 6);
            }
            alertText.text = $"<#{hex}>{message}</color>";
            alertButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        }

        public void Confirm(string message, Action onConfirm)
        {
            alertPanel.SetActive(true);
            alertText.text = message;
            confirmButton.onClick.AddListener(() => { onConfirm.Invoke(); });
        }

        public void Close()
        {
            alertPanel.SetActive(false);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
