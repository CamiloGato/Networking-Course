namespace AirPlaneController.AirPlaneInput
{
    public interface IAirPlaneInput
    {
        float Horizontal();
        float Vertical();
        float Yaw();
        bool Turbo();
        
        
    }
}