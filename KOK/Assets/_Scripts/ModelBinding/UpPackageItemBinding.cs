using KOK.ApiHandler.DTOModels;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Songs;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class UpPackageItemBinding : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public Button PurchaseButton;
        [SerializeField] public TMP_Text PackageName;
        [SerializeField] public TMP_Text PackageAmount;
        [SerializeField] public TMP_Text PackagePrice;
        [SerializeField] public TMP_Text PackageDescription;

        public UpPackage UpPackage = null;

        public void BindData(UpPackage upPackage)
        {
            UpPackage = upPackage;

            PackageName.text = UpPackage.PackageName;
            PackageAmount.text = $"{UpPackage.StarNumber.ToString()} UP";
            PackagePrice.text = $"{UpPackage.MoneyAmount.ToString("#,##0")} VND";
            PackageDescription.text = UpPackage.Description;

            PurchasePackageParam param = new PurchasePackageParam()
            {
                PackageId = (Guid)UpPackage.PackageId,
                PackageName = UpPackage.PackageName,
                MoneyAmount = UpPackage.MoneyAmount,
                StarNumber = (int)UpPackage.StarNumber,
            };
            PurchaseButton.AddEventListener(param, OnPackageClick);
        }

        private void OnPackageClick(PurchasePackageParam param)
        {
            Debug.Log($"User purchase UP package: {param.PackageId} - {param.PackageName} - {param.StarNumber} - {param.MoneyAmount}");
            FindFirstObjectByType<UpPackageHandler>().OpenPackagePurchaseDialog(param);
        }
    }

    public class PurchasePackageParam
    {
        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public decimal MoneyAmount { get; set; }
        public int StarNumber { get; set; }
    }
}
