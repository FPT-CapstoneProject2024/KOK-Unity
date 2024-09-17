using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

namespace KOK
{
    public class VoiceManager : MonoBehaviour
    {
        public static VoiceManager Instance { get; private set; }
        public Recorder recorder;
        public TextMeshProUGUI textMeshProUGUI;
        bool isRequesting;

        public bool HasPermission { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitVoice();
            //StartCoroutine(DebugRecorderPosition());
        }

        public void InitVoice()
        {
#if PLATFORM_ANDROID
            

            if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                HasPermission = true;
            }
            else
            {
                Permission.RequestUserPermission(Permission.Microphone);
                isRequesting = true;
            }
#else
        HasPermission = true;
#endif
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && isRequesting)
            {
                if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
                {
                    HasPermission = true;
                }
                else
                {
                    HasPermission = false;
                }
                isRequesting = false;
            }
        }

        IEnumerator DebugRecorderPosition()
        {
            yield return new WaitForSeconds(1);
            textMeshProUGUI.text = "Recorder position " + SystemInfo.deviceName + ": " + Microphone.GetPosition(SystemInfo.deviceName);
            StartCoroutine(DebugRecorderPosition());
        }

        //public void StartRecording()
        //{
        //    if (HasPermission)
        //    {
        //        recorder.TransmitEnabled = true;
        //        recorder.StartRecording();
        //    }
        //}

        //public void StopRecording()
        //{
        //    if (HasPermission)
        //    {
        //        recorder.StopRecording();
        //        recorder.TransmitEnabled = false;
        //    }
        //}
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
