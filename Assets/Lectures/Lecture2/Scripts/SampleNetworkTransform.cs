using Unity.Netcode;
using UnityEngine;

namespace Lectures.Lecture2.Scripts
{
    public class SampleNetworkTransform : NetworkBehaviour
    {
        private void Update()
        {
            if (IsServer)
            {
                float theta = Time.frameCount / 10f;
                transform.position = new Vector3(Mathf.Sin(theta), 0f, Mathf.Cos(theta));
            }
        }
    }
}