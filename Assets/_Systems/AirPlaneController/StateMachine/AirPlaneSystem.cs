namespace AirPlaneController.StateMachine
{
    public abstract class AirPlaneSystem
    {
        public abstract void StartSystem();
        public abstract void UpdateSystem();
        public abstract void EndSystem();
    }
}