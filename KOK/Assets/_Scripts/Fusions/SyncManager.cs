using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
                var delta = runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerStats>().videoPlayer.time - GetSortedList()[0].videoPlayer.time;

                Debug.Log(runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerStats>().PlayerName + ": " + runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerStats>().videoPlayer.time);
                Debug.Log(GetSortedList()[0].PlayerName + ": " + GetSortedList()[0].videoPlayer.time);

                if (delta < -0.5 && delta > 0.5)
                {
                    videoPlayer.time = GetSortedList()[0].videoPlayer.time;
                    Debug.Log("Delta: " + delta);
                }
            }
            StartCoroutine(SyncVideo());
        }

        private List<PlayerStats> GetSortedList()
        {
            List<PlayerStats> list = FindObjectsOfType<PlayerStats>().ToList();
            list.Sort();
            return list;

        }
    }
}
