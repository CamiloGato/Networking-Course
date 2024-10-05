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

      Rigidbody carRigidbody;
      float steeringAxis;
      float throttleAxis;
      float driftingAxis;
      float localVelocityZ;
      float localVelocityX;
      bool deceleratingCar;
      
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
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

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

      localVelocityX = transform.InverseTransformDirection(carRigidbody.linearVelocity).x;
      localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;
    
      if(Input.GetKey(KeyCode.W)){
        CancelInvoke("DecelerateCar");
        deceleratingCar = false;
        GoForward();
      }
      if(Input.GetKey(KeyCode.S)){
        CancelInvoke("DecelerateCar");
        deceleratingCar = false;
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
        deceleratingCar = false;
        Handbrake();
      }
      if(Input.GetKeyUp(KeyCode.Space)){
        RecoverTraction();
      }
      if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))){
        ThrottleOff();
      }
      if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar){
        InvokeRepeating("DecelerateCar", 0f, 0.1f);
        deceleratingCar = true;
      }
      if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f){
        ResetSteeringAngle();
      }

      AnimateWheelMeshes();

    }

    public void CarSpeedUI(){
        float absoluteCarSpeed = Mathf.Abs(carSpeed);
        carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
    }
    
    public void CarSounds(){
        float engineSoundPitch = _initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.linearVelocity.magnitude) / 25f);
        carEngineSound.pitch = engineSoundPitch;
        
        if((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f)){
          if(!tireScreechSound.isPlaying){
            tireScreechSound.Play();
          }
        }else if((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f)){
          tireScreechSound.Stop();
        }
    }

    private void TurnLeft(){
      steeringAxis -= (Time.deltaTime * 10f * steeringSpeed);
      if(steeringAxis < -1f){
        steeringAxis = -1f;
      }
      float steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void TurnRight(){
      steeringAxis += (Time.deltaTime * 10f * steeringSpeed);
      if(steeringAxis > 1f){
        steeringAxis = 1f;
      }
      float steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void ResetSteeringAngle(){
      if(steeringAxis < 0f){
        steeringAxis += (Time.deltaTime * 10f * steeringSpeed);
      }else if(steeringAxis > 0f){
        steeringAxis -= (Time.deltaTime * 10f * steeringSpeed);
      }
      if(Mathf.Abs(frontLeftCollider.steerAngle) < 1f){
        steeringAxis = 0f;
      }
      float steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void AnimateWheelMeshes(){
      frontLeftCollider.GetWorldPose(out Vector3 FLWPosition, out Quaternion FLWRotation);
      frontLeftMesh.transform.position = FLWPosition;
      frontLeftMesh.transform.rotation = FLWRotation;

      frontRightCollider.GetWorldPose(out Vector3 FRWPosition, out Quaternion FRWRotation);
      frontRightMesh.transform.position = FRWPosition;
      frontRightMesh.transform.rotation = FRWRotation;

      rearLeftCollider.GetWorldPose(out Vector3 RLWPosition, out Quaternion RLWRotation);
      rearLeftMesh.transform.position = RLWPosition;
      rearLeftMesh.transform.rotation = RLWRotation;

      rearRightCollider.GetWorldPose(out Vector3 RRWPosition, out Quaternion RRWRotation);
      rearRightMesh.transform.position = RRWPosition;
      rearRightMesh.transform.rotation = RRWRotation;
    }

    private void GoForward(){
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
      }else{
        isDrifting = false;
        DriftCarPS();
      }
      throttleAxis += (Time.deltaTime * 3f);
      if(throttleAxis > 1f){
        throttleAxis = 1f;
      }
      
      if(localVelocityZ < -1f){
        Brakes();
      }else{
        if(Mathf.RoundToInt(carSpeed) < maxSpeed){
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        }else {
          frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }

    private void GoReverse(){
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
      }else{
        isDrifting = false;
        DriftCarPS();
      }
      
      throttleAxis -= (Time.deltaTime * 3f);
      if(throttleAxis < -1f){
        throttleAxis = -1f;
      }
      
      if(localVelocityZ > 1f){
        Brakes();
      }else{
        if(Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed){
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        }else {
          frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }

    private void ThrottleOff(){
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
    }

    public void DecelerateCar(){
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
      }else{
        isDrifting = false;
        DriftCarPS();
      }
      
      if(throttleAxis != 0f){
        if(throttleAxis > 0f){
          throttleAxis -= (Time.deltaTime * 10f);
        }else if(throttleAxis < 0f){
            throttleAxis += (Time.deltaTime * 10f);
        }
        if(Mathf.Abs(throttleAxis) < 0.15f){
          throttleAxis = 0f;
        }
      }
      carRigidbody.linearVelocity *= 1f / (1f + (0.025f * decelerationMultiplier));
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
      
      if(carRigidbody.linearVelocity.magnitude < 0.25f){
        carRigidbody.linearVelocity = Vector3.zero;
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
      CancelInvoke("RecoverTraction");
      driftingAxis += (Time.deltaTime);
      float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

      if(secureStartingPoint < FLWextremumSlip){
        driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
      }
      if(driftingAxis > 1f){
        driftingAxis = 1f;
      }

      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
      }else{
        isDrifting = false;
      }

      if(driftingAxis < 1f){
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;
      }

      rearLeftCollider.brakeTorque = brakeForce;
      rearRightCollider.brakeTorque = brakeForce;

      isTractionLocked = true;
      DriftCarPS();
    }

    private void DriftCarPS(){
        if(isDrifting){
          RLWParticleSystem.Play();
          RRWParticleSystem.Play();
        }else if(!isDrifting){
          RLWParticleSystem.Stop();
          RRWParticleSystem.Stop();
        }
        if((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f){
          RLWTireSkid.emitting = true;
          RRWTireSkid.emitting = true;
        }else {
          RLWTireSkid.emitting = false;
          RRWTireSkid.emitting = false;
        }
    }

   public void RecoverTraction(){
      isTractionLocked = false;
      driftingAxis -= (Time.deltaTime / 1.5f);
      if(driftingAxis < 0f){
        driftingAxis = 0f;
      }
      if(FLwheelFriction.extremumSlip > FLWextremumSlip){
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        Invoke("RecoverTraction", Time.deltaTime);

      }else if (FLwheelFriction.extremumSlip < FLWextremumSlip){
        FLwheelFriction.extremumSlip = FLWextremumSlip;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        driftingAxis = 0f;
      }
    }

}
