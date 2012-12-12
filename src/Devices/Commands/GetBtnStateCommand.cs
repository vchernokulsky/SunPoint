namespace Intems.Devices.Commands
{
    public class GetBtnStateCommand : Command
    {
        private const byte Function = 0x03;
        public GetBtnStateCommand(byte deviceId) : base(deviceId, Function)
        {}
    }
}
