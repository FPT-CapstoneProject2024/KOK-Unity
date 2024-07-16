using KOK.ApiHandler.Context;
using System;
using UnityEngine;
using Newtonsoft.Json;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Web;

namespace KOK.ApiHandler.Controller
{
    public class AccountController : MonoBehaviour
    {
        private string accountResourceUrl = string.Empty;

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

        public void GetAccountByIdCoroutine(Guid accountId, Action<string> onSuccess, Action<string> onError)
        {
            // Validate account ID
            if (accountId == null)
            {
                Debug.Log("Failed to get account by ID. Account ID is null!");
                return;
            }

            // Prepare and send api request
            var url = accountResourceUrl + "/" + accountId.ToString();
            ApiHelper.Instance.GetCoroutine(url, onSuccess, onError);
        }

        public async Task<Account?> GetAccountByIdAsync(Guid accountId)
        {
            var url = accountResourceUrl + "/" + accountId.ToString();
            var jsonResult = await ApiHelper.Instance.GetAsync(url);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            ResponseResult<Account> result = JsonConvert.DeserializeObject<ResponseResult<Account>>(jsonResult);

            return result.Value;
        }

        /// <summary>
        /// Async method to create new member account.
        /// </summary>
        /// <param name="newAccount">Object contains data for new account. Required property: Username, Password, Email.</param>
        /// <returns>Detail of newly created account.</returns>
        public async Task<Account?> CreateAccountAsync(CreateAccountRequest newAccount)
        {
            var jsonData = JsonConvert.SerializeObject(newAccount);
            var url = accountResourceUrl;
            var jsonResult = await ApiHelper.Instance.PostAsync(url, jsonData);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            Debug.Log(jsonResult);

            ResponseResult<Account> result = JsonConvert.DeserializeObject<ResponseResult<Account>>(jsonResult);

            return result.Value;
        }

        public async Task<DynamicResponseResult<Account>?> GetAccountsFilterPagingAsync(AccountFilter filter, AccountOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = GenerateAccountQueryParams(filter, orderFilter, paging);
            var url = QueryHelper.BuildUrl(accountResourceUrl, queryParams);

            var jsonResult = await ApiHelper.Instance.GetAsync(url);
            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            DynamicResponseResult<Account> result = JsonConvert.DeserializeObject<DynamicResponseResult<Account>>(jsonResult);
            return result;
        }

        private NameValueCollection GenerateAccountQueryParams(AccountFilter filter, AccountOrderFilter orderFilter, PagingRequest paging)
        {
            var queryParams = new NameValueCollection();
            if (filter.UserName != null)
            {
                queryParams.Add(nameof(filter.UserName), filter.UserName);
            }

            if (filter.Email != null)
            {
                queryParams.Add(nameof(filter.Email), filter.Email);
            }

            if (filter.PhoneNumber != null)
            {
                queryParams.Add(nameof(filter.PhoneNumber), filter.PhoneNumber);
            }

            queryParams.Add(nameof(paging.page), paging.page.ToString());
            queryParams.Add(nameof(paging.pageSize), paging.pageSize.ToString());
            queryParams.Add(nameof(paging.OrderType), paging.OrderType.ToString());
            queryParams.Add(nameof(orderFilter), orderFilter.ToString());

            return queryParams;
        }
    }
}
