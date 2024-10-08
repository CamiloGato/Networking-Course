using UnityEngine;
using UnityEngine.UI;

namespace CarController
{
    public class SuperCarController : MonoBehaviour
    {
        [Space(20)] [Header("CAR SETUP")] [Space(10)] [Range(20, 190)]
        public int maxSpeed; //The maximum speed that the car can reach in km/h.

        [Range(10, 120)]
        public int maxReverseSpeed; //The maximum speed that the car can reach while going on reverse in km/h.

        [Range(1, 10)] public int accelerationMultiplier; // How fast the car can speed up.

        // 1 is a slow acceleration and 10 is the fastest.
        [Space(10)] [Range(10, 45)]
        public int maxSteeringAngle; // The maximum angle that the tires can reach while rotating the steering wheel.

        [Range(0.1f, 1f)] public float steeringSpeed; // How fast the steering wheel turns.
        [Space(10)] [Range(100, 600)] public int brakeForce; // The strength of the wheel brakes.

        [Range(1, 10)]
        public int decelerationMultiplier; // How fast the car decelerates when the user is not using the throttle.

        [Range(1, 10)]
        public int handbrakeDriftMultiplier; // How much grip the car loses when the user hit the handbrake.

        [Space(10)] public Vector3 bodyMassCenter; // This is a vector that contains the center of mass the car.
        // I recommend setting this value in the points x = 0 and z = 0 of your car.
        // You can select the value that you want in the y-axis, however,
        // you must notice that the higher this value is,
        // the more unstable the car becomes.
        // Usually the y value goes from 0 to 1.5.

        [Space(20)] [Header("WHEELS")]
        /*
        The following variables are used to store the wheels' data of the car.
        We need both the mesh-only game objects and wheel collider components of the wheels.
        The wheel collider components and 3D meshes of the wheels cannot come from the same
        game object; they must be separate game objects.
        */
        public GameObject frontLeftMesh;

        public WheelCollider frontLeftCollider;
        public GameObject frontRightMesh;
        public WheelCollider frontRightCollider;
        public GameObject rearLeftMesh;
        public WheelCollider rearLeftCollider;
        public GameObject rearRightMesh;
        public WheelCollider rearRightCollider;

        [Space(20)] [Header("EFFECTS")] [Space(10)]
        //The following variable lets you set up particle systems in your car
        // The following particle systems are used as tire smoke when the car drifts.
        public ParticleSystem rLWheelParticleSystem;

        public ParticleSystem rRWheelParticleSystem;

        [Space(10)]
        // The following trail renderers are used as tire skids when the car loses traction.
        public TrailRenderer rLWheelTireSkid;

        public TrailRenderer rRWheelTireSkid;

        [Space(20)] [Header("UI")] [Space(10)]
        //The following variable lets you set up a UI text to display the speed of your car.
        public Text carSpeedText; // Used to store the UI object that is going to show the speed of the car.

        [Space(20)] [Header("Sounds")] [Space(10)]
        //The following variable lets you set up sounds for your car such as the car engine or tire screech sounds.
        public AudioSource carEngineSound; // This variable stores the sound of the car engine.

        public AudioSource tireScreechSound; // This variable stores the sound of the tire screech

        [HideInInspector] public float carSpeed; // Used to store the speed of the car.
        [HideInInspector] public bool isDrifting; // Used to know whether the car is drifting or not.

        [HideInInspector]
        public bool isTractionLocked; // Used to know whether the traction of the car is locked or not.

        /*
        IMPORTANT: The following variables should not be modified manually
        since their values are automatically given via a script.
        */
        private Rigidbody _carRigidbody; // Stores the car's rigidbody.
        private bool _deceleratingCar;

        // It goes from -1 to 1.
        private float _driftingAxis;
        private float _flWheelExtremumSlip;

        /*
        The following variables are used to store information about sideways friction of the wheels (such as
        extremum slip,extremumValue, asymptoteSlip, asymptoteValue and stiffness).
        We change these values to make the car start drifting.
        */
        private WheelFrictionCurve _fLWheelFrictionCurve;
        private float _frWheelExtremumSlip;
        private WheelFrictionCurve _fRWheelFrictionCurve;

