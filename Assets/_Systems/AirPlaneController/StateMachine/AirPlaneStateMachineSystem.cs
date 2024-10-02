using System.Collections.Generic;
using AirPlaneController.StateMachine.AirPlaneStates;


namespace AirPlaneController.StateMachine
{
    public enum AirPlaneStateType
    {
        Dead,
        Flying,
        Landing,
        TakeOff
    }
    
    public class AirPlaneStateMachineSystem
    {
        private AirPlaneStateType _currentStateType;
        public AirPlaneState CurrentState => _states[_currentStateType];
        public AirPlaneStateType CurrentStateType => _currentStateType;
        
        private Dictionary<AirPlaneStateType, AirPlaneState> _states = new Dictionary<AirPlaneStateType, AirPlaneState>();

        public AirPlaneStateMachineSystem(AirPlaneStateType currentStateType)
        {
            _currentStateType = currentStateType;
        }

        public void StartStateMachine()
        {
            _states[_currentStateType].EnterState();
        }
        
        public void AddState(AirPlaneStateType stateType, AirPlaneState state)
        {
            _states.Add(stateType, state);
        }
        
        public void ChangeState(AirPlaneStateType stateType)
        {
            _states[_currentStateType].ExitState();
            _currentStateType = stateType;
            _states[_currentStateType].EnterState();
        }
        
        public void UpdateState()
        {
            _states[_currentStateType].UpdateState();
        }
        
    }
}