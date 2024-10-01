using System.Collections.Generic;

namespace AirPlaneController.StateMachine.AirPlaneStates
{
    public class AirPlaneSystemsState : AirPlaneState
    {
        private readonly List<AirPlaneSystem> _systems;

        public AirPlaneSystemsState(List<AirPlaneSystem> systems)
        {
            _systems = systems;
        }

        public override void EnterState()
        {
            foreach (AirPlaneSystem airPlaneSystem in _systems)
            {
                airPlaneSystem.StartSystem();
            }
        }

        public override void ExitState()
        {
            foreach (AirPlaneSystem airPlaneSystem in _systems)
            {
                airPlaneSystem.EndSystem();
            }
        }

        public override void UpdateState()
        {
            foreach (AirPlaneSystem airPlaneSystem in _systems)
            {
                airPlaneSystem.UpdateSystem();
            }
        }
    }
}