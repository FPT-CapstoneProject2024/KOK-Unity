using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOK
{
    public class PlayerInputHandler : NetworkBehaviour
    {
        [SerializeField] private float speed;
        Vector3 direction;
        string deviceType;

        void Start()
        {
            deviceType = SystemInfo.deviceType.ToString();
            //Debug.Log(deviceType + "============================");
        }
        private void Update()
        {
            if (deviceType == "Desktop")
            {
                direction = GetWindowKeyInput();
            }
            else
            {
                //direction = GetMobileAccelerometerValue();
            }



        }


        public override void FixedUpdateNetwork()
        {
            transform.Translate(direction);
        }

        Vector3 GetWindowKeyInput()
        {
            float horizontal = 0;
            float vertical = 0;
            if (Input.GetKey(KeyCode.W))
            {
                vertical += 1;
            }
            else
            if (Input.GetKey(KeyCode.S))
            {
                vertical -= 1;
            }
            else
            if (Input.GetKey(KeyCode.A))
            {
                horizontal -= 1;
            }
            else
            if (Input.GetKey(KeyCode.D))
            {
                horizontal += 1;
            }
            return new Vector3(horizontal, vertical, 0) * speed;
        }

        Vector3 GetMobileAccelerometerValue()
        {
            Vector3 acc = Vector3.zero;
            float period = 0.0f;

            foreach (AccelerationEvent evnt in Input.accelerationEvents)
            {
                acc += evnt.acceleration * evnt.deltaTime;
                period += evnt.deltaTime;
            }
            if (period > 0)
            {
                acc *= 1.0f / period;
            }
            return acc * speed;
        }
    }

}
