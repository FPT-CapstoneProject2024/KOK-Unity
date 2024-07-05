using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
            if (_viewportContent == null) { return; }
            if (_runner == null) { _runner = NetworkRunner.Instances[0]; }
            foreach (Transform child in _viewportContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (PlayerRef player in _runner.ActivePlayers)
            {
                try
                {
                    GameObject participant = Instantiate(_participantHolderPrefab, _viewportContent.transform);
                    participant.name = _runner.GetPlayerObject(player).GetComponent<PlayerStats>().PlayerName.ToString();
                    participant.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(_runner.GetPlayerObject(player).GetComponent<PlayerStats>().AvatarCode);
                    participant.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _runner.GetPlayerObject(player).GetComponent<PlayerStats>().PlayerName.ToString();
                    participant.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Test";
                }
                catch { }
            }
        }
    }
}
