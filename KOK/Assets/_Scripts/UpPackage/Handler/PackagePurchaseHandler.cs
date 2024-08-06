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
        }

        public void DisplayConfirmPurchase()
        {
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
    }
}
