namespace Intems.Devices.Commands
{
    public class GetChannelStateCommand : Command
    {
        private const byte Function = CmdCodes.GET_CHANNEL_STATE;
        public GetChannelStateCommand(byte devId) : base(devId, Function) {}
        public GetChannelStateCommand(byte devId, byte channel) : base(devId, Function)
        {
            Params = new[]{channel};
        }
    }
}
