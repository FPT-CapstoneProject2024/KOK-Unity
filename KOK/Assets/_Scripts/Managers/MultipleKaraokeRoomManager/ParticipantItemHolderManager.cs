using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

namespace KOK
{
    public class ParticipantItemHandlerManager : MonoBehaviour
    {
        public static ParticipantItemHandlerManager Instance;

        [SerializeField] GameObject _viewportContent;

        [SerializeField] GameObject _participantHolderPrefab;

        private NetworkRunner _runner;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void UpdateParticipantList()
        {
            try
            {
                var runner = FindAnyObjectByType<NetworkRunner>();

                //string test = runner.ActivePlayers.ToList().OrderBy(x => x.ToString()).ToList().ToSeparatedString(",");
                //Debug.Log(test);

                if (_viewportContent == null) { return; }
                if (_runner == null) { _runner = NetworkRunner.Instances[0]; }
                foreach (Transform child in _viewportContent.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (PlayerRef player in _runner.ActivePlayers)
                {
                    GameObject participant = Instantiate(_participantHolderPrefab, _viewportContent.transform);
                    participant.name = _runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerName.ToString();
                    participant.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(_runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().AvatarCode + "AVA");
                    Debug.LogError(_runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().AvatarCode + "AVA");
                    participant.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerName.ToString();
                    participant.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ((RoomRole)(Enum.GetValues(typeof(RoomRole))).GetValue(_runner.GetPlayerObject(player).GetComponent<PlayerNetworkBehavior>().PlayerRole)).ToString();

                }
            }
            catch { }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }

    public enum RoomRole
    {
        Host = 0,
        Participant = 1,
    }
}
