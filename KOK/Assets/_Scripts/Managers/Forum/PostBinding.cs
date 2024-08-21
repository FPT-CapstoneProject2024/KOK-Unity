using KOK.ApiHandler.Controller;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;

namespace KOK
{
    [RequireComponent(typeof(FFMPEG))]
    public class PostBinding : MonoBehaviour
    {
        [SerializeField] Image avatar;
        [SerializeField] TMP_Text userNameLabel;
        [SerializeField] TMP_Text createTimeLabel;
        [SerializeField] TMP_Text captionLabel;
        [SerializeField] TMP_Text scoreLabel;
        [SerializeField] VideoPlayer videoPlayer;
        [SerializeField] Slider progressSlider;
        [SerializeField] TMP_Text giveScoreLabel;
        [SerializeField] AudioMixerGroup audioMixerGroup;
        [SerializeField] RawImage videoRenderTexture;

        private FFMPEG ffmpeg;
        List<CommentBinding> commentBindings = new();
        Recording recording;

        List<string> voiceAudioUrls = new List<string>();
        List<AudioClip> audioClipList = new();
        [SerializeField] List<AudioSource> audioSources = new();

        bool readyToPlay = false;
        bool isDraggingSlider = false; // To track if the user is dragging the slider

        string audioLocalDirectory = "";

        private Post post;

        private void Start()
        {
            ffmpeg = GetComponent<FFMPEG>();
            audioLocalDirectory = Application.persistentDataPath + "/AudioProcess/";
        }
        public void Init(Post post)
        {
            Stop();
            StopAllCoroutines();
            this.post = post;
            ShowThisPost();
        }

        List<bool> isAudioReady = new List<bool>();
        private void ShowThisPost()
        {
            readyToPlay = false;
            //Avatar here

            userNameLabel.text = post.Member.UserName; captionLabel.text = post.Caption;
            //Need score
            scoreLabel.text = $"Điểm: {Random.Range(50, 100)}";

            ApiHelper.Instance.GetComponent<RecordingController>()
                .GetRecordingByIdCoroutine(
                    (System.Guid)post.RecordingId,
                    (recording) =>
                    {
                        this.recording = recording.Value;
                        voiceAudioUrls = new();
                        isAudioReady = new List<bool>();
                        foreach (var audio in this.recording.VoiceAudios)
                        {
                            isAudioReady.Add(false);
                            var localFilePath = Application.persistentDataPath + "/AudioProcess/" + audio.VoiceUrl + ".zip";
                            if (!File.Exists(localFilePath))
                            {
                                ffmpeg.DownloadFile2(localFilePath,
                                    () => { isAudioReady.RemoveAt(0); Debug.Log($"Audio {audio.VoiceUrl} is ready!"); },
                                    () => { }
                                );
                            }
                            else
                            {
                                isAudioReady.RemoveAt(0); Debug.Log($"Audio {audio.VoiceUrl} is ready!");
                            }

                            voiceAudioUrls.Add(localFilePath);
                        }
                        Debug.Log("prepare " + isAudioReady.Count);
                        Debug.Log("voiceAudioUrls " + voiceAudioUrls.Count);
                        StartCoroutine(Prepare(voiceAudioUrls));
                    },
                    (recording) => { }
                );
        }

