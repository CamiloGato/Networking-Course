using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneAudio
{
    public class AirPlaneAudioLanding : AirPlaneSystem
    {
        private AirPlaneConfig _airPlaneConfig;
        
        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            AudioSource engineSoundSource = _airPlaneConfig.EngineSoundSource;
            float defaultSoundPitch = _airPlaneConfig.DefaultSoundPitch;
            
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, defaultSoundPitch, 1f * Time.deltaTime);
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, 1f * Time.deltaTime);
        }

        public override void EndSystem()
        {
            
        }
    }
}