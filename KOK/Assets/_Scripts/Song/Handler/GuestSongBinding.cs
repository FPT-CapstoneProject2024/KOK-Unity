using KOK.ApiHandler.DTOModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class GuestSongBinding : MonoBehaviour
    {
        public SongDetail Song;
        [SerializeField] public TMP_Text TMP_SongName;
        [SerializeField] public TMP_Text TMP_SongArtist;
        [SerializeField] public TMP_Text TMP_SongGenre;
        [SerializeField] public TMP_Text TMP_SongSinger;
        GuestSongManager guestSongManager;

        public void BindingData()
        {
            TMP_SongName = transform.Find("SongName").GetComponent<TMP_Text>();
            TMP_SongArtist = transform.Find("SongArtist").GetComponent<TMP_Text>();
            TMP_SongGenre = transform.Find("SongSinger").GetComponent<TMP_Text>();
            TMP_SongSinger = transform.Find("SongGenre").GetComponent<TMP_Text>();

            TMP_SongName.text = Song.SongName.ToString();
            string artists = "";
            foreach (var a in Song.Artist) artists += a.ToString();
            TMP_SongArtist.text = artists;
            string gernes = "";
            foreach (var g in Song.Genre) gernes += g.ToString();
            TMP_SongGenre.text = gernes;
            string singers = "";
            foreach (var s in Song.Singer) singers += s.ToString();
            TMP_SongSinger.text = singers;
        }

        public void Init(SongDetail song, GuestSongManager guestSongManager)
        {
            Song = song;
            this.guestSongManager = guestSongManager;
            BindingData();
        }
        public void OnPlaySongClick()
        {
            Debug.Log("Play song with url: " + Song.SongUrl);
            FindFirstObjectByType<SongHandler>().StartPreviewSong(Song.SongUrl);
        }
    }
}
