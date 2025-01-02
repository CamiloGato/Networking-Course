using UnityEngine;

namespace CarController
{
    /// <summary>
    /// Manages the visual effects (particle systems, tire skids)
    /// for the car based on the CarController state.
    /// </summary>
    public class CarEffects : MonoBehaviour
    {
        [SerializeField] private CarController car;
        [SerializeField] private bool useEffects;

        public ParticleSystem rlwParticleSystem;
        public ParticleSystem rrwParticleSystem;

        [Header("Trail Renderers")] public TrailRenderer rlwTireSkid;
        public TrailRenderer rrwTireSkid;

        private void Update()
        {
            // If effects are disabled, stop everything and skip.
            if (!useEffects)
            {
                StopAllEffects();
                return;
            }

            // Handle drifting particle systems (smoke).
            HandleParticleEffects();

            // Handle skid trails.
            HandleSkidTrails();
        }

        /// <summary>
        /// Stops all effects (particles and trails).
        /// </summary>
        private void StopAllEffects()
        {
            if (rlwParticleSystem)
                rlwParticleSystem.Stop();

            if (rrwParticleSystem)
                rrwParticleSystem.Stop();

            if (rlwTireSkid)
                rlwTireSkid.emitting = false;

            if (rrwTireSkid)
                rrwTireSkid.emitting = false;
        }

        /// <summary>
        /// Handles the drift smoke particles based on the car drifting state.
        /// </summary>
        private void HandleParticleEffects()
        {
            // If the car is drifting, ensure both ParticleSystems are playing.
            if (car.IsDrifting)
            {
                if (!rlwParticleSystem.isPlaying)
                    rlwParticleSystem.Play();

                if (!rrwParticleSystem.isPlaying)
                    rrwParticleSystem.Play();
            }
            else
            {
                // Otherwise, stop both ParticleSystems.
                rlwParticleSystem.Stop();
                rrwParticleSystem.Stop();
            }
        }

        /// <summary>
        /// Handles the tire skid trails based on traction lock and velocity.
        /// </summary>
        private void HandleSkidTrails()
        {
            if (!car) return;

            // Condition for skid trails: traction locked OR local X velocity > 5,
            // and overall speed > 12.
            bool shouldSkid = (car.IsTractionLocked || Mathf.Abs(car.LocalVelocityX) > 5f)
                              && Mathf.Abs(car.CarSpeed) > 12f;

            if (rlwTireSkid)
                rlwTireSkid.emitting = shouldSkid;

            if (rrwTireSkid)
                rrwTireSkid.emitting = shouldSkid;
        }
    }
}