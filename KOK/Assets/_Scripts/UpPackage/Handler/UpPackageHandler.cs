using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class UpPackageHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public GameObject packageContainer;
        [SerializeField] public GameObject packageItemTemplate;
        [SerializeField] public TMP_Text packageListMessage;
        [SerializeField] public LoadingManager loadingManager;
        [Header("Paging Components")]
        [SerializeField] public Button previousButton;
        [SerializeField] public Button nextButton;
        [Header("Package Purchase Component")]
        [SerializeField] public GameObject packagePurchaseCanvas;
        [SerializeField] PendingPackageBinding pendingPackageBinding;
        [Header("Alert")]
        [SerializeField] public AlertManager MessageAlertManager;

        private int currentPage = 1;
        private int totalPage = 1;
        private int pageSize = 6;
        private UpPackageFilter filter;

        public void SetInitialState()
        {
            currentPage = 1;
            totalPage = 1;
            pageSize = 6;
            filter = new UpPackageFilter()
            {
                Status = PackageStatus.ACTIVE
            };
            ClearContainer();
            SetPackageMessage(string.Empty);
            previousButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }

        private void ClearContainer()
        {
            if (packageContainer.transform.childCount > 0)
            {
                while (packageContainer.transform.childCount > 0)
                {
                    DestroyImmediate(packageContainer.transform.GetChild(0).gameObject);
                }
            }
        }

        private void SetPackageMessage(string message)
        {
            //packageListMessage.text = message;
        }

        private void OnEnable()
        {
            SetInitialState();
            LoadPackages();
        }

        public void LoadPackages()
        {
            loadingManager.DisableUIElement();
            ClearContainer();
            ApiHelper.Instance.GetComponent<UpPackageController>().GetPackagesPagingCoroutine(filter, PackageOrderFilter.StarNumber, new PagingRequest()
            {
                page = currentPage,
                pageSize = pageSize,
            },
            OnLoadPackagesSuccess,
            OnLoadPackagesError
            );
        }

        private void OnLoadPackagesSuccess(DynamicResponseResult<UpPackage> responseResult)
        {
            ClearContainer();
            if (responseResult.Results == null || responseResult.Results.Count == 0)
            {
                SetPackageMessage("Không tìm thấy gói UP");
                return;
            }
            SetPackageMessage(string.Empty);
            SetPagingComponent(responseResult);
            SpawnPackageItems(responseResult.Results);
            loadingManager.EnableUIElement();
        }

        private void OnLoadPackagesError(DynamicResponseResult<UpPackage> responseResult)
        {
            ClearContainer();
            SetPackageMessage("Không tìm thấy gói UP");
            loadingManager.EnableUIElement();
        }

        private void SetPagingComponent(DynamicResponseResult<UpPackage> responseResult)
        {
            totalPage = (int)Math.Ceiling(responseResult.Metadata.Total / (double)responseResult.Metadata.Size);
            // Previous
            if (currentPage > 1)
            {
                previousButton.gameObject.SetActive(true);
            }
            else
            {
                previousButton.gameObject.SetActive(false);
            }
            // Next
            if (currentPage < totalPage)
            {
                nextButton.gameObject.SetActive(true);
            }
            else
            {
                nextButton.gameObject.SetActive(false);
            }
        }

        private void SpawnPackageItems(List<UpPackage> packages)
        {
            GameObject newPackageItem;
            for (int i = 0; i < packages.Count; i++)
            {
                newPackageItem = Instantiate(packageItemTemplate, packageContainer.transform);
                newPackageItem.GetComponent<UpPackageItemBinding>().BindData(packages[i]);
            }
        }

        public void OnPreviousPageClick()
        {
            currentPage--;
            LoadPackages();
        }

        public void OnNextPageClick()
        {
            currentPage++;
            LoadPackages();
        }

        public void OpenPackagePurchaseDialog(PurchasePackageParam param)
        {
            packagePurchaseCanvas.GetComponent<PackagePurchaseHandler>().ShowPurchasePackageDialog(param);
        }

        public void OnOpenPendingPurchasePanelClick()
        {
            pendingPackageBinding.gameObject.SetActive(true);
            pendingPackageBinding.Init(this);

        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
