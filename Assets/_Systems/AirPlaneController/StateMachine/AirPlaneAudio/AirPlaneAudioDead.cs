using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneAudio
{
    public class AirPlaneAudioDead : AirPlaneAudioSystem
    {
        public AirPlaneAudioDead(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats)
            : base(airPlaneConfig, airPlaneStats) {}

        public override void StartSystem()
        {
            
        }

        public override void UpdateSystem()
        {
            AudioSource engineSoundSource = AirPlaneConfig.EngineSoundSource;
            
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, 0f, 10f * Time.deltaTime);
            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, 10f * Time.deltaTime);
        }

        public override void EndSystem()
        {
            
        }
    }
}