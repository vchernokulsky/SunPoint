using System;
using UV.Devices;

namespace Intems.Devices.Interfaces {

    public interface ITransportLayerWorker : IDisposable {
        void   InitDevice();
        int    GetChannelState(int devId, int channel);
        void   SetChannelState(int devId, int channel, int delay, int time);
        void   ResetDevice(int devId);

        string GetDeviceDescription(int devId);
        Buttons GetButtonsState(int devId);
    }
}
