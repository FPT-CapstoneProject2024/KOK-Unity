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
        private PlayerRef host;
        public NetworkRunner Runner { get => runner; set => runner = value; }

        public void StartSyncVideo()
        {
            runner = NetworkRunner.Instances[0];
            foreach(PlayerRef player in runner.ActivePlayers)
            {
                if(runner.GetPlayerObject(player).GetComponent<PlayerStats>().PlayerRole == 0)
                {
                    host = player; 
                }
            }
            StartCoroutine(SyncVideo());
        }

        public void StopSyncVideo()
        {
            StopCoroutine(coroutine);
        }
        IEnumerator SyncVideo()
        {
            yield return new WaitForSeconds(3);
            foreach (PlayerRef player in runner.ActivePlayers)
            {
                var delta = runner.GetPlayerObject(player).GetComponent<PlayerStats>().videoTime - runner.GetPlayerObject(host).GetComponent<PlayerStats>().videoTime;

                Debug.Log(runner.GetPlayerObject(player).GetComponent<PlayerStats>().PlayerName + ": " 
                    + runner.GetPlayerObject(player).GetComponent<PlayerStats>().videoTime + " - "
                    + runner.GetPlayerObject(host).GetComponent<PlayerStats>().videoTime + " = "
                    + delta + " ================================================");
                //Debug.Log(GetSortedList()[0].PlayerName + ": " + GetSortedList()[0].videoTime);

                if (delta < -0.5d || delta > 0.5d)
                {
                    Debug.LogError("Delta: " + delta);
                    runner.GetPlayerObject(player).GetComponent<PlayerStats>().videoTime = runner.GetPlayerObject(host).GetComponent<PlayerStats>().videoTime;
                    runner.GetPlayerObject(player).GetComponent<PlayerStats>().Rpc_SetVideoPlayerSyncTime();
                    
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
