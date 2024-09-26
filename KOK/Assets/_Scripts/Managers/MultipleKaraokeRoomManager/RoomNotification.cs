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
        private const string neutralColorHex = "#FFFFFF";
        private const string successColorHex= "#50FF54";
        private const string errorColorHex= "#F76464";

        private void Start()
        {
        }

        public void ShowNoti(string content)
        {
            StopAllCoroutines();
            notiPanel.SetActive(true);
            notiText.text = $"<color={neutralColorHex}>{content}</color>";
            StartCoroutine(HideNotiPanel());
        }

        public void ShowNoti(string content, bool isSuccess)
        {
            StopAllCoroutines();
            notiPanel.SetActive(true);
            if (isSuccess ) { 
                notiText.text = $"<color={successColorHex}>{content}</color>";
            } 
            else
            {
                notiText.text = $"<color={errorColorHex}>{content}</color>";
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
