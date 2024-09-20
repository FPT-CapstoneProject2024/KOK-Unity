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

        public void PurchasePackagePayOSCoroutine(PayOSPackagePurchaseRequest request, Action<ResponseResult<PayOSPackagePurchaseResponse>> onSuccess, Action<ResponseResult<PayOSPackagePurchaseResponse>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(request);
            ApiHelper.Instance.PostCoroutine(packageResourceUrl + "/purchase/payos", jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<PayOSPackagePurchaseResponse>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<PayOSPackagePurchaseResponse>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void CancelPayOSPackagePurchaseRequestCoroutine(Guid monetaryTransactionId, Action onSuccess, Action<ResponseResult<string>> onError)
        {
            var url = packageResourceUrl + $"/cancel/payos/{monetaryTransactionId}";
            Debug.Log(url);
            ApiHelper.Instance.DeleteCoroutine(url,
                (successValue) =>
                {
                    //var result = JsonConvert.DeserializeObject<ResponseResult<string>>(successValue);
                    onSuccess?.Invoke();
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<string>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void GetMemberPendingPurchaseRequest(Guid memberId, Action<ResponseResult<PayOSPackagePaymentMethodResponse>> onSuccess, Action<ResponseResult<PayOSPackagePaymentMethodResponse>> onError)
        {
            ApiHelper.Instance.GetCoroutine(packageResourceUrl + $"/request/pending/{memberId.ToString()}",
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<PayOSPackagePaymentMethodResponse>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<PayOSPackagePaymentMethodResponse>>(errorValue);
                    onError?.Invoke(result);
                });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
