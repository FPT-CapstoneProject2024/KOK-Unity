using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class NotificationRedDotHandler : MonoBehaviour
    {
        [SerializeField] GameObject redDot;
        public static bool isHaveNewNoti = false;
        private void OnEnable()
        {
            if (isHaveNewNoti)
            {
                ShowNotiRedDot();
            }
            else
            {
                HideNotiRedDot();
            }
        }


        public void ShowNotiRedDot()
        {
            redDot.SetActive(true);
            isHaveNewNoti = true;
        }

        public void HideNotiRedDot()
        {
            redDot.SetActive(false);
            isHaveNewNoti = false;
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
