using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Item;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace KOK
{
    public class ShopItemController : MonoBehaviour
    {
        public TMP_InputField itemIdInput;
        public TMP_InputField itemCodeInput;
        public TMP_InputField itemNameInput;
        public TMP_InputField itemDescriptionInput;
        public TMP_InputField itemTypeInput;
        public TMP_InputField itemPriceInput;
        public TMP_InputField itemStatusInput;
        public Toggle canExpireToggle;
        public Toggle canStackToggle;
        public TMP_Text notificationText;
        public float notificationFadeDuration;

        private string itemResourceUrl = string.Empty;

        private void Start()
        {
            itemResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Items_Resource;
        }

        public void GetItemByIdCoroutine(Guid itemId, Action<string> onSuccess, Action<string> onError)
        {
            if (itemId == null)
            {
                Debug.Log("Failed to get item by ID. Item ID is null!");
                return;
            }

            // Prepare and send api request
            var url = itemResourceUrl + "/" + itemId.ToString();
            ApiHelper.Instance.GetCoroutine(url, onSuccess, onError);
        }

        public async Task<Item?> GetItemByIdAsync(Guid itemId)
        {
            var url = itemResourceUrl + "/" + itemId.ToString();
            var jsonResult = await ApiHelper.Instance.GetAsync(url);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            ResponseResult<Item> result = JsonConvert.DeserializeObject<ResponseResult<Item>>(jsonResult);

            return result.Value;
        }

        public async Task<Item?> CreateItemAsync(CreateItemRequest createItem)
        {
            var jsonData = JsonConvert.SerializeObject(createItem);
            var url = itemResourceUrl;
            var jsonResult = await ApiHelper.Instance.PostAsync(url, jsonData);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            Debug.Log(jsonResult);

            ResponseResult<Item> result = JsonConvert.DeserializeObject<ResponseResult<Item>>(jsonResult);

            return result.Value;
        }

        public async Task<DynamicResponseResult<Item>?> GetItemsFilterPagingAsync(ItemFilter filter, ItemOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = GenerateItemQueryParams(filter, orderFilter, paging);
            var url = BuildUrl(itemResourceUrl, queryParams);

            Debug.Log(url);

            var jsonResult = await ApiHelper.Instance.GetAsync(url);
            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            DynamicResponseResult<Item> result = JsonConvert.DeserializeObject<DynamicResponseResult<Item>>(jsonResult);
            return result;
        }

        public string BuildUrl(string baseUrl, NameValueCollection queryParams)
        {
            var builder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (string key in queryParams)
            {
                query[key] = queryParams[key];
            }

            builder.Query = query.ToString();
            return builder.ToString();
        }

        public NameValueCollection GenerateItemQueryParams(ItemFilter filter, ItemOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            if (filter.ItemCode != null)
            {
                queryParams.Add(nameof(filter.ItemCode), filter.ItemCode);
            }

            if (filter.ItemName != null)
            {
                queryParams.Add(nameof(filter.ItemName), filter.ItemName);
            }

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }

        /*public void OnCreateSubmit()
        {
            Item newItem = new Item
            {
                ItemCode = itemCodeInput.text,
                ItemName = itemNameInput.text,
                ItemDescription = itemDescriptionInput.text,
                ItemType = int.Parse(itemTypeInput.text),
                ItemPrice = decimal.Parse(itemPriceInput.text),
                CanExpire = canExpireToggle.isOn,
                CanStack = canStackToggle.isOn,
                //CreatorId = ,  // login
            };

            StartCoroutine(PostItemToApi(newItem));
        }

        public void OnUpdateSubmit()
        {
            Guid itemId = Guid.Parse(itemIdInput.text);

            Item updateItem = new Item
            {
                ItemCode = itemCodeInput.text,
                ItemName = itemNameInput.text,
                ItemDescription = itemDescriptionInput.text,
                ItemType = int.Parse(itemTypeInput.text),
                ItemPrice = decimal.Parse(itemPriceInput.text),
                ItemStatus = int.Parse(itemStatusInput.text),
                CanExpire = canExpireToggle.isOn,
                CanStack = canStackToggle.isOn
                //CreatorId = ,  // login
            };

            StartCoroutine(PutItemToApi(itemId, updateItem));
        }

        public void OnDeleteSubmit()
        {
            Guid itemId = Guid.Parse(itemIdInput.text);

            StartCoroutine(DeleteItemFromApi(itemId));
        }

        private IEnumerator PostItemToApi(Item item)
        {
            string jsonData = JsonConvert.SerializeObject(item);

            UnityWebRequest request = new UnityWebRequest(baseUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Item successfully sent to the API.");
                Helper helper = new Helper();
                notificationText.text = "Item successfully sent to the API.";
                yield return helper.FadeTextToFullAlpha(notificationFadeDuration, notificationText);
                yield return new WaitForSeconds(2); // Wait for 2 seconds before starting to fade out
                yield return helper.FadeTextToZeroAlpha(notificationFadeDuration, notificationText);
            }
            else
            {
                Debug.LogError("Error sending item to the API: " + request.error);
                Helper helper = new Helper();
                notificationText.text = "Error sending item to the API: " + request.error;
                yield return helper.FadeTextToFullAlpha(notificationFadeDuration, notificationText);
                yield return new WaitForSeconds(2); // Wait for 2 seconds before starting to fade out
                yield return helper.FadeTextToZeroAlpha(notificationFadeDuration, notificationText);
            }
        }

        private IEnumerator PutItemToApi(Guid itemId, Item item)
        {
            string jsonData = JsonConvert.SerializeObject(item);
            string url = $"{baseUrl}/{itemId}";

            UnityWebRequest request = new UnityWebRequest(url, "PUT");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Item successfully sent to the API.");
            }
            else
            {
                Debug.LogError("Error sending item to the API: " + request.error);
            }
        }

        private IEnumerator DeleteItemFromApi(Guid itemId)
        {
            string url = $"{baseUrl}/{itemId}";

            UnityWebRequest request = new UnityWebRequest(url, "DELETE");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Item successfully deleted.");
            }
            else
            {
                Debug.LogError("Error deleting item: " + request.error);
            }
        }*/
    }
}