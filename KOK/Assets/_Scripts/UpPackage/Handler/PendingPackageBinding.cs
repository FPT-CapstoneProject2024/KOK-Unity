using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using QRCoder;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class PendingPackageBinding : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public Button CancelPurchaseButton;
        [SerializeField] TMP_Text packageName;
        [SerializeField] TMP_Text packageAmount;
        [SerializeField] TMP_Text packagePrice;
        [SerializeField] RawImage QRImage;
        [SerializeField] TMP_Text purchaseMessage;
        [SerializeField] LoadingManager loadingManager;
        [SerializeField] AlertManager messageAlertManager;
        UpPackageHandler upPackageHandler;
        PayOSPackagePaymentMethodResponse payOSPackage;
        public void Init(UpPackageHandler upPackageHandler)
        {
            gameObject.SetActive(false);
            this.upPackageHandler = upPackageHandler;
            loadingManager.EnableLoadingSymbol();
            ApiHelper.Instance.GetComponent<UpPackageController>()
                .GetMemberPendingPurchaseRequest(
                    Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    (payOSPackage) =>
                    {
                        this.payOSPackage = payOSPackage.Value;
                        loadingManager.DisableLoadingSymbol();
                        if (payOSPackage.Value == null || !payOSPackage.Result.HasValue || !((bool)payOSPackage.Result))
                        {
                            purchaseMessage.text = payOSPackage.Message;
                            return;
                        }
                        Debug.Log($"Link: {payOSPackage.Value.checkoutUrl}");
                        packageName.text = payOSPackage.Value.PackageName;
                        packageAmount.text = $"{payOSPackage.Value.UpAmount} UP";
                        packagePrice.text = $"{payOSPackage.Value.MoneyAmount:#,##0} VND";
                        string resultMessage = $"Tạo yêu cầu nạp UP thành công. \nQuét QR hoặc nhấn <color=#00BFFF><link=\"{payOSPackage.Value.checkoutUrl}\"><u>tại đây</u></link></color> để đến trang thanh toán.";
                        purchaseMessage.text = resultMessage;
                        var texture = EncodeTextToQRCode(payOSPackage.Value.qrCode);
                        QRImage.texture = texture;
                        gameObject.SetActive(true);
                    },
                    (payOSPackage) =>
                    {
                        loadingManager.DisableLoadingSymbol();
                        Debug.LogError(payOSPackage.Message);
                        upPackageHandler.MessageAlertManager.Alert("Không có gói nạp UP nào đang chờ xử lý!");
                    }
                );
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

        public void OnCancelPayOSPackagePurchase()
        {
            ApiHelper.Instance.GetComponent<UpPackageController>()
                .CancelPayOSPackagePurchaseRequestCoroutine(
                    Guid.Parse(payOSPackage.MonetaryTransactionId),
                    () =>
                    {
                        upPackageHandler.MessageAlertManager.Alert("Huỷ gói nạp đang chờ thành công!", true);
                        gameObject.SetActive(false);
                    },
                    (ex) =>
                    {
                        upPackageHandler.MessageAlertManager.Alert("Có lỗi đã xảy ra!", false);
                    }
                );
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
