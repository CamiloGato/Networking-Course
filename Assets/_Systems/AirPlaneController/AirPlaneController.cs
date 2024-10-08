﻿using System.Collections.Generic;
using AirPlaneController.AirPlaneInfo;
using AirPlaneController.AirPlaneInput;
using AirPlaneController.StateMachine.AirPlaneAudio;
using AirPlaneController.StateMachine.AirPlaneMovement;
using AirPlaneController.StateMachine.AirPlaneStates;
using FiniteStateMachine;
using UnityEngine;

namespace AirPlaneController
{
    
    public enum AirPlaneStateType
    {
        Dead,
        Flying,
        Landing,
        TakeOff
    }
    
    public class AirPlaneController : MonoBehaviour
    {
        [Header("AirPlane Config")]
        [SerializeField] private AirPlaneConfig airPlaneConfig;
        [SerializeField] private AirPlaneStats airPlaneStats;
        
        [Space(20)]
        [Header("AirPlane Components")]
        [SerializeField] private List<AirPlaneCollider> airPlaneColliders;
        [SerializeField] private AudioSource engineSoundSource;
        [SerializeField] private TrailRenderer[] trailRenderer;
        [SerializeField] private Light[] turbineLights;
        [SerializeField] private Transform[] propellers;
        
        private FsmMachine<AirPlaneStateType, AirPlaneState> _stateMachineSystem;
        private Rigidbody _rigidbody;
        private Transform _transform;
        private IAirPlaneInput _input;

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

            _stateMachineSystem = new FsmMachine<AirPlaneStateType, AirPlaneState>(AirPlaneStateType.Flying);
            
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
            _stateMachineSystem.StartFsm();
        }

        private void Update()
        {
            _stateMachineSystem.ExecuteFsm();
        }
    }
}