using System;

namespace Intems.Devices
{
    public enum AnswerType
    {
        BadPackage, //битый пакет
        Ok,
        Error
    }

    public enum ErrorType
    {
        None,
        FunctionNotExist,
        IncorrectQuerryData,
        DeviceBusy,
        DeviceFailed,
        AccessDeny
    }

    public class PackageProcessResult
    {
        public AnswerType Type { get; set; }
        public ErrorType ErrorType { get; set; }

        public byte Address { get; set; }
        public byte Function { get; set; }
        public byte[] Params { get; set; }

        public override bool Equals(object obj)
        {
            bool isEqual = false;

            var val = obj as PackageProcessResult;
            if(val != null)
            {
                isEqual = (val.Address == Address) && (val.Function == Function) && (val.Type == Type);
            }
            return isEqual;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return String.Format("Answer type={0}; [addr={1};]func={2}", Type, Address, Function);
        }
    }

    public class PackageProcessor
    {
        private const int CRCByteLength = 2;
        private const int HeadLength = 2;
        private const int MinPackageLength = HeadLength + CRCByteLength;

        public PackageProcessResult ProcessBytes(byte[] bytes)
        {
            PackageProcessResult result = null;
            if(bytes.Length > MinPackageLength)
            {
                //данные без 2-х байт CRC
                var startCrcIdx = bytes.Length-CRCByteLength;
                var data = new byte[startCrcIdx];
                Array.ConstrainedCopy(bytes, 0, data, 0, data.Length);
                //массив с 2-мя байтами CRC
                var crcBytes = new byte[CRCByteLength];
                Array.ConstrainedCopy(bytes, startCrcIdx, crcBytes, 0, CRCByteLength);
                var crcInPakage = BytesToUshort(crcBytes);

                if(CalcCRC(data)==crcInPakage)
                {
                    result = new PackageProcessResult { Type = AnswerType.Ok, Address = bytes[0], Function = bytes[1] };
                    if(data.Length==HeadLength + 2 && data[2]==0x80)
                    {
                        result.Type = AnswerType.Error;
                        result.ErrorType = (ErrorType) data[3];
                    }
                    result.Params = new byte[bytes.Length - MinPackageLength];
                    Array.ConstrainedCopy(data, HeadLength, result.Params, 0, result.Params.Length);
                }
                else
                {
                    result = new PackageProcessResult {Type = AnswerType.BadPackage};
                }
                
            }
            return result;
        }

        private static ushort BytesToUshort(byte[] bytes)
        {
            ushort crc = 0;
            if (bytes.Length == CRCByteLength)
            {
                crc |= bytes[bytes.Length - 2];
                crc = (ushort) (crc << 8);
                crc |= bytes[bytes.Length - 1];
            }
            else
            {
                var msg = String.Format("Incorrect bytes array length. Expected length: 2 actual length: {0}", bytes.Length);
                throw new ArgumentException(msg, "bytes");
            }
            return crc;
        }

        private ushort CalcCRC(byte[] data)
        {
            int dataLength = data.Length;

            ushort sum = 0;
            int idx = 0;
            while ((dataLength--) != 0)
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
