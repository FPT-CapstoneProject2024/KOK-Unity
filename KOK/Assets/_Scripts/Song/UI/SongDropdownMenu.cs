using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KOK
{
    public class SongDropdownMenu : MonoBehaviour
    {
        [Header("Canvases")]
        [SerializeField] private GameObject allSongsCanvas;
        [SerializeField] private GameObject purchasedSongsCanvas;
        [SerializeField] private GameObject favoriteSongsCanvas;

        private TMP_Dropdown dropdownMenu;

        void Start()
        {
            dropdownMenu = GetComponent<TMP_Dropdown>();
        }

        private void SwitchToAllSong()
        {
            allSongsCanvas.SetActive(true);
            allSongsCanvas.GetComponentInChildren<SongHandler>().SetInitialState();
            purchasedSongsCanvas.SetActive(false);
            favoriteSongsCanvas.SetActive(false);
        }

        private void SwitchToPurchased()
        {
            purchasedSongsCanvas.SetActive(true);
            allSongsCanvas.SetActive(false);
            favoriteSongsCanvas.SetActive(false);
        }

        private void SwitchToFavorite()
        {
            favoriteSongsCanvas.SetActive(true);
            allSongsCanvas.SetActive(false);
            purchasedSongsCanvas.SetActive(false);
        }

        private void SwitchToHome()
        {
            SceneManager.LoadScene("Home");
        }

        public void OnMenuValueChange()
        {
            switch (dropdownMenu.value)
            {
                case 0:
                    SwitchToAllSong();
                    break;
                case 1:
                    SwitchToPurchased();
                    break;
                case 2:
                    SwitchToFavorite();
                    break;
                case 3:
                    SwitchToHome();
                    break;
            }
        }
    }
}
