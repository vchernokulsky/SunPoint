using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intems.Devices.Commands;

namespace Intems.Devices.Interfaces
{
    public interface IDeviceResponse
    {
        void PushBytes(byte[] bytes);
        void PushTimeout();
    }
}
