using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Song;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class ItemDetailBinding : MonoBehaviour
    {
        public Item Item { get; private set; }
        [SerializeField] Image itemImage;
        [SerializeField] TMP_Text itemNameText;
        [SerializeField] TMP_Text itemTypeText;
        [SerializeField] TMP_Text itemPriceText;
        [SerializeField] TMP_Text itemDescriptionText;
        [SerializeField] Button buyButton;
        [SerializeField] Sprite defaultImage;
        private ShopItemManager shopItemManager;

        public void Init(Item item, ShopItemManager shopItemManager)
        {
            this.Item = item;
            itemImage.sprite = null;
            if (item.ItemType == ApiHandler.DTOModels.ItemType.CHARACTER)
            {
                itemImage.sprite = Resources.Load<Sprite>(item.ItemCode + "AVA");
            }
            if (itemImage.sprite == null) itemImage.sprite = defaultImage;
            itemNameText.text = "Tên: " + item.ItemName;
            itemTypeText.text = "Loại: " + item.ItemType.ToString();
            itemPriceText.text = "Giá: " + (int)item.ItemSellPrice;
            itemDescriptionText.text = "Mô tả: \n" + item.ItemDescription;

            if ((bool)item.IsOwned)
            {
                itemPriceText.text = "Giá: " + (int)item.ItemSellPrice + " (Đã sở hữu)";
                buyButton.interactable = false;
            }
            else
            {
                buyButton.interactable = true;
            }

            this.shopItemManager = shopItemManager;
        }

        public void OpenBuyConfirmPanel()
        {
            string tmpName = Item.ItemName;
            if (tmpName.Count() > 7) { itemNameText.text = tmpName.Substring(0, 7) + "..."; }
            shopItemManager.ConfirmAlertManager.Confirm("Xác nhận mua vật phẩm " + tmpName, () =>
            {
                ApiHelper.Instance.GetComponent<ShopController>()
                    .PurchaseItemCoroutine(
                        new ItemPurchaseRequest()
                        {
                            MemberId = Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                            ItemId = (Guid)Item.ItemId,
                        },
                        (itemPurchaseResponse) =>
                        {
                            Debug.Log("Item purchase success: " + itemPurchaseResponse);
                            shopItemManager.MessageAlertManager.Alert("Bạn đã mua " + Item.ItemName, true);
                            shopItemManager.ReloadItem();
                            this.gameObject.SetActive(false);
                        },
                        (ex) =>
                        {
                            shopItemManager.MessageAlertManager.Alert(ex.Message, false);
                        }

                );

            });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}
