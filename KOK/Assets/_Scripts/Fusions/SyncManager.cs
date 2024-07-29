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

        public void StartVideoFollowHost()
        {
            runner = NetworkRunner.Instances[0];
            foreach (PlayerRef player in runner.ActivePlayers)
            {
                runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().Rpc_PrepareVideo();
                if (runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerRole == 0)
                {
                    host = player;
                    runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().Rpc_PlayVideo();
                }
            }
            StartCoroutine(WaitForHostPrepareVideo());
        }

        IEnumerator WaitForHostPrepareVideo()
        {
            yield return new WaitForSeconds(0.1f);
            if (runner.GetPlayerObject(host).GetComponent<PlayerNetworkBehavior>().videoTime > 0)
            {
                foreach (PlayerRef player in runner.ActivePlayers)
                {
                    if (runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerRole != 0)
                    {
                        runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().Rpc_PlayVideo();
                    }
                }
                RPCSongManager.Rpc_StartRecording(runner);
                yield return new WaitForSeconds(1f);
                RPCSongManager.Rpc_RemoveSong(runner, 0);
            }
            else
            {
                StartCoroutine(WaitForHostPrepareVideo());
            }
        }
        public void StartSyncVideo()
        {
            runner = NetworkRunner.Instances[0];
            foreach (PlayerRef player in runner.ActivePlayers)
            {
                if (runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerRole == 0)
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
                var delta = runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().videoTime - runner.GetPlayerObject(host).GetComponent<PlayerNetworkBehavior>().videoTime;

                //Debug.Log(runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerName + ": "
                //    + runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().videoTime + " - "
                //    + runner.GetPlayerObject(host).GetComponent<PlayerNetworkBehavior>().videoTime + " = "
                //    + delta);
                //Debug.Log(GetSortedList()[0].PlayerName + ": " + GetSortedList()[0].videoTime);

                if (delta < -1d || delta > 1d)
                {
                    Debug.LogError("Delta: " + delta);
                    runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().videoTime = runner.GetPlayerObject(host).GetComponent<PlayerNetworkBehavior>().videoTime;
                    runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().Rpc_SetVideoPlayerSyncTime();

                }
            }
            StartCoroutine(SyncVideo());
        }


    }
}
