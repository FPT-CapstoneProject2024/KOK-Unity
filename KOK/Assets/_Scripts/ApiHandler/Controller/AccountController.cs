using KOK.ApiHandler.Instance;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.Json.Serialization;
using KOK.ApiHandler.Model;
using Newtonsoft.Json;
using Google.MiniJSON;
using System.Text.Json;

namespace KOK.ApiHandler.Controller
{
    public class AccountController : MonoBehaviour
    {
        [SerializeField] private GameObject accountPanel;

        public Account Account { get; private set; }

        public List<Account> Accounts { get; private set; }

        private string url = ApiInstance.url + "/accounts";

        public void GetAccountButton()
        {
            //StartCoroutine(GetAccountRequest());
            StartCoroutine(GetAccountByIdRequest(Guid.Parse("94deb139-4c4b-4a2c-8087-bf47de2f67af")));
            //DemoJsonSerialize();
        }
        IEnumerator GetAccountRequest()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                Debug.Log(webRequest.url);
                string[] pages = url.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        break;
                }
            }
        }
        
        IEnumerator GetAccountByIdRequest(Guid accountId)
        {
            url += $"/{accountId}";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                Debug.Log(webRequest.url);
                string[] pages = url.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        ResponseObject response = JsonConvert.DeserializeObject<ResponseObject>(webRequest.downloadHandler.text);
                        //ResponseObject response = System.Text.Json.JsonSerializer.Deserialize<ResponseObject>(webRequest.downloadHandler.text, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        //string test = "{\"accountId\":\"94deb139-4c4b-4a2c-8087-bf47de2f67af\",\"userName\":\"test23\",\"email\":\"string3\",\"gender\":0,\"accountName\":\"string2\",\"role\":0,\"star\":0,\"isOnline\":false,\"fullname\":\"string2\",\"yob\":0,\"identityCardNumber\":\"string\",\"phoneNumber\":\"string\",\"createdTime\":\"2024-06-11T10:41:37.453\",\"characterItemId\":null,\"roomItemId\":null,\"accountStatus\":1}";
                        //Account response1 = System.Text.Json.JsonSerializer.Deserialize<Account>(test, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        //Account = response.value;
                        Account = response.Value;
                        Debug.Log(Account);
                        break;
                }
            }
        }

        public void DemoJsonSerialize()
        {
            Test test = new()
            {
                Name = "Test",
                Age = 20
            };
            Debug.Log("test " + test);
            string json = JsonConvert.SerializeObject(test);
            Debug.Log("json " +json);
            Test test2 = JsonConvert.DeserializeObject<Test>(json);
            Debug.Log("test2 " +test2);
        }

    }

    class Test
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
    
}
