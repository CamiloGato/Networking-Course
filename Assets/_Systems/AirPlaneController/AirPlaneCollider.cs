using System;
using UnityEngine;

namespace AirPlaneController
{
    public class AirPlaneCollider : MonoBehaviour
    {
        private Collider _collider;
        private Rigidbody _rigidbody;
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.GetComponent<AirPlaneCollider>() == null)
            {
                
            }
        }

        public void SetUp()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
            
            // Set the rigidbody properties
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        public void Collision()
        {
            _collider.isTrigger = false;
            Destroy(_rigidbody);
        }
    }
}