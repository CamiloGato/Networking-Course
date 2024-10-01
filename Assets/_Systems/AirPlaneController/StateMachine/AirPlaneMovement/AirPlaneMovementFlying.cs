using AirPlaneController.AirPlaneInfo;
using AirPlaneController.Subsystems.AirPlaneInput;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneMovement
{
    public class AirPlaneMovementFlying : AirPlaneMovementSystem
    {
        private readonly IAirPlaneInput _airPlaneInput;
        
        public AirPlaneMovementFlying(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats, IAirPlaneInput airPlaneInput)
            : base(airPlaneConfig, airPlaneStats)
        {
            _airPlaneInput = airPlaneInput;
        }

        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            TurnOnPropellersAndLights();
            Movement();
            
            if (_airPlaneInput.Turbo() && !AirPlaneStats.TurboOverheat)
            {
                Turbo();
            }
            else
            {
                TurboCooldown();
            }
            
        }

        public override void EndSystem()
        {
            
        }


        private void Movement()
        {
            //Move forward
            AirPlaneConfig.Transform.Translate(Vector3.forward * AirPlaneStats.CurrentSpeed * Time.deltaTime);

            //Store last speed
            AirPlaneStats.LastEngineSpeed = AirPlaneStats.CurrentSpeed;

            //Rotate airplane by inputs
            AirPlaneConfig.Transform.Rotate(
                Vector3.forward * -_airPlaneInput.Horizontal() * AirPlaneStats.CurrentRollSpeed * Time.deltaTime
                );
            AirPlaneConfig.Transform.Rotate(
                Vector3.right * _airPlaneInput.Vertical() * AirPlaneStats.CurrentPitchSpeed * Time.deltaTime
                );

            //Rotate yaw
            AirPlaneConfig.Transform.Rotate(Vector3.up * AirPlaneStats.CurrentYawSpeed * Time.deltaTime);

            //Accelerate and de accelerate
            if ( AirPlaneStats.CurrentSpeed < AirPlaneStats.MaxSpeed)
            {
                AirPlaneStats.CurrentSpeed += AirPlaneConfig.Accelerating * Time.deltaTime;
            }
            else
            {
                AirPlaneStats.CurrentSpeed -= AirPlaneConfig.DeAccelerating * Time.deltaTime;
            }
            
        }

        private void Turbo()
        {
            //Turbo overheating
            if(AirPlaneStats.TurboHeat > 100f)
            {
                AirPlaneStats.TurboHeat = 100f;
                AirPlaneStats.TurboOverheat = true;
            }
            else
            {
                //Add turbo heat
                AirPlaneStats.TurboHeat += Time.deltaTime * AirPlaneConfig.TurboHeatingSpeed;
            }

            //Set speed to turbo speed and rotation to turbo values
            AirPlaneStats.MaxSpeed = AirPlaneConfig.TurboSpeed;
            AirPlaneStats.CurrentYawSpeed = AirPlaneConfig.YawSpeed * AirPlaneConfig.YawTurboMultiplier;
            AirPlaneStats.CurrentPitchSpeed = AirPlaneConfig.PitchSpeed * AirPlaneConfig.PitchTurboMultiplier;
            AirPlaneStats.CurrentRollSpeed = AirPlaneConfig.RollSpeed * AirPlaneConfig.RollTurboMultiplier;

            //Engine lights
            AirPlaneStats.CurrentEngineLightIntensity = AirPlaneConfig.TurbineLightTurbo;

            //Effects
            ChangeWingTrailEffectThickness(AirPlaneConfig.TrailThickness);

            //Audio
            AirPlaneStats.CurrentEngineSoundPitch = AirPlaneConfig.TurboSoundPitch;
        }
        
        private void TurboCooldown()
        {
            if(AirPlaneStats.TurboHeat > 0f)
            {
                AirPlaneStats.TurboHeat -= Time.deltaTime * AirPlaneConfig.TurboCooldownSpeed;
            }
            else
            {
                AirPlaneStats.TurboHeat = 0f;
            }

            //Turbo cooldown
            if (AirPlaneStats.TurboOverheat)
            {
                if(AirPlaneStats.TurboHeat <= AirPlaneConfig.TurboOverheatOver)
                {
                    AirPlaneStats.TurboOverheat = false;
                }
            }

            //Speed and rotation normal
            AirPlaneStats.MaxSpeed = AirPlaneConfig.DefaultSpeed * AirPlaneConfig.SpeedMultiplier;

            AirPlaneStats.CurrentYawSpeed = AirPlaneConfig.YawSpeed;
            AirPlaneStats.CurrentPitchSpeed = AirPlaneConfig.PitchSpeed;
            AirPlaneStats.CurrentRollSpeed = AirPlaneConfig.RollSpeed;

            //Engine lights
            AirPlaneStats.CurrentEngineLightIntensity = AirPlaneConfig.TurbineLightDefault;

            //Effects
            ChangeWingTrailEffectThickness(0f);

            //Audio
            AirPlaneStats.CurrentEngineSoundPitch = AirPlaneConfig.DefaultSoundPitch;
        }
        
    }
}