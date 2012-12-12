using System.Collections.Generic;
using System.Linq;

namespace Intems.Devices.Commands
{
    public class Package
    {
        private const byte FirstByte = 0x7e;
        private const byte LastByte = 0x7f;
        private static readonly byte[] Escapes = new byte[] { 0x7E, 0x7d, 0x7f, 0x20 };


        private readonly List<byte> _bytes;

        public Package(Command cmd)
        {
            _bytes = new List<byte>();
            if (cmd == null) return;

            _bytes.Add(FirstByte);
            foreach (var item in cmd.Bytes)
            {
                if(Escapes.Contains(item))
                {
                    _bytes.Add(0x7d);
                    _bytes.Add((byte)(item ^ 0x20));
                }
                else
                {
                    _bytes.Add(item);
                }
            }
            _bytes.Add(LastByte);
        }

        public byte[] Bytes
        {
            get { return _bytes.ToArray(); }
        }
    }
}
