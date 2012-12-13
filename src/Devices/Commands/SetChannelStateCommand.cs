using System.Linq;

namespace Intems.Devices.Commands
{
    public class SetChannelStateCommand : Command
    {
        private const byte Function = 0x02;
        public SetChannelStateCommand(byte deviceId) : base(deviceId, Function){}
        public SetChannelStateCommand(byte deviceId, byte channel, ushort delay, ushort time) : base(deviceId, Function)
        {
            var channelArr = new[] {channel};
            var delayArr = new[]{(byte) (delay >> 8), (byte)delay};
            var timeArr = new[] {(byte) (time >> 8), (byte)time};
            Params = channelArr.Concat(delayArr.Concat(timeArr)).ToArray();
        }
    }
}
