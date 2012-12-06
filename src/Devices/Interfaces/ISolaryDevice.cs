namespace Intems.Devices.Interfaces {
    public interface ISolaryDevice {
        void Start(SolaryAction action, int time);
        void Stop(SolaryAction action);
        void ResetDevice();

        bool IsAvailable();
    }
}
