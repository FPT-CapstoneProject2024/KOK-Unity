using KOK.Assets._Scripts;
using Newtonsoft.Json;
using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace KOK
{     
    public class ShopItemController : MonoBehaviour
    {
        public TMP_InputField itemIdInput;
        public TMP_InputField itemCodeInput;
        public TMP_InputField itemNameInput;
        public TMP_InputField itemDescriptionInput;
        public TMP_InputField itemTypeInput;
        public TMP_InputField itemPriceInput;
        public TMP_InputField itemStatusInput;
        public Toggle canExpireToggle;
        public Toggle canStackToggle;
        public TMP_Text notificationText;
        public float notificationFadeDuration;

        public string baseUrl = "https://localhost:7017/api/items";

        public void OnCreateSubmit()
        {
            Item newItem = new Item
            {
                ItemCode = itemCodeInput.text,
                ItemName = itemNameInput.text,
                ItemDescription = itemDescriptionInput.text,
                ItemType = int.Parse(itemTypeInput.text),
                ItemPrice = decimal.Parse(itemPriceInput.text),
                CanExpire = canExpireToggle.isOn,
                CanStack = canStackToggle.isOn,
                //CreatorId = ,  // login
            };

            StartCoroutine(PostItemToApi(newItem));
        }

        public void OnUpdateSubmit()
        {
            Guid itemId = Guid.Parse(itemIdInput.text);

            Item updateItem = new Item
            {               
                ItemCode = itemCodeInput.text,
                ItemName = itemNameInput.text,
                ItemDescription = itemDescriptionInput.text,
                ItemType = int.Parse(itemTypeInput.text),
                ItemPrice = decimal.Parse(itemPriceInput.text),
                ItemStatus = int.Parse(itemStatusInput.text),
                CanExpire = canExpireToggle.isOn,
                CanStack = canStackToggle.isOn
                //CreatorId = ,  // login
            };

            StartCoroutine(PutItemToApi(itemId, updateItem));           
        }

        public void OnDeleteSubmit()
        {
            Guid itemId = Guid.Parse(itemIdInput.text);
         
            StartCoroutine(DeleteItemFromApi(itemId));
        }

        private IEnumerator PostItemToApi(Item item)
        {
            string jsonData = JsonConvert.SerializeObject(item);

            UnityWebRequest request = new UnityWebRequest(baseUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Item successfully sent to the API.");
                Helper helper = new Helper();
                notificationText.text = "Item successfully sent to the API.";
                yield return helper.FadeTextToFullAlpha(notificationFadeDuration, notificationText);
                yield return new WaitForSeconds(2); // Wait for 2 seconds before starting to fade out
                yield return helper.FadeTextToZeroAlpha(notificationFadeDuration, notificationText);               
            }
            else
            {
                Debug.LogError("Error sending item to the API: " + request.error);
                Helper helper = new Helper();
                notificationText.text = "Error sending item to the API: " + request.error;
                yield return helper.FadeTextToFullAlpha(notificationFadeDuration, notificationText);
                yield return new WaitForSeconds(2); // Wait for 2 seconds before starting to fade out
                yield return helper.FadeTextToZeroAlpha(notificationFadeDuration, notificationText);               
            }
        }

        private IEnumerator PutItemToApi(Guid itemId, Item item)
        {
            string jsonData = JsonConvert.SerializeObject(item);
            string url = $"{baseUrl}/{itemId}";

            UnityWebRequest request = new UnityWebRequest(url, "PUT");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Item successfully sent to the API.");
            }
            else
            {
                Debug.LogError("Error sending item to the API: " + request.error);
            }
        }

        private IEnumerator DeleteItemFromApi(Guid itemId)
        {
            string url = $"{baseUrl}/{itemId}";

            UnityWebRequest request = new UnityWebRequest(url, "DELETE");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Item successfully deleted.");
            }
            else
            {
                Debug.LogError("Error deleting item: " + request.error);
            }
        }
    }
}  