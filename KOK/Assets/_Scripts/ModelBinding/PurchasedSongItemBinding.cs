using KOK.ApiHandler.DTOModels;
using KOK.UISprite;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class PurchasedSongItemBinding : SongItemBinding
    {
        public PurchasedSong PurchasedSong;

        public void BindData(PurchasedSong purchasedSong)
        {
            PurchasedSong = purchasedSong;

            SongName.text = PurchasedSong.SongName;
            SongPrice.text = String.Format("{0:0.00}", PurchasedSong.Price);

            string artist = string.Empty;
            foreach (var a in PurchasedSong.Artists)
            {
                artist += a + " ";
            }
            SongArtist.text = artist.Trim();

            string singer = string.Empty;
            foreach (var s in PurchasedSong.Singers)
            {
                singer += s + " ";
            }
            SongSinger.text = singer.Trim();

            string genre = string.Empty;
            foreach (var g in PurchasedSong.Genres)
            {
                genre += g + " ";
            }
            SongGenre.text = genre.Trim();

            PlaySongButton.AddEventListener(PurchasedSong.SongUrl, OnPlaySongClick);

            BuySongButton.interactable = false;
            BuySongButton.gameObject.GetComponent<Image>().color = Color.gray;

            if (PurchasedSong.IsFavorite)
            {
                TurnFavoriteToggleOn();
            }
            else
            {
                TurnFavoriteToggleOff();
            }

            var favoriteSongParam = new FavoriteSongParam()
            {
                SongId = PurchasedSong.SongId,
                SongName = PurchasedSong.SongName,
                IsFavorited = PurchasedSong.IsFavorite,
                SongItem = gameObject
            };
            FavoriteSongToggle.AddEventListener(favoriteSongParam, OnFavoriteButtonToggle);
        }

        public void OnPlaySongClick(string songUrl)
        {
            FindFirstObjectByType<PurchasedSongHandler>().StartPreviewSong(songUrl);
        }

        public void OnFavoriteButtonToggle(FavoriteSongParam song, bool isOn)
        {
            DisableFavoriteToggle();
            FindFirstObjectByType<PurchasedSongHandler>().OnFavoriteSongToggle(isOn, song);
        }
    }
}
