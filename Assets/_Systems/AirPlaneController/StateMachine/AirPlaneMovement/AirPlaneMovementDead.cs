using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneMovement
{
    public class AirPlaneMovementDead : AirPlaneMovementSystem
    {
        public AirPlaneMovementDead(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats)
            : base(airPlaneConfig, airPlaneStats) {}

        public override void StartSystem()
        {
            //Set rigidbody to non cinematic
            AirPlaneConfig.Rigidbody.isKinematic = false;
            AirPlaneConfig.Rigidbody.useGravity = true;
            AirPlaneConfig.Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            
            //Add last speed to rb
            AirPlaneConfig.Rigidbody.AddForce(AirPlaneConfig.Transform.forward * AirPlaneStats.lastEngineSpeed, ForceMode.VelocityChange);
        }

        public override void UpdateSystem()
        {
            TurnOffPropellersAndLights();
        }

        public override void EndSystem()
        {
            AirPlaneConfig.Rigidbody.isKinematic = true;
            AirPlaneConfig.Rigidbody.useGravity = false;
            AirPlaneConfig.Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            foreach (AirPlaneCollider airPlaneCollider in AirPlaneConfig.AirPlaneColliders)
            {
                airPlaneCollider.SetUp();
            }
        }
    }
}