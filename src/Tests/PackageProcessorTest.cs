using Intems.Devices;
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

        [Test]
        public void BadCrcPackageTest()
        {
            var pkgBytes = new byte[] { 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x3c, /*CRC bytes ->*/0x05, 0x11 };

            var expected = new PackageProcessResult {Type = AnswerType.BadPackage, Address = 0x00, Function = 0x00 };
            var actual = _processor.ProcessBytes(pkgBytes);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ErrorInAnswerTest()
        {
            var errType = (byte) ErrorType.FunctionNotExist;
            var pkgBytes = new byte[] { 0x01, 0x8a, errType, /*CRC bytes ->*/0x9a, 0x20};

            var expected = new PackageProcessResult {Type = AnswerType.Error, Address = 0x01, Function = 0x0a};
            var actual = _processor.ProcessBytes(pkgBytes);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(ErrorType.FunctionNotExist, actual.ErrorType);
        }
    }
}
