using UnityEngine;

namespace CarController.CarInfo
{
    [CreateAssetMenu(fileName = "CarConfig", menuName = "Car/Config", order = 0)]
    public class CarConfig : ScriptableObject
    {
        [Header("CAR SETUP")]
        [Space(10)]
        [Range(20, 190)]
        [SerializeField] private int maxSpeed = 90; //The maximum speed that the car can reach in km/h.
        [Range(10, 120)]
        [SerializeField] private int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.
        [Range(1, 10)]
        [SerializeField] private int accelerationMultiplier = 2; // How fast the car can speed up. 1 is a slow acceleration and 10 is the fastest.
        [Space(10)]
        [Range(10, 45)]
        [SerializeField] private int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
        [Range(0.1f, 1f)]
        [SerializeField] private float steeringSpeed = 0.5f; // How fast the steering wheel turns.
        [Space(10)]
        [Range(100, 600)]
        [SerializeField] private int brakeForce = 350; // The strength of the wheel brakes.
        [Range(1, 10)]
        [SerializeField] private int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
        [Range(1, 10)]
        [SerializeField] private int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
        [Space(10)]
        [SerializeField] private Vector3 bodyMassCenter; // This is a vector that contains the center of mass on the car. I recommend set this value.
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
        [SerializeField] private GameObject frontLeftMesh;
        [SerializeField] private WheelCollider frontLeftCollider;
        [Space(10)]
        [SerializeField] private GameObject frontRightMesh;
        [SerializeField] private WheelCollider frontRightCollider;
        [Space(10)]
        [SerializeField] private GameObject rearLeftMesh;
        [SerializeField] private WheelCollider rearLeftCollider;
        [Space(10)]
        [SerializeField] private GameObject rearRightMesh;
        [SerializeField] private WheelCollider rearRightCollider;
        
        
        [Space(20)]
        [Header("EFFECTS")]
        [Space(10)]
        //The following variable lets you set up particle systems in your car
        [SerializeField] private bool useEffects;

        // The following particle systems are used as tire smoke when the car drifts.
        [SerializeField] private ParticleSystem rlWheelParticleSystem;
        [SerializeField] private ParticleSystem rrWheelParticleSystem;

        [Space(10)]
        // The following trail renderers are used as tire skids when the car loses traction.
        [SerializeField] private TrailRenderer rlWheelTireSkid;
        [SerializeField] private TrailRenderer rrWheelTireSkid;
        
        
        [Space(20)]
        [Header("Sounds")]
        [Space(10)]
        //The following variable lets you set up sounds for your car such as the car engine or tire screech sounds.
        [SerializeField] private AudioSource carEngineSound; // This variable stores the sound of the car engine.
        [SerializeField] private AudioSource tireScreechSound; // This variable stores the sound of the tire screech
                                             // when the car is drifting.
        
        // Components
        private Rigidbody _rigidbody; // Stores the car's rigidbody.
        
    }
}