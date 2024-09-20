using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class AccountItemBinding : MonoBehaviour
    {
        [SerializeField] Image itemImage;
        [SerializeField] TMP_Text itemNameText;
        AccountItem accountItem;
        InventoryManager inventoryManager;
        public void Init(AccountItem accountItem, InventoryManager inventoryManager)
        {
            this.accountItem = accountItem;
            this.inventoryManager = inventoryManager;
            if (accountItem.Item.ItemType == ApiHandler.DTOModels.ItemType.CHARACTER)
            {
                itemImage.sprite = Resources.Load<Sprite>(accountItem.Item.ItemCode + "AVA");
            }
            string tmpName = accountItem.Item.ItemName;
            if (tmpName.Count() > 7) { itemNameText.text = tmpName.Substring(0, 7) + "..."; }
            else { itemNameText.text = tmpName; }
        }

        public void ShowDetailPanel()
        {
            inventoryManager.AccountItemDetailBinding.gameObject.SetActive(true);
            inventoryManager.AccountItemDetailBinding.Init(accountItem, inventoryManager);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}
