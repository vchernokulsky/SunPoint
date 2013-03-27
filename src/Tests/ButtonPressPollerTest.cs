using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intems.Devices.DevicePollers;
using Intems.Devices.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests
{
    [TestFixture]
    public class ButtonPressPollerTest
    {
        private MockRepository _mockery;
        private ITransportLayerWorker _worker;
        private ButtonPressPoller _poller;

        [SetUp]
        public void Initialization()
        {
            _mockery = new MockRepository();
            _worker = _mockery.Stub<ITransportLayerWorker>();
            _poller = new ButtonPressPoller(_worker);
        }

        [Test]
        public void PressButtonRecieved()
        {
            var startBtnBytes = new byte[] {0x00, 0x01, 0x00, 0x01, 0xe8, 0x00, 0xcf, 0x11};
            var stopBtnBytes  = new byte[] {0x00, 0x01, 0x00, 0x02, 0xf0, 0x00, 0xe7, 0x72};

            _poller.PushBytes(startBtnBytes);
            Assert.AreEqual(PressedBtn.StartBtn, _poller.Button);

            _poller.PushBytes(stopBtnBytes);
            Assert.AreEqual(PressedBtn.StopBtn, _poller.Button);
        }
    }
}
