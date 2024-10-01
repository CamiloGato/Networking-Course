using System;

namespace AirPlaneController.AirPlaneInfo
{
    [Serializable]
    public class AirPlaneStats
    {
        public float MaxSpeed { get; set; }
        public float CurrentYawSpeed { get; set; }
        public float CurrentPitchSpeed { get; set; }
        public float CurrentRollSpeed { get; set; }
        public float CurrentSpeed { get; set; }
        public float CurrentEngineLightIntensity { get; set; }
        public float LastEngineSpeed { get; set; }
        public float CurrentEngineSoundPitch { get; set; }
        public float TurboHeat { get; set; }
        public bool TurboOverheat { get; set; }
    }
}