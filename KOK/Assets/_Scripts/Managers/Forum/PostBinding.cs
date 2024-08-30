using Fusion;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.Controller;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.Post;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;
using WebSocketSharp;

namespace KOK
{
    public class PostBinding : MonoBehaviour
    {
        [Header("Post infor component")]
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
        [SerializeField] Sprite defaultAvatar;
        public List<Selectable> selectableList;
        [SerializeField] EditPostBinding editPostBinding;

        [Header("Option")]
        [SerializeField] TMP_Dropdown optionDropdown;
        List<string> ownPostOptions = new List<string>() { "Chỉnh sửa", "Xoá", "" };
        List<string> otherPostOptions = new List<string>() { "Báo cáo", "" };

        [Header("Score Give")]
        [SerializeField] GameObject scoreGivePanel;
        [SerializeField] ScoreBinding scoreBinding;
        [SerializeField] Gradient gradient;

        [Header("Comment")]
        [SerializeField] GameObject commentPanel;
        [SerializeField] GameObject commentPanelContent;
        [SerializeField] GameObject parentCommentPrefab;
        [SerializeField] GameObject childCommentPrefab;


        private int ownScore = 0;
        List<CommentBinding> commentBindings = new();
        Recording recording;

        List<string> voiceAudioUrls = new List<string>();
        List<AudioClip> audioClipList = new();
        [SerializeField] List<AudioSource> audioSources = new();

        bool readyToPlay = false;
        bool isDraggingSlider = false; // To track if the user is dragging the slider

        string audioLocalDirectory = "";

        public Post post;
        private ForumNewFeedManager forumNewFeedManager;

        private void Start()
        {
            audioLocalDirectory = Application.persistentDataPath + "/AudioProcess/";
        }
        public void Init(Post post, bool isOwnPostProfile, ForumNewFeedManager forumNewFeedManager)
        {
            Clear();            
            Stop();
            StopAllCoroutines();
            this.post = post;
            this.forumNewFeedManager = forumNewFeedManager;
            ShowThisPost();
        }

        public void Clear()
        {
            foreach (var selectable in selectableList)
            {
                selectable.interactable = false;
            }
            avatar.sprite = defaultAvatar;
            userNameLabel.text = "";
            createTimeLabel.text = "";
            captionLabel.text = "";
            scoreLabel.text = "";
            giveScoreLabel.text = "";
            videoRenderTexture.gameObject.SetActive(false);
            optionDropdown.ClearOptions();
            videoPlayer.url = null;
            ResetAudioSourceComponent();
        }

