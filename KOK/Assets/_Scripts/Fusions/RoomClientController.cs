using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class RoomClientController : MonoBehaviour
{
    [SerializeField] Toggle muteToggle;
    [SerializeField] Toggle echoToggle;
    [SerializeField] Recorder recorder;


    public TextMeshProUGUI textMeshProUGUI;

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

    public void CheckSinger(bool isSinger)
    {
        if (!isSinger)
        {
            TurnOffMic();
            recorder.TransmitEnabled = false;
            muteToggle.enabled = false;
        }
        else
        {
            muteToggle.enabled = true;
            recorder.TransmitEnabled = true;
        }
    }

    public void TurnOffMic()
    {
        muteToggle.isOn = true;
        recorder.TransmitEnabled = false;
    }
    public void TurnOnMic()
    {
        muteToggle.isOn = false;
        recorder.TransmitEnabled = true;
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


    IEnumerator DebugTest()
    {
        yield return new WaitForSeconds(1);
        textMeshProUGUI.text = "Recorder: " + recorder.TransmitEnabled + " | " + recorder.DebugEchoMode;
        StartCoroutine(DebugTest());
    }
}
