using System.Collections.Generic;
using UnityEngine;

namespace AirPlaneController.AirPlaneInfo
{
    [CreateAssetMenu(fileName = "AirPlaneConfig", menuName = "AirPlane/Config", order = 0)]
    public class AirPlaneConfig : ScriptableObject
    {
        private Transform _transform;
        private Rigidbody _rigidbody;
        private List<AirPlaneCollider> _airPlaneColliders;
        
        [Header("Wing trail effects")]
        [Range(0.01f, 1f)]
        [SerializeField] private float trailThickness = 0.045f;
        [SerializeField] private TrailRenderer[] wingTrailEffects;

        [Header("Rotating speeds")]
        [Range(5f, 500f)]
        [SerializeField] private float yawSpeed = 50f;

        [Range(5f, 500f)]
        [SerializeField] private float pitchSpeed = 100f;

        [Range(5f, 500f)]
        [SerializeField] private float rollSpeed = 200f;

        [Header("Rotating speeds multipliers when turbo is used")]
        [Range(0.1f, 5f)]
        [SerializeField] private float yawTurboMultiplier = 0.3f;
        
        [Range(0.1f, 5f)]
        [SerializeField] private float pitchTurboMultiplier = 0.5f;

        [Range(0.1f, 5f)]
        [SerializeField] private float rollTurboMultiplier = 1f;

        [Header("Moving speed")]
        [Range(1, 5)]
        [SerializeField] private float speedMultiplier = 1f;
        
        [Range(5f, 100f)]
        [SerializeField] private float defaultSpeed = 10f;

        [Range(10f, 200f)]
        [SerializeField] private float turboSpeed = 20f;

        [Range(0.1f, 50f)]
        [SerializeField] private float accelerating = 10f;

        [Range(0.1f, 50f)]
        [SerializeField] private float deAccelerating = 5f;

        [Header("Turbo settings")]
        [Range(0f, 100f)]
        [SerializeField] private float turboHeatingSpeed;

        [Range(0f, 100f)]
        [SerializeField] private float turboCooldownSpeed;

        // [Header("Turbo heat values")]
        // [Tooltip("Real-time information about the turbo's current temperature (do not change in the editor)")]
        // [Range(0f, 100f)]
        // [SerializeField] private float turboHeat;

        [Tooltip("You can set this to determine when the turbo should cease overheating and become operational again")]
        [Range(0f, 100f)]
        [SerializeField] private float turboOverheatOver;
        // [SerializeField] private bool turboOverheat;

        [Header("Side-way force")]
        [Range(0.1f, 15f)]
        [SerializeField] private float sidewaysMovement = 15f;

        [Range(0.001f, 0.05f)]
        [SerializeField] private float sidewaysMovementXRot = 0.012f;

        [Range(0.1f, 5f)]
        [SerializeField] private float sidewaysMovementYRot = 1.5f;

        [Range(-1, 1f)]
        [SerializeField] private float sidewaysMovementYPos = 0.1f;

        [Header("Engine sound settings")]
        [SerializeField] private AudioSource engineSoundSource;
        [SerializeField] private float maxEngineSound = 1f;
        [SerializeField] private float defaultSoundPitch = 1f;
        [SerializeField] private float turboSoundPitch = 1.5f;

        [Header("Engine propellers settings")]
        [Range(10f, 10000f)]
        [SerializeField] private float propelSpeedMultiplier = 100f;
        [SerializeField] private Transform[] propellers;

        [Header("Turbine light settings")]
        [Range(0.1f, 20f)]
        [SerializeField] private float turbineLightDefault = 1f;

        [Range(0.1f, 20f)]
        [SerializeField] private float turbineLightTurbo = 5f;

        [SerializeField] private Light[] turbineLights;

        [Header("Colliders")]
        [SerializeField] private Transform crashCollidersRoot;

        [Header("Takeoff settings")]
        [Tooltip("How far must the plane be from the runway before it can be controlled again")]
        [SerializeField] private float takeoffLenght = 30f;

        
        #region Properties
        
        public Transform Transform => _transform;
        
        public Rigidbody Rigidbody => _rigidbody;
        
        public List<AirPlaneCollider> AirPlaneColliders => _airPlaneColliders;

        public float TrailThickness => trailThickness;

        public TrailRenderer[] WingTrailEffects => wingTrailEffects;

        public float YawSpeed => yawSpeed;

        public float PitchSpeed => pitchSpeed;

        public float RollSpeed => rollSpeed;

        public float YawTurboMultiplier => yawTurboMultiplier;

        public float PitchTurboMultiplier => pitchTurboMultiplier;

        public float RollTurboMultiplier => rollTurboMultiplier;

        public float SpeedMultiplier => speedMultiplier;
        
        public float DefaultSpeed => defaultSpeed;

        public float TurboSpeed => turboSpeed;

        public float Accelerating => accelerating;

        public float DeAccelerating => deAccelerating;

        public float TurboHeatingSpeed => turboHeatingSpeed;

        public float TurboCooldownSpeed => turboCooldownSpeed;
        
        public float TurboOverheatOver => turboOverheatOver;

        public float SidewaysMovement => sidewaysMovement;

        public float SidewaysMovementXRot => sidewaysMovementXRot;

        public float SidewaysMovementYRot => sidewaysMovementYRot;

        public float SidewaysMovementYPos => sidewaysMovementYPos;

        public AudioSource EngineSoundSource => engineSoundSource;

        public float MaxEngineSound => maxEngineSound;

        public float DefaultSoundPitch => defaultSoundPitch;

        public float TurboSoundPitch => turboSoundPitch;

        public float PropelSpeedMultiplier => propelSpeedMultiplier;

        public Transform[] Propellers => propellers;

        public float TurbineLightDefault => turbineLightDefault;

        public float TurbineLightTurbo => turbineLightTurbo;

        public Light[] TurbineLights => turbineLights;

        public Transform CrashCollidersRoot => crashCollidersRoot;

        public float TakeoffLenght => takeoffLenght;

        #endregion
        
    }
}