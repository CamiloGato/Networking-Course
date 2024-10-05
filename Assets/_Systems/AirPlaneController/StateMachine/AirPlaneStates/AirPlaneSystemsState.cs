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

        public override void Enter()
        {
            foreach (AirPlaneSystem airPlaneSystem in _systems)
            {
                airPlaneSystem.StartSystem();
            }
        }

        public override void Exit()
        {
            foreach (AirPlaneSystem airPlaneSystem in _systems)
            {
                airPlaneSystem.EndSystem();
            }
        }

        public override void Execute()
        {
            foreach (AirPlaneSystem airPlaneSystem in _systems)
            {
                airPlaneSystem.UpdateSystem();
            }
        }
    }
}