        // (when the car is drifting).
        private float _initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.
        private float _localVelocityX;
        private float _localVelocityZ;
        private float _rlWheelExtremumSlip;
        private WheelFrictionCurve _rLWheelFrictionCurve;
        private float _rrWheelExtremumSlip;
        private WheelFrictionCurve _rRWheelFrictionCurve;

        private float _steeringAxis; // Used to know whether the steering wheel has reached the maximum value.

        // It goes from -1 to 1.
        private float _throttleAxis; // Used to know whether the throttle has reached the maximum value.

        // Start is called before the first frame update
        private void Start()
        {
            // In this part, we set the 'carRigidbody' value with the Rigidbody attached to this gameObject.
            // Also, we define the center of mass the car with the Vector3 given by the inspector.
            _carRigidbody = gameObject.GetComponent<Rigidbody>();
            _carRigidbody.centerOfMass = bodyMassCenter;

            // Initial setup to calculate the drift value of the car.
            // This part could look a bit complicated, but do not be afraid,
            // the only thing we're doing here is to save the default
            // friction values of the car wheels so we can set an appropriate drifting value later.
            _fLWheelFrictionCurve = CreateFrictionCurve(frontLeftCollider);
            _flWheelExtremumSlip = _fLWheelFrictionCurve.extremumSlip;
            _fRWheelFrictionCurve = CreateFrictionCurve(frontRightCollider);
            _frWheelExtremumSlip = _fRWheelFrictionCurve.extremumSlip;
            _rLWheelFrictionCurve = CreateFrictionCurve(rearLeftCollider);
            _rlWheelExtremumSlip = _rLWheelFrictionCurve.extremumSlip;
            _rRWheelFrictionCurve = CreateFrictionCurve(rearRightCollider);
            _rrWheelExtremumSlip = _rRWheelFrictionCurve.extremumSlip;

            // We save the initial pitch of the car engine sound.
            _initialCarEngineSoundPitch = carEngineSound.pitch;

            // We invoke 2 methods inside this script.
            // CarSpeedUI() changes the text of the UI object that stores the speed of the car and CarSounds()
            // controls the engine and drifting sounds.
            // Both methods are invoked in 0 seconds, and repeatedly called every 0.1 seconds.
            InvokeRepeating(nameof(CarSpeedUI), 0f, 0.1f);
            InvokeRepeating(nameof(CarSounds), 0f, 0.1f);
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateCarData();
            HandleCarControls();
            AnimateWheelMeshes();
        }

        private void UpdateCarData()
        {
            // We determine the speed of the car.
            carSpeed = 2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60 / 1000;
            // Save the local velocity of the car in the x-axis. Used to know if the car is drifting.
            _localVelocityX = transform.InverseTransformDirection(_carRigidbody.linearVelocity).x;
            // Save the local velocity of the car in the z-axis. Used to know if the car is going forward or backwards.
            _localVelocityZ = transform.InverseTransformDirection(_carRigidbody.linearVelocity).z;
            // We clamp the throttleAxis value between -1 and 1.
            _throttleAxis = Mathf.Clamp(_throttleAxis, -1f, 1f);
        }

        private void HandleCarControls()
        {
            // The next part is regarding the car controller.
            // First, it checks if the user wants to use touch controls (for mobile devices) or analog input
            // controls (WASD + Space).

            // The following methods are called whenever a certain key is pressed.
            // For example, in the first 'if' we call the method GoForward() if the user has pressed W.

            // In this part of the code, we specify what the car needs to do if the user presses
            // W (throttle), S (reverse), A (turn left), D (turn right) or Space bar (handbrake).
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
            // If the forces applied to the rigidbody in the 'x' asis are greater than 3f,
            // it means that the car is losing traction,
            // then the car will start emitting particle systems.
            isDrifting = Mathf.Abs(_localVelocityX) > 2.5f;
            DriftCarPS();

            // The following part sets the throttle power to 1 smoothly.
            _throttleAxis += Time.deltaTime * 3f;

            // If the car is going backwards, then apply brakes to avoid strange behaviors.
            // If the local velocity in the 'z' axis is less than -1f, then it
            // is safe to apply positive torque to go forward.
            if (_localVelocityZ < -1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.RoundToInt(carSpeed) < maxSpeed)
                {
                    // Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
                    ApplyMotorTorque(accelerationMultiplier * 50f * _throttleAxis);
                    ApplyBrakeTorque(0);
                }
                else
                {
                    // If the maxSpeed has been reached, then stop applying torque to the wheels.
                    // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
                    // could be a bit higher than expected.
                    ApplyMotorTorque(0);
                }
            }
        }

