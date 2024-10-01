using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneMovement
{
    public class AirPlaneMovementTakeOff : AirPlaneMovementSystem
    {
        public AirPlaneMovementTakeOff(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats)
            : base(airPlaneConfig, airPlaneStats) {}

        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            TurnOnPropellersAndLights();

            // Accelerating
            if (AirPlaneStats.CurrentSpeed < AirPlaneConfig.TurboSpeed)
            {
                AirPlaneStats.CurrentSpeed += (AirPlaneConfig.Accelerating * 2f) * Time.deltaTime;
            }

            // Move Forward
            AirPlaneConfig.Transform.Translate(Vector3.forward * AirPlaneStats.CurrentSpeed * Time.deltaTime);
            
        }

        public override void EndSystem()
        {
            
        }
    }
}