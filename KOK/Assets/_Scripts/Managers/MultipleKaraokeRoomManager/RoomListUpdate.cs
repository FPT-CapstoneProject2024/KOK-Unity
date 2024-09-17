using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KOK
{
    public class RoomListUpdate : MonoBehaviour
    {
        [SerializeField] GameObject _roomHolderPrefab;
        [SerializeField] Transform _roomViewportContent;
        public void UpdateRoomList(List<SessionInfo> sessionList)
        {
            foreach (Transform child in _roomViewportContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (var session in sessionList)
            {
                var roomHolderObject = Instantiate(_roomHolderPrefab, _roomViewportContent);
                roomHolderObject.name = session.Name;
                roomHolderObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = session.Name;
                roomHolderObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                roomHolderObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = session.PlayerCount.ToString() + "/10";
            }
        }

        public void ClearRoomList()
        {
            foreach (Transform child in _roomViewportContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
