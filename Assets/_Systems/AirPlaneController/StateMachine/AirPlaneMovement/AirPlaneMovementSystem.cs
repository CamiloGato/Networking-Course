using AirPlaneController.AirPlaneInfo;
using UnityEngine;

namespace AirPlaneController.StateMachine.AirPlaneMovement
{
    public abstract class AirPlaneMovementSystem : AirPlaneSystem
    {
        private readonly AirPlaneConfig _airPlaneConfig;
        private readonly AirPlaneStats _airPlaneStats;

        protected AirPlaneConfig AirPlaneConfig => _airPlaneConfig;
        protected AirPlaneStats AirPlaneStats => _airPlaneStats;
        
        protected AirPlaneMovementSystem(AirPlaneConfig airPlaneConfig, AirPlaneStats airPlaneStats)
        {
            _airPlaneConfig = airPlaneConfig;
            _airPlaneStats = airPlaneStats;
        }
        
        protected void TurnOffPropellersAndLights()
        {
            Transform[] propellers = _airPlaneConfig.Propellers;
            Light[] turbineLights = _airPlaneConfig.TurbineLights;
            
            if (propellers.Length > 0)
            {
                RotatePropellers(propellers, 0f);
            }

            if (turbineLights.Length > 0)
            {
                ControlEngineLights(turbineLights, 0f);
            }
        }
        
        protected void TurnOnPropellersAndLights()
        {
            Transform[] propellers = _airPlaneConfig.Propellers;
            Light[] turbineLights = _airPlaneConfig.TurbineLights;
            float propelSpeedMultiplier = _airPlaneConfig.PropelSpeedMultiplier;
            
            float currentSpeed = _airPlaneStats.CurrentSpeed;
            float currentEngineLightIntensity = _airPlaneStats.CurrentEngineLightIntensity;

            if (propellers.Length > 0)
            {
                RotatePropellers(propellers, currentSpeed * propelSpeedMultiplier);
            }

            if (turbineLights.Length > 0)
            {
                ControlEngineLights(turbineLights, currentEngineLightIntensity);
            }
        }

        protected void ChangeWingTrailEffectThickness(float thickness)
        {
            TrailRenderer[] wingTrailEffects = _airPlaneConfig.WingTrailEffects;

            foreach (TrailRenderer trailEffect in wingTrailEffects)
            {
                trailEffect.startWidth = Mathf.Lerp(trailEffect.startWidth, thickness, Time.deltaTime * 10f);
            }
        }
        
        #region Private methods
        
        private void RotatePropellers(Transform[] rotate, float speed)
        {
            foreach (Transform transform in rotate)
            {
                transform.Rotate(Vector3.forward * -speed * Time.deltaTime);
            }
        }

        private void ControlEngineLights(Light[] lights, float intensity)
        {
            foreach (Light light in lights)
            {
                light.intensity = Mathf.Lerp(light.intensity, intensity, 10f * Time.deltaTime);
            }
        }


        #endregion
        
    }
}