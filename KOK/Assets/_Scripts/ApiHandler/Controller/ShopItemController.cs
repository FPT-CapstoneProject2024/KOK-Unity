using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
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
        private string itemResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Items_Resource;

        private void Start()
        {
            itemResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Items_Resource;
        }

        public void GetItemByIdCoroutine(Guid itemId, Action<Item> onSuccess, Action<string> onError)
        {
            if (itemId == null)
            {
                Debug.Log("Failed to get item by ID. Item ID is null!");
                return;
            }

            // Prepare and send api request
            var url = itemResourceUrl + "/" + itemId.ToString(); 
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<Item>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve items list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void GetItemsFilterCoroutine(ItemFilter filter, ItemOrderFilter orderFilter, Action<List<Item>> onSuccess, Action<string> onError)
        {
            filter.ItemStatus = ItemStatus.ENABLE;
            var queryParams = GenerateItemQueryParams(filter, orderFilter);
            var url = QueryHelper.BuildUrl(itemResourceUrl, queryParams);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<Item>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve items list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void GetShopItemOfAMemberCoroutine(Guid memberId, ItemFilter filter, ItemOrderFilter orderFilter, Action<List<Item>> onSuccess, Action<string> onError)
        {
            filter.ItemStatus = ItemStatus.ENABLE;
            var queryParams = GenerateItemQueryParams(filter, orderFilter);
            var url = itemResourceUrl + $"/get-shop-item/{memberId}";
            url = QueryHelper.BuildUrl(url, queryParams);
            Debug.Log(url);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<Item>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve items list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void BuySong()
        {

        }


        public NameValueCollection GenerateItemQueryParams(ItemFilter filter, ItemOrderFilter orderFilter)
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

            if (filter.ItemStatus != null)
            {
                queryParams.Add(nameof(filter.ItemStatus), filter.ItemStatus.ToString());
            }

            //queryParams.Add(nameof(paging.page), paging.page.ToString());
            //queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            //queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            //queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }
        public NameValueCollection GenerateItemQueryParams(ItemFilter filter, ItemOrderFilter orderFilter, PagingRequest pagingRequest)
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

            if (filter.ItemStatus != null)
            {
                queryParams.Add(nameof(filter.ItemStatus), filter.ItemStatus.ToString());
            }

            //queryParams.Add(nameof(paging.page), paging.page.ToString());
            //queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            //queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            //queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}