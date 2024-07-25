using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class AddSongMenuSingle : MonoBehaviour
    {
        [SerializeField] SinglePlayerManager playerManager;

        void Start ()
        {
            playerManager = FindAnyObjectByType<SinglePlayerManager>();
        }
        public void OnButtonAddSongClick()
        {
            playerManager.AddSongToQueue(GetComponentInChildren<SongBinding>().Song.SongCode);
            Destroy(gameObject);
        }

        public void OnButtonPrioritizeSongClick()
        {
            playerManager.PrioritizeSongToQueue(GetComponentInChildren<SongBinding>().Song.SongCode);
            Destroy(gameObject);
        }
    }
}
