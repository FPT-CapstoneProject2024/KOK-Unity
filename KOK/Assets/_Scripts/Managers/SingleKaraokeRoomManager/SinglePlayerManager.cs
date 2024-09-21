using Fusion;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using WebSocketSharp;

namespace KOK
{
    public class SinglePlayerManager : MonoBehaviour
    {
        [SerializeField] List<SongDetail> songList = new();
        [SerializeField] List<SongDetail> queueSongList = new();
        [SerializeField] List<SongDetail> purchasedSongList = new();
        [SerializeField] List<FavoriteSong> favoriteSongList = new();

        [SerializeField] TMP_InputField searchSongInput;
        [SerializeField] GameObject searchSongPanelContent;
        [SerializeField] GameObject queueSongPanelContent;
        [SerializeField] GameObject searchSongHolderPrefab;
        [SerializeField] GameObject queueSongHolderPrefab;

        [SerializeField] Toggle favToggle;

        [SerializeField] VideoPlayer videoPlayer;

        [SerializeField] VoiceRecorder voiceRecorder;
        [SerializeField] TMP_Text playerNameText;


        [SerializeField] Toggle songPanelFavouriteToggle;
        [SerializeField] Toggle songPanelPurchasedToggle;

        [SerializeField] LoadingManager _loadingManager;

        [SerializeField] Animator _characterAnim;
        [SerializeField] GameObject _roomDeco;

        Guid karaokeRoomId;

