using UnityEngine;

namespace CarController.CarInfo
{
    [CreateAssetMenu(fileName = "CarConfig", menuName = "Car/Config", order = 0)]
    public class CarConfig : ScriptableObject
    {
        [Header("CAR SETUP")]
        [Space(10)]
        [Range(20, 190)]
        public int maxSpeed = 90; //The maximum speed that the car can reach in km/h.
        [Range(10, 120)]
        public int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.
        [Range(1, 10)]
        public int accelerationMultiplier = 2; // How fast the car can speed up. 1 is a slow acceleration and 10 is the fastest.
        [Space(10)]
        [Range(10, 45)]
        public int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
        [Range(0.1f, 1f)]
        public float steeringSpeed = 0.5f; // How fast the steering wheel turns.
        [Space(10)]
        [Range(100, 600)]
        public int brakeForce = 350; // The strength of the wheel brakes.
        [Range(1, 10)]
        public int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
        [Range(1, 10)]
        public int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
        [Space(10)]
        public Vector3 bodyMassCenter; // This is a vector that contains the center of mass on the car. I recommend set this value.
                                        // In the points x = 0 and z = 0 of your car. You can select the value that you want in the y-axis,
                                        // however, you must notice that the higher this value is, the more unstable the car becomes.
                                        // Usually the y value goes from 0 to 1.5.

                                        
        // Components
        
        private Rigidbody _rigidbody; // Stores the car's rigidbody.
        
        
        /*
        The following variables are used to store information about sideways friction of the wheels (such as
        extremumSlip, extremumValue, asymptoteSlip, asymptoteValue and stiffness).We change these values to
        make the car start drifting.
        */
        private WheelFrictionCurve _flWheelFriction;
        private float _flWExtremumSlip;
        private WheelFrictionCurve _fRWheelFriction;
        private float _frWExtremumSlip;
        private WheelFrictionCurve _rLWheelFriction;
        private float _rlWExtremumSlip;
        private WheelFrictionCurve _rRWheelFriction;
        private float _rrWExtremumSlip;
        
        
    }
}