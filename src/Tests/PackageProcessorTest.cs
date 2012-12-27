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
            var pkgBytes = new byte[] { 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x3c, /*CRC bytes ->*/0x41, 0xb8 };

            var expected = new PackageProcessResult {Type = AnswerType.Ok, Address = 0x00, Function = 0x01};
            var actual = _processor.ProcessBytes(pkgBytes);

            Assert.AreEqual(expected, actual);
        }
    }
}
