using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intems.Devices.Commands
{
    public class GetChannelStateCommand : Command
    {
        private const byte Function = CmdCodes.GET_CHANNEL_STATE;
        public GetChannelStateCommand(byte devId) : base(devId, Function) {}
    }
}
