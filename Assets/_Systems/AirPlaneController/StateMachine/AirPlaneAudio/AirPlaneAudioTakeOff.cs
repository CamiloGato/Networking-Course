using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneAudio
{
    public class AirPlaneAudioTakeOff : AirPlaneSystem
    {
        private AirPlaneConfig _airPlaneConfig;
        
        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            AudioSource engineSoundSource = _airPlaneConfig.EngineSoundSource;
            float turboSoundPitch = _airPlaneConfig.TurboSoundPitch;
            float maxEngineSound = _airPlaneConfig.MaxEngineSound;
            
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, turboSoundPitch, 1f * Time.deltaTime);
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, maxEngineSound, 1f * Time.deltaTime);
        }

        public override void EndSystem()
        {
            
        }
    }
}