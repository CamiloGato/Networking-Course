using System;

namespace CarController.Subsystems
{
    [Serializable]
    public abstract class CarSystem
    {
        public abstract void StartSystem();
        public abstract void HandleSystem();
    }
}