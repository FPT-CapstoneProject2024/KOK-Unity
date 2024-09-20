using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Item;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;

namespace KOK
{
    public class AccountItemController : MonoBehaviour
    {
        private string accountItemResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.AccountItems_Resource;

        private void Start()
        {
            accountItemResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.AccountItems_Resource;
        }

        public void GetItemByIdCoroutine(Guid accountItemId, Action<string> onSuccess, Action<string> onError)
        {
            if (accountItemId == null)
            {
                Debug.Log("Failed to get accountItem by ID. Item ID is null!");
                return;
            }

            // Prepare and send api request
            var url = accountItemResourceUrl + "/" + accountItemId.ToString();
            ApiHelper.Instance.GetCoroutine(url, onSuccess, onError);
        }

        public void GetItemsFilterCoroutine(AccountItemFilter filter, Action<List<AccountItem>> onSuccess, Action<string> onError)
        {
            var queryParams = GenerateItemQueryParams(filter);
            var url = QueryHelper.BuildUrl(accountItemResourceUrl, queryParams);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<AccountItem>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve accountItems list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public NameValueCollection GenerateItemQueryParams(AccountItemFilter filter)
        {
            var queryParams = new NameValueCollection();
            if (filter.MemberId != null)
            {
                queryParams.Add(nameof(filter.MemberId), filter.MemberId.ToString());
            }

            return queryParams;
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
