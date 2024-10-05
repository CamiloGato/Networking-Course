using CarController.CarInfo;
using UnityEngine;

namespace CarController.StateMachine.CarMovement
{
    public abstract class CarMovementSystem : CarSystem
    {
        private CarConfig _carConfig;
        private CarStats _carStats;

        protected CarConfig CarConfig => _carConfig;
        protected CarStats CarStats => _carStats;
        
        protected CarMovementSystem(CarConfig carConfig, CarStats carStats)
        {
            _carConfig = carConfig;
            _carStats = carStats;
        }
        
    }
}