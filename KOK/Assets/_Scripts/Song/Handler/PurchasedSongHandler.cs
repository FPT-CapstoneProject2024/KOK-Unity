using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class PurchasedSongHandler : MonoBehaviour
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
        [SerializeField] LoadingManager loadingManager;

        private PurchasedSongFilter filter;
        private int currentPage = 1;
        private int totalPage = 1;
        private string searchKeyword = string.Empty;

        private void OnEnable()
        {
            SetInitialState();
            LoadPurchasedSongs();
        }

        public void LoadPurchasedSongs()
        {
            loadingManager.DisableUIElement();
            ClearContainer();
            var controller = ApiHelper.Instance.gameObject.GetComponent<PurchasedSongController>();
            controller.GetMemberPurchasedSongFilterCoroutine(filter, PurchasedSongOrderFilter.PurchaseDate, new PagingRequest()
            {
                page = currentPage,
                OrderType = SortOrder.Descending
            }, OnLoadSongSuccess, OnLoadSongError);
        }

        public void OnLoadSongSuccess(DynamicResponseResult<PurchasedSong> responseResult)
        {
            ClearContainer();
            if (responseResult.Results == null || responseResult.Results.Count == 0)
            {
                SetSongMessage("Không tìm thấy bài hát đã mua");
                pagingDisplay.text = $"{0}/{0}";
                loadingManager.EnableUIElement();
                return;
            }
            SetSongMessage(string.Empty);
            SetPagingData(responseResult);
            SpawnSongItem(responseResult.Results);
            loadingManager.EnableUIElement();
        }

        public void OnLoadSongError(DynamicResponseResult<PurchasedSong> responseResult)
        {
            ClearContainer();
            SetSongMessage("Không tìm thấy bài hát đã mua");
            pagingDisplay.text = $"{0}/{0}";
            loadingManager.EnableUIElement();
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

        private void SpawnSongItem(List<PurchasedSong> songs)
        {
            GameObject newSongItem;
            for (int i = 0; i < songs.Count; i++)
            {
                newSongItem = Instantiate(songItemTemplate, songContainer.transform);
                newSongItem.GetComponent<PurchasedSongItemBinding>().BindData(songs[i]);
            }
        }

        public void OnPreviousPageClick()
        {
            currentPage--;
            LoadPurchasedSongs();
        }

        public void OnNextPageClick()
        {
            currentPage++;
            LoadPurchasedSongs();
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

        private void SetPagingData(DynamicResponseResult<PurchasedSong> responseResult)
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
            filter.SongName = searchKeyword;
            LoadPurchasedSongs();
        }

        public void SetInitialState()
        {
            currentPage = 1;
            totalPage = 1;
            var accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            filter = new PurchasedSongFilter()
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
            if (isOn)
            {
                HandleAddFavoriteSong(favoriteSongParam);
            }
            else
            {
                HandleDeleteFavoriteSong(favoriteSongParam);
            }
        }

        private void HandleDeleteFavoriteSong(FavoriteSongParam favoriteSongParam)
        {
            if (favoriteSongParam == null || !favoriteSongParam.IsFavorited)
            {
                Debug.Log("[Purchased Songs] Failed to delete favorite song - Invalid param");
                return;
            }
            string accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            if (string.IsNullOrEmpty(accountId))
            {
                Debug.Log("[Purchased Songs] Failed to delete favorite song - Player ID not found");
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
                    Debug.Log("[Purchased Songs] Successfully delete favorite song");
                    favoriteSongParam.IsFavorited = false;
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().EnableFavoriteToggle();
                },
                (errorValue) =>
                {
                    Debug.Log("[Purchased Songs] Failed to delete favorite song - Error api call");
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().TurnFavoriteToggleOn();
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().EnableFavoriteToggle();
                });
        }

        private void HandleAddFavoriteSong(FavoriteSongParam favoriteSongParam)
        {
            if (favoriteSongParam == null || favoriteSongParam.IsFavorited)
            {
                Debug.Log("[Purchased Songs] Failed to add favorite song");
                return;
            }
            string accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            if (string.IsNullOrEmpty(accountId))
            {
                Debug.Log("[Purchased Songs] Failed to add favorite song - Player ID not found");
                return;
            }
            var request = new AddFavoriteSongRequest()
            {
                MemberId = Guid.Parse(accountId),
                SongId = favoriteSongParam.SongId,
            };
            ApiHelper.Instance.GetComponent<FavoriteSongController>().AddFavoriteSongCoroutine(request,
                (successValue) =>
                {
                    Debug.Log("[Purchased Songs] Successfully add favorite song");
                    favoriteSongParam.IsFavorited = true;
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().EnableFavoriteToggle();
                },
                (errorValue) =>
                {
                    Debug.Log("[Purchased Songs] Failed to add favorite song - Error api call");
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().TurnFavoriteToggleOff();
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().EnableFavoriteToggle();
                });
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
