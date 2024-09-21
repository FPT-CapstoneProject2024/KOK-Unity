using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class SongHandler : MonoBehaviour
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
        [SerializeField] SongPurchaseHandler songPurchaseHandler;
        [SerializeField] LoadingManager loadingManager;
        [SerializeField] TMP_Dropdown categoryDropdown;

        private SongFilter filter;
        private int currentPage = 1;
        private int totalPage = 1;
        private string searchKeyword = string.Empty;

        private void Start()
        {
            if (SystemNavigation.IsToPurchasedSong())
            {
                categoryDropdown = GameObject.Find("CategoryDropdown").GetComponent<TMP_Dropdown>();
                categoryDropdown.value = 1;
                return;
            }
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
                !string.IsNullOrEmpty(accountId) ? accountId : Guid.Empty.ToString(), 
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
                newSongItem.GetComponent<AllSongItemBinding>().BindData(songs[i]);
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

        private void HandleAddFavoriteSong(FavoriteSongParam favoriteSongParam)
        {
            if (favoriteSongParam == null || favoriteSongParam.IsFavorited)
            {
                Debug.Log("[All Songs] Failed to add favorite song");
                return;
            }
            string accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            if (string.IsNullOrEmpty(accountId))
            {
                Debug.Log("[All Songs] Failed to add favorite song - Player ID not found");
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
                    Debug.Log("[All Songs] Successfully add favorite song");
                    favoriteSongParam.IsFavorited = true;
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().EnableFavoriteToggle();
                },
                (errorValue) =>
                {
                    Debug.Log("[All Songs] Failed to add favorite song - Error api call");
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().TurnFavoriteToggleOff();
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().EnableFavoriteToggle();
                });
        }

        private void HandleDeleteFavoriteSong(FavoriteSongParam favoriteSongParam)
        {
            if (favoriteSongParam == null || !favoriteSongParam.IsFavorited)
            {
                Debug.Log("[All Songs] Failed to delete favorite song - Invalid param");
                return;
            }
            string accountId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
            if (string.IsNullOrEmpty(accountId))
            {
                Debug.Log("[All Songs] Failed to delete favorite song - Player ID not found");
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
                    Debug.Log("[All Songs] Successfully delete favorite song");
                    favoriteSongParam.IsFavorited = false;
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().EnableFavoriteToggle();
                },
                (errorValue) =>
                {
                    Debug.Log("[All Songs] Failed to delete favorite song - Error api call");
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().TurnFavoriteToggleOn();
                    favoriteSongParam.SongItem.GetComponent<SongItemBinding>().EnableFavoriteToggle();
                });
        }

        public void StartPurchaseSong(BuySongParam buySongParam)
        {
            songPurchaseCanvas.SetActive(true);
            songPurchaseHandler.ShowPurchaseSongDialog(buySongParam);
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
