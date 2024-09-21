using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace KOK
{
    public class CreatePostBinding : MonoBehaviour
    {
        [SerializeField] GameObject createPostPanel;
        [SerializeField] TMP_InputField captionInputField;
        [SerializeField] TMP_Dropdown recordingDropDown;
        [SerializeField] ForumNewFeedManager forumNewFeedManager;


        List<Recording> recordingList = new List<Recording>();

        private Recording cloneRecording;
        public void InitCreatePostPanel()
        {
            if (!createPostPanel.activeSelf)
            {
                createPostPanel.SetActive(true);
            }
            recordingDropDown.options.Clear();
            ApiHelper.Instance.GetComponent<RecordingController>()
                .GetRecordingsByOwnerIdCoroutine(
                    Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                    (recordings) =>
                    {
                        recordingList = recordings;
                        foreach (Recording recording in recordingList)
                        {
                            recordingDropDown.options.Add(new TMP_Dropdown.OptionData() { text = recording.RecordingName });
                        }
                        recordingDropDown.enabled = false;
                        recordingDropDown.enabled = true;
                        if (recordingList.Count > 1) {
                            recordingDropDown.value = 1;
                            recordingDropDown.value = 0;
                        }
                    },
                    (msg) => { }
                );
        }
        public void CreatePost()
        {
            CloneRecording(recordingList[recordingDropDown.value],
                    () =>
                    {
                        ApiHelper.Instance.GetComponent<PostController>()
                .CreatePostCoroutine(
                    new CreatePostRequest()
                    {
                        Caption = captionInputField.text,
                        MemberId = Guid.Parse(PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId)),
                        RecordingId = recordingList[recordingDropDown.value].RecordingId,
                        PostType = 0,

                    },
                    (post) =>
                    {
                        Debug.Log("Create post success " + post);
                        forumNewFeedManager.Refresh();
                        createPostPanel.SetActive(false);
                    },
                    (msg) =>
                    {
                        Debug.LogError(msg);
                        createPostPanel.SetActive(false);
                    }
                );
                    },
                    () => { }
                );


        }

        private void CloneRecording(Recording recording, Action onSuccess, Action onError)
        {
            List<CreateVoiceAudioRequest> createVoiceAudioRequests = new List<CreateVoiceAudioRequest>();
            foreach (var audio in recording.VoiceAudios)
            {
                createVoiceAudioRequests.Add(new()
                {
                    VoiceUrl = audio.VoiceUrl,
                    StartTime = audio.StartTime,
                    EndTime = audio.EndTime,
                    MemberId = audio.MemberId,
                });
            }
            CreateRecordingRequest request = new CreateRecordingRequest()
            {
                RecordingName = recording.RecordingName,
                RecordingType = (int)recording.RecordingType,
                Score = recording.Score,
                PurchasedSongId = recording.PurchasedSongId,
                HostId = recording.HostId,
                OwnerId = Guid.Parse("f464d4b4-0c97-40a9-bcbe-58bfdbe6bbf6"),
                KaraokeRoomId = recording.KaraokeRoomId,
                StartTime = recording.StartTime,
                EndTime = recording.EndTime,
                VoiceAudios = createVoiceAudioRequests,
            };

            ApiHelper.Instance.GetComponent<RecordingController>()
                .AddRecordingCoroutine(
                    request,
                    (recordingResult) => { cloneRecording = recordingResult.Value; onSuccess.Invoke(); },
                    (msg) => { Debug.Log("Clone recording fail " + msg); onError.Invoke(); }
                );
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
