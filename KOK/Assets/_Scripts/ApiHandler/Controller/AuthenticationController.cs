using KOK.ApiHandler.Context;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK
{
    public class AuthenticationController : MonoBehaviour
    {
        private string authenticationResourceUrl = string.Empty;

        private void Start()
        {
            authenticationResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Authentication_Resource;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var jsonData = JsonConvert.SerializeObject(loginRequest);
            var url = authenticationResourceUrl + "/login";

            var jsonResult = await ApiHelper.Instance.PostAsync(url, jsonData);

            if (string.IsNullOrEmpty(jsonResult))
            {
                return null;
            }

            var result = JsonConvert.DeserializeObject<LoginResponse>(jsonResult);

            return result;
        }

        public void LoginCoroutine(LoginRequest loginRequest, Action<LoginResponse> onSuccess, Action<LoginResponse> onError)
        {
            var jsonData = JsonConvert.SerializeObject(loginRequest);
            var url = authenticationResourceUrl + "/login";

            ApiHelper.Instance.PostCoroutine(url, jsonData,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<LoginResponse>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<LoginResponse>(errorValue);
                    onError?.Invoke(result);
                });
        }

        public void SignUpCoroutine(MemberRegisterRequest registerRequest, Action<ResponseResult<Account>> onSuccess, Action<ResponseResult<Account>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(registerRequest);
            var url = authenticationResourceUrl + "/sign-up/member";

            ApiHelper.Instance.PostCoroutine(url, jsonData,
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

        public void SendVerificationCoroutine(string email, Action<ResponseResult<bool>> onSuccess, Action<ResponseResult<bool>> onError)
        {
            var url = authenticationResourceUrl + $"/verify/{email}";
            ApiHelper.Instance.GetCoroutine(url,
                (successValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<bool>>(successValue);
                    onSuccess?.Invoke(result);
                },
                (errorValue) =>
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult<bool>>(errorValue);
                    onSuccess?.Invoke(result);
                });
        }

        public void VerifyMemberAccountCoroutine(MemberAccountVerifyRequest verifyRequest, Action<ResponseResult<Account>> onSuccess, Action<ResponseResult<Account>> onError)
        {
            var jsonData = JsonConvert.SerializeObject(verifyRequest);
            var url = authenticationResourceUrl + "/verify";

            ApiHelper.Instance.PostCoroutine(url, jsonData,
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
