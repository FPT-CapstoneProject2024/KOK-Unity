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
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Songs;
using KOK.ApiHandler.Context;
using KOK.Assets._Scripts.Shop;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.Utilities;

namespace KOK
{
    public class ShopSongLayout : MonoBehaviour
    {
        private List<SongDetail> songList = new List<SongDetail>();
        private SongController shopSongController;
        public SongPreviewDisplay songPreview;

        public GameObject displayPanel;
        public GameObject displayButton;
        private string songResourceUrl = string.Empty;

        private void Start()
        {
            songResourceUrl = KokApiContext.KOK_Host_Url + KokApiContext.Songs_Resource;
                       
            GetSongsFilterPaging();
        }

        public void GetSongsFilterPaging()
        {
            songList = new();
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<SongController>()
                .GetSongsFilterPagingCoroutine( new SongFilter(),
                                                new SongOrderFilter(),
                                                new PagingRequest(),
                                                LayoutGenerate,
                                                OnError
                );
        }

        private void OnSuccess(List<SongDetail> songList)
        {
            Debug.LogError(songList);
        }

        private void LayoutGenerate(List<SongDetail> songList)
        {
            for (int i = 0; i < songList.Count; i++)
            {
                GameObject gameObj = Instantiate(displayButton, displayPanel.transform);
                gameObj.transform.GetChild(0).GetComponent<TMP_Text>().text = songList[i].SongCode;

                int index = i;
                Debug.Log(songList[i].SongId);
                var song = songList[i];
                gameObj.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    SongClicked(song);
                });
            }
        }

        private void SongClicked(SongDetail song)
        {
            songPreview.ShowPopup(song);
            Debug.Log("Song clicked: " + song.SongId);
        }

        public void CloseClicked()
        {
            songPreview.HidePopup();
        }

        public void RefreshClicked()
        {
            for (int i = 0; i < displayPanel.transform.childCount; i++)
            {
                GameObject child = displayPanel.transform.GetChild(i).gameObject;
                Destroy(child);
            }

            GetSongsFilterPaging();
        }

        /*private IEnumerator GetSongsFilterPagingCoroutine(SongFilter filter, SongOrderFilter orderFilter, PagingRequest paging)
        {
            string url = QueryHelper.BuildUrl(songResourceUrl, shopSongController.GenerateSongQueryParams(filter, orderFilter, paging));

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
        }*/

   /*     private void OnSuccessTest(List<SongDetail> songDetails)
        {
            songCodes.Clear();
            songList.Clear();

            foreach (SongDetail song in songDetails)
            {
                songCodes.Add(song.SongCode);
                songList.Add(song);
            }

            LayoutGenerate();
            //Debug.Log(response);
        }*/

        private void OnError(string error)
        {
            Debug.LogError(error);
        }
    }
}
