using UnityEngine;

namespace AirPlaneController.AirPlaneInput
{
    public class AirPlaneKeyboardInput : IAirPlaneInput
    {
        public float Horizontal()
        {
            return Input.GetAxis("Horizontal");
        }

        public float Vertical()
        {
            return Input.GetAxis("Vertical");
        }

        public float Yaw()
        {
            return Input.GetKey(KeyCode.Q) ? -1 : Input.GetKey(KeyCode.E) ? 1 : 0;
        }

        public bool Turbo()
        {
            return Input.GetKey(KeyCode.LeftShift);
        }
    }
}