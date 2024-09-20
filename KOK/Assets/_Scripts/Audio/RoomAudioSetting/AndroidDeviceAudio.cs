using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace KOK
{
    public class AndroidDeviceAudio : MonoBehaviour
    {
        void Start()
        {
            
            if (SystemInfo.deviceType.Equals(DeviceType.Handheld))
            {
                SetDeviceVolume(GetDeviceMaxVolume());
            }
        }

        private static int GetDeviceMaxVolume()
        {
            int streammusic = 3;
            return deviceAudio.Call<int>("getStreamMaxVolume", streammusic);
        }

        private static void SetDeviceVolume(int value)
        {
            int streammusic = 3;
            int flagshowui = 0;

            deviceAudio.Call("setStreamVolume", streammusic, value, flagshowui);
        }

        private static AndroidJavaObject audioManager;

        private static AndroidJavaObject deviceAudio
        {
            get
            {
                if (audioManager == null)
                {
                    AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject context = up.GetStatic<AndroidJavaObject>("currentActivity");

                    string audioName = context.GetStatic<string>("AUDIO_SERVICE");

                    audioManager = context.Call<AndroidJavaObject>("getSystemService", audioName);
                }
                return audioManager;
            }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}
