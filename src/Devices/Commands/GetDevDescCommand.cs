namespace Intems.Devices.Commands
{
    public class GetDevDescCommand : Command
    {
        private const byte Function = 0x00;

        public GetDevDescCommand(byte deviceId) : base(deviceId, Function)
        {}
    }
}
