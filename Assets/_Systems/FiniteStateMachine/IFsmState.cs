namespace FiniteStateMachine
{
    public interface IFsmState
    {
        void Enter();
        void Execute();
        void Exit();
    }
}