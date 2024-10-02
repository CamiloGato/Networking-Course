using System;
using System.Collections.Generic;
using AirPlaneController.AirPlaneInfo;
using AirPlaneController.StateMachine;
using AirPlaneController.StateMachine.AirPlaneAudio;
using AirPlaneController.StateMachine.AirPlaneMovement;
using AirPlaneController.StateMachine.AirPlaneStates;
using AirPlaneController.Subsystems.AirPlaneInput;
using UnityEngine;

namespace AirPlaneController
{
    public class AirPlaneController : MonoBehaviour
    {
        private IAirPlaneInput _input;
        [SerializeField] private AirPlaneConfig airPlaneInfo;
        [SerializeField] private AirPlaneStats airPlaneStats;
        
        private AirPlaneStateMachineSystem _stateMachineSystem;

        private void Awake()
        {
            _input = new AirPlaneKeyboardInput();

            _stateMachineSystem = new AirPlaneStateMachineSystem(AirPlaneStateType.Flying);
            
            AirPlaneSystem audioTakeOff = new AirPlaneAudioTakeOff(airPlaneInfo, airPlaneStats);
            AirPlaneSystem movementTakeOff = new AirPlaneMovementTakeOff(airPlaneInfo, airPlaneStats);
            AirPlaneSystemsState takeOffState = new AirPlaneSystemsState(
                new List<AirPlaneSystem>(){audioTakeOff, movementTakeOff}
            );
            
            AirPlaneSystem audioFlying = new AirPlaneAudioFlying(airPlaneInfo, airPlaneStats);
            AirPlaneSystem movementFlying = new AirPlaneMovementFlying(airPlaneInfo, airPlaneStats, _input);
            AirPlaneSystemsState flyingState = new AirPlaneSystemsState(
                new List<AirPlaneSystem>(){audioFlying, movementFlying}
            );
            
            AirPlaneSystem audioLanding = new AirPlaneAudioLanding(airPlaneInfo, airPlaneStats);
            AirPlaneSystem movementLanding = new AirPlaneMovementLanding(airPlaneInfo, airPlaneStats);
            AirPlaneSystemsState landingState = new AirPlaneSystemsState(
                new List<AirPlaneSystem>(){audioLanding, movementLanding}
            );
            
            AirPlaneSystem audioDead = new AirPlaneAudioDead(airPlaneInfo, airPlaneStats);
            AirPlaneSystem movementDead = new AirPlaneMovementDead(airPlaneInfo, airPlaneStats);
            AirPlaneSystemsState deadState = new AirPlaneSystemsState(
                new List<AirPlaneSystem>(){audioDead, movementDead}
            );
            
            _stateMachineSystem.AddState(AirPlaneStateType.TakeOff, takeOffState);
            _stateMachineSystem.AddState(AirPlaneStateType.Flying, flyingState);
            _stateMachineSystem.AddState(AirPlaneStateType.Landing, landingState);
            _stateMachineSystem.AddState(AirPlaneStateType.Dead, deadState);
        }

        private void Start()
        {
            _stateMachineSystem.StartStateMachine();
        }

        private void Update()
        {
            _stateMachineSystem.UpdateState();
        }
    }
}