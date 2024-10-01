using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneMovement
{
    public class AirPlaneMovementLanding : AirPlaneMovementSystem
    {
        public AirPlaneMovementLanding(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats)
            : base(airPlaneConfig, airPlaneStats) {}
        
        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            TurnOnPropellersAndLights();
            ChangeWingTrailEffectThickness(0f);
            
            // Stop Speed
            AirPlaneStats.CurrentSpeed = Mathf.Lerp(AirPlaneStats.CurrentSpeed, 0f, 1f * Time.deltaTime);
            
            // Set local rotation to zero
            AirPlaneConfig.Transform.localRotation = Quaternion.Lerp(
                AirPlaneConfig.Transform.localRotation,
                Quaternion.identity,
                2f * Time.deltaTime
            );
            
        }

        public override void EndSystem()
        {
            
        }
    }
}