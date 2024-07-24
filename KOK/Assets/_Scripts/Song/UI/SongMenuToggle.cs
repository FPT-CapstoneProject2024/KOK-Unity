using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class SongMenuToggle : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Toggle toggleButton;

        public void ToggleMenu()
        {
            menuPanel.SetActive(toggleButton.isOn);
        }
    }
}
