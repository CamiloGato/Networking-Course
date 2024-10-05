using UnityEngine;

namespace CarController.CarInput
{
    public class CarKeyboardInput : ICarInput
    {
        public float Horizontal()
        {
            return Input.GetAxis("Horizontal");
        }

        public float Vertical()
        {
            return Input.GetAxis("Vertical");
        }

        public bool Brake()
        {
            return Input.GetKey(KeyCode.Space);
        }
    }
}