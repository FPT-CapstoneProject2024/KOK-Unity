/*using Newtonsoft.Json;
using SU24SE069_PLATFORM_KAROKE_DataAccess.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace KOK
{
    public class ShopLayout : MonoBehaviour
    {
        [SerializeField] private List<Item> itemList = new List<Item>();

        private string baseUrl = "https://localhost:7017/api/items";
        private List<string> itemNames = new List<string>();
        public GameObject displayPanel;
        public GameObject displayButton;

        void Start()
        {
            StartCoroutine(GetData(baseUrl));
            //RefreshClicked();
        }

        void LayoutGenerate()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                //GameObject button = displayPanel.transform.GetChild(i).gameObject; 
                GameObject gameObj = Instantiate(displayButton, displayPanel.transform);
                gameObj.transform.GetChild(0).GetComponent<TMP_Text>().text = itemNames[i];

                int index = i;
                gameObj.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    ItemClicked(index);
                });
            }
             
            //Destroy(buttonDisplay);
        }

        void ItemClicked(int itemIndex)
        {
            Debug.Log("item" + itemIndex);
        }

        public void RefreshClicked()
        {
            for (int i = 0; i < displayPanel.transform.childCount; i++)
            {
                Destroy(displayPanel.transform.GetChild(i).gameObject);
            }

            StartCoroutine(GetData(baseUrl));
        }

        IEnumerator GetData(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;

                var responseObject = JsonConvert.DeserializeObject<ResponseObject>(response);
                var items = responseObject.Results; 
                 
                if (itemNames == null)
                {
                    itemNames = new List<string>();
                }
                if (itemList == null)
                {
                    itemList = new List<Item>();
                }

                // clear before adding
                itemNames.Clear();
                itemList.Clear();  

                foreach (Item item in items)
                {
                    itemNames.Add(item.ItemName);
                    itemList.Add(item);  
                }

                LayoutGenerate(); 

                Debug.Log(response);
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        public class ResponseObject
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public Metadata Metadata { get; set; }
            public List<Item> Results { get; set; }
        }

        public class Metadata
        {
            public int Page { get; set; }
            public int Size { get; set; }
            public int Total { get; set; }
        }
    }
}
*/