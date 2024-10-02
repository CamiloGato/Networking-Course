using System;

namespace AirPlaneController.AirPlaneInfo
{
    [Serializable]
    public class AirPlaneStats
    {
        public float maxSpeed;
        public float currentYawSpeed;
        public float currentPitchSpeed;
        public float currentRollSpeed;
        public float currentSpeed;
        public float currentEngineLightIntensity;
        public float lastEngineSpeed;
        public float currentEngineSoundPitch;
        public float turboHeat;
        public bool turboOverheat;
    }
}