using KOK.UISprite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
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

        public void EnableBuySongButton()
        {
            BuySongButton.interactable = true;
            BuySongButton.gameObject.GetComponent<Image>().color = Color.white;
        }

        public void DisableBuySongButton()
        {
            BuySongButton.interactable = false;
            BuySongButton.gameObject.GetComponent<Image>().color = Color.gray;
        }

        public void DisableFavoriteToggle()
        {
            FavoriteSongToggle.interactable = false;
            FavoriteSongToggle.GetComponentInChildren<Image>().color = Color.gray;
        }

        public void EnableFavoriteToggle()
        {
            FavoriteSongToggle.interactable = true;
            FavoriteSongToggle.GetComponentInChildren<Image>().color = Color.white;
        }
    }
}
