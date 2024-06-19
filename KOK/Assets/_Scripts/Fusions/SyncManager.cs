using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace KOK
{
    public class SyncManager : MonoBehaviour
    {
        private NetworkRunner runner;
        private VideoPlayer videoPlayer;
        private Coroutine coroutine;
        public NetworkRunner Runner { get => runner; set => runner = value; }

        public void StartSyncVideo()
        {
            runner = NetworkRunner.Instances[0];
            videoPlayer = runner.GetPlayerObject(runner.LocalPlayer).gameObject.GetComponent<PlayerStats>().videoPlayer;
            coroutine = StartCoroutine(SyncVideo());
        }

        public void StopSyncVideo()
        {
            StopCoroutine(coroutine);
        }
        IEnumerator SyncVideo()
        {
            yield return new WaitForSeconds(3);
            foreach (NetworkRunner runner in NetworkRunner.Instances)
            {
                var delta = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerStats>().videoPlayer.time - videoPlayer.time;
                if (delta < -1 && delta > 1)
                {
                    runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerStats>().videoPlayer.time = videoPlayer.time;
                }
                Debug.Log(runner.GetPlayerObject(runner.LocalPlayer) + ": " + videoPlayer.time);
            }
            StartCoroutine(SyncVideo());
        }
    }
}
