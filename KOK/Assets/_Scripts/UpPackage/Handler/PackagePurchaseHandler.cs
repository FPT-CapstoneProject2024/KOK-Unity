using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using QRCoder;
using System;
using System.Collections.Specialized;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KOK
{
    public class PackagePurchaseHandler : MonoBehaviour, IPointerClickHandler
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
        [SerializeField] public RawImage qrImage;

        private PurchasePackageParam purchasePackageParam;
        private Texture2D storeEncodedTexture;

        private void Start()
        {
            storeEncodedTexture = new Texture2D(256, 256);
            purchasePackageParam = null;
            gameObject.SetActive(false);
        }

        public void ShowPurchasePackageDialog(PurchasePackageParam param)
        {
            purchasePackageParam = param;
            // Update UI
            PackageName.text = param.PackageName;
            PackageAmount.text = $"{param.StarNumber} UP";
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
            //MoMoPaymentRequest paymentRequest = new MoMoPaymentRequest()
            //{
            //    PackageId = purchasePackageParam.PackageId,
            //    MemberId = Guid.Parse(accountId),
            //};
            PayOSPackagePurchaseRequest purchaseRequest = new PayOSPackagePurchaseRequest()
            {
                PackageId = purchasePackageParam.PackageId,
                MemberId = Guid.Parse(accountId),
            };
            //ApiHelper.Instance.GetComponent<MoMoController>().CreatePackagePurchaseMoMoRequestCoroutine(paymentRequest, OnCreateMoMoPaymentSuccess, OnCreateMoMoPaymentError);
            ApiHelper.Instance.GetComponent<UpPackageController>().PurchasePackagePayOSCoroutine(purchaseRequest, OnCreatePayOSPaymentSuccess, OnCreatePayOSPaymentError);
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

        private void OnCreatePayOSPaymentSuccess(ResponseResult<PayOSPackagePurchaseResponse> responseResult)
        {
            if (responseResult.Value == null || !responseResult.Result.HasValue || !((bool)responseResult.Result))
            {
                SetPurchaseResultMessage(responseResult.Message);
                DisplayPurchaseResult();
                return;
            }
            Debug.Log($"Link: {responseResult.Value.checkoutUrl}");

            string resultMessage = $"Tạo yêu cầu nạp UP thành công. \nQuét QR hoặc nhấn <color=#00BFFF><link=\"{responseResult.Value.checkoutUrl}\"><u>tại đây</u></link></color> để đến trang thanh toán.";
            SetPurchaseResultMessage(resultMessage);
            SetPayOSQrImage(responseResult.Value);
            DisplayPurchaseResult();
            //Debug.Log(responseResult.Value.GetDataAsString());
            //string targetAppDeepLink = responseResult.Value.Deeplink;
            //Application.OpenURL(targetAppDeepLink);
        }

        private void OnCreatePayOSPaymentError(ResponseResult<PayOSPackagePurchaseResponse> responseResult)
        {
            if (responseResult == null)
            {
                SetPurchaseResultMessage("Tạo yêu cầu nạp UP thất bại. Vui lòng thử lại!");
                SetPayOSQrImage(null);
                DisplayPurchaseResult();
                return;
            }
            SetPurchaseResultMessage(responseResult.Message);
            SetPayOSQrImage(null);
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
            //SubscribeToEvent();
        }

        private void OnDisable()
        {
            //UnsubscribeToEvent();
        }

        private void SetPayOSQrImage(PayOSPackagePurchaseResponse purchaseResponse)
        {
            qrImage.gameObject.SetActive(false);
            if (purchaseResponse == null)
            {
                return;
            }
            qrImage.gameObject.SetActive(true);
            var texture = EncodeTextToQRCode(purchaseResponse.qrCode);
            qrImage.texture = texture;
        }

        private Texture2D EncodeTextToQRCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            // Get raw pixel data from the QR code
            var qrCodeMatrix = qrCodeData.ModuleMatrix;

            // Create a texture based on the size of the QR code matrix
            int size = qrCodeMatrix.Count;
            Texture2D texture = new Texture2D(size, size);

            // Set each pixel in the texture based on the QR code matrix
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // If true, set the pixel to black, otherwise white
                    UnityEngine.Color color = qrCodeMatrix[y][x] ? UnityEngine.Color.black : UnityEngine.Color.white;
                    texture.SetPixel(x, y, color);
                }
            }

            // Apply the changes to the texture
            texture.filterMode = FilterMode.Point; // Ensures sharpness for pixelated images
            texture.Apply();

            return texture;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Get the index of the link that was clicked
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(PurchaseResultMessage, Input.mousePosition, Camera.main);

            if (linkIndex != -1)
            {
                // Extract the link ID from the text
                TMP_LinkInfo linkInfo = PurchaseResultMessage.textInfo.linkInfo[linkIndex];
                string linkId = linkInfo.GetLinkID();

                // Open the link in the browser
                Application.OpenURL(linkId);
            }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
