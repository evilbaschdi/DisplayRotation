namespace DisplayRotation.Core
{
    public interface IAutoStart
    {
        void Enable();

        bool IsEnabled { get; }

        void Disable();
    }
}