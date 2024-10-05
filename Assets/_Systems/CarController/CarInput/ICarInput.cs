namespace CarController.CarInput
{
    public interface ICarInput
    {
        float Horizontal();
        float Vertical();
        bool Brake();
    }
}