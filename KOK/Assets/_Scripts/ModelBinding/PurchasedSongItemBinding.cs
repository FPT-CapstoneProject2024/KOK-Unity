using KOK.ApiHandler.DTOModels;
using KOK.UISprite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class PurchasedSongItemBinding : MonoBehaviour
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

        public PurchasedSong PurchasedSong;

        public void BindData(PurchasedSong purchasedSong)
        {
            PurchasedSong = purchasedSong;

            SongName.text = PurchasedSong.SongName;
            SongPrice.text = PurchasedSong.Price.ToString();

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
                FavoriteSongToggle.isOn = true;
                ToggleSwapSprite.ToggleSprite();
            }
            else
            {
                FavoriteSongToggle.isOn = false;
                ToggleSwapSprite.ToggleSprite();
            }

            var favoriteSongParam = new ExtendedFavoriteSongParam()
            {
                SongId = PurchasedSong.SongId,
                SongName = PurchasedSong.SongName,
                IsFavorited = PurchasedSong.IsFavorite,
                FavoriteSongItem = gameObject
            };
            FavoriteSongToggle.AddEventListener(favoriteSongParam, OnFavoriteButtonToggle);
        }

        public void OnPlaySongClick(string songUrl)
        {
            Debug.Log("[Purchased Songs] Play song with url: " + songUrl);
            FindFirstObjectByType<PurchasedSongHandler>().StartPreviewSong(songUrl);
        }

        public void OnFavoriteButtonToggle(ExtendedFavoriteSongParam song, bool isOn)
        {
            Debug.Log($"[Purchased Songs] Toggle: {isOn}, song ID: {song.SongId}, song name: {song.SongName}, is favorited: {song.IsFavorited}");
            FindFirstObjectByType<PurchasedSongHandler>().OnFavoriteSongToggle(isOn, song);
        }

        public void TurnFavoriteToggleOn()
        {
            FavoriteSongToggle.isOn = true;
            ToggleSwapSprite.ToggleSprite();
        }

        public void TurnFavoriteToggleOff()
        {
            FavoriteSongToggle.isOn = false;
            ToggleSwapSprite.ToggleSprite();
        }
    }
}
