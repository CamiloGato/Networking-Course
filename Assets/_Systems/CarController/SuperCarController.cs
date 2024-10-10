using UnityEngine;

namespace CarController
{
    public class SuperCarController : MonoBehaviour
    {
        [Header("CAR SETUP")] [Space(10)] [Range(20, 190)]
        public int maxSpeed;

        [Range(10, 120)] public int maxReverseSpeed;
        [Range(1, 10)] public int accelerationMultiplier;
        [Space(10)] [Range(10, 45)] public int maxSteeringAngle;
        [Range(0.1f, 1f)] public float steeringSpeed;
        [Space(10)] [Range(100, 600)] public int brakeForce;
        [Range(1, 10)] public int decelerationMultiplier;
        [Range(1, 10)] public int handbrakeDriftMultiplier;
        [Space(10)] public Vector3 bodyMassCenter;

        [Space(20)] [Header("WHEELS")]
        public WheelCollider frontLeftCollider, frontRightCollider, rearLeftCollider, rearRightCollider;

        public GameObject frontLeftMesh, frontRightMesh, rearLeftMesh, rearRightMesh;

        [Space(20)] [Header("EFFECTS")] [Space(10)]
        public ParticleSystem rLWheelParticleSystem, rRWheelParticleSystem;

        [Space(10)] public TrailRenderer rLWheelTireSkid, rRWheelTireSkid;

        [Space(20)] [Header("Sounds")] [Space(10)]
        public AudioSource carEngineSound, tireScreechSound;

        [HideInInspector] public float carSpeed;
        [HideInInspector] public bool isDrifting;
        [HideInInspector] public bool isTractionLocked;

        private Rigidbody _carRigidbody;
        private bool _deceleratingCar;

        private float _initialCarEngineSoundPitch;

        private float _localVelocityX, _localVelocityZ;

        private float _flWheelExtremumSlip, _frWheelExtremumSlip, _rlWheelExtremumSlip, _rrWheelExtremumSlip;

        private WheelFrictionCurve _fLWheelFrictionCurve,
            _fRWheelFrictionCurve,
            _rLWheelFrictionCurve,
            _rRWheelFrictionCurve;

        private float _driftingAxis;
        private float _steeringAxis;
        private float _throttleAxis;

        private void Start()
        {
            _carRigidbody = gameObject.GetComponent<Rigidbody>();
            _carRigidbody.centerOfMass = bodyMassCenter;

            _fLWheelFrictionCurve = CreateFrictionCurve(frontLeftCollider);
            _flWheelExtremumSlip = _fLWheelFrictionCurve.extremumSlip;
            _fRWheelFrictionCurve = CreateFrictionCurve(frontRightCollider);
            _frWheelExtremumSlip = _fRWheelFrictionCurve.extremumSlip;
            _rLWheelFrictionCurve = CreateFrictionCurve(rearLeftCollider);
            _rlWheelExtremumSlip = _rLWheelFrictionCurve.extremumSlip;
            _rRWheelFrictionCurve = CreateFrictionCurve(rearRightCollider);
            _rrWheelExtremumSlip = _rRWheelFrictionCurve.extremumSlip;

            _initialCarEngineSoundPitch = carEngineSound.pitch;

            InvokeRepeating(nameof(CarSounds), 0f, 0.1f);
        }

        private void Update()
        {
            UpdateCarData();
            HandleCarControls();
            AnimateWheelMeshes();
        }

        private void UpdateCarData()
        {
            carSpeed = 2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60 / 1000;
            _localVelocityX = transform.InverseTransformDirection(_carRigidbody.linearVelocity).x;
            _localVelocityZ = transform.InverseTransformDirection(_carRigidbody.linearVelocity).z;
            _throttleAxis = Mathf.Clamp(_throttleAxis, -1f, 1f);
        }

        private void HandleCarControls()
        {
            HandleThrottle();
            HandleSteering();
            HandleHandbrake();
            HandleDeceleration();
        }

        private void HandleThrottle()
        {
            if (Input.GetKey(KeyCode.W))
            {
                CancelInvoke(nameof(DecelerateCar));
                _deceleratingCar = false;
                GoForward();
            }
            else if (Input.GetKey(KeyCode.S))
            {
                CancelInvoke(nameof(DecelerateCar));
                _deceleratingCar = false;
                GoReverse();
            }
            else
            {
                ThrottleOff();
            }
        }

        private void HandleSteering()
        {
            if (Input.GetKey(KeyCode.A))
                TurnLeft();
            else if (Input.GetKey(KeyCode.D))
                TurnRight();
            else if (_steeringAxis != 0f) ResetSteeringAngle();
        }

