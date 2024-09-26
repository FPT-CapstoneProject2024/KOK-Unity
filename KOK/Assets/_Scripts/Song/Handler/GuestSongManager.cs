using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class GuestSongManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public GameObject songContainer;
        [SerializeField] public GameObject songItemTemplate;
        [SerializeField] public TMP_Text songListMessage;
        [SerializeField] LoadingManager loadingManager;
        [Header("Paging Components")]
        [SerializeField] public TMP_Text pagingDisplay;
        [SerializeField] public Button previousButton;
        [SerializeField] public Button nextButton;
        [Header("Search Components")]
        [SerializeField] public TMP_InputField searchInput;
        [Header("Preview Components")]
        [SerializeField] public GameObject songPreviewCanvas;

        private SongFilter filter;
        private int currentPage = 1;
        private int totalPage = 1;
        private string searchKeyword = string.Empty;

        private void Start()
        {
            SetInitialState();
            LoadSongs();
        }


        public void LoadSongs()
        {
            loadingManager.DisableUIElement();
            ClearContainer();
            string accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            ApiHelper.Instance.GetComponent<SongController>()
                .GetSongsFilterPagingCoroutine(
                filter,
                SongOrderFilter.SongName,
                new PagingRequest()
                {
                    page = currentPage,
                },
                OnLoadSongSuccess,
                OnLoadSongError);
        }

        public void OnLoadSongSuccess(DynamicResponseResult<SongDetail> responseResult)
        {
            ClearContainer();
            if (responseResult.Results == null || responseResult.Results.Count == 0)
            {
                SetSongMessage("Không tìm thấy bài hát");
                pagingDisplay.text = $"{0}/{0}";
                loadingManager.EnableUIElement();
                return;
            }
            SetSongMessage(string.Empty);
            SetPagingData(responseResult);
            SpawnSongItem(responseResult.Results);
            loadingManager.EnableUIElement();
        }

        public void OnLoadSongError(DynamicResponseResult<SongDetail> responseResult)
        {
            ClearContainer();
            SetSongMessage("Không tìm thấy bài hát");
            pagingDisplay.text = $"{0}/{0}";
            loadingManager.EnableUIElement();
        }

        private void SetSongMessage(string message)
        {
            songListMessage.text = message;
        }

        private void ClearContainer()
        {
            try
            {
                if (songContainer.transform.childCount > 0)
                {
                    while (songContainer.transform.childCount > 0)
                    {
                        DestroyImmediate(songContainer.transform.GetChild(0).gameObject);
                    }
                }
            }
            catch { }
        }

        private void SpawnSongItem(List<SongDetail> songs)
        {
            GameObject newSongItem;
            for (int i = 0; i < songs.Count; i++)
            {
                newSongItem = Instantiate(songItemTemplate, songContainer.transform);
                newSongItem.GetComponent<GuestSongBinding>().Init(songs[i], this);
            }
        }

        public void OnPreviousPageClick()
        {
            currentPage--;
            LoadSongs();
        }

        public void OnNextPageClick()
        {
            currentPage++;
            LoadSongs();
        }

        private void EnableButton(Button button)
        {
            button.enabled = true;
            button.gameObject.GetComponent<Image>().color = Color.white;
        }

        private void DisableButton(Button button)
        {
            button.enabled = false;
            button.gameObject.GetComponent<Image>().color = Color.gray;
        }

        private void SetPagingData(DynamicResponseResult<SongDetail> responseResult)
        {
            totalPage = (int)Math.Ceiling(responseResult.Metadata.Total / (double)responseResult.Metadata.Size);
            // Previous
            if (currentPage > 1)
            {
                EnableButton(previousButton);
            }
            else
            {
                DisableButton(previousButton);
            }
            // Next
            if (currentPage < totalPage)
            {
                EnableButton(nextButton);
            }
            else
            {
                DisableButton(nextButton);
            }
            // Paging display
            pagingDisplay.text = $"{currentPage}/{totalPage}";
        }

        public void OnSearchSongClick()
        {
            SetSongMessage(string.Empty);
            currentPage = 1;
            searchKeyword = searchInput.text;
            SetFilterKeyword();
            LoadSongs();
        }

        public void SetInitialState()
        {
            currentPage = 1;
            totalPage = 1;
            filter = new SongFilter();
            searchKeyword = string.Empty;
            searchInput.text = searchKeyword;
            ClearContainer();
            SetSongMessage(string.Empty);
            DisableButton(previousButton);
            DisableButton(nextButton);
            pagingDisplay.text = $"{0}/{0}";
        }

        public void StartPreviewSong(string songUrl)
        {
            songPreviewCanvas.GetComponent<PreviewSongHandler>().OnOpenPreviewSong(songUrl);
        }


        private void SetFilterKeyword()
        {
            filter.SongName = searchKeyword;
            filter.SongCode = searchKeyword;
            filter.GenreName = searchKeyword;
            filter.ArtistName = searchKeyword;
            filter.SingerName = searchKeyword;
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
