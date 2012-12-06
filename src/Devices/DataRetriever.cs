using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intems.Devices
{
    enum PackageParseState
    {
        PackageStart,
        EscSymbolFind,
        PackageEnd
    }

    class DataRetriever
    {
        private readonly List<byte> _response = new List<byte>();
        private readonly List<byte> _bytes = new List<byte>();

        public void AddBytes(byte[] bytes)
        {
            _bytes.AddRange(bytes);
            ProcessData();
        }

        private PackageParseState _parseState = PackageParseState.PackageEnd;
        private void ProcessData()
        {
            bool esc = false;
            foreach (var @byte in _bytes)
            {
                switch (_parseState)
                {
                    case PackageParseState.PackageStart:
                        switch (@byte)
                        {
                            case 0x7d:
                                _parseState = PackageParseState.EscSymbolFind;
                                break;
                            case 0x7f:
                                _parseState = PackageParseState.PackageEnd;
                                break;
                            default:
                                _response.Add(@byte);
                                break;
                        }
                        break;

                    case PackageParseState.EscSymbolFind:
                        _response.Add((byte)(@byte ^ 20));
                        _parseState = PackageParseState.PackageStart;
                        break;

                    case PackageParseState.PackageEnd:
                        if(@byte == 0x7e)
                            _parseState = PackageParseState.PackageStart;
                        break;
                }
            }
        }
    }
}
