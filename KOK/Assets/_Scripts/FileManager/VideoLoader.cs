
using KOK.ApiHandler.DTOModels;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.VoiceAudios;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace KOK.Assets._Scripts.FileManager
{
    public class VideoLoader : MonoBehaviour
    {
        private FFMPEG ffmpeg;
        public VideoPlayer videoPlayer;
        public RecordingLoader recordingLoader;
        private List<AudioSource> audioSources = new();
        public Slider progressSlider; // Slider to display and control video progress
        private bool isDraggingSlider = false; // To track if the user is dragging the slider
        private string filePathLocalWav;
        private string filePathLocalZip;

        void Start()
        {
            // Ensure the slider is interactive
            progressSlider.interactable = true;
            if (!TryGetComponent<FFMPEG>(out ffmpeg)) ffmpeg = gameObject.AddComponent<FFMPEG>();

        }

        //void Update()
        //{
        //    if (!isDraggingSlider && videoPlayer.isPlaying)
        //    {
        //        progressSlider.value = (float)videoPlayer.time;
        //    }
        //}

        //public void ShowPopup(string videoUrl, string filePath, float startTimeRecording, float startTimeSong)
        //{
        //    filePathLocalWav = filePath;
        //    filePathLocalZip = filePath.Replace(".wav", ".zip");
        //    gameObject.SetActive(true);
        //    AudioClip audioClip = ffmpeg.LoadAudioClip(filePath);
        //    Display(videoUrl, audioClip, startTimeRecording, startTimeSong);
        //}
        
        public void ShowPopup(string videoSongUrl, List<string> voiceAudioUrls)
        {
            StartCoroutine(ShowPopUpCoroutine(videoSongUrl, voiceAudioUrls));
           
        }

        private IEnumerator ShowPopUpCoroutine(string videoSongUrl, List<string> voiceAudioUrls)
        {
            List<string> filePathLocalWavs = new();
            List<string> filePathLocalZips = new();

            List<AudioClip> audioClipList = new();
            foreach (var voiceAudioUrl in voiceAudioUrls)
            {
                filePathLocalWav = voiceAudioUrl.Replace(".zip", ".wav");
                filePathLocalWavs.Add(filePathLocalWav);
                filePathLocalZips.Add(voiceAudioUrl);
                if (!TryGetComponent<FFMPEG>(out ffmpeg)) ffmpeg = gameObject.AddComponent<FFMPEG>();
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

                if (File.Exists(voiceAudioUrl))
                {
                    File.Delete(voiceAudioUrl);
                    Debug.Log($"Deleted local zip file: {voiceAudioUrl}");
                }

                if (File.Exists(filePathLocalWav))
                {
                    File.Delete(filePathLocalWav);
                    Debug.Log($"Deleted local wav file: {filePathLocalWav}");
                }
            }

            gameObject.SetActive(true);

            Display(videoSongUrl, audioClipList, 0, new() { });
        }

        public void Load(string videoUrl, string filePath, float startTimeRecording, float startTimeSong)
        {
            //gameObject.SetActive(true);
            AudioClip audioClip = ffmpeg.LoadAudioClip(filePath);
            Display(videoUrl, audioClip, startTimeRecording, startTimeSong);
        }

        private void Display(string videoUrl, AudioClip audioClip, float startTimeRecording, float startTimeSong)
        {
            //audioSource = gameObject.AddComponent<AudioSource>();
            //audioSource.clip = audioClip;
            //audioSource.loop = false;

            //videoPlayer.Stop();
            //videoPlayer.url = videoUrl;

            //videoPlayer.Prepare();
            //StartCoroutine(SyncStart(startTimeRecording, startTimeSong));
        }
        
        private void Display(string videoUrl, List<AudioClip> audioClips, float videoStartTime, List<float> audioStartTime)
        {
            
            foreach (var audioClip in audioClips)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                audioSource.loop = false;
                audioSource.playOnAwake = false;
                audioSources.Add(audioSource);
            }

            videoPlayer.Stop();
            videoPlayer.url = videoUrl;
            videoPlayer.Prepare();
            videoPlayer.Play();
            videoPlayer.SetDirectAudioVolume(0, 0.5f);
            StartCoroutine(SyncStart(videoStartTime, audioStartTime, audioSources));
        }

        IEnumerator SyncStart(float videoStartTime, List<float> audioStartTime, List<AudioSource> audioSources)
        {
            yield return new WaitForSeconds(0.1f);
            if (videoPlayer.time > 0)
            {
                foreach(AudioSource audioSource in audioSources)
                {
                    audioSource.Play();
                }
                if (progressSlider.minValue != 0 || progressSlider.maxValue != (float)videoPlayer.length)
                {
                    progressSlider.minValue = 0;
                    progressSlider.maxValue = (float)videoPlayer.length;

                    StartCoroutine(SliderFollowVideo());
                }
            } else
            {
                StartCoroutine(SyncStart(videoStartTime, audioStartTime, audioSources));
            }
        }

        IEnumerator SliderFollowVideo()
        {
            yield return new WaitForSeconds(0.2f);
            if (!isDraggingSlider)
            {
                progressSlider.value = (float)videoPlayer.time;
            }else
            {
                videoPlayer.time = progressSlider.value;
            }
            StartCoroutine(SliderFollowVideo());
        }

        public void StopPlaying()
        {
            //Clean up audio source
            List<AudioSource> oldAudioSources = GetComponents<AudioSource>().ToArray().ToList();
            foreach (var audioSource in oldAudioSources)
            {
                Destroy(audioSource);
            }
            videoPlayer.Stop();
        }



        private IEnumerator SyncStart(float startTimeRecording, float startTimeSong)
        {
            yield return new WaitForSeconds(0.1f);
            if (videoPlayer.isPrepared && audioSources.Count > 0)
            {
                videoPlayer.Play();
                foreach (var audioSource in audioSources)
                {
                    audioSource.Play();
                }
                StartCoroutine(SyncStart2(startTimeRecording));
                videoPlayer.time = startTimeSong;

                // Initialize the slider only once
                if (progressSlider.minValue != 0 || progressSlider.maxValue != (float)videoPlayer.length)
                {
                    progressSlider.minValue = 0;
                    progressSlider.maxValue = (float)videoPlayer.length;

                }
            }
            else
            {
                StartCoroutine(SyncStart(startTimeRecording, startTimeSong));
            }
        }

        //it works, dont ask
        private IEnumerator SyncStart2(float startTimeRecording)
        {
            yield return new WaitForSeconds(0.02f);
            if (videoPlayer.time > 0)
            {
                foreach(var audioSource in audioSources)
                {
                    audioSource.Play();
                    audioSource.time = startTimeRecording;
                    //need to change to voice audio start time
                }
                SyncVoiceAudioWithVideo();
            }
            else
            {
                StartCoroutine(SyncStart2(startTimeRecording));
            }
        }

        IEnumerable SyncVoiceAudioWithVideo()
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
                }
            }
        }

        public void OnSliderValueChanged()
        {
            if (isDraggingSlider)
            {
                videoPlayer.time = progressSlider.value;
                foreach(AudioSource audioSource in audioSources)
                {
                    audioSource.time = progressSlider.value;
                }
            }
        }

        public void OnSliderDragStart()
        {
            isDraggingSlider = true;
            videoPlayer.Pause();
        }

        public void OnSliderDragEnd()
        {
            isDraggingSlider = false;
            videoPlayer.Play();
            videoPlayer.time = progressSlider.value;
            foreach (AudioSource audioSource in audioSources)
            {
                if (!audioSource.isPlaying) audioSource.Play();
                if (progressSlider.value < audioSource.clip.length)
                {
                    audioSource.time = progressSlider.value;
                    Debug.Log(audioSource.time);
                }else
                {
                    audioSource.time = audioSource.clip.length;
                }
            }
        }

        public void Hide()
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            Destroy(audioSource);
            ffmpeg.CleanUp(filePathLocalWav);
            ffmpeg.CleanUp(filePathLocalZip);
            gameObject.SetActive(false);
            //recordingLoader.Show();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
