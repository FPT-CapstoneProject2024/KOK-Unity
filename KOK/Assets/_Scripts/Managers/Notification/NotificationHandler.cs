﻿using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class NotificationHandler : MonoBehaviour
    {
        [SerializeField] Transform notificationViewPortContent;
        [SerializeField] GameObject notificationTemplatePrefab;
        [SerializeField] TMP_Text notificationMessage;
        [SerializeField] SystemNavigation systemNavigation;

        public AlertManager MessageAlertManager;
        public AlertManager ConfirmAlertManager;
        [SerializeField] LoadingManager loadingManager;
        List<NotificationResponse> notificationList = new();
        List<NotificationBinding> notificationBindingList = new();

        private void OnEnable()
        {
            ReloadNotification();
        }
        public void ReloadNotification()
        {
            foreach (Transform child in notificationViewPortContent)
            {
                GameObject.Destroy(child.gameObject);
            }
            loadingManager.EnableLoadingSymbol();
            loadingManager.EnableLoadingSymbol();
            ApiHelper.Instance.GetComponent<NotificationController>()
                .GetMemberReadAndUnreadNotifications(
                    Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    (notification) =>
                    {
                        loadingManager.DisableLoadingSymbol();
                        notificationList = notification.Value;
                        foreach (var noti in notificationList)
                        {
                            var notiObject = Instantiate(notificationTemplatePrefab, notificationViewPortContent);
                            notificationBindingList.Add(notiObject.GetComponent<NotificationBinding>());
                            notiObject.GetComponent<NotificationBinding>().Init(noti, this, systemNavigation);
                        }
                    },
                    (notification) => {
                        notificationMessage.text = "Chưa có thông báo nào";
                    }

                );

        }
    }
}