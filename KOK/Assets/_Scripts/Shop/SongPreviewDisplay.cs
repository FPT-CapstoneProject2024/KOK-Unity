using KOK.ApiHandler.DTOModels;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Songs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace KOK.Assets._Scripts.Shop
{
    public class SongPreviewDisplay : MonoBehaviour
    {
        public TMP_Text songNameDisplay;
        public TMP_Text songDescriptionDisplay;
        public TMP_Text songBuyPriceDisplay;
        public VideoPlayer videoPlayer;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void ShowPopup(SongDetail song)
        {
            gameObject.SetActive(true);
            Display(song);
        }

        public void HidePopup()
        {
            gameObject.SetActive(false);
        }

        public void Display(SongDetail song)
        {
            songNameDisplay.text = song.SongName;
            songDescriptionDisplay.text = song.SongDescription;
            songBuyPriceDisplay.text = song.Price.ToString() + " Stars";
            string songUrl = song.SongUrl;
            PlaySong(songUrl);
        }

        void PlaySong(string url)
        {
            videoPlayer.Stop();
            videoPlayer.url = url;
            videoPlayer.Prepare();
            videoPlayer.Play();
        }
    }
}
