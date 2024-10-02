using System.Collections.Generic;
using AirPlaneController.AirPlaneInfo;
using AirPlaneController.AirPlaneInput;
using AirPlaneController.StateMachine;
using AirPlaneController.StateMachine.AirPlaneAudio;
using AirPlaneController.StateMachine.AirPlaneMovement;
using AirPlaneController.StateMachine.AirPlaneStates;
using UnityEngine;

namespace AirPlaneController
{
    public class AirPlaneController : MonoBehaviour
    {
        private IAirPlaneInput _input;
        [SerializeField] private AirPlaneConfig airPlaneConfig;
        [SerializeField] private List<AirPlaneCollider> airPlaneColliders;
        [SerializeField] private AirPlaneStats airPlaneStats;
        [SerializeField] private AudioSource engineSoundSource;
        [SerializeField] private TrailRenderer[] trailRenderer;
        [SerializeField] private Light[] turbineLights;
        [SerializeField] private Transform[] propellers;
        
        private AirPlaneStateMachineSystem _stateMachineSystem;
        private Rigidbody _rigidbody;
        private Transform _transform;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _transform = transform;

            airPlaneConfig.Initialize(
                _transform,
                _rigidbody,
                airPlaneColliders,
                engineSoundSource,
                trailRenderer,
                turbineLights,
                propellers
            );
            
            _input = new AirPlaneKeyboardInput();

            _stateMachineSystem = new AirPlaneStateMachineSystem(AirPlaneStateType.Flying);
            
            AirPlaneSystem audioTakeOff = new AirPlaneAudioTakeOff(airPlaneConfig, airPlaneStats);
            AirPlaneSystem movementTakeOff = new AirPlaneMovementTakeOff(airPlaneConfig, airPlaneStats);
            AirPlaneSystemsState takeOffState = new AirPlaneSystemsState(
                new List<AirPlaneSystem>(){audioTakeOff, movementTakeOff}
            );
            
            AirPlaneSystem audioFlying = new AirPlaneAudioFlying(airPlaneConfig, airPlaneStats);
            AirPlaneSystem movementFlying = new AirPlaneMovementFlying(airPlaneConfig, airPlaneStats, _input);
            AirPlaneSystemsState flyingState = new AirPlaneSystemsState(
                new List<AirPlaneSystem>(){audioFlying, movementFlying}
            );
            
            AirPlaneSystem audioLanding = new AirPlaneAudioLanding(airPlaneConfig, airPlaneStats);
            AirPlaneSystem movementLanding = new AirPlaneMovementLanding(airPlaneConfig, airPlaneStats);
            AirPlaneSystemsState landingState = new AirPlaneSystemsState(
                new List<AirPlaneSystem>(){audioLanding, movementLanding}
            );
            
            AirPlaneSystem audioDead = new AirPlaneAudioDead(airPlaneConfig, airPlaneStats);
            AirPlaneSystem movementDead = new AirPlaneMovementDead(airPlaneConfig, airPlaneStats);
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