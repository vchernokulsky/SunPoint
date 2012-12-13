namespace Intems.Devices.Commands
{
    public class GetDevDescCommand : Command
    {
        private static readonly byte Function = CmdCodes.GET_DEV_DESCRIPTION;

        public GetDevDescCommand(byte deviceId) : base(deviceId, Function)
        {}
    }
}
