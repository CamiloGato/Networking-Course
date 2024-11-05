using Unity.Netcode;
using UnityEngine;

namespace Lectures.Lecture2.Scripts
{
    public class SampleNetworkPlayer : NetworkBehaviour
    {
        private readonly NetworkVariable<Vector3> _position = new();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Move();
            }
        }

        public void Move()
        {
            SubmitPositionRequestRpc();
        }

        [Rpc(SendTo.Server)]
        private void SubmitPositionRequestRpc(RpcParams rpcParams = default)
        {
            Vector3 randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition;
            _position.Value = randomPosition;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        private void Update()
        {
            transform.position = _position.Value;
        }
    }
}