using UnityEngine;

namespace CarController
{
    [RequireComponent(typeof(AudioSource))]
    public class CarAudio : MonoBehaviour
    {
        [SerializeField] private CarController car;
        [SerializeField] private bool useSounds;

        [Header("Audio Sources")]
        public AudioSource carEngineSound;
        public AudioSource tireScreechSound;

        private float _initialCarEnginePitch;

        private void Start()
        {
            if (carEngineSound)
            {
                _initialCarEnginePitch = carEngineSound.pitch;
            }
        }

        private void Update()
        {
            if (!useSounds)
            {
                StopSounds();
                return;
            }

            if (carEngineSound)
            {
                float newPitch = _initialCarEnginePitch + Mathf.Abs(car.LinearVelocityMagnitude) / 25f;
                carEngineSound.pitch = newPitch;
            }

            if (tireScreechSound)
            {
                if ((car.IsDrifting) || (car.IsTractionLocked && Mathf.Abs(car.CarSpeed) > 12f))
                {
                    if (!tireScreechSound.isPlaying) tireScreechSound.Play();
                }
                else
                {
                    tireScreechSound.Stop();
                }
            }
        }

        private void StopSounds()
        {
            if (carEngineSound && carEngineSound.isPlaying)
            {
                carEngineSound.Stop();
            }

            if (tireScreechSound && tireScreechSound.isPlaying)
            {
                tireScreechSound.Stop();
            }
        }
    }
}