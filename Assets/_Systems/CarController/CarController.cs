using CarController.CarInfo;
using CarController.StateMachine.CarStates;
using FiniteStateMachine;
using UnityEngine;

namespace CarController
{
    public enum CarStateType
    {
        Idle,
        Accelerating,
        Decelerating,
        Braking,
    }
    
    public class CarController : MonoBehaviour
    {
        [Header("Car Config")]
        [SerializeField] private CarConfig carConfig;
        [SerializeField] private CarStats carStats;
        
        [Header("Car Components")]
        // TODO: Components
        
        private FsmMachine<CarStateType, CarState> _fsmCarMachine;
        
    }
}