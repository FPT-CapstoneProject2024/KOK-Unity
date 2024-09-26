using Fusion;
using KOK.ApiHandler.DTOModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class PlayerInputHandler : NetworkBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] PlayerNetworkBehavior playerNetworkBehavior;
        public bool upPress = false;
        public bool downPress = false;
        public bool leftPress = false;
        public bool rightPress = false;
        public bool faceUp = false;
        public bool faceDown = false;
        public bool faceLeft = false;
        public bool faceRight = false;
        public Vector3 direction;
        string deviceType;

        void Start()
        {
            deviceType = SystemInfo.deviceType.ToString();
            if (HasStateAuthority)
            {
                PlayerController controller = FindAnyObjectByType<PlayerController>();
                controller.Init(
                    () => ButtonUpPress(),
                    () => ButtonUpRelease(),
                    () => ButtonDownPress(),
                    () => ButtonDownRelease(),
                    () => ButtonLeftPress(),
                    () => ButtonLeftRelease(),
                    () => ButtonRightPress(),
                    () => ButtonRightRelease()
                    );
            }
        }
        private void Update()
        {
            //if (deviceType == "Desktop")
            //{
            //    GetKeyBoardInput();
            //}
            direction = GetDirectionVector();
            AnimationPlayer();

        }


        public override void FixedUpdateNetwork()
        {
            transform.Translate(direction);

        }

        Vector3 GetDirectionVector()
        {
            float horizontal = 0;
            float vertical = 0;
            if (upPress)
            {
                vertical += 1;
            }
            else if (downPress)
            {
                vertical -= 1;
            }
            else if (leftPress)
            {
                horizontal -= 1;
            }
            else if (rightPress)
            {
                horizontal += 1;
            }
            return new Vector3(horizontal, vertical, 0) * speed;
        }

        private void ResetFace()
        {
            faceUp = false;
            faceDown = false;
            faceLeft = false;
            faceRight = false;
        }
        public void GetKeyBoardInput()
        {

            if (Input.GetKey(KeyCode.W))
            {
                upPress = true;
                ResetFace();
                faceUp = true;
            }
            else
            {
                upPress = false;
            }

            if (Input.GetKey(KeyCode.S))
            {
                downPress = true;
                ResetFace();
                faceDown = true;
            }
            else
            {
                downPress = false;
            }

            if (Input.GetKey(KeyCode.A))
            {
                leftPress = true;
                ResetFace();
                faceLeft = true;
            }
            else
            {
                leftPress = false;
            }

            if (Input.GetKey(KeyCode.D))
            {
                rightPress = true;
                ResetFace();
                faceRight = true;
            }
            else
            {
                rightPress = false;
            }
        }

        #region Mobie Control
        public void ButtonUpPress()
        {
            upPress = true;
            ResetFace();
            faceUp = true;
            Debug.LogError("ButtonUpPress" + upPress);
        }

        public void ButtonUpRelease()
        {
            upPress = false;
            Debug.LogError("ButtonUpPress" + upPress);
        }
        public void ButtonDownPress()
        {
            downPress = true;
            ResetFace();
            faceDown = true;
        }

        public void ButtonDownRelease()
        {
            downPress = false;
        }
        public void ButtonLeftPress()
        {
            leftPress = true;
            ResetFace();
            faceLeft = true;
        }

        public void ButtonLeftRelease()
        {
            leftPress = false;
        }
        public void ButtonRightPress()
        {
            rightPress = true;
            ResetFace();
            faceRight = true;
        }

        public void ButtonRightRelease()
        {
            rightPress = false;
        }

        #endregion

        #region Animation

        public void AnimationPlayer()
        {
            if (direction.x == 0f && direction.y == 0f)
            {
                if (faceUp)
                {
                    playerNetworkBehavior.PlayAnimation(AnimationName.IdleBack);
                }
                else if (faceDown)
                {
                    playerNetworkBehavior.PlayAnimation(AnimationName.IdleFront);
                }
                else if (faceRight)
                {
                    playerNetworkBehavior.PlayAnimation(AnimationName.IdleRight);
                }
                else if (faceLeft)
                {
                    playerNetworkBehavior.PlayAnimation(AnimationName.IdleLeft);
                }
            }
            else
            {
                if (upPress)
                {
                    playerNetworkBehavior.PlayAnimation(AnimationName.WalkBack);
                }
                else if (downPress)
                {
                    playerNetworkBehavior.PlayAnimation(AnimationName.WalkFront);
                }
                else if (rightPress)
                {
                    playerNetworkBehavior.PlayAnimation(AnimationName.WalkRight);
                }
                else if (leftPress)
                {
                    playerNetworkBehavior.PlayAnimation(AnimationName.WalkLeft);
                }
            }
        }

        #endregion
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }

}
