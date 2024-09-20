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

    public class AllSongItemBinding : SongItemBinding
    {
        public SongDetail SongDetail;

        public void BindData(SongDetail songDetail)
        {
            SongDetail = songDetail;

            SongName.text = SongDetail.SongName;
            SongPrice.text = String.Format("{0:0.00}", SongDetail.Price);

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
                //BuySongButton.gameObject.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                var songParam = new BuySongParam()
                {
                    SongId = (Guid)SongDetail.SongId,
                    SongName = SongDetail.SongName,
                    Price = SongDetail.Price,
                    IsPurchased = SongDetail.isPurchased,
                    SongItem = gameObject
                };
                BuySongButton.interactable = true;
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
                SongItem = gameObject
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
            FindFirstObjectByType<SongHandler>().StartPurchaseSong(song);
        }

        public void OnFavoriteButtonToggle(FavoriteSongParam song, bool isOn)
        {
            DisableFavoriteToggle();
            FindFirstObjectByType<SongHandler>().OnFavoriteSongToggle(isOn, song);
        }
    }

    public class BuySongParam
    {
        public Guid SongId { get; set; }
        public string SongName { get; set; }
        public decimal Price { get; set; }
        public bool IsPurchased { get; set; }
        public GameObject SongItem { get; set; } = null;
    }

    public class FavoriteSongParam
    {
        public Guid SongId { get; set; }
        public string SongName { get; set; }
        public bool IsFavorited { get; set; }
        public GameObject SongItem { get; set; } = null;
    }
}
