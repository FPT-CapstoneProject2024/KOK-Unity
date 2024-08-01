﻿using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class FavoriteSongHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public GameObject songContainer;
        [SerializeField] public GameObject songItemTemplate;
        [SerializeField] public TMP_Text songListMessage;
        [Header("Paging Components")]
        [SerializeField] public TMP_Text pagingDisplay;
        [SerializeField] public Button previousButton;
        [SerializeField] public Button nextButton;
        [Header("Search Components")]
        [SerializeField] public TMP_InputField searchInput;
        [Header("Preview Components")]
        [SerializeField] public GameObject songPreviewCanvas;
        [Header("Purchase Components")]
        [SerializeField] public GameObject songPurchaseCanvas;

        private FavoriteSongFilter filter;
        private int currentPage = 1;
        private int totalPage = 1;
        private string searchKeyword = string.Empty;

        private void OnEnable()
        {
            SetInitialState();
            LoadFavoriteSongs();
        }

        void Start()
        {
            //SetInitialState();
        }

        public void LoadFavoriteSongs()
        {
            try
            {
                var controller = ApiHelper.Instance.gameObject.GetComponent<FavoriteSongController>();
                controller.GetMemberFavoriteSongFilterCoroutine(filter, FavoriteSongOrderFilter.SongId, new PagingRequest()
                {
                    page = currentPage,
                }, OnLoadSongSuccess, OnLoadSongError);

            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                throw;
            }
            
        }

        public void OnLoadSongSuccess(DynamicResponseResult<FavoriteSong> responseResult)
        {
            ClearContainer();
            if (responseResult.Results == null || responseResult.Results.Count == 0)
            {
                SetSongMessage("Không tìm thấy bài hát yêu thích");
                pagingDisplay.text = $"{0}/{0}";
                return;
            }
            SetSongMessage(string.Empty);
            SetPagingData(responseResult);
            SpawnSongItem(responseResult.Results);
        }

        public void OnLoadSongError(DynamicResponseResult<FavoriteSong> responseResult)
        {
            ClearContainer();
            SetSongMessage("Không tìm thấy bài hát yêu thích");
            pagingDisplay.text = $"{0}/{0}";
        }

        private void SetSongMessage(string message)
        {
            songListMessage.text = message;
        }

        private void ClearContainer()
        {
            if (songContainer.transform.childCount > 0)
            {
                while (songContainer.transform.childCount > 0)
                {
                    DestroyImmediate(songContainer.transform.GetChild(0).gameObject);
                }
            }
        }

        private void SpawnSongItem(List<FavoriteSong> songs)
        {
            GameObject newSongItem;
            for (int i = 0; i < songs.Count; i++)
            {
                newSongItem = Instantiate(songItemTemplate, songContainer.transform);
                newSongItem.GetComponent<FavoriteSongItemBinding>().BindData(songs[i]);
            }
        }

        public void OnPreviousPageClick()
        {
            currentPage--;
            LoadFavoriteSongs();
        }

        public void OnNextPageClick()
        {
            currentPage++;
            LoadFavoriteSongs();
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

        private void SetPagingData(DynamicResponseResult<FavoriteSong> responseResult)
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
            currentPage = 1;
            searchKeyword = searchInput.text;
            filter.SongName = searchKeyword;
            LoadFavoriteSongs();
        }

        public void SetInitialState()
        {
            currentPage = 1;
            totalPage = 1;
            var accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            filter = new FavoriteSongFilter() 
            {
                MemberId = string.IsNullOrEmpty(accountId) ? Guid.Empty : Guid.Parse(accountId),
            };
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

        public void OnFavoriteSongToggle(bool isOn, FavoriteSongParam favoriteSongParam)
        {
            if (!isOn)
            {
                HandleDeleteFavoriteSong(favoriteSongParam);
            }
        }

        private void HandleDeleteFavoriteSong(FavoriteSongParam favoriteSongParam)
        {
            if (favoriteSongParam == null || !favoriteSongParam.IsFavorited)
            {
                Debug.Log("[Favorite Songs] Failed to delete favorite song - Invalid param");
                return;
            }
            string accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            if (string.IsNullOrEmpty(accountId))
            {
                Debug.Log("[Favorite Songs] Failed to delete favorite song - Player ID not found");
                return;
            }
            var request = new RemoveFavoriteSongRequest()
            {
                MemberId = Guid.Parse(accountId),
                SongId = favoriteSongParam.SongId,
            };
            ApiHelper.Instance.GetComponent<FavoriteSongController>().RemoveFavoriteSongCoroutine(request,
                (successValue) =>
                {
                    Debug.Log("[Favorite Songs] Successfully delete favorite song");
                    Destroy(favoriteSongParam.SongItem);
                },
                (errorValue) =>
                {
                    Debug.Log("[Favorite Songs] Failed to delete favorite song - Error api call");
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().TurnFavoriteToggleOn();
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().EnableFavoriteToggle();
                });
        }

        public void StartPurchaseSong(BuySongParam buySongParam)
        {
            songPurchaseCanvas.GetComponent<SongPurchaseHandler>().ShowPurchaseSongDialog(buySongParam);
        }
    }
}
