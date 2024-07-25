using KOK.ApiHandler.DTOModels;
using KOK.UISprite;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class FavoriteSongItemBinding : MonoBehaviour
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

        public FavoriteSong FavoriteSong;

        public void BindData(FavoriteSong favoriteSong)
        {
            FavoriteSong = favoriteSong;

            SongName.text = FavoriteSong.SongName;
            SongPrice.text = FavoriteSong.Price.ToString();

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
                BuySongButton.interactable = false;
                BuySongButton.gameObject.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                var songParam = new BuySongParam()
                {
                    SongId = (Guid)FavoriteSong.SongId,
                    SongName = FavoriteSong.SongName,
                    Price = FavoriteSong.Price,
                };
                BuySongButton.AddEventListener(songParam, OnBuySongClick);
            }

            FavoriteSongToggle.isOn = true;
            ToggleSwapSprite.ToggleSprite();

        }

        public void OnPlaySongClick(string songUrl)
        {
            Debug.Log("[Favorite Songs] Play song with url: " + songUrl);
            FindFirstObjectByType<FavoriteSongHandler>().StartPreviewSong(songUrl);
        }

        public void OnBuySongClick(BuySongParam song)
        {
            Debug.Log($"[Favorite Songs] Member buy song [{song.SongName}] with ID [{song.SongId}] cost [{song.Price}] UP.");
        }
    }
}
