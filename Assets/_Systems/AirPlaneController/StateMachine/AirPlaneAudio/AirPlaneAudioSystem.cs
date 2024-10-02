using AirPlaneController.AirPlaneInfo;

namespace AirPlaneController.StateMachine.AirPlaneAudio
{
    public abstract class AirPlaneAudioSystem : AirPlaneSystem
    {
        private readonly AirPlaneConfig _airPlaneConfig;
        private readonly AirPlaneStats _airPlaneStats;
        
        protected AirPlaneConfig AirPlaneConfig => _airPlaneConfig;
        protected AirPlaneStats AirPlaneStats => _airPlaneStats;

        protected AirPlaneAudioSystem(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats)
        {
            _airPlaneConfig = airPlaneConfig;
            _airPlaneStats = airPlaneStats;
        }
    }
}