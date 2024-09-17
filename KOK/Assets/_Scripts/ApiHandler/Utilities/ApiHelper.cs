using System.Text;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using KOK.ApiHandler.Controller;
using KOK.Assets._Scripts.ApiHandler.Controller;

namespace KOK.ApiHandler.Utilities
{
    [RequireComponent(typeof(AccountController))]
    [RequireComponent(typeof(AuthenticationController))]
    [RequireComponent(typeof(FavoriteSongController))]
    [RequireComponent(typeof(KaraokeRoomController))]
    [RequireComponent(typeof(PostCommentController))]
    [RequireComponent(typeof(PostController))]
    [RequireComponent(typeof(PostRatingController))]
    [RequireComponent(typeof(PurchasedSongController))]
    [RequireComponent(typeof(RecordingController))]
    [RequireComponent(typeof(ShopController))]
    [RequireComponent(typeof(ShopItemController))]
    [RequireComponent(typeof(SongController))]
    [RequireComponent (typeof(UpPackageController))]
    [RequireComponent(typeof(MoMoController))]
    [RequireComponent (typeof(InAppTransactionController))]
    [RequireComponent (typeof(ReportController))]
    [RequireComponent(typeof(NotificationController))]
    [RequireComponent(typeof(AccountItemController))]
    /// <summary>
    /// A singleton helper class to handle API calls in Unity.
    /// </summary>
    public class ApiHelper : Singleton<ApiHelper>
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private string jwtToken = string.Empty;

        private void Start()
        {
            jwtToken = string.Empty;
        }

