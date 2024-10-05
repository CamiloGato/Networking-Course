using System.Collections.Generic;

namespace FiniteStateMachine
{
    public class FsmMachine<TEnum, TState>
        where TEnum : System.Enum
        where TState : IFsmState
    {
        public TEnum CurrentState { get; private set; }
        public TState CurrentStateType => _states.GetValueOrDefault(CurrentState);
        
        private readonly Dictionary<TEnum, TState> _states = new Dictionary<TEnum, TState>();

        public FsmMachine(TEnum currentState)
        {
            CurrentState = currentState;
        }

        public void StartFsm()
        {
            _states[CurrentState].Enter();
        }

        public void AddState(TEnum stateType, TState state)
        {
            _states.Add(stateType, state);
        }
        
        public void ChangeState(TEnum stateType)
        {
            _states[CurrentState].Exit();
            CurrentState = stateType;
            _states[CurrentState].Enter();
        }
        
        public void ExecuteFsm()
        {
            _states[CurrentState].Execute();
        }
        
    }
}