        private void OnEnable()
        {
            playerNameText.text = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_UserName);
            _characterAnim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
                PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_CharaterItemCode) + "/"
                + PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_CharaterItemCode) + "Animator"
                );
            _characterAnim.Play(AnimationName.IdleFront.ToString());
            Debug.Log(
                PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_CharaterItemCode) + "/"
                + PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_CharaterItemCode) + "Animator");

            string logFileName = "RoomLog_" + PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_UserName).ToString() + "_" + DateTime.Now + ".txt";
            logFileName = logFileName.Replace(" ", "");
            logFileName = logFileName.Replace(":", "");
            logFileName = logFileName.Replace("/", "");
            ApiHelper.Instance.GetComponent<KaraokeRoomController>().AddKaraokeRoomCoroutine(
                new AddKaraokeRoomRequest()
                {
                    RoomLog = logFileName,
                    CreatorId = Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                },
                (kr) => { karaokeRoomId = kr.Value.RoomId; },
                (er) => { Debug.LogError(er); }
                );

        }
        void Start()
        {
            if (voiceRecorder == null) { voiceRecorder = FindAnyObjectByType<VoiceRecorder>(); }
            StartCoroutine(LoadSong());

        }

        public void ReloadSong()
        {
            StartCoroutine(LoadSong());
        }

        IEnumerator LoadSong()
        {
            ClearSearchSongList();
            _loadingManager.DisableUIElement();
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Start to load song");
            if (songPanelFavouriteToggle == null)
            {
                songPanelFavouriteToggle = transform.Find("SongPanelFavouriteToggle").GetComponent<Toggle>();
            }

            //Call api load song
            songList = new();

            _loadingManager.EnableLoadingSymbol();
            FindAnyObjectByType<ApiHelper>().gameObject
                    .GetComponent<SongController>()
                    .GetSongsFilterPagingCoroutine(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId),
                                                    new SongFilter(),
                                                    SongOrderFilter.SongName,
                                                    new PagingRequest()
                                                    {
                                                        pageSize = 100
                                                    },
                                                    (drr) =>
                                                    {
                                                        _loadingManager.DisableLoadingSymbol();
                                                        songList = drr.Results.ToList();
                                                        UpdateSearchSongUI();
                                                        Debug.Log("Reload song success!");
                                                    },
                                                    (ex) => Debug.LogError(ex));


            //Call api load purchased song
            purchasedSongList = new();


            //Load song from devide
        }

        
        public void ClearSearchSongList()
        {
            try
            {
                foreach (Transform child in searchSongPanelContent.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            catch { }
        }
        public void UpdateSearchSongUI()
        {
            ClearSearchSongList();
            string searchKeyword = searchSongInput.text;
            List<SongDetail> songListSearch = new();
            if (!searchKeyword.IsNullOrEmpty())
            {
                songListSearch = songList.Where(s => s.SongName.ContainsInsensitive(searchKeyword)
                                                || s.Artist.ToCommaSeparatedString().ContainsInsensitive(searchKeyword)
                                                || s.Singer.ToCommaSeparatedString().ContainsInsensitive(searchKeyword)
                                                || s.Genre.ToCommaSeparatedString().ContainsInsensitive(searchKeyword)).ToList();
            }
            else
            {
                songListSearch = songList;
            }

            if (songPanelFavouriteToggle.isOn)
            {
                songListSearch = songListSearch.Where(s => s.isFavorite == true).ToList();
            }

            if (songPanelPurchasedToggle.isOn)
            {
                songListSearch = songListSearch.Where(s => s.isPurchased == true).ToList();
            }



            foreach (var song in songListSearch)
            {
                try
                {
                    GameObject songHolder = Instantiate(searchSongHolderPrefab, searchSongPanelContent.transform);
                    songHolder.name = song.SongName;
                    songHolder.GetComponentInChildren<SongBinding>().BindingData(song);
                    songHolder.transform.GetChild(0).name = song.SongId.ToString();
                    var favToggle = songHolder.transform.Find("FavouriteToggle").GetComponent<Toggle>();
                    if (song.isFavorite)
                    {
                        favToggle.isOn = true;
                    }
                    else
                    {
                        favToggle.isOn = false;
                    }
                    if (!song.isPurchased)
                    {
                        songHolder.GetComponentInChildren<Image>().color = Color.grey;
                    }
                }
                catch { }
            }

        }
        public void UpdateQueueSongUI()
        {

            foreach (Transform child in queueSongPanelContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var song in queueSongList)
            {
                try
                {
                    GameObject songHolder = Instantiate(queueSongHolderPrefab, queueSongPanelContent.transform);
                    songHolder.name = song.SongName;
                    songHolder.GetComponentInChildren<SongBinding>().BindingData(song);
                    songHolder.transform.GetChild(0).name = song.SongId.ToString();


                }
                catch { }
            }

        }



        public SongDetail GetSongBySongCode(string songCode)
        {
            return songList.Find(x => x.SongCode.Equals(songCode.ToString()));
        }
        public void CreateAddSongPopUp()
        {

        }
        public void AddSongToQueue(string songCode)
        {
            var song = GetSongBySongCode(songCode);
            queueSongList.Add(song);
        }


        public void PrioritizeSongToQueue(string songCode)
        {
            queueSongList.Insert(0, GetSongBySongCode(songCode));
        }

        public void RemoveSongFromQueue()
        {

        }

        public void ClearQueue()
        {
            queueSongList.Clear();
        }

        public void OnPlaySongVideoButtonClick()
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                voiceRecorder.StopRecording();
            }
            else
            {
                videoPlayer.url = queueSongList[0].SongUrl;
                videoPlayer.Play();

                var audioFile = queueSongList[0].SongCode + "_" + PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_UserName) + "_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
                //audioFile = audioFile.Replace(" ", "");
                //audioFile = audioFile.Replace(":", "");
                //audioFile = audioFile.Replace("/", "");

                voiceRecorder.StartRecording(audioFile);

                Guid songId = (Guid)queueSongList[0].SongId;



                string recordingName = queueSongList[0].SongName;

                queueSongList.RemoveAt(0);
                PurchasedSongFilter filter = new()
                {
                    MemberId = Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    SongId = songId,
                };
                Debug.Log(filter);
                ApiHelper.Instance.GetComponent<PurchasedSongController>()
                    .GetMemberPurchasedSongFilterCoroutine(
                        filter,
                        PurchasedSongOrderFilter.SongId,
                        new()
                        {
                            pageSize = 1,
                        },
                        (successValue) =>
                        {
                            CreateRecording(recordingName,
                            audioFile,
                            successValue.Results[0].PurchasedSongId.ToString());
                            Debug.Log("Get purchased song success: " + successValue.Results[0].SongName.ToString());
                        },
                        (er) => { }
                        );


            }
        }

        public void CreateRecording(string recordingName, string audioFile, string purSongId)
        {
            RecordingManager.Instance.CreateRecording(
                    recordingName,
                    0,
                    UnityEngine.Random.Range(50, 100),
                    purSongId,
                    PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId),
                    PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId),
                    karaokeRoomId.ToString(),
                    new List<string>()
                    {
                        audioFile,
                    },
                    new List<string>()
                    {
                        PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)
                    }
                    );
        }

        public void OnPlayNextSongVideoButtonClick()
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
            videoPlayer.url = queueSongList[0].SongUrl;
            videoPlayer.Play();
            queueSongList.RemoveAt(0);
        }

        public void RefreshFavSongList()
        {
            //StartCoroutine(LoadFavoriteSong());
            favoriteSongList = new();
            FindAnyObjectByType<ApiHelper>().gameObject
                        .GetComponent<SongController>()
                        .GetSongsFilterPagingCoroutine(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId),
                                                        new SongFilter(),
                                                        SongOrderFilter.SongName,
                                                        new PagingRequest(),
                                                        (drr) => { songList = drr.Results.ToList().Where(s => s.isFavorite == true).ToList(); UpdateSearchSongUI(); },
                                                        (ex) => Debug.LogError(ex));
        }


        private void CreateRecording()
        {
        }


        public void LeaveRoom()
        {
            //Change to home page scene
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
