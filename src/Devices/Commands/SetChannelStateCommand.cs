using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intems.Devices.Commands
{
    public class SetChannelStateCommand : Command
    {
        private const byte Function = 0x02;
        public SetChannelStateCommand(byte deviceId) : base(deviceId, Function)
        {}
    }
}
