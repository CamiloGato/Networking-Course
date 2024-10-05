using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarControllerV2 : MonoBehaviour
{
      [Header("Car Setup")]
      [Range(20, 190)]
      public int maxSpeed = 90;
      [Range(10, 120)]
      public int maxReverseSpeed = 45;
      [Range(1, 10)]
      public int accelerationMultiplier = 2;
      [Space(10)]
      [Range(10, 45)]
      public int maxSteeringAngle = 27;
      [Range(0.1f, 1f)]
      public float steeringSpeed = 0.5f;
      [Space(10)]
      [Range(100, 600)]
      public int brakeForce = 350;
      [Range(1, 10)]
      public int decelerationMultiplier = 2; 
      [Range(1, 10)]
      public int handbrakeDriftMultiplier = 5;
      [Space(10)]
      public Vector3 bodyMassCenter;
      
      public GameObject frontLeftMesh;
      public WheelCollider frontLeftCollider;
      public GameObject frontRightMesh;
      public WheelCollider frontRightCollider;
      public GameObject rearLeftMesh;
      public WheelCollider rearLeftCollider;
      public GameObject rearRightMesh;
      public WheelCollider rearRightCollider;

      [Header("Particles")]
      public ParticleSystem RLWParticleSystem;
      public ParticleSystem RRWParticleSystem;
      public TrailRenderer RLWTireSkid;
      public TrailRenderer RRWTireSkid;

      [Header("UI")]
      public Text carSpeedText;

      [Header("Sound")]
      public AudioSource carEngineSound; 
      public AudioSource tireScreechSound; 
      float _initialCarEngineSoundPitch;
      
      [HideInInspector]
      public float carSpeed;
      [HideInInspector]
      public bool isDrifting;
      [HideInInspector]
      public bool isTractionLocked;

      Rigidbody _carRigidbody;
      float _steeringAxis;
      float _throttleAxis;
      float _driftingAxis;
      float _localVelocityZ;
      float _localVelocityX;
      bool _deceleratingCar;
      
      WheelFrictionCurve FLwheelFriction;
      float FLWextremumSlip;
      WheelFrictionCurve FRwheelFriction;
      float FRWextremumSlip;
      WheelFrictionCurve RLwheelFriction;
      float RLWextremumSlip;
      WheelFrictionCurve RRwheelFriction;
      float RRWextremumSlip;

    private WheelFrictionCurve SetupWheelFriction(WheelCollider collider)
    {
      var friction = new WheelFrictionCurve
      {
        extremumSlip = collider.sidewaysFriction.extremumSlip,
        extremumValue = collider.sidewaysFriction.extremumValue,
        asymptoteSlip = collider.sidewaysFriction.asymptoteSlip,
        asymptoteValue = collider.sidewaysFriction.asymptoteValue,
        stiffness = collider.sidewaysFriction.stiffness
      };
      return friction;
    }
    
    void Start()
    {
        _carRigidbody = gameObject.GetComponent<Rigidbody>();
        _carRigidbody.centerOfMass = bodyMassCenter;

        FLwheelFriction = SetupWheelFriction(frontLeftCollider);
        FRwheelFriction = SetupWheelFriction(frontRightCollider);
        RLwheelFriction = SetupWheelFriction(rearLeftCollider);
        RRwheelFriction = SetupWheelFriction(rearRightCollider);
        
        if(carEngineSound != null){
          _initialCarEngineSoundPitch = carEngineSound.pitch;
        }
        
        InvokeRepeating("CarSpeedUI", 0f, 0.1f);
        InvokeRepeating("CarSounds", 0f, 0.1f);
    }

    void Update()
    {
      carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;

      _localVelocityX = transform.InverseTransformDirection(_carRigidbody.linearVelocity).x;
      _localVelocityZ = transform.InverseTransformDirection(_carRigidbody.linearVelocity).z;
    
      isDrifting = Mathf.Abs(_localVelocityX) > 2.5f;

      DriftCarPS();
      
      if(Input.GetKey(KeyCode.W)){
        CancelInvoke("DecelerateCar");
        _deceleratingCar = false;
        GoForward();
      }
      if(Input.GetKey(KeyCode.S)){
        CancelInvoke("DecelerateCar");
        _deceleratingCar = false;
        GoReverse();
      }

      if(Input.GetKey(KeyCode.A)){
        TurnLeft();
      }
      if(Input.GetKey(KeyCode.D)){
        TurnRight();
      }
      if(Input.GetKey(KeyCode.Space)){
        CancelInvoke("DecelerateCar");
        _deceleratingCar = false;
        Handbrake();
      }
      if(Input.GetKeyUp(KeyCode.Space)){
        RecoverTraction();
      }
      if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))){
        ThrottleOff();
      }
      if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !_deceleratingCar){
        InvokeRepeating("DecelerateCar", 0f, 0.1f);
        _deceleratingCar = true;
      }
      if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && _steeringAxis != 0f){
        ResetSteeringAngle();
      }

      AnimateWheelMeshes();

    }

    public void CarSpeedUI(){
        float absoluteCarSpeed = Mathf.Abs(carSpeed);
        carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
    }
    
    public void CarSounds(){
        float engineSoundPitch = _initialCarEngineSoundPitch + (Mathf.Abs(_carRigidbody.linearVelocity.magnitude) / 25f);
        carEngineSound.pitch = engineSoundPitch;
        
        if((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f)){
          if(!tireScreechSound.isPlaying){
            tireScreechSound.Play();
          }
        }else if((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f)){
          tireScreechSound.Stop();
        }
    }

    private void TurnLeft() {
      _steeringAxis -= (Time.deltaTime * 10f * steeringSpeed);
      if (_steeringAxis < -1f) 
        _steeringAxis = -1f;
      ApplySteering();
    }

    private void TurnRight() {
      _steeringAxis += (Time.deltaTime * 10f * steeringSpeed);
      if (_steeringAxis > 1f) 
        _steeringAxis = 1f;
      ApplySteering();
    }

    private void ApplySteering() {
      float steeringAngle = _steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void ResetSteeringAngle(){
      if(_steeringAxis < 0f){
        _steeringAxis += (Time.deltaTime * 10f * steeringSpeed);
      }else if(_steeringAxis > 0f){
        _steeringAxis -= (Time.deltaTime * 10f * steeringSpeed);
      }
      if(Mathf.Abs(frontLeftCollider.steerAngle) < 1f){
        _steeringAxis = 0f;
      }
      float steeringAngle = _steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void AnimateWheel(WheelCollider collider, GameObject mesh){
      collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
      mesh.transform.position = position;
      mesh.transform.rotation = rotation;
    }
    
    private void AnimateWheelMeshes(){
      AnimateWheel(frontLeftCollider, frontLeftMesh);
      AnimateWheel(frontRightCollider, frontRightMesh);
      AnimateWheel(rearLeftCollider, rearLeftMesh);
      AnimateWheel(rearRightCollider, rearRightMesh);
    }

    private void GoForward(){
      
      _throttleAxis += (Time.deltaTime * 3f);
      _throttleAxis = Mathf.Clamp(_throttleAxis, -1f, 1f);
      
      if(_localVelocityZ < -1f){
        Brakes();
      }else{
        if(carSpeed < maxSpeed)
        {
          SpeedUp();
        }else {
          ThrottleOff();
    		}
      }
    }
    
    private void GoReverse(){
      
      _throttleAxis -= (Time.deltaTime * 3f);
      _throttleAxis = Mathf.Clamp(_throttleAxis, -1f, 1f);
      
      if(_localVelocityZ > 1f){
        Brakes();
      }else{
        if(Mathf.Abs(carSpeed) < maxReverseSpeed){
          SpeedUp();
        }else{
          ThrottleOff();
        }
      }
    }

    private void ThrottleOff(){
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
    }
    
    private void SpeedUp()
    {
      frontLeftCollider.brakeTorque = 0;
      frontRightCollider.brakeTorque = 0;
      rearLeftCollider.brakeTorque = 0;
      rearRightCollider.brakeTorque = 0;
      
      frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
      frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
      rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
      rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * _throttleAxis;
    }
    
    public void DecelerateCar(){
      
      switch (_throttleAxis)
      {
        case > 0f:
          _throttleAxis -= (Time.deltaTime * 10f);
          break;
        case < 0f:
          _throttleAxis += (Time.deltaTime * 10f);
          break;
      }

      if(Mathf.Abs(_throttleAxis) < 0.15f){
        _throttleAxis = 0f;
      }
      
      _carRigidbody.linearVelocity *= 1f / (1f + (0.025f * decelerationMultiplier));
      ThrottleOff();
      
      if(_carRigidbody.linearVelocity.magnitude < 0.25f){
        _carRigidbody.linearVelocity = Vector3.zero;
        CancelInvoke("DecelerateCar");
      }
    }

    private void Brakes(){
      frontLeftCollider.brakeTorque = brakeForce;
      frontRightCollider.brakeTorque = brakeForce;
      rearLeftCollider.brakeTorque = brakeForce;
      rearRightCollider.brakeTorque = brakeForce;
    }

    private void Handbrake()
    {
      _driftingAxis += (Time.deltaTime);
      float secureStartingPoint = _driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

      if(secureStartingPoint < FLWextremumSlip){
        _driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
      }
      if(_driftingAxis > 1f){
        _driftingAxis = 1f;
      }
      if(_driftingAxis < 1f)
      {
        ApplyWheelFriction(handbrakeDriftMultiplier * _driftingAxis);
      }

      rearLeftCollider.brakeTorque = brakeForce;
      rearRightCollider.brakeTorque = brakeForce;
      
      ThrottleOff();
      
      isTractionLocked = true;
    }
    
    private void DriftCarPS(){
        if(isDrifting){
          RLWParticleSystem.Play();
          RRWParticleSystem.Play();
        }else if(!isDrifting){
          RLWParticleSystem.Stop();
          RRWParticleSystem.Stop();
        }
        
        if((isTractionLocked || Mathf.Abs(_localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f){
          RLWTireSkid.emitting = true;
          RRWTireSkid.emitting = true;
        }else {
          RLWTireSkid.emitting = false;
          RRWTireSkid.emitting = false;
        }
    }

   public void RecoverTraction(){
      isTractionLocked = false;
      _driftingAxis -= (Time.deltaTime / 1.5f);
      if(_driftingAxis < 0f){
        _driftingAxis = 0f;
      }
      if(FLwheelFriction.extremumSlip > FLWextremumSlip){
        ApplyWheelFriction(handbrakeDriftMultiplier * _driftingAxis);
        
        Invoke("RecoverTraction", Time.deltaTime);

      }else if (FLwheelFriction.extremumSlip < FLWextremumSlip){
        ApplyWheelFriction(1f);

        _driftingAxis = 0f;
      }
    }
   
    private void ApplyWheelFriction(float multiplier)
    {
      FLwheelFriction.extremumSlip = FLWextremumSlip * multiplier;
      frontLeftCollider.sidewaysFriction = FLwheelFriction;

      FRwheelFriction.extremumSlip = FRWextremumSlip * multiplier;
      frontRightCollider.sidewaysFriction = FRwheelFriction;

      RLwheelFriction.extremumSlip = RLWextremumSlip * multiplier;
      rearLeftCollider.sidewaysFriction = RLwheelFriction;

      RRwheelFriction.extremumSlip = RRWextremumSlip * multiplier;
      rearRightCollider.sidewaysFriction = RRwheelFriction;
    }
    
}