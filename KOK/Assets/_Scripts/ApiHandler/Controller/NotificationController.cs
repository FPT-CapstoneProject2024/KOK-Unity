using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KOK.ApiHandler.Controller
{
    public class NotificationController : MonoBehaviour
    {
        private string notificationResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Notification_Resource;

        private void Start()
        {
            notificationResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Notification_Resource;
        }

        public void GetMemberUnreadNotifications(Guid memberId,
            Action<ResponseResult<List<NotificationResponse>>> onSuccess,
            Action<ResponseResult<List<NotificationResponse>>> onError)
        {
            ApiHelper.Instance.GetCoroutine(notificationResourceUrl + $"/unread/{memberId.ToString()}",
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<List<NotificationResponse>>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<List<NotificationResponse>>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void UpdateNotificationStatus(int notificationId,
            NotificationStatusUpdateRequest updateRequest,
            Action<ResponseResult<NotificationResponse>> onSuccess,
            Action<ResponseResult<NotificationResponse>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(updateRequest);
            ApiHelper.Instance.PostCoroutine(notificationResourceUrl + $"/{notificationId}/update", jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<NotificationResponse>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<NotificationResponse>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void UpdateUnreadNotificationsToRead(Guid memberId,
            Action<ResponseResult<bool>> onSuccess,
            Action<ResponseResult<bool>> onError)
        {
            ApiHelper.Instance.PostCoroutine(notificationResourceUrl + $"/unread/{memberId.ToString()}/read-all", string.Empty,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<bool>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<bool>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void GetMemberReadAndUnreadNotifications(Guid memberId,
            Action<ResponseResult<List<NotificationResponse>>> onSuccess,
            Action<ResponseResult<List<NotificationResponse>>> onError)
        {
            ApiHelper.Instance.GetCoroutine(notificationResourceUrl + $"/read-and-unread/{memberId.ToString()}",
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<List<NotificationResponse>>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<List<NotificationResponse>>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void UpdateReadNotificationsToDelete(Guid memberId,
            Action<ResponseResult<bool>> onSuccess,
            Action<ResponseResult<bool>> onError)
        {
            ApiHelper.Instance.PostCoroutine(notificationResourceUrl + $"/read/{memberId.ToString()}/delete-all", string.Empty,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<bool>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<bool>>(errorValue);
                    onError?.Invoke(result);
                });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
