using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intems.Devices;
using Intems.Devices.Commands;
using Intems.Devices.Helpers;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    class PackageProcessorTest
    {
        private PackageProcessor _processor;

        [SetUp]
        public void Initialize()
        {
            _processor = new PackageProcessor();
        }

        [Test]
        public void GoodPackageTest()
        {
            //пакет без стартового и стопового байтов
            var pkgBytes = new byte[] { 0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c, /*CRC bytes ->*/0x70, 0x35 };

            var expected = new PackageProcessResult {Type = AnswerType.Ok, Address = 0x00, Function = 0x01};
            var data = _processor.ProcessBytes(pkgBytes);

            Assert.NotNull(data);
        }
    }
}
