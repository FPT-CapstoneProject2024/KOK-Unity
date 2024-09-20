using KOK.ApiHandler.DTOModels;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

namespace KOK
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }
        //private const string NotificationHubUrl = "https://localhost:7017/notificationHub";
        private const string NotificationHubUrl = "https://kok-api.azurewebsites.net/notificationHub";

        private const string NotificationSendingMethodName = "PushNotification";
        private const string OnClientConnectedMethodName = "NotificationHubConnected";

        private HubConnection hubConnection;

        private bool isConnected = false;
        private const int RetryIntervalSeconds = 5;
        private const int MaxRetries = 10;
        private int retryCount = 0;

        private NotificationResponse notification = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            StartCoroutine(TriggerNotification());
        }
        private async void Start()
        {
            await ConnectToNotificationHub();
            
        }

        public async Task ConnectToNotificationHub()
        {
            string userId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);

            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogWarning("User has not logged in. Cancel trying to connect to notification hub.");
                return;
            }

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{NotificationHubUrl}?userId={userId}")
                .Build();

            // Handle logic when connected
            hubConnection.On<string>(OnClientConnectedMethodName, HandleInitialConnectingLogic);

            // Handle logic when receiving notification
            hubConnection.On<NotificationResponse>(NotificationSendingMethodName, HandleNotificationReceived);

            while (!isConnected && retryCount < MaxRetries)
            {
                try
                {
                    await hubConnection.StartAsync();
                    isConnected = true;
                    Debug.Log("Connected to the SignalR notification hub.");
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Debug.LogError($"Failed to connect to SignalR notification hub: {ex.Message}. Retrying in {RetryIntervalSeconds} seconds...");
                    await Task.Delay(RetryIntervalSeconds * 1000);
                }
            }

            if (isConnected)
            {
                Debug.Log("Successfully connected to the notification hub after retrying.");
            }
            else
            {
                Debug.LogError("Failed to connect to the notification hub after max retries (10).");
            }
        }

        public async Task DisconnectFromHub()
        {
            if (isConnected)
            {
                await hubConnection.StopAsync();
                isConnected = false;
                Debug.Log("Disconnected from the SignalR notification hub.");
            }
        }

        private void HandleNotificationReceived(NotificationResponse notification)
        {
            Debug.Log($"New notification with object data: {notification.NotificationId} - {notification.Description} - {notification.NotificationType.ToString()} - {notification.Status.ToString()} - {notification.CreateDate.ToString()} - {notification.AccountId.ToString()}");
            this.notification = notification;
        }

        IEnumerator TriggerNotification()
        {
            yield return new WaitForSeconds(1);
            //Debug.Log(notification);
            if (notification != null)
            {
                try
                {
                    //nếu ở trang khác thì enable cái red dot lên
                    Debug.Log(FindAnyObjectByType<NotificationRedDotHandler>());
                    var redDot = FindAnyObjectByType<NotificationRedDotHandler>();
                    Debug.Log(redDot + " enable red dot for notification! ");
                    if (redDot != null)
                    {

                        redDot.ShowNotiRedDot();
                    }

                    //nếu ở trang thông báo thì sẽ thêm cái noti lên đầu
                    NotificationHandler notificationHandler = FindAnyObjectByType<NotificationHandler>();
                    if (notificationHandler != null && notification != null)
                    {
                        notificationHandler.AddNewNotificationOnTop(notification);
                    }   

                    //nếu notiType là nạp tiền thì gọi upbalancehandler để reset
                    UpBalanceHandler upBalanceHandler = FindAnyObjectByType<UpBalanceHandler>();
                    if (upBalanceHandler != null)
                    {
                        upBalanceHandler.ReloadUserUpBalance();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
                notification = null;
            }
            StartCoroutine(TriggerNotification());
        }

        private void HandleInitialConnectingLogic(string message)
        {
            Debug.Log($"Connected to SignalR: {message}");
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
