using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneAudio
{
    public class AirPlaneAudioFly : AirPlaneSystem
    {
        private AirPlaneStats _airPlaneStats;
        private AirPlaneConfig _airPlaneConfig;
        
        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            // Initialize Variables.
            
            AudioSource engineSoundSource = _airPlaneConfig.EngineSoundSource;
            float maxEngineSound = _airPlaneConfig.MaxEngineSound;
            float currentEngineSoundPitch = _airPlaneStats.CurrentEngineSoundPitch;
            
            
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, currentEngineSoundPitch, 10f * Time.deltaTime);
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, maxEngineSound, 1f * Time.deltaTime);
            
        }

        public override void EndSystem()
        {
            
        }
    }
}