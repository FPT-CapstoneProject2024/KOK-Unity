using KOK.ApiHandler.DTOModels;
using KOK.UISprite;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class FavoriteSongItemBinding : SongItemBinding
    {
        public FavoriteSong FavoriteSong;

        public void BindData(FavoriteSong favoriteSong)
        {
            FavoriteSong = favoriteSong;

            SongName.text = FavoriteSong.SongName;
            SongPrice.text = String.Format("{0:0.00}", FavoriteSong.Price);

            string artist = string.Empty;
            foreach (var a in FavoriteSong.Artists)
            {
                artist += a + " ";
            }
            SongArtist.text = artist.Trim();

            string singer = string.Empty;
            foreach (var s in FavoriteSong.Singers)
            {
                singer += s + " ";
            }
            SongSinger.text = singer.Trim();

            string genre = string.Empty;
            foreach (var g in FavoriteSong.Genres)
            {
                genre += g + " ";
            }
            SongGenre.text = genre.Trim();

            PlaySongButton.AddEventListener(FavoriteSong.SongUrl, OnPlaySongClick);

            if (FavoriteSong.IsPurchased)
            {
                DisableBuySongButton();
            }
            else
            {
                var songParam = new BuySongParam()
                {
                    SongId = (Guid)FavoriteSong.SongId,
                    SongName = FavoriteSong.SongName,
                    Price = FavoriteSong.Price,
                    SongItem = gameObject
                };
                BuySongButton.AddEventListener(songParam, OnBuySongClick);
            }

            TurnFavoriteToggleOn();

            var favoriteSongParam = new FavoriteSongParam()
            {
                SongId = (Guid)FavoriteSong.SongId,
                SongName = FavoriteSong.SongName,
                IsFavorited = true,
                SongItem = gameObject
            };
            FavoriteSongToggle.AddEventListener(favoriteSongParam, OnFavoriteButtonToggle);
        }

        public void OnPlaySongClick(string songUrl)
        {
            FindFirstObjectByType<FavoriteSongHandler>().StartPreviewSong(songUrl);
        }

        public void OnBuySongClick(BuySongParam song)
        {
            FindFirstObjectByType<FavoriteSongHandler>().StartPurchaseSong(song);
        }

        public void OnFavoriteButtonToggle(FavoriteSongParam song, bool isOn)
        {
            DisableFavoriteToggle();
            FindFirstObjectByType<FavoriteSongHandler>().OnFavoriteSongToggle(isOn, song);
        }
    }
}
