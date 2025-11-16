namespace CrazyZoo.Domain.Interfaces
{
    public interface Flyable
    {
        void Fly();
        bool IsFlying { get; }
    }
}