using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class SongPurchaseHandler : MonoBehaviour
    {
        [Header("Purchase Components")]
        [SerializeField] public GameObject PurchaseComponent;
        [SerializeField] public TMP_Text SongName;
        [SerializeField] public TMP_Text SongPrice;
        [Header("Notify Components")]
        [SerializeField] public GameObject NotifyComponent;
        [SerializeField] public TMP_Text NotifyMessage;
        [Header("Loading Components")]
        [SerializeField] public GameObject LoadingComponent;
        [Header("Up Balance")]
        [SerializeField] public UpBalanceHandler UpBalanceHandler;

        private BuySongParam SongParam;

        void Start()
        {
            SongParam = null;
            gameObject.SetActive(false);
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

        public void ShowPurchaseSongDialog(BuySongParam buySongParam)
        {
            Debug.LogError(buySongParam.ToString());
            SongParam = buySongParam;
            // Update UI
            SongName.text = SongParam.SongName;
            SongPrice.text = SongParam.Price.ToString("F0") + " UP";
            DisplayPurchase();
            gameObject.SetActive(true);
        }

        public void OnCancelPurchaseSong()
        {
            SongParam = null;
            gameObject.SetActive(false);
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
            ApiHelper.Instance.GetComponent<ShopController>().PurchaseSongCoroutine(purchaseRequest, OnPurchaseSongSuccess, OnPurchaseSongError);
        }

        private void OnPurchaseSongSuccess(ResponseResult<SongPurchaseResponse> response)
        {
            SetNotifyMessage(response.Message);
            DisplayNotification();
            SongParam.SongItem.GetComponent<SongItemBinding>().DisableBuySongButton();

            UpBalanceHandler.ReloadUserUpBalance();
        }

        private void OnPurchaseSongError(ResponseResult<SongPurchaseResponse> response)
        {
            SetNotifyMessage(response.Message);
            DisplayNotification();
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
