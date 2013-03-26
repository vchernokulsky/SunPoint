using System;
using Intems.Devices.Commands;

namespace Intems.Devices.Interfaces {

    public interface ITransportLayerWorker {
        void SendPackage(Package package, IDeviceResponse deviceResponse);
    }
}
