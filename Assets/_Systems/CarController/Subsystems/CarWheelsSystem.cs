using System;
using UnityEngine;

namespace CarController.Subsystems
{
    [Serializable]
    public class CarWheelsSystem : ICarSystem
    {
        [Space(20)]
        [Header("Wheels SetUp")]
        [SerializeField] private GameObject frontLeftWheel;
        [SerializeField] private GameObject frontRightWheel;
        [SerializeField] private GameObject rearLeftWheel;
        [SerializeField] private GameObject rearRightWheel;

        [Space(10)]
        [SerializeField] private WheelCollider frontLeftWheelCollider;
        [SerializeField] private WheelCollider frontRightWheelCollider;
        [SerializeField] private WheelCollider rearLeftWheelCollider;
        [SerializeField] private WheelCollider rearRightWheelCollider;

        private WheelFrictionCurve _frontLeftWheelFrictionCurve;
        private WheelFrictionCurve _frontRightWheelFrictionCurve;
        private WheelFrictionCurve _rearLeftWheelFrictionCurve;
        private WheelFrictionCurve _rearRightWheelFrictionCurve;

        public void StartSystem()
        {
            _frontLeftWheelFrictionCurve = CreateFrictionCurve(frontLeftWheelCollider);
            _frontRightWheelFrictionCurve = CreateFrictionCurve(frontRightWheelCollider);
            _rearLeftWheelFrictionCurve = CreateFrictionCurve(rearLeftWheelCollider);
            _rearRightWheelFrictionCurve = CreateFrictionCurve(rearRightWheelCollider);
        }

        public void HandleSystem()
        {

        }

        #region Private Methods

        private WheelFrictionCurve CreateFrictionCurve(WheelCollider wheelCollider)
        {
            WheelFrictionCurve frictionCurve = new WheelFrictionCurve()
            {
                extremumSlip = wheelCollider.sidewaysFriction.extremumSlip,
                extremumValue = wheelCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = wheelCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = wheelCollider.sidewaysFriction.asymptoteValue,
                stiffness = wheelCollider.sidewaysFriction.stiffness
            };

            return frictionCurve;
        }

        #endregion

    }
}