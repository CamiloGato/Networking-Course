using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneAudio
{
    public class AirPlaneAudioDead : AirPlaneSystem
    {
        private AirPlaneConfig _airPlaneConfig;
        
        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            AudioSource engineSoundSource = _airPlaneConfig.EngineSoundSource;
            
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, 0f, 10f * Time.deltaTime);
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, 10f * Time.deltaTime);
        }

        public override void EndSystem()
        {
            
        }
    }
}