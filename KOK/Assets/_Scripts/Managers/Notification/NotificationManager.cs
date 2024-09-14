using KOK.ApiHandler.DTOModels;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK
{
    public class NotificationManager : MonoBehaviour
    {
        //private const string NotificationHubUrl = "https://localhost:7017/notificationHub";
        private const string NotificationHubUrl = "https://kok-api.azurewebsites.net/notificationHub";

        private const string NotificationSendingMethodName = "PushNotification";
        private const string OnClientConnectedMethodName = "NotificationHubConnected";

        private HubConnection hubConnection;

        private bool isConnected = false;
        private const int RetryIntervalSeconds = 5;
        private const int MaxRetries = 10;
        private int retryCount = 0;

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
        }

        private void HandleInitialConnectingLogic(string message)
        {
            Debug.Log($"Connected to SignalR: {message}");
        } 
    }
}
