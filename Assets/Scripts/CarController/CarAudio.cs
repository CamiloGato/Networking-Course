using UnityEngine;

namespace CarController
{
    [RequireComponent(typeof(AudioSource))]
    public class CarAudio : MonoBehaviour
    {
        [SerializeField] private CarController car;
        [SerializeField] private bool useSounds;

        [Header("Audio Sources")] public AudioSource carEngineSound;
        public AudioSource tireScreechSound;

        private float _initialCarEnginePitch;

        private void Start()
        {
            if (carEngineSound != null)
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

            if (carEngineSound != null)
            {
                float newPitch = _initialCarEnginePitch + Mathf.Abs(car.LinearVelocityMagnitude) / 25f;
                carEngineSound.pitch = newPitch;
            }

            if (tireScreechSound != null)
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
            if (carEngineSound != null && carEngineSound.isPlaying)
            {
                carEngineSound.Stop();
            }

            if (tireScreechSound != null && tireScreechSound.isPlaying)
            {
                tireScreechSound.Stop();
            }
        }
    }
}