        List<bool> isAudioReady = new List<bool>();
        public void ShowThisPost()
        {
            readyToPlay = false;
            //Avatar here
            avatar.sprite = Resources.Load<Sprite>(post.Member.CharaterItemCode + "AVA");
            userNameLabel.text = post.Member.UserName; captionLabel.text = post.Caption;
            createTimeLabel.text = post.UploadTime.Value.ToString("hh:mm  dd/MM/yyyy");
            //Score
            if (post.Score == null)
            {
                scoreLabel.text = $"Chưa có điểm!";
                scoreLabel.color = Color.white;
            }
            else
            {
                scoreLabel.text = $"Điểm: {post.Score}";
                scoreLabel.color = gradient.Evaluate(post.Score.Value / 100);
            }

            //Option Dropdown
            InitOptionDropdownValue();

            List<RecordingHelper> recordingHelpers = gameObject.GetComponents<RecordingHelper>().ToList();
            foreach (RecordingHelper recordingHelper in recordingHelpers)
            {
                Destroy(recordingHelper);
            }
            ResetAudioSourceComponent();
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
                            var localFilePathZip = "AudioProcess/" + audio.VoiceUrl + ".wav";
                            if (!File.Exists(localFilePathZip))
                            {
                                var audioSource = gameObject.AddComponent<AudioSource>();
                                RecordingHelper recordingHelper = gameObject.AddComponent<RecordingHelper>();
                                recordingHelper.PrepareAudioSourceDownloadAudio(
                                        audio.VoiceUrl,
                                        (audioClip) =>
                                        {
                                            Debug.Log(audioClip.name);
                                            audioSource.clip = audioClip;
                                            audioSources.Add(audioSource);
                                            isAudioReady.RemoveAt(0);
                                        },
                                        () => { }
                                    );
                            }
                            else
                            {
                                var audioSource = gameObject.AddComponent<AudioSource>();
                                RecordingHelper recordingHelper = gameObject.AddComponent<RecordingHelper>();
                                recordingHelper.PrepareAudioSourceLoadAudioClip(
                                        audio.VoiceUrl,
                                        (audioClip) =>
                                        {
                                            audioSource.clip = audioClip;
                                            audioSources.Add(audioSource);
                                            isAudioReady.RemoveAt(0);
                                        },
                                        () => { }

                                    );
                                Debug.Log($"Audio {audio.VoiceUrl} is ready!");
                            }


                        }
                        Debug.Log("prepare " + isAudioReady.Count);
                        Debug.Log("voiceAudioUrls " + voiceAudioUrls.Count);
                        StartCoroutine(Prepare());
                        foreach (var selectable in selectableList)
                        {
                            selectable.interactable = true;
                        }
                    },
                    (recording) => { }
                );
            ApiHelper.Instance.GetComponent<PostRatingController>()
                .GetPostRatingOfAMember(
                    (Guid)post.PostId,
                    Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    (postRating) =>
                    {
                        if (postRating != null)
                        {
                            ownScore = (int)postRating.Score;
                            giveScoreLabel.text = $"Điểm của bạn: {ownScore}";
                        }
                        else
                        {
                            ownScore = -1;
                            giveScoreLabel.text = $"Bạn chưa chấm điểm";
                        }
                    },
                    (ex) =>
                    {
                        ownScore = -1;
                        giveScoreLabel.text = $"Bạn chưa chấm điểm";
                    }
                );
        }

        public void ShowGiveScorePanel()
        {
            scoreGivePanel.gameObject.SetActive(true);
            scoreBinding.Init(ownScore, this);
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
        private IEnumerator Prepare()
        {
            yield return new WaitUntil(() => isAudioReady.Count == 0);

            Debug.Log("prepare audio complete ");
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

        private void InitOptionDropdownValue()
        {
            optionDropdown.ClearOptions();
            if (post.MemberId.Equals(Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId))))
            {
                optionDropdown.AddOptions(ownPostOptions);
                optionDropdown.value = ownPostOptions.Count - 1;
            }
            else
            {
                optionDropdown.AddOptions(otherPostOptions);
                optionDropdown.value = otherPostOptions.Count - 1;
            }
        }

        public void OnOptionDropdownValueChange()
        {
            Debug.Log(optionDropdown.options[optionDropdown.value]);
            if (optionDropdown.options[optionDropdown.value].text.Equals("Xoá"))
            {
                OpenDeletePostConfirm();
            }
            else if (optionDropdown.options[optionDropdown.value].text.Equals("Chỉnh sửa"))
            {
                OpenEditPostPanel();
            }

            if (post.MemberId.Equals(Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId))))
            {
                optionDropdown.value = ownPostOptions.Count - 1;
            }
            else
            {
                optionDropdown.value = otherPostOptions.Count - 1;
            }
        }

        private void OpenEditPostPanel()
        {
            editPostBinding.InitEditPostPanel(post, forumNewFeedManager);
        }

        private void OpenDeletePostConfirm()
        {
            forumNewFeedManager.ConfirmAlertManager.Confirm
                (
                    "Xác nhận xoá post",
                    () =>
                    {
                        ApiHelper.Instance.GetComponent<PostController>()
                            .DeletePostByIdCoroutine
                            (
                                (Guid)post.PostId,
                                () =>
                                {
                                    forumNewFeedManager.MessageAlertManager.Alert("Xoá post thành công", true);
                                    forumNewFeedManager.Refresh();
                                },
                                () =>
                                {
                                    forumNewFeedManager.MessageAlertManager.Alert("Xoá post thất bại!", false);
                                }
                            );
                    }
                );
        }

        public void ShowCommentPanel()
        {
            commentPanel.SetActive(true);
            foreach (Transform child in commentPanelContent.transform)
            {
                Destroy(child.gameObject);
            }
            ApiHelper.Instance.GetComponent<PostCommentController>()
                .GetAllCommentsOfAPost
                (
                    (Guid)post.PostId,
                    (comments) => { 
                        foreach(var comment in comments)
                        {
                            var commentObject = Instantiate(parentCommentPrefab, commentPanelContent.transform);
                            commentObject.GetComponent<CommentBinding>().Init(comment, commentPanelContent.transform);
                        }    
                    },
                    (ex) => { }
                );
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