        private void HandleHandbrake()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                CancelInvoke(nameof(DecelerateCar));
                _deceleratingCar = false;
                Handbrake();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                RecoverTraction();
            }
        }

        private void HandleDeceleration()
        {
            if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.Space) &&
                !_deceleratingCar)
            {
                InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
                _deceleratingCar = true;
            }
        }

        #region Steering Methods

        private void TurnLeft()
        {
            _steeringAxis -= Time.deltaTime * 10f * steeringSpeed;
            if (_steeringAxis < -1f) _steeringAxis = -1f;

            var steeringAngle = _steeringAxis * maxSteeringAngle;
            frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
            frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
        }

        private void TurnRight()
        {
            _steeringAxis += Time.deltaTime * 10f * steeringSpeed;
            if (_steeringAxis > 1f) _steeringAxis = 1f;

            var steeringAngle = _steeringAxis * maxSteeringAngle;
            frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
            frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
        }

        private void ResetSteeringAngle()
        {
            if (_steeringAxis < 0f)
                _steeringAxis += Time.deltaTime * 10f * steeringSpeed;
            else if (_steeringAxis > 0f) _steeringAxis -= Time.deltaTime * 10f * steeringSpeed;

            if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f) _steeringAxis = 0f;

            var steeringAngle = _steeringAxis * maxSteeringAngle;
            frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
            frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
        }

        #endregion

        #region Engine And Braking Methods

        private void GoForward()
        {
            isDrifting = Mathf.Abs(_localVelocityX) > 2.5f;
            DriftCarPS();

            _throttleAxis += Time.deltaTime * 3f;

            if (_localVelocityZ < -1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.RoundToInt(carSpeed) < maxSpeed)
                {
                    ApplyMotorTorque(accelerationMultiplier * 50f * _throttleAxis);
                    ApplyBrakeTorque(0);
                }
                else
                {
                    ApplyMotorTorque(0);
                }
            }
        }

        private void GoReverse()
        {
            isDrifting = Mathf.Abs(_localVelocityX) > 2.5f;
            DriftCarPS();

            _throttleAxis -= Time.deltaTime * 3f;

            if (_localVelocityZ > 1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
                {
                    ApplyMotorTorque(accelerationMultiplier * 50f * _throttleAxis);
                    ApplyBrakeTorque(0);
                }
                else
                {
                    ApplyMotorTorque(0);
                }
            }
        }

        private void ThrottleOff()
        {
            frontLeftCollider.motorTorque = 0;
            frontRightCollider.motorTorque = 0;
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
        }

        public void DecelerateCar()
        {
            isDrifting = Mathf.Abs(_localVelocityX) > 2.5f;
            DriftCarPS();

            if (_throttleAxis != 0)
            {
                int inverseThrottle = _throttleAxis > 0 ? -1 : 1;
                _throttleAxis += Time.deltaTime * 10f * inverseThrottle;

                if (Mathf.Abs(_throttleAxis) < 0.15f) _throttleAxis = 0f;
            }

            _carRigidbody.linearVelocity *= 1f / (1f + 0.025f * decelerationMultiplier);

            ApplyMotorTorque(0);

            if (_carRigidbody.linearVelocity.magnitude < 0.25f)
            {
                _carRigidbody.linearVelocity = Vector3.zero;
                CancelInvoke(nameof(DecelerateCar));
            }
        }

        public void RecoverTraction()
        {
            isTractionLocked = false;

            _driftingAxis = Mathf.Max(0f, _driftingAxis - Time.deltaTime / 1.5f);

            if (_driftingAxis > 0f)
            {
                UpdateWheelFriction(_driftingAxis * handbrakeDriftMultiplier);
                Invoke(nameof(RecoverTraction), Time.deltaTime);
            }
            else
            {
                ResetWheelFriction();
                _driftingAxis = 0f;
            }
        }

        private void UpdateWheelFriction(float driftMultiplier)
        {
            _fLWheelFrictionCurve.extremumSlip = _flWheelExtremumSlip * driftMultiplier;
            frontLeftCollider.sidewaysFriction = _fLWheelFrictionCurve;

            _fRWheelFrictionCurve.extremumSlip = _frWheelExtremumSlip * driftMultiplier;
            frontRightCollider.sidewaysFriction = _fRWheelFrictionCurve;

            _rLWheelFrictionCurve.extremumSlip = _rlWheelExtremumSlip * driftMultiplier;
            rearLeftCollider.sidewaysFriction = _rLWheelFrictionCurve;

            _rRWheelFrictionCurve.extremumSlip = _rrWheelExtremumSlip * driftMultiplier;
            rearRightCollider.sidewaysFriction = _rRWheelFrictionCurve;
        }

        private void ResetWheelFriction()
        {
            _fLWheelFrictionCurve.extremumSlip = _flWheelExtremumSlip;
            frontLeftCollider.sidewaysFriction = _fLWheelFrictionCurve;

            _fRWheelFrictionCurve.extremumSlip = _frWheelExtremumSlip;
            frontRightCollider.sidewaysFriction = _fRWheelFrictionCurve;

            _rLWheelFrictionCurve.extremumSlip = _rlWheelExtremumSlip;
            rearLeftCollider.sidewaysFriction = _rLWheelFrictionCurve;

            _rRWheelFrictionCurve.extremumSlip = _rrWheelExtremumSlip;
            rearRightCollider.sidewaysFriction = _rRWheelFrictionCurve;
        }

        #endregion

        #region Breaks Methods

        private void Brakes()
        {
            frontLeftCollider.brakeTorque = brakeForce;
            frontRightCollider.brakeTorque = brakeForce;
            rearLeftCollider.brakeTorque = brakeForce;
            rearRightCollider.brakeTorque = brakeForce;
        }

        private void Handbrake()
        {
            isDrifting = Mathf.Abs(_localVelocityX) > 2.5f;
            DriftCarPS();

            CancelInvoke(nameof(RecoverTraction));

            _driftingAxis += Time.deltaTime;
            var secureStartingPoint = _driftingAxis * _flWheelExtremumSlip * handbrakeDriftMultiplier;

            if (secureStartingPoint < _flWheelExtremumSlip)
                _driftingAxis = _flWheelExtremumSlip / (_flWheelExtremumSlip * handbrakeDriftMultiplier);

            if (_driftingAxis > 1f) _driftingAxis = 1f;

            if (_driftingAxis < 1f) UpdateWheelFriction(_driftingAxis * handbrakeDriftMultiplier);

            ApplyBrakeTorque(brakeForce);

            isTractionLocked = true;
        }

        #endregion

        #region Private Methods & Helpers

        private WheelFrictionCurve CreateFrictionCurve(WheelCollider wheelCollider)
        {
            var frictionCurve = new WheelFrictionCurve
            {
                extremumSlip = wheelCollider.sidewaysFriction.extremumSlip,
                extremumValue = wheelCollider.sidewaysFriction.extremumValue,
                asymptoteSlip = wheelCollider.sidewaysFriction.asymptoteSlip,
                asymptoteValue = wheelCollider.sidewaysFriction.asymptoteValue,
                stiffness = wheelCollider.sidewaysFriction.stiffness
            };

            return frictionCurve;
        }

        private void ApplyMotorTorque(float torque)
        {
            frontLeftCollider.motorTorque = torque;
            frontRightCollider.motorTorque = torque;
            rearLeftCollider.motorTorque = torque;
            rearRightCollider.motorTorque = torque;
        }

        private void ApplyBrakeTorque(float torque)
        {
            frontLeftCollider.brakeTorque = torque;
            frontRightCollider.brakeTorque = torque;
            rearLeftCollider.brakeTorque = torque;
            rearRightCollider.brakeTorque = torque;
        }

        #endregion

        #region Systems

        public void CarSounds()
        {
            var engineSoundPitch =
                _initialCarEngineSoundPitch + Mathf.Abs(_carRigidbody.linearVelocity.magnitude) / 25f;
            carEngineSound.pitch = engineSoundPitch;

            if (isDrifting || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
            {
                if (!tireScreechSound.isPlaying) tireScreechSound.Play();
            }
            else if (!isDrifting && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
            {
                tireScreechSound.Stop();
            }
        }

        private void DriftCarPS()
        {
            switch (isDrifting)
            {
                case true:
                    rLWheelParticleSystem.Play();
                    rRWheelParticleSystem.Play();
                    break;
                case false:
                    rLWheelParticleSystem.Stop();
                    rRWheelParticleSystem.Stop();
                    break;
            }

            if ((isTractionLocked || Mathf.Abs(_localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
            {
                rLWheelTireSkid.emitting = true;
                rRWheelTireSkid.emitting = true;
            }
            else
            {
                rLWheelTireSkid.emitting = false;
                rRWheelTireSkid.emitting = false;
            }
        }

        private void AnimateWheelMeshes()
        {
            frontLeftCollider.GetWorldPose(out var flwPosition, out var flwRotation);
            frontLeftMesh.transform.position = flwPosition;
            frontLeftMesh.transform.rotation = flwRotation;

            frontRightCollider.GetWorldPose(out var frwPosition, out var frwRotation);
            frontRightMesh.transform.position = frwPosition;
            frontRightMesh.transform.rotation = frwRotation;

            rearLeftCollider.GetWorldPose(out var rlwPosition, out var rlwRotation);
            rearLeftMesh.transform.position = rlwPosition;
            rearLeftMesh.transform.rotation = rlwRotation;

            rearRightCollider.GetWorldPose(out var rrwPosition, out var rrwRotation);
            rearRightMesh.transform.position = rrwPosition;
            rearRightMesh.transform.rotation = rrwRotation;
        }

        #endregion
    }
}