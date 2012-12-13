using System.Linq;

namespace Intems.Devices.Commands
{
    public class Command
    {
        private readonly byte _deviceId;
        private readonly byte _function;
        protected byte[] Params;

        public Command(byte deviceId, byte function)
        {
            _deviceId = deviceId;
            _function = function;
        }

        public Command(byte deviceId, byte function, byte[] @params) : this(deviceId, function)
        {
            Params = @params;
        }

        public byte[] Bytes
        {
            get
            {
                var chunk1 = new[]{_deviceId, _function};
                if(Params != null)
                    chunk1 = chunk1.Concat(Params).ToArray();
                //считаем и добавляем CRC в команду
                var crc = CalculateCRC(0, chunk1);
                byte[] result = chunk1.Concat(new[] {(byte) (crc >> 8), (byte) (crc)}).ToArray();
                return result;
            }
        }

        private static ushort CalculateCRC(ushort sum, byte[] data)
        {
            int len = data.Length;
            int idx = 0;
            while ((len--) != 0)
            {
                byte tmp = data[idx++];
                for (int i = 0; i < 8; ++i)
                {
                    long osum = sum;
                    sum <<= 1;
                    if ((tmp & 0x80) == 128) sum |= 1;
                    if ((osum & 0x8000) == 32768) sum ^= 0x1021;
                    tmp <<= 1;
                }
            }
            return sum;
        }
    }
}
