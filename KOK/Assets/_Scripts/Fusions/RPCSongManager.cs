using Fusion;
using KOK.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace KOK
{
    public class RPCSongManager : NetworkBehaviour
    {
        [Rpc]
        public static void Rpc_PlaySongByGettingURL(NetworkRunner runner)
        {
            //return runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().GetSongURLToPlay();
        }

        [Rpc]
        public static void Rpc_AddSong(NetworkRunner runner, string songCode, string singer1Name, string singer2Name)
        {
                runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().AddSongToQueue(songCode, singer1Name, singer2Name);
            
        }
        [Rpc]
        public static void Rpc_PrioritizeSong(NetworkRunner runner, string songCode, string singer1Name, string singer2Name)
        {
            runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().PrioritizeSongToQueue(songCode, singer1Name, singer2Name);
        }

        [Rpc]
        public static void Rpc_RemoveSong(NetworkRunner runner, int index)
        {
            runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().RemoveSongFromQueue(index);
        }
        [Rpc]
        public static void Rpc_MoveSongToIndex(NetworkRunner runner, int moveIndex, int targetIndex)
        {

        }
        [Rpc]
        public static void Rpc_ClearSongQueue(NetworkRunner runner)
        {
            runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().ClearSongQueue();
        }

        [Rpc]
        public static void Rpc_SetSingerAuto(NetworkRunner runner)
        {
            runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().SetSinger();
        }

        [Rpc]
        public static void Rpc_StartRecording(NetworkRunner runner)
        {
            runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().StartRecording();
        }
        
        [Rpc]
        public static void Rpc_StopRecording(NetworkRunner runner)
        {
            runner.GetPlayerObject(runner.LocalPlayer).GetComponent<PlayerNetworkBehavior>().StopRecording();
        }

        [Rpc]
        public static void Rpc_ShowNoti(NetworkRunner runner, string content, bool isSuccess)
        {
            FindAnyObjectByType<RoomClientController>().roomNotification.ShowNoti(content, isSuccess);

        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
