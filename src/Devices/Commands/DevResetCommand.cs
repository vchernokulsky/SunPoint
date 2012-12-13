using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intems.Devices.Commands
{
    public class DevResetCommand : Command
    {
        private const byte Function = CmdCodes.DEV_RESET;
        public DevResetCommand(byte deviceId) : base(deviceId, Function){}
    }
}