        /// <summary>
        /// Sets the JWT token to be used for API requests.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        public void SetJwtToken(string token)
        {
            jwtToken = token;
            if (!string.IsNullOrEmpty(jwtToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
            }
        }

        public bool IsJwtTokenEmpty()
        {
            return string.IsNullOrEmpty(jwtToken);
        }

        #region CallApiByCoroutine

        /// <summary>
        /// Sends a GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to send the GET request to.</param>
        /// <param name="onSuccess">Callback invoked when the request is successful, with the response text as a parameter.</param>
        /// <param name="onError">Callback invoked when the request fails, with the error message as a parameter.</param>
        public void GetCoroutine(string url, Action<string> onSuccess, Action<string> onError)
        {
            StartCoroutine(GetRequest(url, onSuccess, onError));
        }

        /// <summary>
        /// Sends a POST request to the specified URL with the given JSON data.
        /// </summary>
        /// <param name="url">The URL to send the POST request to.</param>
        /// <param name="jsonData">The JSON data to include in the POST request.</param>
        /// <param name="onSuccess">Callback invoked when the request is successful, with the response text as a parameter.</param>
        /// <param name="onError">Callback invoked when the request fails, with the error message as a parameter.</param>
        public void PostCoroutine(string url, string jsonData, Action<string> onSuccess, Action<string> onError)
        {
            StartCoroutine(PostRequest(url, jsonData, onSuccess, onError));
        }

        /// <summary>
        /// Sends a PUT request to the specified URL with the given JSON data.
        /// </summary>
        /// <param name="url">The URL to send the PUT request to.</param>
        /// <param name="jsonData">The JSON data to include in the PUT request.</param>
        /// <param name="onSuccess">Callback invoked when the request is successful, with the response text as a parameter.</param>
        /// <param name="onError">Callback invoked when the request fails, with the error message as a parameter.</param>
        public void PutCoroutine(string url, string jsonData, Action<string> onSuccess, Action<string> onError)
        {
            StartCoroutine(PutRequest(url, jsonData, onSuccess, onError));
        }

        /// <summary>
        /// Sends a DELETE request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to send the DELETE request to.</param>
        /// <param name="onSuccess">Callback invoked when the request is successful, with the response text as a parameter.</param>
        /// <param name="onError">Callback invoked when the request fails, with the error message as a parameter.</param>
        public void DeleteCoroutine(string url, Action<string> onSuccess, Action<string> onError)
        {
            StartCoroutine(DeleteRequest(url, onSuccess, onError));
        }

        /// <summary>
        /// Coroutine to send a GET request.
        /// </summary>
        /// <param name="url">The URL to send the GET request to.</param>
        /// <param name="onSuccess">Callback invoked when the request is successful, with the response text as a parameter.</param>
        /// <param name="onError">Callback invoked when the request fails, with the error message as a parameter.</param>
        /// <returns>IEnumerator for coroutine handling.</returns>
        private IEnumerator GetRequest(string url, Action<string> onSuccess, Action<string> onError)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    webRequest.SetRequestHeader("Authorization", "Bearer " + jwtToken);
                }

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"{url} - GET - {webRequest.error}");
                    onError?.Invoke(webRequest.downloadHandler.text);
                }
                else
                {
                    onSuccess?.Invoke(webRequest.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// Coroutine to send a POST request.
        /// </summary>
        /// <param name="url">The URL to send the POST request to.</param>
        /// <param name="jsonData">The JSON data to include in the POST request.</param>
        /// <param name="onSuccess">Callback invoked when the request is successful, with the response text as a parameter.</param>
        /// <param name="onError">Callback invoked when the request fails, with the error message as a parameter.</param>
        /// <returns>IEnumerator for coroutine handling.</returns>
        private IEnumerator PostRequest(string url, string jsonData, Action<string> onSuccess, Action<string> onError)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                if (!string.IsNullOrEmpty(jwtToken))
                {
                    webRequest.SetRequestHeader("Authorization", "Bearer " + jwtToken);
                }

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"{url} - POST - {webRequest.error}");
                    onError?.Invoke(webRequest.downloadHandler.text);
                }
                else
                {
                    onSuccess?.Invoke(webRequest.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// Coroutine to send a PUT request.
        /// </summary>
        /// <param name="url">The URL to send the PUT request to.</param>
        /// <param name="jsonData">The JSON data to include in the PUT request.</param>
        /// <param name="onSuccess">Callback invoked when the request is successful, with the response text as a parameter.</param>
        /// <param name="onError">Callback invoked when the request fails, with the error message as a parameter.</param>
        /// <returns>IEnumerator for coroutine handling.</returns>
        private IEnumerator PutRequest(string url, string jsonData, Action<string> onSuccess, Action<string> onError)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "PUT"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                if (!string.IsNullOrEmpty(jwtToken))
                {
                    webRequest.SetRequestHeader("Authorization", "Bearer " + jwtToken);
                }

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"{url} - PUT - {webRequest.error}");
                    onError?.Invoke(webRequest.downloadHandler.text);
                }
                else
                {
                    onSuccess?.Invoke(webRequest.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// Coroutine to send a DELETE request.
        /// </summary>
        /// <param name="url">The URL to send the DELETE request to.</param>
        /// <param name="onSuccess">Callback invoked when the request is successful, with the response text as a parameter.</param>
        /// <param name="onError">Callback invoked when the request fails, with the error message as a parameter.</param>
        /// <returns>IEnumerator for coroutine handling.</returns>
        private IEnumerator DeleteRequest(string url, Action<string> onSuccess, Action<string> onError)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Delete(url))
            {
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    webRequest.SetRequestHeader("Authorization", "Bearer " + jwtToken);
                }

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"{url} - DELETE - {webRequest.error}");
                    onError?.Invoke(webRequest.error.ToString());
                }
                else
                {
                    onSuccess?.Invoke(webRequest.result.ToString());
                }
            }
        }

        #endregion

        #region CallApiByHttpClient

        /// <summary>
        /// Sends a GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to send the GET request to.</param>
        /// <returns>The response text from the GET request.</returns>
        public async Task<string> GetAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"GET Request Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Sends a POST request to the specified URL with the given JSON data.
        /// </summary>
        /// <param name="url">The URL to send the POST request to.</param>
        /// <param name="jsonData">The JSON data to include in the POST request.</param>
        /// <returns>The response text from the POST request.</returns>
        public async Task<string> PostAsync(string url, string jsonData)
        {
            try
            {
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"POST Request Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Sends a PUT request to the specified URL with the given JSON data.
        /// </summary>
        /// <param name="url">The URL to send the PUT request to.</param>
        /// <param name="jsonData">The JSON data to include in the PUT request.</param>
        /// <returns>The response text from the PUT request.</returns>
        public async Task<string> PutAsync(string url, string jsonData)
        {
            try
            {
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"PUT Request Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Sends a DELETE request to the specified URL.
        /// </summary>
        /// <param name="url">The URL to send the DELETE request to.</param>
        /// <returns>The response text from the DELETE request.</returns>
        public async Task<string> DeleteAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"DELETE Request Error: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}
