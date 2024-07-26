using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KOK
{
    public class RoomLogManager : MonoBehaviour
    {
        public string FolderPath { get; private set; } = "KOK/RoomLog";
        public string FileName { get; private set; } = "";

        private void Awake()
        {
            FolderPath = Path.Combine(Application.persistentDataPath, FolderPath);
            FileName = Path.Combine(FolderPath, FileName);
        }


        public void GenerateRoomLogFile()
        {

        }

        public void DownloadRoomLogFile()
        {

        }

        public void UploadRoomLogFile()
        {

        }
    }
}
