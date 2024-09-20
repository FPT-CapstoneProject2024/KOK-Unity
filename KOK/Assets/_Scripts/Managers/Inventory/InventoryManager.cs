using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] Transform accountItemViewPortContent;
        [SerializeField] GameObject accountItemTemplatePrefab;
        public AccountItemDetailBinding AccountItemDetailBinding;
        public AlertManager MessageAlertManager;
        public AlertManager ConfirmAlertManager;
        [SerializeField] TMP_Dropdown categoryDropdown;
        [SerializeField] TMP_Text upLabel;
        [SerializeField] LoadingManager loadingManager;

        List<AccountItem> accountItems = new();
        List<AccountItemBinding> accountItemBindings = new List<AccountItemBinding>();

        private void OnEnable()
        {
            ReloadAccountItem();
        }
        public void ReloadAccountItem()
        {
            foreach (Transform child in accountItemViewPortContent)
            {
                GameObject.Destroy(child.gameObject);
            }
            loadingManager.EnableLoadingSymbol();
            ApiHelper.Instance.GetComponent<AccountItemController>()
                .GetItemsFilterCoroutine(
                    new AccountItemFilter()
                    {
                        MemberId = Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId))
                    },
                    (accountItemList) =>
                    {
                        accountItems = accountItemList;

                        foreach (var accountItem in accountItemList)
                        {
                            var accountItemObject = Instantiate(accountItemTemplatePrefab, accountItemViewPortContent);
                            var accountItemBinding = accountItemObject.GetComponent<AccountItemBinding>();
                            accountItemBinding.Init(accountItem, this);
                        }
                        loadingManager.DisableLoadingSymbol();
                    },
                    (ex) => {
                        loadingManager.DisableLoadingSymbol();
                        MessageAlertManager.Alert("Đã có lỗi xảy ra!", false);
                    }

                );
            ApiHelper.Instance.GetComponent<AccountController>()
                .GetAccountByIdCoroutine(
                    Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    (account) => { upLabel.text = (int)account.UpBalance + ""; },
                    (ex) => { }
                );
        }

        public void OnCategoryChange()
        {
            foreach (Transform child in accountItemViewPortContent)
            {
                GameObject.Destroy(child.gameObject);
            }

            var accountItemType = categoryDropdown.value switch
            {
                0 => ItemType.DEFAULT,
                1 => ItemType.CHARACTER,
                2 => ItemType.ROOM,
                _ => ItemType.DEFAULT,
            };

            foreach (var accountItem in accountItems)
            {
                if (accountItem.Item.ItemType == accountItemType || accountItemType == ItemType.DEFAULT)
                {
                    var accountItemObject = Instantiate(accountItemTemplatePrefab, accountItemViewPortContent);
                    var accountItemBinding = accountItemObject.GetComponent<AccountItemBinding>();
                    accountItemBinding.Init(accountItem, this);
                }
            }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
