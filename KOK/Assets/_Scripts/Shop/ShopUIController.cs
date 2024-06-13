using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KOK
{
    public class ShopUIController : MonoBehaviour
    {
        // A dictionary to map category names to their respective panels
        public Dictionary<string, GameObject> categoryPanels = new Dictionary<string, GameObject>();
        // A dictionary to map category names to their respective buttons
        public Dictionary<string, Button> categoryButtons = new Dictionary<string, Button>();

        // A list to assign in the inspector for categories and corresponding panels
        public List<string> categories;
        public List<GameObject> panels;
        public List<Button> buttons;

        void Start()
        {
            Assign();
        }

        // Assign panels and buttons
        void Assign()
        {
            for (int i = 0; i < categories.Count; i++)
            {
                categoryPanels[categories[i]] = panels[i];
                categoryButtons[categories[i]] = buttons[i];

                string category = categories[i];
                categoryButtons[category].onClick.AddListener(() => ShowPanel(category));
            }

            if (categories.Count > 0)
            {
                ShowPanel(categories[0]);
            }
        }

        void ShowPanel(string category)
        {
            foreach (var panel in categoryPanels.Values)
            {
                panel.SetActive(false);
            }

            if (categoryPanels.ContainsKey(category))
            {
                categoryPanels[category].SetActive(true);
            }
        }
    }
}
