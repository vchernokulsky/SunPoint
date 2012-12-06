namespace UV.Devices
{
    public class DeviceHelper
    {
        //Function codes
        public static byte GET_DEV_DESCRIPTION = 0;
        public static byte GET_DEV_STATE = 1;
        public const byte GET_CHANNEL_STATE = 3;
        public const byte SET_CHANNEL_STATE = 2;
        public const byte DEV_RESET = 4;

        //Device answer codes
        public const byte OK = 0;
        private const byte FUNC_INCORRECT = 1;
        private const byte INCORRECT_QUERY = 2;
        private const byte DEV_BUSY = 3;
        private const byte DEV_FAILED = 4;
        private const byte ACCSESS_DENIED = 5;

    }
}