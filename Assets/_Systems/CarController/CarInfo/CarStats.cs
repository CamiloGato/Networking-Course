using System;

namespace CarController.CarInfo
{
    [Serializable]
    public class CarStats
    {
        // SOUNDS
        public float initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.
        
        // CAR DATA
        public float carSpeed;
        public bool isDrifting;
        public bool isTractionLocked;
        
        public float steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
        public float throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
        public float driftingAxis;
        public float localVelocityZ;
        public float localVelocityX;
        public bool deceleratingCar;
        public bool touchControlsSetup;
        
        // WHEELS
        public float fLWheelExtremumSlip;
        public float fRWheelExtremumSlip;
        public float rLWheelExtremumSlip;
        public float rRWheelExtremumSlip;

    }
}