        private void ResetAudioSourceComponent()
        {
            //Reset audio source component
            audioClipList.Clear();
            voiceAudioUrls.Clear();
            audioSources = GetComponents<AudioSource>().ToList();
            foreach (var audioSource in audioSources)
            {
                Destroy(audioSource);
            }
        }
        private IEnumerator Prepare(List<string> voiceAudioUrls)
        {
            yield return new WaitUntil(() => isAudioReady.Count == 0);

            Debug.Log("prepare audio complete ");
            List<string> filePathLocalWavs = new();
            List<string> filePathLocalZips = new();

            foreach (var voiceAudioUrl in voiceAudioUrls)
            {
                var filePathLocalWav = voiceAudioUrl.Replace(".zip", ".wav");
                filePathLocalWavs.Add(filePathLocalWav);
                filePathLocalZips.Add(voiceAudioUrl);
                var audioClip = ffmpeg.LoadAudioClipWavHelper(voiceAudioUrl.Replace(".zip", ".wav"));
                yield return new WaitUntil(() => audioClip != null);
                if (audioClip != null)
                {
                    audioClipList.Add(audioClip);
                }
                else
                {
                    Debug.LogError("Can not load audio clip!");
                    yield break;
                }

            }

            Debug.Log("audioClipList " + audioClipList.Count);
            audioSources = new List<AudioSource>();
            foreach (var audioClip in audioClipList)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log(audioSource);
                audioSource.clip = audioClip;
                audioSource.loop = false;
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = audioMixerGroup;
                audioSources.Add(audioSource);
            }
            Debug.Log("audioSources " + audioSources.Count);
            videoPlayer.Stop();
            videoPlayer.url = post.SongUrl;
            videoPlayer.Prepare();
            videoPlayer.SetDirectAudioVolume(0, 0.2f);
            
            readyToPlay = true;

        }

        public void OnPlayButtonClick()
        {
            StartCoroutine(PlayButtonClickCoroutine());
        }

        IEnumerator PlayButtonClickCoroutine()
        {
            if (videoPlayer.isPlaying)
            {
                Pause();
            }
            else
            {
                yield return new WaitUntil(() => readyToPlay);
                Play();
            }

        }
        private void Play()
        {
            if (!readyToPlay)
            {
                return;
            }

            videoRenderTexture.gameObject.SetActive(true);
            videoPlayer.Play();
            StartCoroutine(SyncVoiceAudioWithVideo());
            StartCoroutine(SyncStart(recording.StartTime, audioSources));
        }
        IEnumerator SyncStart(float videoStartTime, List<AudioSource> audioSources)
        {
            yield return new WaitForSeconds(0.1f);
            if (videoPlayer.time > 0)
            {
                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.Play();
                    audioSource.time = (float)videoPlayer.time;
                }
                if (progressSlider.minValue != 0 || progressSlider.maxValue != (float)videoPlayer.length)
                {
                    progressSlider.minValue = 0;
                    progressSlider.maxValue = (float)videoPlayer.length;

                    StartCoroutine(SliderFollowVideo());
                }
                
            }
            else
            {
                StartCoroutine(SyncStart(videoStartTime, audioSources));
            }
        }

        IEnumerator SyncVoiceAudioWithVideo()
        {
            yield return new WaitForSeconds(0.5f);
            if (videoPlayer.isPlaying)
            {
                foreach (AudioSource audioSource in audioSources)
                {
                    //if (!audioSource.isPlaying) audioSource.Play();
                    if (videoPlayer.time < audioSource.clip.length)
                    {
                        audioSource.time = (float)videoPlayer.time;
                    }
                    else
                    {
                        audioSource.time = audioSource.clip.length;
                    }
                    //Debug.Log(videoPlayer.time + " | " + audioSource.time);
                }
            }
        }

        private void Pause()
        {
            foreach (var audioSource in audioSources)
            {
                audioSource.Pause();
            }
            videoPlayer.Pause();
        }

        private void Stop()
        {
            ResetAudioSourceComponent();
            videoPlayer.Stop();
            videoRenderTexture.gameObject.SetActive(false);
            progressSlider.value = 0;   
        }
        IEnumerator SliderFollowVideo()
        {
            yield return new WaitForSeconds(0.2f);
            if (!isDraggingSlider)
            {
                progressSlider.value = (float)videoPlayer.time;
            }
            
            StartCoroutine(SliderFollowVideo());
        }
        public void OnSliderDragStart()
        {
            isDraggingSlider = true;
            Pause();
        }

        public void OnSliderDragEnd()
        {
            isDraggingSlider = false;
            if (!readyToPlay)
            {
                return;
            }
            Play();
            videoPlayer.time = progressSlider.value;
        }

        private void OnDestroy()
        {
            DirectoryInfo dir = new DirectoryInfo(audioLocalDirectory);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

        }
    }
}
