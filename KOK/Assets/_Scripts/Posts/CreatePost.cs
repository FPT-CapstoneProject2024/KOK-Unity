using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Request.Post;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response;
using KOK.Assets._Scripts.ApiHandler.DTOModels.Response.VoiceAudios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace KOK.Assets._Scripts.Posts
{
    public class CreatePost : MonoBehaviour
    {
        //private List<(Recording recording, List<VoiceAudio> voiceAudio)> optionMappings;
        private List<Recording> optionMappings;
        /*public GameObject recordingPrefab;
        public GameObject displayPanel;*/

        public TMP_Dropdown TmpDropDown;
        public TMP_InputField CaptionText;
        private string playerId;

        private void Awake()
        {
            playerId = PlayerPrefsHelper.GetString(PlayerPrefsHelper.Key_AccountId);
        }

        private void Start()
        {
            GetRecordingByOwnerId(Guid.Parse(playerId));
        }

        public void GetRecordingByOwnerId(Guid ownerId)
        {
            ApiHelper.Instance.gameObject
                .GetComponent<RecordingController>()
                .GetRecordingsByOwnerIdCoroutine(ownerId,
                                                    PopulateTMPDropdown,
                                                    OnError
                                                    );
        }

        private void PopulateTMPDropdown(List<Recording> recordings)
        {
            // Clear existing options
            TmpDropDown.ClearOptions();
            optionMappings = new List<Recording>();

            // Create a list to store new options
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (var recording in recordings)
            {
                string optionText = $"{recording.RecordingName}";
                options.Add(new TMP_Dropdown.OptionData(optionText));
                optionMappings.Add(recording);
            }

            TmpDropDown.AddOptions(options);
        }     

        private void AddPost(Recording recording)
        {
            CreatePostRequest createPostRequest = new CreatePostRequest()
            {
                Caption = CaptionText.text,
                MemberId = Guid.Parse(playerId),
                RecordingId = recording.RecordingId,
                Status = 1,
                PostType = 0
            };

            ApiHelper.Instance.GetComponent<PostController>().AddPostCoroutine(
                createPostRequest,
                (post) => { Debug.Log(post); },
                (ex) => { Debug.LogError(ex.Message); }
            );
        }

        public void CreateCloneRecording()
        {
            int index = TmpDropDown.value;
            if (index >= 0 && index < optionMappings.Count)
            {
                var selectedRecording = optionMappings[index];
                var recordingName = "Clone_" + selectedRecording.RecordingName;
                List<string> voiceAudioList = new List<string>();
                List<string> ownerList = new List<string>();
                //Guid recordingId = selectedMapping.recording.RecordingId;
                foreach (VoiceAudio voiceAudio in selectedRecording.VoiceAudios)
                {
                    voiceAudioList.Add(voiceAudio.VoiceUrl);
                    ownerList.Add(voiceAudio.MemberId.ToString());
                }

                //Create Recording request
                CreateRecordingRequest createRecordingRequest = new CreateRecordingRequest()
                {
                    RecordingName = recordingName,
                    RecordingType = 0,
                    Score = selectedRecording.Score,
                    PurchasedSongId = selectedRecording.PurchasedSongId,
                    HostId = selectedRecording.HostId,
                    //give a fixed clone ownerid"
                    OwnerId = Guid.Parse("2e5abe65-907d-469e-b79b-d4df8dc9a29f"),
                    KaraokeRoomId = selectedRecording.KaraokeRoomId,
                    StartTime = selectedRecording.StartTime,
                    EndTime = selectedRecording.EndTime,
                    VoiceAudios = new List<CreateVoiceAudioRequest>()
                };
                for (int i = 0; i < voiceAudioList.Count; i++)
                {
                    createRecordingRequest.VoiceAudios.Add(
                        new()
                        {
                            VoiceUrl = voiceAudioList[i],
                            DurationSecond = 0,
                            StartTime = 0f,
                            EndTime = 0f,
                            MemberId = Guid.Parse(ownerList[i]),
                        });
                }

                // Add post with clone recording
                ApiHelper.Instance.GetComponent<RecordingController>().AddRecordingCoroutine(
                    createRecordingRequest,
                    (rr) => { AddPost(rr.Value); },
                    (ex) => { Debug.LogError(ex.Message); }
                );
            }
        }

        private void OnError(string error)
        {
            Debug.Log(error);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}