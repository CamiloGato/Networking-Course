using System;
using UnityEngine;

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
        
        /*
        The following variables are used to store information about sideways friction of the wheels (such as
        extremumSlip, extremumValue, asymptoteSlip, asymptoteValue and stiffness).We change this values to
        make the car start drifting.
        */
        public WheelFrictionCurve FlWheelFriction;
        public float flWheelExtremumSlip;
        public WheelFrictionCurve FrWheelFriction;
        public float frWheelExtremumSlip;
        public WheelFrictionCurve RlWheelFriction;
        public float rlWheelExtremumSlip;
        public WheelFrictionCurve RrWheelFriction;
        public float rrWheelExtremumSlip;

        public void InitializeWheelFrictionCurves(
            WheelCollider frontLeftCollider,
            WheelCollider frontRightCollider,
            WheelCollider rearLeftCollider,
            WheelCollider rearRightCollider
        )
        {
            FlWheelFriction = new WheelFrictionCurve
            {
                extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip,
                extremumValue = frontLeftCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue,
                stiffness = frontLeftCollider.sidewaysFriction.stiffness
            };
            flWheelExtremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
            
            FrWheelFriction = new WheelFrictionCurve
            {
                extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip,
                extremumValue = frontRightCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue,
                stiffness = frontRightCollider.sidewaysFriction.stiffness
            };
            frWheelExtremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
            
            RlWheelFriction = new WheelFrictionCurve
            {
                extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip,
                extremumValue = rearLeftCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue,
                stiffness = rearLeftCollider.sidewaysFriction.stiffness
            };
            rlWheelExtremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
            
            RrWheelFriction = new WheelFrictionCurve
            {
                extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip,
                extremumValue = rearRightCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue,
                stiffness = rearRightCollider.sidewaysFriction.stiffness
            };
            rrWheelExtremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        }
        
    }
}