        private void GoReverse()
        {
            // If the forces applied to the rigidbody in the 'x' asis are greater than
            // 3f, it means that the car is losing traction,
            // then the car will start emitting particle systems.
            isDrifting = Mathf.Abs(_localVelocityX) > 2.5f;
            DriftCarPS();

            // The following part sets the throttle power to -1 smoothly.
            _throttleAxis -= Time.deltaTime * 3f;

            // If the car is still going forward, then apply brakes to avoid strange behaviors.
            // If the local velocity in the 'z' axis is greater than 1f, then it
            // is safe to apply negative torque to go reverse.
            if (_localVelocityZ > 1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
                {
                    //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
                    ApplyMotorTorque(accelerationMultiplier * 50f * _throttleAxis);
                    ApplyBrakeTorque(0);
                }
                else
                {
                    //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
                    // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation;
                    // the speed of the car could be a bit higher than expected.
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
            // If the forces applied to the rigidbody in the 'x' asis are greater than
            // 3f, it means that the car is losing traction,
            // then the car will start emitting particle systems.
            isDrifting = Mathf.Abs(_localVelocityX) > 2.5f;
            DriftCarPS();

            if (_throttleAxis != 0)
            {
                var inverseThrottle = _throttleAxis > 0 ? -1 : 1;
                _throttleAxis += Time.deltaTime * 10f * inverseThrottle;

                if (Mathf.Abs(_throttleAxis) < 0.15f) _throttleAxis = 0f;
            }

            _carRigidbody.linearVelocity *= 1f / (1f + 0.025f * decelerationMultiplier);
            // Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
            ApplyMotorTorque(0);

            // If the magnitude of the car's velocity is less than 0.25f (very slow velocity),
            // then stop the car completely and also cancel the invoking of this method.
            if (_carRigidbody.linearVelocity.magnitude < 0.25f)
            {
                _carRigidbody.linearVelocity = Vector3.zero;
                CancelInvoke(nameof(DecelerateCar));
            }
        }

        public void RecoverTraction()
        {
            // Set isTractionLocked false to indicate that the car is regaining traction.
            isTractionLocked = false;

            // Gradually decrease the driftingAxis value over time to smoothly recover traction.
            _driftingAxis = Mathf.Max(0f, _driftingAxis - Time.deltaTime / 1.5f);

            // If the driftingAxis is still greater than 0,
            // continue updating the wheel friction and invoke this method again.
            if (_driftingAxis > 0f)
            {
                UpdateWheelFriction(_driftingAxis * handbrakeDriftMultiplier);
                Invoke(nameof(RecoverTraction), Time.deltaTime);
            }
            else
            {
                // If the driftingAxis has reached 0, reset the wheel friction to its original values.
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
            // If the forces applied to the rigidbody in the 'x' axis are greater than 2.5f,
            // it means that the car lost its traction,
            // then the car will start emitting particle systems.
            isDrifting = Mathf.Abs(_localVelocityX) > 2.5f;
            DriftCarPS();

            CancelInvoke(nameof(RecoverTraction));

            // Start losing traction smoothly using the 'driftingAxis' variable.
            // This variable will start from 0 and will reach a top value of 1,
            // which means that the maximum drifting value has been reached.
            _driftingAxis += Time.deltaTime;
            var secureStartingPoint = _driftingAxis * _flWheelExtremumSlip * handbrakeDriftMultiplier;

            if (secureStartingPoint < _flWheelExtremumSlip)
                _driftingAxis = _flWheelExtremumSlip / (_flWheelExtremumSlip * handbrakeDriftMultiplier);

            if (_driftingAxis > 1f) _driftingAxis = 1f;

            // Continue increasing the sideways friction of the wheels until driftingAxis = 1f.
            if (_driftingAxis < 1f) UpdateWheelFriction(_driftingAxis * handbrakeDriftMultiplier);

            // Apply brake torque to the rear wheels
            ApplyBrakeTorque(brakeForce);

            // Set 'isTractionLocked = true' to simulate the wheel skids.
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

        public void CarSpeedUI()
        {
            var absoluteCarSpeed = Mathf.Abs(carSpeed);
            carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
        }

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