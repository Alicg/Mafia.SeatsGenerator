namespace Mafia.SeatsGenerator.Utils
{
    public interface IVibrator
    {
        void Vibrate(int milliseconds);
        bool CanVibrate { get; }
    }
}