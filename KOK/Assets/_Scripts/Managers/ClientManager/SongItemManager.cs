using Fusion;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace KOK
{
    public class SongItemManager : MonoBehaviour
    {
        public static SongItemManager Instance;

        [SerializeField] GameObject _viewportContent;

        [SerializeField] GameObject _songHolderPrefab;

        [SerializeField] TMP_InputField _searchSongInput;

        private NetworkRunner _runner;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void UpdateSongList()
        {
            if (_viewportContent == null) { return; }
            if (_runner == null) { _runner = NetworkRunner.Instances[0]; }
            foreach (Transform child in _viewportContent.transform)
            {
                Destroy(child.gameObject);
            }
            //Call get all song api here
            List<DemoSong> songList = SongManager.songs;

            //Search có thể thay bằng gọi api
            string searchKeyword = _searchSongInput.text;
            if (!searchKeyword.IsNullOrEmpty()) {
                songList = songList.Where(s => s.songName.ContainsInsensitive(searchKeyword)).ToList();
            }

            foreach(var song in songList)
            {
                try
                {
                    GameObject songHolder = Instantiate(_songHolderPrefab, _viewportContent.transform);
                    songHolder.name = song.songName;
                    songHolder.transform.GetChild(0).name = song.songCode;
                    songHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = song.songName;
                    songHolder.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = song.songArtist;
                    //Thêm các thông tin khác ở đây
                }
                catch { }
            }
        }

        
    }
}
