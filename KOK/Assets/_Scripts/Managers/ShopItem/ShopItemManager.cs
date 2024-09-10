using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Item;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class ShopItemManager : MonoBehaviour
    {
        [SerializeField] Transform itemViewPortContent;
        [SerializeField] GameObject itemTemplatePrefab;
        public ItemDetailBinding ItemDetailBinding;
        public AlertManager MessageAlertManager;
        public AlertManager ConfirmAlertManager;
        [SerializeField] TMP_Dropdown categoryDropdown;

        List<Item> items = new();
        List<ItemBinding> itemBindings = new List<ItemBinding>();

        private void Start()
        {
            ReloadItem();
        }
        public void ReloadItem()
        {
            foreach (Transform child in itemViewPortContent)
            {
                GameObject.Destroy(child.gameObject);
            }
            ApiHelper.Instance.GetComponent<ShopItemController>()
                .GetShopItemOfAMemberCoroutine(
                    Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    new ItemFilter()
                    {

                    },
                    new ItemOrderFilter()
                    {

                    },
                    (itemList) =>
                    {
                        items = itemList;

                        foreach (var item in itemList)
                        {
                            var itemObject = Instantiate(itemTemplatePrefab, itemViewPortContent);
                            var itemBinding = itemObject.GetComponent<ItemBinding>();
                            itemBinding.Init(item, this);
                        }
                    },
                    (ex) => { }

                );
        }

        public void OnCategoryChange()
        {
            foreach (Transform child in itemViewPortContent)
            {
                GameObject.Destroy(child.gameObject);
            }

            var itemType = categoryDropdown.value switch
            {
                0 => ItemType.DEFAULT,
                1 => ItemType.CHARACTER,
                2 => ItemType.ROOM,
                _ => ItemType.DEFAULT,
            };

            foreach (var item in items)
            {
                if (item.ItemType == itemType || itemType == ItemType.DEFAULT)
                {
                    var itemObject = Instantiate(itemTemplatePrefab, itemViewPortContent);
                    var itemBinding = itemObject.GetComponent<ItemBinding>();
                    itemBinding.Init(item, this);
                }
            }
        }

    }
}
