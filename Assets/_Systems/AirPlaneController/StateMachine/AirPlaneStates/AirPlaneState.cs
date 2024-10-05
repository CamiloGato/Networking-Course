using FiniteStateMachine;

namespace AirPlaneController.StateMachine.AirPlaneStates
{
    public abstract class AirPlaneState : IFsmState
    {
        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();
    }
}