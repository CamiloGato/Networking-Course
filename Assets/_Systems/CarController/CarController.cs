using CarController.Subsystems;
using UnityEngine;

namespace CarController
{
    public class CarController : MonoBehaviour
    {
        private ICarSystem[] _systems;

        private void Start()
        {
            foreach (ICarSystem carSystem in _systems)
            {
                carSystem.StartSystem();
            }
        }

    }
}