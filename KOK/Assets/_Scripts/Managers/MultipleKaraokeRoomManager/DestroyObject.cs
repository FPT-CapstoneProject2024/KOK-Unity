using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class DestroyObject : MonoBehaviour
    {
        [SerializeField] private GameObject _objectToDestroy;

        public void DestroyTargetGameObject()
        {
            Destroy(_objectToDestroy);
        }
    }
}
