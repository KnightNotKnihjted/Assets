using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace JaasUtilities
{
    public class WorldObjectButton : MonoBehaviour
    {
        public UnityEvent onClick = new ();
        public Action onClickAction = new (() => { });
        private void Awake()
        {
            onClickAction += () => onClick.Invoke();
        }
        private void OnMouseDown()
        {
            onClickAction.Invoke();
        }
    }
}
