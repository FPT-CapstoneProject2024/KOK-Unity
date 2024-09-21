using Fusion;
using Fusion.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

namespace KOK
{
    public class RoomNotification : MonoBehaviour
    {
        [SerializeField] GameObject notiPanel;
        [SerializeField] TMP_Text notiText;
        [SerializeField] Color neutralColor;
        [SerializeField] Color successColor;
        [SerializeField] Color errorColor;
        private string neutralColorHex;
        private string successColorHex;
        private string errorColorHex;

        private void Start()
        {
            neutralColorHex = neutralColor.ToHexString().Substring(0, 6);
            neutralColorHex = neutralColor.ToHexString().Substring(0, 6);
            neutralColorHex = neutralColor.ToHexString().Substring(0, 6);
        }

        public void ShowNoti(string content)
        {
            StopAllCoroutines();
            notiPanel.SetActive(true);
            notiText.text = $"<#{neutralColor}>{content}</color>".Replace("<#>", "");
            StartCoroutine(HideNotiPanel());
        }

        public void ShowNoti(string content, bool isSuccess)
        {
            StopAllCoroutines();
            notiPanel.SetActive(true);
            if (isSuccess ) { 
                notiText.text = $"<#{successColorHex}>{content}</color>";
            } 
            else
            {
                notiText.text = $"<#{errorColor}>{content}</color>";
            }
            StartCoroutine(HideNotiPanel());
        }



        IEnumerator HideNotiPanel()
        {
            yield return new WaitForSeconds(7);
            notiPanel.SetActive(false);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
