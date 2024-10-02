using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneAudio
{
    public class AirPlaneAudioFlying : AirPlaneAudioSystem
    {
        public AirPlaneAudioFlying(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats)
            : base(airPlaneConfig, airPlaneStats) {}

        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            // Initialize Variables.
            
            AudioSource engineSoundSource = AirPlaneConfig.EngineSoundSource;
            float maxEngineSound = AirPlaneConfig.MaxEngineSound;
            float currentEngineSoundPitch = AirPlaneStats.CurrentEngineSoundPitch;
            
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, currentEngineSoundPitch, 10f * Time.deltaTime);
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, maxEngineSound, 1f * Time.deltaTime);
            
        }

        public override void EndSystem()
        {
            
        }
    }
}