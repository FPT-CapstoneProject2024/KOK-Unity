using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using KOK.ApiHandler.DTOModels;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Item;
using KOK.ApiHandler.Context;

namespace KOK
{
    public class ShopLayout : MonoBehaviour
    {
        [SerializeField] private List<Item> itemList = new List<Item>();
        [SerializeField] private ShopItemController shopItemController;

        private List<string> itemCodes = new List<string>();
        public GameObject displayPanel;
        public GameObject displayButton;
        private string itemResourceUrl = string.Empty;

        private void Start()
        {
            itemResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Items_Resource;
            shopItemController = new ShopItemController();
            StartCoroutine(GetItemsFilterPagingCoroutine(new ItemFilter(), new ItemOrderFilter(), new PagingRequest()));
        }

        private void LayoutGenerate()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                GameObject gameObj = Instantiate(displayButton, displayPanel.transform);
                gameObj.transform.GetChild(0).GetComponent<TMP_Text>().text = itemCodes[i];

                int index = i;
                gameObj.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    ItemClicked(index);
                });

                //gameObj.tag = "Item Display";
            }
        }

        private void ItemClicked(int itemIndex)
        {
            Debug.Log("Item clicked: " + itemIndex);
        }

        public void RefreshClicked()
        {
            for (int i = 0; i < displayPanel.transform.childCount; i++)
            {
                GameObject child = displayPanel.transform.GetChild(i).gameObject;
                Destroy(child);
                ///*if (child.CompareTag("Item Display"))
                //{

                //}*/
            }

            StartCoroutine(GetItemsFilterPagingCoroutine(new ItemFilter(), new ItemOrderFilter(), new PagingRequest()));
        }

        private IEnumerator GetItemsFilterPagingCoroutine(ItemFilter filter, ItemOrderFilter orderFilter, PagingRequest paging)
        {
            string url = shopItemController.BuildUrl(itemResourceUrl, shopItemController.GenerateItemQueryParams(filter, orderFilter, paging));

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                var responseObject = JsonConvert.DeserializeObject<DynamicResponseResult<Item>>(response);
                var items = responseObject.Results;

                itemCodes.Clear();
                itemList.Clear();

                foreach (Item item in items)
                {
                    itemCodes.Add(item.ItemCode);
                    itemList.Add(item);
                }

                LayoutGenerate();
                Debug.Log(response);
            }
            else
            {
                Debug.LogError(request.error);
            }
        }
    }
}

