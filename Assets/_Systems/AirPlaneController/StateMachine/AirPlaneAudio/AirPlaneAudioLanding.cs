using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneAudio
{
    public class AirPlaneAudioLanding : AirPlaneAudioSystem
    {
        public AirPlaneAudioLanding(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats)
            : base(airPlaneConfig, airPlaneStats) {}

        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            AudioSource engineSoundSource = AirPlaneConfig.EngineSoundSource;
            float defaultSoundPitch = AirPlaneConfig.DefaultSoundPitch;
            
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, defaultSoundPitch, 1f * Time.deltaTime);
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, 1f * Time.deltaTime);
        }

        public override void EndSystem()
        {
            
        }
    }
}