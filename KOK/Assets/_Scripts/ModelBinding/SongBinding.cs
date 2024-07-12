using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class SongBinding : MonoBehaviour
    {
        [SerializeField] public DemoSong Song;
        [SerializeField] public TMP_Text TMP_SongName;
        [SerializeField] public TMP_Text TMP_SongArtist;
        [SerializeField] public TMP_Text TMP_SongGenre;
        [SerializeField] public TMP_Text TMP_SongSinger;

        private void Start()
        {
            
        }

        public void BindingData()
        {
            TMP_SongName = transform.Find("SongName").GetComponent<TMP_Text>();
            TMP_SongArtist = transform.Find("SongArtist").GetComponent<TMP_Text>();
            TMP_SongGenre = transform.Find("SongSinger").GetComponent<TMP_Text>();
            TMP_SongSinger = transform.Find("SongGenre").GetComponent<TMP_Text>();

            TMP_SongName.text = Song.songName.ToString();
            TMP_SongArtist.text = Song.songArtist.ToString();
            TMP_SongGenre.text = "Song genre test";
            TMP_SongSinger.text = "Song singer test";
        }

        public void BindingData(DemoSong song)
        {
            Song = song;
            BindingData();
        }
    }
}
