/*using KOK.ApiHandler.DTOModels;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace KOK.Assets._Scripts.FileManager
{
    public class VideoLoader : MonoBehaviour
    {
        private FFMPEG ffmpeg = new FFMPEG();
        public VideoPlayer videoPlayer;
        public RecordingLoader recordingLoader;
        private AudioSource audioSource;
        public Slider progressSlider; // Slider to display and control video progress
        private bool isDraggingSlider = false; // To track if the user is dragging the slider

        void Update()
        {
            Debug.Log(videoPlayer.time);
            if (!isDraggingSlider && videoPlayer.isPlaying)
            {
                progressSlider.value = (float)videoPlayer.time;
            }
        }

        public void ShowPopup(string videoUrl, string filePath, float startTime)
        {
            gameObject.SetActive(true);
            AudioClip audioClip = ffmpeg.LoadAudioClip(filePath);
            Display(videoUrl, audioClip, startTime);
        }

        private void Display(string videoUrl, AudioClip audioClip, float startTime)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = false;

            videoPlayer.Stop();
            videoPlayer.url = videoUrl;

            videoPlayer.Prepare();
            StartCoroutine(SyncStart(startTime));           
        }       

        public IEnumerator SyncStart(float startTime)
        {
            yield return new WaitForSeconds(0.1f);
            if (videoPlayer.isPrepared && audioSource != null)
            {
                videoPlayer.Play();
                audioSource.Play();
                videoPlayer.time = startTime;
                // Initialize the slider
                progressSlider.minValue = 0;
                progressSlider.maxValue = (float)videoPlayer.length;
                progressSlider.onValueChanged.AddListener(OnSliderValueChanged);
            }
            else
            {
                StartCoroutine(SyncStart(startTime));
            }
        }

        private void OnSliderValueChanged(float value)
        {
            if (isDraggingSlider)
            {
                videoPlayer.time = value;
                audioSource.time = value;
            }
        }

        public void OnSliderDragStart()
        {
            isDraggingSlider = true;
        }

        public void OnSliderDragEnd()
        {
            isDraggingSlider = false;
            videoPlayer.time = progressSlider.value;
            audioSource.time = (float)progressSlider.value;
            videoPlayer.Play();
            audioSource.Play();
        }

        public void Tua()
        {
            videoPlayer.time = 20f;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            recordingLoader.Show();
        }
    }
}
*/

using KOK.ApiHandler.DTOModels;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace KOK.Assets._Scripts.FileManager
{
    public class VideoLoader : MonoBehaviour
    {
        private FFMPEG ffmpeg = new FFMPEG();
        public VideoPlayer videoPlayer;
        public RecordingLoader recordingLoader;
        private AudioSource audioSource;
        public Slider progressSlider; // Slider to display and control video progress
        private bool isDraggingSlider = false; // To track if the user is dragging the slider

        void Start()
        {
            // Ensure the slider is interactive
            progressSlider.interactable = true;
            
        }

        void Update()
        {
            if (!isDraggingSlider && videoPlayer.isPlaying)
            {
                progressSlider.value = (float)videoPlayer.time;
            }
        }

        public void ShowPopup(string videoUrl, string filePath, float startTimeRecording, float startTimeVoiceAudio)
        {
            gameObject.SetActive(true);
            AudioClip audioClip = ffmpeg.LoadAudioClip(filePath);
            Display(videoUrl, audioClip, startTimeRecording, startTimeVoiceAudio);
        }

        private void Display(string videoUrl, AudioClip audioClip, float startTimeRecording, float startTimeVoiceAudio)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = false;

            videoPlayer.Stop();
            videoPlayer.url = videoUrl;

            videoPlayer.Prepare();
            StartCoroutine(SyncStart(startTimeRecording, startTimeVoiceAudio));
        }

        private IEnumerator SyncStart(float startTimeRecording, float startTimeVoiceAudio)
        {
            yield return new WaitForSeconds(0.1f);
            if (videoPlayer.isPrepared && audioSource != null)
            {
                videoPlayer.Play();
                //audioSource.Play();
                StartCoroutine(SyncStart2(startTimeVoiceAudio));
                videoPlayer.time = startTimeRecording;

                // Initialize the slider only once
                if (progressSlider.minValue != 0 || progressSlider.maxValue != (float)videoPlayer.length)
                {
                    progressSlider.minValue = 0;
                    progressSlider.maxValue = (float)videoPlayer.length;

                    progressSlider.onValueChanged.AddListener(OnSliderValueChanged);
                }
            }
            else
            {
                StartCoroutine(SyncStart(startTimeRecording, startTimeVoiceAudio));
            }
        }

        //it works, dont ask
        private IEnumerator SyncStart2(float startTimeVoiceAudio)
        {
            yield return new WaitForSeconds(0.02f);
            if (videoPlayer.time > 0)
            {
                audioSource.Play();
                audioSource.time = startTimeVoiceAudio;
            }
            else
            {
                StartCoroutine(SyncStart2(startTimeVoiceAudio));
            }
        }

        public void OnSliderValueChanged(float value)
        {
            if (isDraggingSlider)
            {
                videoPlayer.time = value;
                audioSource.time = value;
            }
        }

        public void OnSliderDragStart()
        {
            isDraggingSlider = true;
        }

        public void OnSliderDragEnd()
        {
            isDraggingSlider = false;
            videoPlayer.time = progressSlider.value;
            audioSource.time = (float)progressSlider.value;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            recordingLoader.Show();
        }
    }
}
