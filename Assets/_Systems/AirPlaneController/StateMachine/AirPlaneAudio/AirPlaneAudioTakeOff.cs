using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneAudio
{
    public class AirPlaneAudioTakeOff : AirPlaneAudioSystem
    {
        public AirPlaneAudioTakeOff(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats)
            : base(airPlaneConfig, airPlaneStats) {}

        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            AudioSource engineSoundSource = AirPlaneConfig.EngineSoundSource;
            float turboSoundPitch = AirPlaneConfig.TurboSoundPitch;
            float maxEngineSound = AirPlaneConfig.MaxEngineSound;
            
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, turboSoundPitch, 1f * Time.deltaTime);
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, maxEngineSound, 1f * Time.deltaTime);
        }

        public override void EndSystem()
        {
            
        }
    }
}