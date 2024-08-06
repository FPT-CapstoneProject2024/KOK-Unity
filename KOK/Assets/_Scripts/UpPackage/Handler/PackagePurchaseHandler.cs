using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using System.Collections.Specialized;
using System.Web;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class PackagePurchaseHandler : MonoBehaviour
    {
        [Header("Confirm Components")]
        [SerializeField] public GameObject ConfirmComponent;
        [SerializeField] public TMP_Text PackageName;
        [SerializeField] public TMP_Text PackageAmount;
        [SerializeField] public TMP_Text PackagePrice;
        [Header("Loading Component")]
        [SerializeField] public GameObject LoadingComponent;
        [Header("Notification Component")]
        [SerializeField] public GameObject PurchaseResultComponent;
        [SerializeField] public TMP_Text PurchaseResultMessage;

        private PurchasePackageParam purchasePackageParam;

        private void Start()
        {
            purchasePackageParam = null;
            gameObject.SetActive(false);
        }

        public void ShowPurchasePackageDialog(PurchasePackageParam param)
        {
            purchasePackageParam = param;
            // Update UI
            PackageName.text = param.PackageName;
            PackageAmount.text = $"{param.StarNumber.ToString()} UP";
            PackagePrice.text = $"{param.MoneyAmount.ToString("#,##0")} VND";
            gameObject.SetActive(true);
            DisplayConfirmPurchase();
        }

        public void OnCancelPurchasePackage()
        {
            purchasePackageParam = null;
            gameObject.SetActive(false);
        }

        public void OnConfirmPurchaseClick()
        {
            DisplayLoadingProcess();
            string accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            if (string.IsNullOrEmpty(accountId) || Guid.Parse(accountId) == Guid.Empty)
            {
                SetPurchaseResultMessage("Không tìm thấy ID của người dùng. Vui lòng đăng nhập lại!");
                return;
            }
            MoMoPaymentRequest paymentRequest = new MoMoPaymentRequest()
            {
                PackageId = purchasePackageParam.PackageId,
                MemberId = Guid.Parse(accountId),
            };
            ApiHelper.Instance.GetComponent<MoMoController>().CreatePackagePurchaseMoMoRequestCoroutine(paymentRequest, OnCreateMoMoPaymentSuccess, OnCreateMoMoPaymentError);
        }

        private void OnCreateMoMoPaymentSuccess(ResponseResult<MoMoCreatePaymentResponse> responseResult)
        {
            if (responseResult.Value == null || !responseResult.Result.HasValue || !((bool)responseResult.Result))
            {
                SetPurchaseResultMessage(responseResult.Message);
                DisplayPurchaseResult();
                return;
            }
            Debug.Log(responseResult.Value.GetDataAsString());
            string targetAppDeepLink = responseResult.Value.Deeplink;
            Application.OpenURL(targetAppDeepLink);
        }

        private void OnCreateMoMoPaymentError(ResponseResult<MoMoCreatePaymentResponse> responseResult)
        {
            if (responseResult == null)
            {
                SetPurchaseResultMessage("Tạo yêu cầu nạp UP thất bại. Vui lòng thử lại!");
                DisplayPurchaseResult();
                return;
            }
            SetPurchaseResultMessage(responseResult.Message);
            DisplayPurchaseResult();
        }

        public void DisplayConfirmPurchase()
        {
            SetPurchaseResultMessage(string.Empty);
            ConfirmComponent.SetActive(true);
            LoadingComponent.SetActive(false);
            PurchaseResultComponent.SetActive(false);
        }

        public void DisplayLoadingProcess()
        {
            LoadingComponent.SetActive(true);
            ConfirmComponent.SetActive(false);
            PurchaseResultComponent.SetActive(false);
        }

        public void DisplayPurchaseResult()
        {
            PurchaseResultComponent.SetActive(true);
            ConfirmComponent.SetActive(false);
            LoadingComponent.SetActive(false);
        }

        private void SetPurchaseResultMessage(string message)
        {
            PurchaseResultMessage.text = message;
        }

        private void SubscribeToEvent()
        {
#if UNITY_ANDROID
            Debug.LogWarning("Subscribe to 'deepLinkActivated' event");
            Application.deepLinkActivated += OnDeepLinkActivated;
#endif
        }

        private void OnDeepLinkActivated(string url)
        {
            Debug.Log($"DeepLink Activated with URL: {url}");
            string deepLinkUrl = url;
            Uri uri = new Uri(deepLinkUrl);
            NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);
            string resultCodeString = queryParams["resultCode"];
            if (string.IsNullOrEmpty(resultCodeString))
            {
                Debug.LogWarning("Failed to get MoMo transaction result code!");
                SetPurchaseResultMessage("Không lấy được kết quả giao dịch MoMo. Vui lòng thử lại!");
                DisplayPurchaseResult();
                return;
            }
            int resultCode = int.Parse(resultCodeString);

            // Transaction canceled by user
            if (resultCode == 1006)
            {
                SetPurchaseResultMessage("Nạp UP không thành công. Người dùng đã hủy giao dịch!");
                DisplayPurchaseResult();
            }
            // Transaction failed because of insufficient user's balance
            else if (resultCode == 1001)
            {
                SetPurchaseResultMessage("Nạp UP không thành công. Số dư tài khoản MoMo của người dùng không đủ!");
                DisplayPurchaseResult();
            }
            // Transaction process successful
            else if (resultCode == 0 || resultCode == 9000)
            {
                var upBalanceHandler = FindAnyObjectByType<UpBalanceHandler>();
                if (upBalanceHandler != null) 
                {
                    upBalanceHandler.ReloadUserUpBalance();
                }
                SetPurchaseResultMessage("Giao dịch nạp UP đã được xử lý. Người dùng đã nạp UP thành công!");
                DisplayPurchaseResult();
            }
            else
            {
                SetPurchaseResultMessage("Có lỗi xảy ra trong quá trình xử lý giao dịch. Vui lòng liên hệ với Admin để được hỗ trợ!");
                DisplayPurchaseResult();
            }
        }

        private void UnsubscribeToEvent()
        {
#if UNITY_ANDROID
            Debug.LogWarning("Unsubscribe to 'deepLinkActivated' event");
            Application.deepLinkActivated -= OnDeepLinkActivated;
#endif
        }

        private void OnEnable()
        {
            SubscribeToEvent();
        }

        private void OnDisable()
        {
            UnsubscribeToEvent();
        }
    }
}
