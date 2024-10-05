using FiniteStateMachine;

namespace CarController.StateMachine.CarStates
{
    public abstract class CarState : IFsmState
    {
        public abstract void Enter();

        public abstract void Execute();

        public abstract void Exit();
    }
}