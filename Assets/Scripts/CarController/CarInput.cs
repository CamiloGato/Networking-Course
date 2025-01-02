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
        public PrometeoTouchInput throttleButton;
        public PrometeoTouchInput reverseButton;
        public PrometeoTouchInput turnRightButton;
        public PrometeoTouchInput turnLeftButton;
        public PrometeoTouchInput handbrakeButton;

        private void Update()
        {
            if (car == null) return;

            if (useTouchControls)
            {
                // Adelante
                if (throttleButton != null && throttleButton.buttonPressed)
                {
                    car.StopDeceleration();
                    car.GoForward();
                }

                // Reversa
                if (reverseButton != null && reverseButton.buttonPressed)
                {
                    car.StopDeceleration();
                    car.GoReverse();
                }

                // Gira izquierda
                if (turnLeftButton != null && turnLeftButton.buttonPressed)
                {
                    car.TurnLeft();
                }

                // Gira derecha
                if (turnRightButton != null && turnRightButton.buttonPressed)
                {
                    car.TurnRight();
                }

                // Freno de mano
                if (handbrakeButton != null && handbrakeButton.buttonPressed)
                {
                    car.StopDeceleration();
                    car.Handbrake();
                }
                else
                {
                    car.RecoverTraction();
                }

                // Soltar adelante y atrás
                if ((!throttleButton.buttonPressed && !reverseButton.buttonPressed))
                {
                    car.ThrottleOff();
                }

                // Iniciar desaceleración si no se pulsa nada
                if (!throttleButton.buttonPressed && !reverseButton.buttonPressed &&
                    !handbrakeButton.buttonPressed)
                {
                    car.StartDeceleration();
                }

                // Reset del giro si no se pulsa izquierda/derecha
                if (!turnLeftButton.buttonPressed && !turnRightButton.buttonPressed)
                {
                    car.ResetSteeringAngle();
                }
            }
            else
            {
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
}