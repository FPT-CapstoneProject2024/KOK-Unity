using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using UnityEngine;

namespace KOK.ApiHandler.Controller
{
    public class UpPackageController : MonoBehaviour
    {
        private string packageResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.UpPackage_Resource;

        private void Start()
        {
            packageResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.UpPackage_Resource;
        }

        public void GetPackagesPagingCoroutine(UpPackageFilter filter, PackageOrderFilter orderFilter, PagingRequest paging, Action<DynamicResponseResult<UpPackage>> onSuccess, Action<DynamicResponseResult<UpPackage>> onError)
        {
            var queryParams = GeneratePackagesQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(packageResourceUrl, queryParams);
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<UpPackage>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<UpPackage>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        private NameValueCollection GeneratePackagesQueryParams(UpPackageFilter filter, PackageOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();

            queryParams.Add(nameof(filter.Status), filter.Status.ToString());

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }
    }
}
