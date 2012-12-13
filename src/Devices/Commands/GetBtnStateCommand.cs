namespace Intems.Devices.Commands
{
    public class GetBtnStateCommand : Command
    {
        private static readonly byte Function = CmdCodes.GET_DEV_STATE;

        public GetBtnStateCommand(byte deviceId) : base(deviceId, Function)
        {}
    }
}
