using System.Collections.Generic;

namespace Intems.Devices.Helpers 
{
    public class PackageHelper 
    {
        public List<byte> PackToPackage(List<byte> data) 
        {
            ushort crc = CalcCRC(0, data.ToArray());
            data.Add((byte) (crc >> 8));
            data.Add((byte) (crc));

            var pack = new List<byte> {0x7e}; //первый байт пакета
            foreach (byte item in data) {
                byte symbol = item;
                if ((symbol == 0x7E) || (symbol == 0x7d) || (symbol == 0x7f) || (symbol == 0x20)) {
                    pack.Add(0x7d);
                    pack.Add((byte) ((symbol) ^ 0x20));
                }
                else {
                    pack.Add(symbol);
                }
            }
            pack.Add(0x7f); //последний байт пакета
            return pack;
        }

        public byte[] SplitToArray(ushort number) {
            byte[] result = new byte[2];
            result[0] = (byte) (number >> 8);
            result[1] = (byte) number;

            return result;
        }

        public ushort CombineToUshort(IList<byte> buffer) {
            ushort result = 0;

            result |= buffer[buffer.Count - 2];
            result = (ushort) ((result) << 8);
            result |= buffer[buffer.Count - 1];

            return result;
        }

        public ushort CalcCRC(ushort sum, byte[] data) 
        {
            int len = data.Length;
            int idx = 0;
            while ((len--) != 0) {
                byte tmp = data[idx++];
                for (int i = 0; i < 8; ++i) {
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