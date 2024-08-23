using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.AccountItem;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class AccountItemController : MonoBehaviour
    {
        private string accountItemResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.AccountItems_Resource;

        private void Start()
        {
            accountItemResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.AccountItems_Resource;
        }

        //public void GetAccountItemsFilterCoroutine(AccountItemFilter filter,Action<List<AccountItemDetail>> onSuccess, Action<string> onError)
        //{
        //    var queryParams = GenerateAccountItemQueryParams(filter, orderFilter, paging);
        //    var url = QueryHelper.BuildUrl(accountItemsResourceUrl, queryParams);
        //    ApiHelper.Instance.GetCoroutine(url,
        //        (successValue) =>
        //        {
        //            var result = JsonConvert.DeserializeObject<DynamicResponseResult<AccountItemDetail>>(successValue);
        //            onSuccess?.Invoke(result.Results);
        //        },
        //        (errorValue) =>
        //        {
        //            Debug.LogError($"Error when trying to retrieve accountItems list: {errorValue}");
        //            onError?.Invoke(errorValue);
        //        });
        //}
    }
}
