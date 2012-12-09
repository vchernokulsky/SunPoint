using System;
using System.Collections.Generic;

namespace Intems.Devices
{
    enum PackageParseState
    {
        PackageStart,
        EscSymbolFind,
        PackageEnd
    }

    public class PackageDataArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }

    class DataRetriever
    {
        //здесь храним обработанный пакет
        private readonly List<byte> _response = new List<byte>();
        private readonly List<byte> _bytes = new List<byte>();

        public event EventHandler<PackageDataArgs> PackageRetrieved;

        public void AddBytes(byte[] bytes)
        {
            _bytes.AddRange(bytes);
            ProcessData();
        }

        private PackageParseState _parseState = PackageParseState.PackageEnd;
        private void ProcessData()
        {
            foreach (var @byte in _bytes)
            {
                switch (_parseState)
                {
                    case PackageParseState.PackageStart:
                        ProcessByteOfPackage(@byte);
                        break;

                    case PackageParseState.EscSymbolFind:
                        ProcessEscByte(@byte);
                        break;

                    case PackageParseState.PackageEnd:
                        if (@byte == 0x7e)
                            _parseState = PackageParseState.PackageStart;
                        break;
                }
            }
            _bytes.Clear();
        }

        private void ProcessEscByte(byte @byte)
        {
            _response.Add((byte) (@byte ^ 20));
            _parseState = PackageParseState.PackageStart;
        }

        private void ProcessByteOfPackage(byte @byte)
        {
            switch (@byte)
            {
                case 0x7d:
                    _parseState = PackageParseState.EscSymbolFind;
                    break;
                case 0x7f:
                    CreateData();
                    _parseState = PackageParseState.PackageEnd;
                    break;
                default:
                    _response.Add(@byte);
                    break;
            }
        }

        private void CreateData()
        {
            var args = new PackageDataArgs {Data = _response.ToArray()};
            RaisePackageRetrieved(args);
            _response.Clear();
        }

        private void RaisePackageRetrieved(PackageDataArgs e)
        {
            var handler = PackageRetrieved;
            if (handler != null) handler(this, e);
        }
    }
}
