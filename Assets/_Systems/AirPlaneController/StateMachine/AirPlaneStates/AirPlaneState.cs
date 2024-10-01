namespace AirPlaneController.StateMachine.AirPlaneStates
{
    public abstract class AirPlaneState
    {
        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();
    }
}