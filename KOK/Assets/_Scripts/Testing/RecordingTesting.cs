using KOK.ApiHandler.Controller;
using KOK.ApiHandler.DTOModels;
using KOK.ApiHandler.Utilities;
using KOK.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class RecordingTesting : MonoBehaviour
    {
        public static RecordingTesting Instance { get; private set; }

        [SerializeField] RecordingManager recordingManager;
        [SerializeField] VoiceRecorder voiceRecorder;

        string audioFile1 = "";
        string audioFile2 = "";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartRecording()
        {
            audioFile1 = "SongCode_SingerUsername_" + DateTime.Now.ToString();
            audioFile1 = audioFile1.Replace(" ", "");
            audioFile1 = audioFile1.Replace(":", "");
            audioFile1 = audioFile1.Replace("/", "");
            voiceRecorder.StartRecording(audioFile1);
        }

        public void StopRecording()
        {
            voiceRecorder.StopRecording();
            CreateRecording(
                "testing",
                UnityEngine.Random.Range(1, 99),
                "ebe4174c-5069-4767-a5c3-a962563d813f",
                "b7e64218-d316-4f2f-bde1-b96ec24057ec",
                "b7e64218-d316-4f2f-bde1-b96ec24057ec",
                "60135c7c-003b-453a-b7c1-6c2c2835a423",
                new List<string>()
                {
                    "b7e64218-d316-4f2f-bde1-b96ec24057ec",
                }
                );
        }

        public void CreateRecording(string recordingName, int score, string purchasedSongId, string hostId, string ownerId, string karaokeRoomId, List<string> memberIds)
        {
            //Get information


            //Create Recording request
            CreateRecordingRequest createRecordingRequest = new CreateRecordingRequest()
            {
                RecordingName = recordingName,
                RecordingType = 0,
                Score = score,
                PurchasedSongId = Guid.Parse(purchasedSongId),
                HostId = Guid.Parse(hostId),
                OwnerId = Guid.Parse(ownerId),
                KaraokeRoomId = Guid.Parse(karaokeRoomId),
                StartTime = 0,
                EndTime = 0,
                VoiceAudios = new List<CreateVoiceAudioRequest>()
                {
                    new CreateVoiceAudioRequest()
                    {
                        VoiceUrl = audioFile1,
                        DurationSecond = 0,
                        StartTime = 0f,
                        EndTime = 0f,
                        MemberId = Guid.Parse(memberIds[0]),
                    }
                }
            };
           

            //Call api create recording, set by file name

            ApiHelper.Instance.GetComponent<RecordingController>().AddRecordingCoroutine(
                createRecordingRequest,
                (rr) => { Debug.LogError(rr.Value); },
                (ex) => { Debug.LogError(ex.Message); }
            );

        }



    }
}
