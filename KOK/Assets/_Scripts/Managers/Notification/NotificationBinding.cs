using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KOK
{
    public class NotificationBinding : MonoBehaviour
    {
        [SerializeField] Image notiImage;
        [SerializeField] TMP_Text notiContent;
        [SerializeField] TMP_Text notiDateTime;

        [SerializeField] public Image panelBackground;
        [SerializeField] Color unreadColor;
        [SerializeField] Color readColor;

        NotificationResponse notificationResponse;
        NotificationHandler notificationHandler;
        SystemNavigation systemNavigation;
        public void Init(NotificationResponse notificationResponse, NotificationHandler notificationHandler, SystemNavigation systemNavigation)
        {
            this.notificationResponse = notificationResponse;
            this.notificationHandler = notificationHandler;
            this.systemNavigation = systemNavigation;

            //Set image here

            notiContent.text = notificationResponse.Description;
            notiDateTime.text = notificationResponse.CreateDate.ToString("hh:mm dd/MM/yyyy");
            if (notificationResponse.Status.Equals(NotificationStatus.UNREAD))
            {
                panelBackground.color = unreadColor;
            } else if (notificationResponse.Status.Equals(NotificationStatus.READ))
            {
                panelBackground.color = readColor;
            }
        }

        public void OnClick()
        {
            MarkAsRead();
            if (notificationResponse.NotificationType.Equals(NotificationType.TRANSACTION_NOTI))
            {
                //Jump to in-app transaction page
                systemNavigation.ToTransactionScene();
            }
            else if (notificationResponse.NotificationType.Equals(NotificationType.FRIEND_REQUEST)) { }

        }

        private void MarkAsRead()
        {
            if (notificationResponse.Status.Equals(NotificationStatus.READ)) return;
            ApiHelper.Instance.GetComponent<NotificationController>()
            .UpdateUnreadNotificationsToRead(
                Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                (isSuccess) => { },
                (isSuccess) => { Debug.LogError("Error at mark at read!"); }
            );
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
