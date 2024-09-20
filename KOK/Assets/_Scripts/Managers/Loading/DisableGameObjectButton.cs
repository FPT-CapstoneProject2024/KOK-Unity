using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class DisableGameObjectButton : MonoBehaviour
    {
        [SerializeField] GameObject gameObjectToDisable;

        public void OnClick()
        {
            gameObjectToDisable.SetActive(false);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
