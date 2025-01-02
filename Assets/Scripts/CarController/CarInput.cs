using UnityEngine;

namespace CarController
{
    /// <summary>
    /// Captura Input (teclado o botones táctiles) y llama a los métodos de CarController.
    /// </summary>
    public class CarInput : MonoBehaviour
    {
        [SerializeField] private CarController car;

        [Header("Touch Controls")] public bool useTouchControls = false;

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

            // Cuando no se presiona W o S
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                car.ThrottleOff();
            }

            // Desacelerar
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.Space))
            {
                car.StartDeceleration();
            }

            // Regresar ángulo de giro
            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                car.ResetSteeringAngle();
            }
        }
    }
}