using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KOK
{
    public class PlayerController : MonoBehaviour
    {
        public Action buttonUpPressAction;
        public Action buttonUpReleaseAction;
        public Action buttonDownPressAction;
        public Action buttonDownReleaseAction;
        public Action buttonLeftPressAction;
        public Action buttonLeftReleaseAction;
        public Action buttonRightPressAction;
        public Action buttonRightReleaseAction;

        public void Init(Action buttonUpPressAction, Action buttonUpReleaseAction,
                            Action buttonDownPressAction, Action buttonDownReleaseAction,
                            Action buttonLeftPressAction, Action buttonLeftReleaseAction,
                            Action buttonRightPressAction, Action buttonRightReleaseAction)
        {
            this.buttonUpPressAction = buttonUpPressAction;
            this.buttonUpReleaseAction = buttonUpReleaseAction;
            this.buttonDownPressAction = buttonDownPressAction;
            this.buttonDownReleaseAction = buttonDownReleaseAction;
            this.buttonLeftPressAction = buttonLeftPressAction;
            this.buttonLeftReleaseAction = buttonLeftReleaseAction;
            this.buttonRightPressAction = buttonRightPressAction;
            this.buttonRightReleaseAction = buttonRightReleaseAction;
        }

        public void ButtonUpPress()
        {
            buttonUpPressAction.Invoke();
        }

        public void ButtonUpRelease()
        {
            buttonUpReleaseAction.Invoke();
        }
        public void ButtonDownPress()
        {
            buttonDownPressAction.Invoke();
        }

        public void ButtonDownRelease()
        {
            buttonDownReleaseAction.Invoke();
        }
        public void ButtonLeftPress()
        {
            buttonLeftPressAction.Invoke();
        }

        public void ButtonLeftRelease()
        {
            buttonLeftReleaseAction.Invoke();
        }
        public void ButtonRightPress()
        {
            buttonRightPressAction.Invoke();
        }

        public void ButtonRightRelease()
        {
            buttonRightReleaseAction.Invoke();
        }
    }
}
