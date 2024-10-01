using System.Collections.Generic;

namespace AirPlaneController.StateMachine
{
    public enum AirplaneState
    {
        Flying,
        Landing,
        Takeoff,
        Crashed
    }
    
    public class AirPlaneStateMachineSystem
    {
        private AirplaneState _currentState;
        
        private Dictionary<AirplaneState, AirplaneState> _states = new Dictionary<AirplaneState, AirplaneState>();
        
    }
}