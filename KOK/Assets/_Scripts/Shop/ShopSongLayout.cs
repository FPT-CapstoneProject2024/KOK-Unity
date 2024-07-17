using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using KOK.ApiHandler.DTOModels;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Song;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Song;
using KOK.ApiHandler.Context;
using KOK.Assets._Scripts.Shop;

namespace KOK
{
    public class ShopSongLayout : MonoBehaviour
    {
        [SerializeField] private List<Song> songList = new List<Song>();
        private ShopSongController shopSongController;
        public SongPreviewDisplay songPreview;

        private List<string> songCodes = new List<string>();
        public GameObject displayPanel;
        public GameObject displayButton;
        private string songResourceUrl = string.Empty;

        private void Start()
        {
            songResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Songs_Resource;
            shopSongController = new ShopSongController();
            StartCoroutine(GetSongsFilterPagingCoroutine(new SongFilter(), new SongOrderFilter(), new PagingRequest()));
        }

        private void LayoutGenerate()
        {
            for (int i = 0; i < songList.Count; i++)
            {
                GameObject gameObj = Instantiate(displayButton, displayPanel.transform);
                gameObj.transform.GetChild(0).GetComponent<TMP_Text>().text = songCodes[i];

                int index = i;
                Debug.Log(songList[i].SongId);
                var song = songList[i];
                gameObj.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    SongClicked(song);
                });
            }
        }

        private void SongClicked(Song song)
        {
            songPreview.ShowPopup(song);
            Debug.Log("Song clicked: " + song.SongId);
        }

        public void RefreshClicked()
        {
            for (int i = 0; i < displayPanel.transform.childCount; i++)
            {
                GameObject child = displayPanel.transform.GetChild(i).gameObject;
                Destroy(child);
            }

            StartCoroutine(GetSongsFilterPagingCoroutine(new SongFilter(), new SongOrderFilter(), new PagingRequest()));
        }

        private IEnumerator GetSongsFilterPagingCoroutine(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging)
        {
            string url = shopSongController.BuildUrl(songResourceUrl, shopSongController.GenerateSongQueryParams(filter, orderFilter, paging));

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                var responseObject = JsonConvert.DeserializeObject<DynamicResponseResult<Song>>(response);
                var songs = responseObject.Results;

                songCodes.Clear();
                songList.Clear();

                foreach (Song song in songs)
                {
                    songCodes.Add(song.SongCode);
                    songList.Add(song);
                }

                LayoutGenerate();
                Debug.Log(response);
            }
            else
            {
                Debug.LogError(request.error);
            }
        }
    }
}
