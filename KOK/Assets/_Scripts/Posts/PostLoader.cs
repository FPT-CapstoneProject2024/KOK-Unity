using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.VoiceAudios;
using KOK.Assets._Scripts.FileManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using WebSocketSharp;

namespace KOK.Assets._Scripts.Posts
{
    public class PostLoader : MonoBehaviour
    {
        private string songUrl;
        public VideoLoader videoLoader;
        private List<VoiceAudio> voiceAudioList = new List<VoiceAudio>();
        private FFMPEG ffmpeg;

        void Start()
        {
            ffmpeg = GetComponent<FFMPEG>();
            if (ffmpeg == null) ffmpeg = gameObject.AddComponent<FFMPEG>();
        }

        public void GetRecordingById(Guid recordingId)
        {
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<RecordingController>()
                .GetRecordingsByIdCoroutine2( recordingId,
                                            LoadVideo,
                                            test2
                                            );
        }

        public void GetPurchasedSongCoroutine(Guid purchasedSongId)
        {
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<PurchasedSongController>()
                .GetPurchasedSongByIdCoroutine(purchasedSongId,
                                                GetSongCoroutine,
                                                OnError2
                                                );
        }

        // Dont ask me about the song[0], fk api
        private void GetSongCoroutine(List<PurchasedSong> song)
        {
            FindAnyObjectByType<ApiHelper>().gameObject
                .GetComponent<SongController>()
                .GetSongByIdCoroutine(song[0].SongId,
                                        SetSongUrl,
                                        OnError3
                                        );
        }

        private void SetSongUrl(ResponseResult<SongDetail> song)
        {
            songUrl = song.Value.SongUrl;
            //Process(songUrl, recordingUrl);
        }

        List<bool> isAudioReady = new List<bool>();
        private void LoadVideo(List<Recording> recordings)
        {
            foreach (var recording in recordings)
            {
                GetPurchasedSongCoroutine(recording.PurchasedSongId);
                var startTimeSong = recording.StartTime;

                Debug.Log(recording.VoiceAudios);
                isAudioReady.Clear();
                foreach (var voiceAudio in recording.VoiceAudios)
                {
                    isAudioReady.Add(false);
                    var startTimeVoiceAudio = voiceAudio.StartTime;
                    string voiceFilePath = Path.Combine(Application.persistentDataPath + "/AudioProcess/" + voiceAudio.VoiceUrl + ".zip");
                    ffmpeg.DownloadFile2(voiceFilePath,
                            () => { isAudioReady.RemoveAt(0); },
                            () => { }
                        );
                    StartCoroutine(PlayVideo(voiceFilePath, startTimeVoiceAudio, startTimeSong, voiceAudio.VoiceUrl));
                }
            }
        }

        public IEnumerator PlayVideo(string voiceFilePath, float startTimeVoiceAudio, float startTimeSong, string voiceUrl)
        {
            yield return new WaitUntil(() => isAudioReady.Count == 0);
            string voiceFilePath2 = Path.Combine(Application.persistentDataPath + "/AudioProcess/" + voiceUrl + ".wav");

            if (songUrl.IsNullOrEmpty() && !File.Exists(voiceFilePath2))
            {
                StartCoroutine(PlayVideo(voiceFilePath, startTimeVoiceAudio, startTimeSong, voiceUrl));
            }
            else
            {
                videoLoader.Load(songUrl, voiceFilePath2, startTimeVoiceAudio, startTimeSong);
            }
        }

        private void test2 (string error)
        {
            Debug.Log (error);
        }

        private void OnError2(string error)
        {
            Debug.Log(error);
        }

        private void OnError3(ResponseResult<SongDetail> detail)
        {
            Debug.Log(detail);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
