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
using System.Net;

namespace KOK.ApiHandler.Controller
{
    public class AccountController : MonoBehaviour
    {
        [SerializeField] private GameObject accountPanel;

        public Account Account { get;  set; }

        public List<Account> Accounts { get;  set; }

        private string url = ApiInstance.url + "/accounts";

        public void GetAccountButton()
        {
            StartCoroutine(DemoCoroutine());
            
        }
        public IEnumerator DemoCoroutine()
        {
            yield return StartCoroutine(GetAccountByIdRequest(Guid.Parse("94deb139-4c4b-4a2c-8087-bf47de2f67af")));

            Account.accountName = "ffffffffffffffffffffffff";
            Account.password = "2113r12r";
            Debug.Log(Account);
            yield return StartCoroutine(PostAccount(Account));
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
                        ResponseObject<Account> response = JsonConvert.DeserializeObject<ResponseObject<Account>>(webRequest.downloadHandler.text);
                        Debug.LogError(response.Message);
                        Account = response.Value;
                        break;
                }
            }
        }

        IEnumerator PostAccount(Account account)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(url, account.ToString(), "application/json"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    ResponseObject<string> response = JsonConvert.DeserializeObject<ResponseObject<string>>(www.downloadHandler.text);
                    Debug.LogError(www.error + response.Message);
                }
                else
                {
                    Debug.Log("Posts complete!");
                }
            }
        }

        IEnumerator PutAccount(Account account)
        {
            byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
            using (UnityWebRequest www = UnityWebRequest.Put("https://www.my-server.com/upload", myData))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("Upload complete!");
                }
            }
        }




    }

  


}
