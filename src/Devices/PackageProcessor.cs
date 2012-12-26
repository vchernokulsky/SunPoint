using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intems.Devices.Commands;

namespace Intems.Devices
{
    public enum AnswerType
    {
        Ok,
        Error,
        BadPackage
    }

    public class PackageProcessResult
    {
        public AnswerType Type { get; set; }

        public byte Address { get; set; }
        public byte Function { get; set; }
    }

    public class PackageProcessor
    {
        public PackageProcessResult ProcessBytes(byte[] bytes)
        {
            var result = new PackageProcessResult();

            return result;
        }
    }
}
