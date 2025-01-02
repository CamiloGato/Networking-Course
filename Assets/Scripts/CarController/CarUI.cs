using UnityEngine;
using UnityEngine.UI;

namespace CarController
{
    public class CarUI : MonoBehaviour
    {
        [SerializeField] private CarController car;
        [SerializeField] private Text speedText;
        [SerializeField] private bool useUI = false;

        // Podemos actualizar la UI en Update o en un InvokeRepeating, según prefieras.
        private void Update()
        {
            if (!useUI) return;
            if (car == null) return;
            if (speedText == null) return;

            float absoluteCarSpeed = Mathf.Abs(car.CarSpeed);
            speedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
        }
    }
}