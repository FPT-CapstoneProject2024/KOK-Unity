using KOK.ApiHandler.Context;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using Newtonsoft.Json;
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

        private async void Update()
        {
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
    }
}
