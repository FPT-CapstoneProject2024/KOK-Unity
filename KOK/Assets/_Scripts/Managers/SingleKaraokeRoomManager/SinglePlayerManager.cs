using Fusion;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using WebSocketSharp;

namespace KOK
{
    public class SinglePlayerManager : MonoBehaviour
    {
        [SerializeField] List<SongDetail> songList = new();
        [SerializeField] List<SongDetail> queueSongList = new();
        [SerializeField] List<SongDetail> purchasedSongList = new();
        [SerializeField] List<FavoriteSong> favoriteSongList = new();

        [SerializeField] TMP_InputField searchSongInput;
        [SerializeField] GameObject searchSongPanelContent;
        [SerializeField] GameObject queueSongPanelContent;
        [SerializeField] GameObject searchSongHolderPrefab;
        [SerializeField] GameObject queueSongHolderPrefab;

        [SerializeField] Toggle favToggle;

        [SerializeField] VideoPlayer videoPlayer;
        void Start()
        {
            StartCoroutine(LoadSong());
        }

        IEnumerator LoadSong()
        {
            yield return new WaitForSeconds(1);
            //Call api load song
            songList = new();

            FindAnyObjectByType<ApiHelper>().gameObject
                        .GetComponent<SongController>()
                        .GetSongsFilterPagingCoroutine(new SongFilter(),
                                                        SongOrderFilter.SongName,
                                                        new PagingRequest(),
                                                        (list) => { songList = list; UpdateSearchSongUI(); },
                                                        (ex) => Debug.LogError(ex));

            //Call api load purchased song
            purchasedSongList = new();

            //favoriteSongList = new();
            //FindAnyObjectByType<ApiHelper>().gameObject
            //            .GetComponent<FavoriteSongController>()
            //            .GetMemberFavoriteSongCoroutine(new FavoriteSongFilter() { MemberId = new(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)) },
            //                                            FavoriteSongOrderFilter.SongId,
            //                                            new PagingRequest(),
            //                                            (list) => { favoriteSongList = list.Results; 
            //                                                //UpdateSearchSongUI(); 
            //                                            },
            //                                            (ex) => Debug.LogError(ex));

            //Load song from devide
        }

        IEnumerator LoadFavoriteSong()
        {
            yield return new WaitForSeconds (1);
            purchasedSongList = new();

            favoriteSongList = new();
            FindAnyObjectByType<ApiHelper>().gameObject
                        .GetComponent<FavoriteSongController>()
                        .GetMemberFavoriteSongCoroutine(new FavoriteSongFilter() { MemberId = new(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)) },
                                                        FavoriteSongOrderFilter.SongId,
                                                        new PagingRequest(),
                                                        (list) => { 
                                                            favoriteSongList = list.Results; 
                                                            //UpdateSearchSongUI(); 
                                                        },
                                                        (ex) => Debug.LogError(ex));
        }

        public void UpdateSearchSongUI()
        {
            string searchKeyword = searchSongInput.text;
            List<SongDetail> songListSearch = new();
            if (!searchKeyword.IsNullOrEmpty())
            {
                songListSearch = songList.Where(s => s.SongName.ContainsInsensitive(searchKeyword)
                                            || s.Artist.ToCommaSeparatedString().ContainsInsensitive(searchKeyword)).ToList();
            }
            else
            {
                songListSearch = songList;
            }
            //if (favToggle.isOn)
            //{
            //    songListSearch = songListSearch.Where(s => favoriteSongList.FirstOrDefault(f => f.SongId == s.SongId) != null).ToList();
            //}
            foreach (Transform child in searchSongPanelContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var song in songListSearch)
            {
                try
                {
                    GameObject songHolder = Instantiate(searchSongHolderPrefab, searchSongPanelContent.transform);
                    songHolder.name = song.SongName;
                    songHolder.GetComponentInChildren<SongBinding>().BindingData(song);
                    songHolder.transform.GetChild(0).name = song.SongId.ToString();
                    //var favToggle = songHolder.transform.Find("FavouriteToggle").GetComponent<Toggle>();
                    //if (favoriteSongList.FirstOrDefault(f => f.SongId == song.SongId) != null)
                    //{
                    //    favToggle.isOn = true;
                    //}
                    //else
                    //{
                    //    favToggle.isOn = false;
                    //}
                }
                catch { }
            }
        }
        public void UpdateQueueSongUI()
        {

            foreach (Transform child in queueSongPanelContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var song in queueSongList)
            {
                try
                {
                    GameObject songHolder = Instantiate(queueSongHolderPrefab, queueSongPanelContent.transform);
                    songHolder.name = song.SongName;
                    songHolder.GetComponentInChildren<SongBinding>().BindingData(song);
                    songHolder.transform.GetChild(0).name = song.SongId.ToString();
                }
                catch { }
            }
        }

        

        public SongDetail GetSongBySongCode(string songCode)
        {
            return songList.Find(x => x.SongCode.Equals(songCode.ToString()));
        }
        public void CreateAddSongPopUp()
        {

        }
        public void AddSongToQueue(string songCode)
        {
            queueSongList.Add(GetSongBySongCode(songCode));
        }


        public void PrioritizeSongToQueue(string songCode)
        {
            queueSongList.Insert(0, GetSongBySongCode(songCode));
        }

        public void RemoveSongFromQueue()
        {

        }

        public void ClearQueue()
        {
            queueSongList.Clear();
        }

        public void OnPlaySongVideoButtonClick()
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            else
            {
                videoPlayer.url = queueSongList[0].SongUrl;
                videoPlayer.Play();
                queueSongList.RemoveAt(0);
            }
        }

        public void OnPlayNextSongVideoButtonClick()
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            videoPlayer.url = queueSongList[0].SongUrl;
            videoPlayer.Play();
            queueSongList.RemoveAt(0);
        }

        public void RefreshFavSongList()
        {
            //StartCoroutine(LoadFavoriteSong());
            //favoriteSongList = new();
            //FindAnyObjectByType<ApiHelper>().gameObject
            //            .GetComponent<FavoriteSongController>()
            //            .GetMemberFavoriteSongCoroutine(new FavoriteSongFilter() { MemberId = new(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)) },
            //                                            FavoriteSongOrderFilter.SongId,
            //                                            new PagingRequest(),
            //                                            (list) => {
            //                                                favoriteSongList = list.Results;
            //                                                UpdateSearchSongUI();
            //                                            },
            //                                            (ex) => Debug.LogError(ex));
        }
        private void CreateRecording()
        {
        }


        public void LeaveRoom()
        {
            //Change to home page scene
        }
    }
}
