using System.Linq;
using Intems.Devices;
using Intems.Devices.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests
{
    [TestFixture]
    public class TicksUpdaterTest
    {
        private TicksUpdater _updater;
        private MockRepository _mockery;
        private ITransportLayerWorker _transport;


        [SetUp]
        public void Initialization()
        {
            _mockery = new MockRepository();
            _transport = _mockery.Stub<ITransportLayerWorker>();

            _updater = new TicksUpdater(_transport);
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

        private static byte[] AddCrc(byte[] data)
        {
            ushort crc = CalculateCRC(0, data);
            var answer = data.Concat(new[] { (byte)(crc >> 8), (byte)crc });
            return answer.ToArray();
        }

        [Test]
        public void CorrectResultRecieved()
        {
            var data = new byte[]{0x01, 0x03, 0x00, 0x00, 0x00, 120};
            var answer = AddCrc(data);

            _updater.PushBytes(answer);
            Assert.AreEqual(120, _updater.Ticks);
        }

        [Test]
        public void DeviceFailedReceived()
        {
            const byte errType = (byte)ErrorType.DeviceFailed;
            var data = new byte[] {0x01, 0x83, errType};
            var answer = AddCrc(data);
            //запустили таймер
            _updater.Start();
            Assert.IsTrue(_updater.FreqTimer.Enabled);
            //получили ответ "устройство мертвое"
            _updater.PushBytes(answer);
            Assert.AreEqual(ErrorType.DeviceFailed, _updater.DevError);
            Assert.IsFalse(_updater.FreqTimer.Enabled);
        }

        [Test]
        public void DeviceBusyRecieved()
        {
            const byte errType = (byte) ErrorType.DeviceBusy;
            var data = new byte[] {0x01, 0x83, errType};
            var answer = AddCrc(data);

            _updater.Start();
            Assert.IsTrue(_updater.FreqTimer.Enabled);

            _updater.PushBytes(answer);
            Assert.AreEqual(ErrorType.DeviceBusy, _updater.DevError);
            Assert.IsFalse(_updater.FreqTimer.Enabled);
        }

        [Test]
        public void BadPackageRecieved()
        {
            const byte errType = (byte)ErrorType.DeviceBusy;
            var data = new byte[] { 0x01, 0x83, errType, 0x02, 0x52 };

            _updater.PushBytes(data);
            Assert.AreEqual(1, _updater.BadPkgCount);
        }

        [Test]
        public void TimeoutProcessingCriticalCount()
        {
            for (int i = 0; i < _updater.TimeoutCount; i++)
                _updater.PushTimeout();
            Assert.IsTrue(_updater.IsTimeout);
        }

        [Test]
        public void TimeoutProcessingBeforeCriticalCount()
        {
            for(int i = 0; i < _updater.TimeoutCount - 1; i++)
                _updater.PushTimeout();
            Assert.IsFalse(_updater.IsTimeout);
        }
    }
}
