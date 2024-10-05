using UnityEngine;

namespace CarController.CarInfo
{
    [CreateAssetMenu(fileName = "CarConfig", menuName = "Car/Config", order = 0)]
    public class CarConfig : ScriptableObject
    {
        [Header("CAR SETUP")]
        [Space(10)]
        [Range(20, 190)]
        public int maxSpeed = 90; //The maximum speed that the car can reach in km/h.
        [Range(10, 120)]
        public int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.
        [Range(1, 10)]
        public int accelerationMultiplier = 2; // How fast the car can speed up. 1 is a slow acceleration and 10 is the fastest.
        [Space(10)]
        [Range(10, 45)]
        public int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
        [Range(0.1f, 1f)]
        public float steeringSpeed = 0.5f; // How fast the steering wheel turns.
        [Space(10)]
        [Range(100, 600)]
        public int brakeForce = 350; // The strength of the wheel brakes.
        [Range(1, 10)]
        public int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
        [Range(1, 10)]
        public int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
        [Space(10)]
        public Vector3 bodyMassCenter; // This is a vector that contains the center of mass on the car. I recommend set this value.
                                        // In the points x = 0 and z = 0 of your car. You can select the value that you want in the y-axis,
                                        // however, you must notice that the higher this value is, the more unstable the car becomes.
                                        // Usually the y value goes from 0 to 1.5.
        
        
        [Space(20)]
        [Header("WHEELS")]
        [Space(10)]
        /*
        The following variables are store the wheel's data of the car.We need both the mesh-only game objects and wheel
        collider components of the wheels.The wheel collider components and 3D meshes of the wheels cannot come from the same
        game object; they must be separate game objects.
        */
        public GameObject frontLeftMesh;
        public WheelCollider frontLeftCollider;
        [Space(10)]
        public GameObject frontRightMesh;
        public WheelCollider frontRightCollider;
        [Space(10)]
        public GameObject rearLeftMesh;
        public WheelCollider rearLeftCollider;
        [Space(10)]
        public GameObject rearRightMesh;
        public WheelCollider rearRightCollider;
        
        
        [Space(20)]
        [Header("EFFECTS")]
        [Space(10)]
        //The following variable lets you set up particle systems in your car
        public bool useEffects;

        // The following particle systems are used as tire smoke when the car drifts.
        public ParticleSystem rlWheelParticleSystem;
        public ParticleSystem rrWheelParticleSystem;

        [Space(10)]
        // The following trail renderers are used as tire skids when the car loses traction.
        public TrailRenderer rlWheelTireSkid;
        public TrailRenderer rrWheelTireSkid;
        
        
        [Space(20)]
        [Header("Sounds")]
        [Space(10)]
        
        //The following variable lets you set up sounds for your car such as the car engine or tire screech sounds.
        public AudioSource carEngineSound; // This variable stores the sound of the car engine.
        public AudioSource tireScreechSound; // This variable stores the sound of the tire screech
                                             // when the car is drifting.
        
        // Components
        private Rigidbody _rigidbody; // Stores the car's rigidbody.
        
    }
}