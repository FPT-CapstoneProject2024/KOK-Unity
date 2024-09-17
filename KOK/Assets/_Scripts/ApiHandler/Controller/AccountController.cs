using KOK.ApiHandler.Context;
using System;
using UnityEngine;
using Newtonsoft.Json;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Web;
using System.Collections.Generic;
using WebSocketSharp;

namespace KOK.ApiHandler.Controller
{
    public class AccountController : MonoBehaviour
    {
        private string accountResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Accounts_Resource;

        private void Start()
        {
            accountResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Accounts_Resource;
        }

        private async void Update()
        {
            #region Testing

            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    //SendGetAccountRequest();
            //    var account = await GetAccountByIdAsync(Guid.Parse("E04BF9EE-247B-47EC-92D1-B04C04F77724"));
            //    if (account == null)
            //    {
            //        Debug.Log("Failed to get account detail by API request");
            //    }
            //    else
            //    {
            //        Debug.Log("YOB: " + account.Yob);
            //    }

            //}

            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    CreateAccountRequest account = new CreateAccountRequest()
            //    {
            //        UserName = "Hoa",
            //        Password = "111111",
            //        Email = "hoa@gmail.com",
            //    };
            //    var result = await CreateAccountAsync(account);
            //    if (result == null)
            //    {
            //        Debug.Log("Failed to create new member account by API request");
            //    }
            //    else
            //    {
            //        Debug.Log("YOB: " + result.Yob);
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    var orderFilter = AccountOrderFilter.UserName;
            //    var paging = new PagingRequest() { OrderType = SortOrder.Ascending };

            //    var result = await GetAccountsFilterPagingAsync(new AccountFilter(), orderFilter, paging);
            //    if (result == null)
            //    {
            //        Debug.Log("Failed to get accounts by API request");
            //    }
            //    else
            //    {
            //        Debug.Log("Total Count: " + result.Metadata.Total);
            //    }
            //}

            #endregion
        }

        public void GetAccountByIdCoroutine(Guid accountId, Action<Account> onSuccess, Action<string> onError)
        {
            // Validate account ID
            if (accountId == null)
            {
                Debug.Log("Failed to get account by ID. Account ID is null!");
                return;
            }

            // Prepare and send api request
            var url = accountResourceUrl + "/" + accountId.ToString();
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Account>>(successValue);
                    onSuccess?.Invoke(result.Value);
                },
                (errorValue) =>
                {
                    Debug.LogError($"{url} \nError when trying to retrieve an account by ID [{accountId.ToString()}]: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }


        private NameValueCollection GenerateAccountQueryParams(AccountFilter filter, AccountOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            if (!filter.UserName.IsNullOrEmpty())
            {
                queryParams.Add(nameof(filter.UserName), filter.UserName);
            }

            if (!filter.Email.IsNullOrEmpty())
            {
                queryParams.Add(nameof(filter.Email), filter.Email);
            }

            if (!filter.PhoneNumber.IsNullOrEmpty())
            {
                queryParams.Add(nameof(filter.PhoneNumber), filter.PhoneNumber);
            }

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }

        public void CreateAccountCoroutine(CreateAccountRequest newAccount, Action<Account> onSuccess, Action<string> onError)
        {
            var jsonData = JsonConvert.SerializeObject(newAccount);
            var url = accountResourceUrl;
            Debug.Log(url + "\n" + jsonData);
            ApiHelper.Instance.PostCoroutine(url, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Account>>(successValue);
                    onSuccess?.Invoke(result.Value);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to create new account: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void GetAccountsFilterPagingCoroutine(AccountFilter filter, AccountOrderFilter orderFilter, PagingRequest paging, Action<List<Account>> onSuccess, Action<string> onError)
        {
            var queryParams = GenerateAccountQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(accountResourceUrl, queryParams);

            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<DynamicResponseResult<Account>>(successValue);
                    onSuccess?.Invoke(result.Results);
                },
                (errorValue) =>
                {
                    Debug.LogError($"Error when trying to retrieve account list: {errorValue}");
                    onError?.Invoke(errorValue);
                });
        }

        public void UpdateAccountCoroutine(Guid accountId, UpdateAccountRequest updateAccountRequest, Action<ResponseResult<Account>> onSuccess, Action<ResponseResult<Account>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(updateAccountRequest);
            var url = accountResourceUrl + "/" + accountId;
            Debug.Log(url + "\n" + jsonData);
            ApiHelper.Instance.PutCoroutine(url, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Account>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Account>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void GetAccountCoroutine(Guid accountId, Action<ResponseResult<Account>> onSuccess, Action<ResponseResult<Account>> onError)
        {
            var url = accountResourceUrl + $"/{accountId.ToString()}";
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Account>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<Account>>(errorValue);
                    onError?.Invoke(result);
                });
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
