using UnityEngine;

namespace CarController
{
    public class CarInput : MonoBehaviour
    {
        [SerializeField] private CarController car;

        private void Update()
        {
            if (!car) return;

            // INPUT DE TECLADO
            if (Input.GetKey(KeyCode.W))
            {
                car.StopDeceleration();
                car.GoForward();
            }

            if (Input.GetKey(KeyCode.S))
            {
                car.StopDeceleration();
                car.GoReverse();
            }

            if (Input.GetKey(KeyCode.A))
            {
                car.TurnLeft();
            }

            if (Input.GetKey(KeyCode.D))
            {
                car.TurnRight();
            }

            if (Input.GetKey(KeyCode.Space))
            {
                car.StopDeceleration();
                car.Handbrake();
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                car.RecoverTraction();
            }

            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                car.ThrottleOff();
            }

            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.Space))
            {
                car.StartDeceleration();
            }

            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                car.ResetSteeringAngle();
            }
        }
    }
}