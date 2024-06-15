using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using UnityEngine.UI;
using Fusion;

public class RoomClientController : MonoBehaviour
{
    [SerializeField] Toggle muteToggle;
    [SerializeField] Toggle echoToggle;
    [SerializeField] Recorder recorder;

    void Start()
    {
        ToggleMic();
        ToggleEcho();
    }

    public void ToggleMic()
    {
        if (muteToggle.isOn == true)
        {
            recorder.TransmitEnabled = false;
        }
        else
        {
            recorder.TransmitEnabled = true;
        }
    }

    public void ToggleEcho()
    {
        if (echoToggle.isOn == true)
        {
            recorder.DebugEchoMode = true;
        }
        else
        {
            recorder.DebugEchoMode = false;
        }
    }

    public static void TestRPC()
    {

        RPCVideoPlayerDemo.Rpc_TestPlayerList(FindAnyObjectByType<NetworkRunner>(), 1);
    }

    //public static void Rpc_PrepareVideo()
    //{
    //    RPCVideoPlayerDemo.Rpc_Prepare(FindAnyObjectByType<NetworkRunner>(), 1);
    //}
}
