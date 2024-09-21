using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Song;
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
    public class AccountItemDetailBinding : MonoBehaviour
    {
        public AccountItem AccountItem { get; private set; }
        [SerializeField] Image itemImage;
        [SerializeField] TMP_Text itemNameText;
        [SerializeField] TMP_Text itemTypeText;
        [SerializeField] TMP_Text itemPriceText;
        [SerializeField] TMP_Text itemDescriptionText;
        [SerializeField] Button useButton;
        [SerializeField] Sprite defaultImage;
        InventoryManager inventoryManager;

        public void Init(AccountItem accountItem, InventoryManager inventoryManager)
        {
            this.AccountItem = accountItem;
            itemImage.sprite = null;
            if (accountItem.Item.ItemType == ApiHandler.DTOModels.ItemType.CHARACTER)
            {
                itemImage.sprite = Resources.Load<Sprite>(accountItem.Item.ItemCode + "AVA");
            }
            if (itemImage.sprite == null) itemImage.sprite = defaultImage;
            itemNameText.text = "Tên: " + accountItem.Item.ItemName;
            itemTypeText.text = "Loại: " + accountItem.Item.ItemType.ToString();
            itemPriceText.text = "Giá: " + (int)accountItem.Item.ItemSellPrice;
            itemDescriptionText.text = "Mô tả: \n" + accountItem.Item.ItemDescription;

            this.inventoryManager = inventoryManager;
        }

        public void UseItem()
        {
            if (AccountItem.Item.ItemType == ItemType.CHARACTER)
            {
                UpdateCharacter();
            }
            else if (AccountItem.Item.ItemType == ItemType.ROOM)
            {

            }
            else if (AccountItem.Item.ItemType == ItemType.DEFAULT)
            {

            }
        }

        private void UpdateCharacter()
        {
            if (AccountItem.Item.ItemType == ItemType.CHARACTER)
            {
                ApiHelper.Instance.GetComponent<AccountController>()
                    .UpdateAccountCoroutine(
                        Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                        new()
                        {
                            CharacterItemId = AccountItem.AccountItemId,
                            UserName = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_UserName)
                        },
                        (account) =>
                        {
                            inventoryManager.MessageAlertManager.Alert("Bạn đã thay đổi sang nhân vật " + AccountItem.Item.ItemName, true);
                            inventoryManager.AccountItemDetailBinding.gameObject.SetActive(false);
                            PlayerPrefsHelper.SetString(PlayerPrefsHelper.Key_CharaterItemCode, account.Value.CharaterItemCode);
                        },
                        (ex) =>
                        {
                            inventoryManager.MessageAlertManager.Alert("Có lỗi đã xảy ra!", false);
                            Debug.LogError(ex.Message);
                        }
                    );
            }

        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
