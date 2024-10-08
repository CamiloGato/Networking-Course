using System;
using UnityEngine;

namespace CarController
{
    public class CarControllerV2 : MonoBehaviour
    {
        [Header("Car SetUp")]
        [Range(20, 190)] [SerializeField] private int maxSpeed;
        [Range(10, 120)] [SerializeField] private int maxReverseSpeed;
        [Range(1, 10)] [SerializeField] private int accelerationMultiplier;

        [Space(10)]
        [Range(10, 45)] [SerializeField] private int maxSteeringAngle;
        [Range(0.1f, 1f)] [SerializeField] private float steeringSpeed;

        [Space(10)]
        [Range(100, 600)] [SerializeField] private float brakeForce;
        [Range(1, 10)] [SerializeField] private float decelerationMultiplier;
        [Range(1, 10)] [SerializeField] private float handbrakeDriftMultiplier;

        private float _currentCarSpeed;
        private bool _isDrifting;
        private bool _isTractionLocked;
        private bool _isDecelerating;

        private Rigidbody _carRigidbody;
        private float _steeringAxis;
        private float _throttleAxis;
        private float _driftAxis;

        private float _localVelocityZ;
        private float _localVelocityX;

        [Space(10)]
        [SerializeField] private Vector3 bodyCenterOfMass;

        [Space(20)]
        [Header("Effects SetUp")]
        [SerializeField] private ParticleSystem rareLeftWheelSmoke;
        [SerializeField] private ParticleSystem rareRightWheelSmoke;

        [Space(10)]
        [SerializeField] private TrailRenderer rareLeftWheelTireSkid;
        [SerializeField] private TrailRenderer rareRightWheelTireSkid;

        [Space(20)]
        [Header("Sound SetUp")]
        [SerializeField] private AudioSource engineSound;
        [SerializeField] private AudioSource tireScreechSound;
        private float _initialCarEngineSoundPitch;

    }
}