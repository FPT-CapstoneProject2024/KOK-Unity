using KOK.ApiHandler.DTOModels;
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

            var songParam = new BuySongParam()
            {
                SongId = (Guid)SongDetail.SongId,
                SongName = SongDetail.SongName,
                Price = SongDetail.Price,
            };
            BuySongButton.AddEventListener(songParam, OnBuySongClick);
        }

        public void OnPlaySongClick(string songUrl)
        {
            Debug.Log("Play song with url: " + songUrl);
        }

        public void OnBuySongClick(BuySongParam song)
        {
            Debug.Log($"Member buy song [{song.SongName}] with ID [{song.SongId}] cost [{song.Price}] UP.");
        }
    }

    public class BuySongParam
    {
        public Guid SongId { get; set; }
        public string SongName { get; set; }
        public decimal Price { get; set; }
    }
}
