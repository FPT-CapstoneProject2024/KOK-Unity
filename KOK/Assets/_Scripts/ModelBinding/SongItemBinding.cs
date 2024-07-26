using KOK.ApiHandler.DTOModels;
using KOK.UISprite;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public static class ButtonExtension
    {
        public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
        {
            button.onClick.AddListener(() =>
            {
                OnClick(param);
            });
        }
    }

    public static class ToggleExtension
    {
        public static void AddEventListener<T>(this Toggle toggle, T param, Action<T, bool> OnValueChanged)
        {
            toggle.onValueChanged.AddListener((bool isOn) =>
            {
                OnValueChanged(param, isOn);
            });
        }
    }

    public class SongItemBinding : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] public TMP_Text SongName;
        [SerializeField] public TMP_Text SongArtist;
        [SerializeField] public TMP_Text SongSinger;
        [SerializeField] public TMP_Text SongGenre;
        [SerializeField] public TMP_Text SongPrice;
        [SerializeField] public Button BuySongButton;
        [SerializeField] public Button PlaySongButton;
        [SerializeField] public ToggleSwapSprite ToggleSwapSprite;
        [SerializeField] public Toggle FavoriteSongToggle;

        public SongDetail SongDetail;

        public void BindData(SongDetail songDetail)
        {
            SongDetail = songDetail;

            SongName.text = SongDetail.SongName;
            SongPrice.text = SongDetail.Price.ToString();
            string artist = string.Empty;
            foreach (var a in SongDetail.Artist)
            {
                artist += a + " ";
            }
            SongArtist.text = artist.Trim();
            string singer = string.Empty;
            foreach (var s in SongDetail.Singer)
            {
                singer += s + " ";
            }
            SongSinger.text = singer.Trim();
            string genre = string.Empty;
            foreach (var g in SongDetail.Genre)
            {
                genre += g + " ";
            }
            SongGenre.text = genre.Trim();

            PlaySongButton.AddEventListener(SongDetail.SongUrl, OnPlaySongClick);

            if (SongDetail.isPurchased)
            {
                BuySongButton.interactable = false;
                BuySongButton.gameObject.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                var songParam = new BuySongParam()
                {
                    SongId = (Guid)SongDetail.SongId,
                    SongName = SongDetail.SongName,
                    Price = SongDetail.Price,
                    IsPurchased = SongDetail.isPurchased,
                };
                BuySongButton.AddEventListener(songParam, OnBuySongClick);
            }

            if (SongDetail.isFavorite)
            {
                FavoriteSongToggle.isOn = true;
                ToggleSwapSprite.ToggleSprite();
            }
            else
            {
                FavoriteSongToggle.isOn = false;
                ToggleSwapSprite.ToggleSprite();
            }
            var favoriteSongParam = new FavoriteSongParam()
            {
                SongId = (Guid)SongDetail.SongId,
                SongName = SongDetail.SongName,
                IsFavorited = SongDetail.isFavorite,
            };
            FavoriteSongToggle.AddEventListener(favoriteSongParam, OnFavoriteButtonToggle);
        }

        public void OnPlaySongClick(string songUrl)
        {
            Debug.Log("Play song with url: " + songUrl);
            FindFirstObjectByType<SongHandler>().StartPreviewSong(songUrl);
        }

        public void OnBuySongClick(BuySongParam song)
        {
            Debug.Log($"Member buy song [{song.SongName}] with ID [{song.SongId}] cost [{song.Price}] UP.");
        }

        public void OnFavoriteButtonToggle(FavoriteSongParam song, bool isOn)
        {
            Debug.Log($"[All Songs] Toggle: {isOn}, song ID: {song.SongId}, song name: {song.SongName}, is favorited: {song.IsFavorited}");
            FindFirstObjectByType<SongHandler>().OnFavoriteSongToggle(isOn, song);
        }
    }

    public class BuySongParam
    {
        public Guid SongId { get; set; }
        public string SongName { get; set; }
        public decimal Price { get; set; }
        public bool IsPurchased { get; set; }
    }

    public class FavoriteSongParam
    {
        public Guid SongId { get; set; }
        public string SongName { get; set; }
        public bool IsFavorited { get; set; }
    }
}
