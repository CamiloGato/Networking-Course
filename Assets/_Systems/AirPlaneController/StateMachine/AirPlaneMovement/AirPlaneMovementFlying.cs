using AirPlaneController.AirPlaneInfo;
using AirPlaneController.AirPlaneInput;
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
            SidewaysForce();
            Movement();
            
            if (_airPlaneInput.Turbo() && !AirPlaneStats.turboOverheat)
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
            AirPlaneConfig.Transform.Translate(Vector3.forward * (AirPlaneStats.currentSpeed * Time.deltaTime));

            //Store last speed
            AirPlaneStats.lastEngineSpeed = AirPlaneStats.currentSpeed;

            //Rotate airplane by inputs
            AirPlaneConfig.Transform.Rotate(
                Vector3.forward * (-_airPlaneInput.Horizontal() * AirPlaneStats.currentRollSpeed * Time.deltaTime)
                );
            AirPlaneConfig.Transform.Rotate(
                Vector3.right * (_airPlaneInput.Vertical() * AirPlaneStats.currentPitchSpeed * Time.deltaTime)
                );

            //Rotate yaw
            float yaw = _airPlaneInput.Yaw();
            AirPlaneConfig.Transform.Rotate(Vector3.up * (AirPlaneStats.currentYawSpeed * yaw * Time.deltaTime));

            //Accelerate and de accelerate
            if ( AirPlaneStats.currentSpeed < AirPlaneStats.maxSpeed)
            {
                AirPlaneStats.currentSpeed += AirPlaneConfig.Accelerating * Time.deltaTime;
            }
            else
            {
                AirPlaneStats.currentSpeed -= AirPlaneConfig.DeAccelerating * Time.deltaTime;
            }
            
        }

        private void Turbo()
        {
            //Turbo overheating
            if(AirPlaneStats.turboHeat > 100f)
            {
                AirPlaneStats.turboHeat = 100f;
                AirPlaneStats.turboOverheat = true;
            }
            else
            {
                //Add turbo heat
                AirPlaneStats.turboHeat += Time.deltaTime * AirPlaneConfig.TurboHeatingSpeed;
            }

            //Set speed to turbo speed and rotation to turbo values
            AirPlaneStats.maxSpeed = AirPlaneConfig.TurboSpeed;
            AirPlaneStats.currentYawSpeed = AirPlaneConfig.YawSpeed * AirPlaneConfig.YawTurboMultiplier;
            AirPlaneStats.currentPitchSpeed = AirPlaneConfig.PitchSpeed * AirPlaneConfig.PitchTurboMultiplier;
            AirPlaneStats.currentRollSpeed = AirPlaneConfig.RollSpeed * AirPlaneConfig.RollTurboMultiplier;

            //Engine lights
            AirPlaneStats.currentEngineLightIntensity = AirPlaneConfig.TurbineLightTurbo;

            //Effects
            ChangeWingTrailEffectThickness(AirPlaneConfig.TrailThickness);

            //Audio
            AirPlaneStats.currentEngineSoundPitch = AirPlaneConfig.TurboSoundPitch;
        }
        
        private void TurboCooldown()
        {
            if(AirPlaneStats.turboHeat > 0f)
            {
                AirPlaneStats.turboHeat -= Time.deltaTime * AirPlaneConfig.TurboCooldownSpeed;
            }
            else
            {
                AirPlaneStats.turboHeat = 0f;
            }

            //Turbo cooldown
            if (AirPlaneStats.turboOverheat)
            {
                if(AirPlaneStats.turboHeat <= AirPlaneConfig.TurboOverheatOver)
                {
                    AirPlaneStats.turboOverheat = false;
                }
            }

            //Speed and rotation normal
            AirPlaneStats.maxSpeed = AirPlaneConfig.DefaultSpeed * AirPlaneConfig.SpeedMultiplier;

            AirPlaneStats.currentYawSpeed = AirPlaneConfig.YawSpeed;
            AirPlaneStats.currentPitchSpeed = AirPlaneConfig.PitchSpeed;
            AirPlaneStats.currentRollSpeed = AirPlaneConfig.RollSpeed;

            //Engine lights
            AirPlaneStats.currentEngineLightIntensity = AirPlaneConfig.TurbineLightDefault;

            //Effects
            ChangeWingTrailEffectThickness(0f);

            //Audio
            AirPlaneStats.currentEngineSoundPitch = AirPlaneConfig.DefaultSoundPitch;
        }
        
        private void SidewaysForce()
        {
            float multiplierXRot = AirPlaneConfig.SidewaysMovement * AirPlaneConfig.SidewaysMovementXRot;
            float multiplierYRot = AirPlaneConfig.SidewaysMovement * AirPlaneConfig.SidewaysMovementYRot;

            float multiplierYPos = AirPlaneConfig.SidewaysMovement * AirPlaneConfig.SidewaysMovementYPos;

            //Right side 
            if (AirPlaneConfig.Transform.localEulerAngles.z is > 270f and < 360f)
            {
                float angle = (AirPlaneConfig.Transform.localEulerAngles.z - 270f) / (360f - 270f);
                float invert = 1f - angle;

                AirPlaneConfig.Transform.Rotate(Vector3.up * (invert * multiplierYRot * Time.deltaTime));
                AirPlaneConfig.Transform.Rotate(Vector3.right * (-invert * multiplierXRot * AirPlaneStats.currentPitchSpeed * Time.deltaTime));

                AirPlaneConfig.Transform.Translate(AirPlaneConfig.Transform.up * (invert * multiplierYPos * Time.deltaTime));
            }

            //Left side
            if (AirPlaneConfig.Transform.localEulerAngles.z is > 0f and < 90f)
            {
                float angle = AirPlaneConfig.Transform.localEulerAngles.z / 90f;

                AirPlaneConfig.Transform.Rotate(-Vector3.up * (angle * multiplierYRot * Time.deltaTime));
                AirPlaneConfig.Transform.Rotate(Vector3.right * (-angle * multiplierXRot * AirPlaneStats.currentPitchSpeed * Time.deltaTime));

                AirPlaneConfig.Transform.Translate(AirPlaneConfig.Transform.up * (angle * multiplierYPos * Time.deltaTime));
            }

            //Right side down
            if (AirPlaneConfig.Transform.localEulerAngles.z is > 90f and < 180f)
            {
                float angle = (AirPlaneConfig.Transform.localEulerAngles.z - 90f) / (180f - 90f);
                float invert = 1f - angle;

                AirPlaneConfig.Transform.Translate(AirPlaneConfig.Transform.up * (invert * multiplierYPos * Time.deltaTime));
                AirPlaneConfig.Transform.Rotate(Vector3.right * (-invert * multiplierXRot * AirPlaneStats.currentPitchSpeed * Time.deltaTime));
            }

            //Left side down
            if (AirPlaneConfig.Transform.localEulerAngles.z is > 180f and < 270f)
            {
                float angle = (AirPlaneConfig.Transform.localEulerAngles.z - 180f) / (270f - 180f);

                AirPlaneConfig.Transform.Translate(AirPlaneConfig.Transform.up * (angle * multiplierYPos * Time.deltaTime));
                AirPlaneConfig.Transform.Rotate(Vector3.right * (-angle * multiplierXRot * AirPlaneStats.currentPitchSpeed * Time.deltaTime));
            }
        }
        
    }
}