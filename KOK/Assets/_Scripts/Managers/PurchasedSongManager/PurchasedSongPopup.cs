using Fusion;
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
    public class PurchasedSongPopup : MonoBehaviour
    {
        public BuySongParam SongParam { get; set; }

        [Header("Purchase Components")]
        [SerializeField] public GameObject PurchaseComponent;
        [SerializeField] public TMP_Text SongName;
        [SerializeField] public TMP_Text SongPrice;
        [Header("Notify Components")]
        [SerializeField] public GameObject NotifyComponent;
        [SerializeField] public TMP_Text NotifyMessage;
        [Header("Loading Components")]
        [SerializeField] public GameObject LoadingComponent;
        int _roomType;

        public void InitParam(BuySongParam songParam, int roomType)
        {
            SongParam = songParam;
            SongName.text = songParam.SongName;
            SongPrice.text = songParam.Price.ToString();
            _roomType = roomType;
        }
        public void OnConfirmPurchaseSong()
        {
            DisplayLoading();
            Debug.Log("Confirm Purchase Song");
            string accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            if (string.IsNullOrEmpty(accountId))
            {
                SetNotifyMessage("Failed to purchase song - Player ID not found");
                return;
            }
            var purchaseRequest = new SongPurchaseRequest()
            {
                MemberId = Guid.Parse(accountId),
                SongId = SongParam.SongId,
            };
            ApiHelper.Instance.GetComponent<ShopController>().PurchaseSongCoroutine(purchaseRequest,
                (response) =>
                {
                    OnPurchaseSongSuccess(response);
                },
                (response) =>
                {
                    OnPurchaseSongError(response);
                });
        }

        public void OnCancelPurchaseSong()
        {
            Destroy(PurchaseComponent.transform.parent.gameObject);
        }

        public void OnCompletePurchasedSuccessConfirm()
        {
            SongParam = null;
            Destroy(gameObject);
        }

        private void OnPurchaseSongSuccess(ResponseResult<SongPurchaseResponse> response)
        {
            SetNotifyMessage(response.Message);
            DisplayNotification();
            if (_roomType == 0)
            {
                SinglePlayerManager singlePlayerManager = FindAnyObjectByType<SinglePlayerManager>();
                singlePlayerManager.ReloadSong();
            }
            else if (_roomType == 1)
            {
                var runner = NetworkRunner.Instances[0];
                runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().RefreshSearchSongUI();
            }
        }

        private void OnPurchaseSongError(ResponseResult<SongPurchaseResponse> response)
        {
            SetNotifyMessage(response.Message);
            DisplayNotification();
        }

        private void DisplayPurchase()
        {
            PurchaseComponent.SetActive(true);
            NotifyComponent.SetActive(false);
            LoadingComponent.SetActive(false);
        }

        private void DisplayNotification()
        {
            NotifyComponent.SetActive(true);
            PurchaseComponent.SetActive(false);
            LoadingComponent.SetActive(false);
        }

        private void DisplayLoading()
        {
            LoadingComponent.SetActive(true);
            PurchaseComponent.SetActive(false);
            NotifyComponent.SetActive(false);
        }

        private void SetNotifyMessage(string text)
        {
            NotifyMessage.text = text;
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
