using Fusion;
using KOK.ApiHandler.Controller;
using KOK.ApiHandler.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KOK
{
    public class RoomLogManager : MonoBehaviour
    {
        public string FolderPath { get; private set; } = "RoomLog/";

        public string FullPath { get; private set; } = "";
        public string FileName { get; private set; } = "DefaultLog.txt";

        public static RoomLogManager Instance { get; private set; }

        public Guid roomId;

        private bool allowAppendLog = false;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }

        public void CreateRoomLog(string fileLogName, Guid creatorId)
        {
            FileName = fileLogName;
            FullPath = string.Empty;
            FolderPath = Path.Combine(Application.persistentDataPath, FolderPath);
            FullPath = Path.Combine(FolderPath, FileName);
            ResetRoomLogFile();
            allowAppendLog = true;
            StartPushLogToFirebase();

            //Call api create room log here
            ApiHelper.Instance.GetComponent<KaraokeRoomController>().AddKaraokeRoomCoroutine(
                new()
                {
                    RoomLog = fileLogName,
                    CreatorId = creatorId
                },
                (kr) =>
                {
                    //Debug.LogError(kr.Value.RoomId + "  |  " + kr.Value.ToString()); 
                    roomId = kr.Value.RoomId;
                },
                (ex) =>
                {
                    Debug.LogError(ex);
                });
        }

        public void AssignHostRole()
        {
            NetworkRunner runner = NetworkRunner.Instances[0];
            if (runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().PlayerRole == 0)
            {
                DownloadRoomLogFile();
            }
        }

        public void ResetRoomLogFile()
        {
            if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);

            System.IO.DirectoryInfo di = new(FolderPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            if (!File.Exists(FullPath))
            {
                using (FileStream stream = File.Create(FullPath))
                {
                }
            }
            else
            {
                //File.WriteAllText(FullPath, string.Empty);
            }
        }

        public void DownloadRoomLogFile()
        {
            FirebaseStorageManager.Instance.DownLoadRoomLogFile(FullPath,
                                                                () => { StartPushLogToFirebase(); allowAppendLog = true; },
                                                                () => { });
        }

        public void UploadRoomLogFile()
        {
            FirebaseStorageManager.Instance.UploadRoomLogFile(FullPath,
                                                              (sm) =>
                                                              {
                                                                  Debug.Log("Upload Success room log " + sm.Name);
                                                                  //Delete local room log here

                                                              },
                                                              (ex) => { });
        }

        public void AppendLogToFile(string message)
        {
            if (!allowAppendLog) return;
            if (!File.Exists(FullPath))
            {
                ResetRoomLogFile();
                File.AppendAllText(FullPath, message);
            }
            else
            {
                File.AppendAllText(FullPath, message);
            }
        }

        public void StartPushLogToFirebase()
        {
            StartCoroutine(PushLogToFirebase());
            Debug.Log("StartPushLogToFirebase");
        }

        IEnumerator PushLogToFirebase()
        {
            yield return new WaitForSeconds(60f);
            UploadRoomLogFile();
            StartCoroutine(PushLogToFirebase());
        }

        public void OnButtonSendChatClick()
        {
            ChatManager.Instance.SendChat();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            DirectoryInfo dir = new DirectoryInfo(FolderPath);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

        }
    }
}
