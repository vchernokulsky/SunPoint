using System;
using Intems.Devices;
using Intems.Devices.Commands;
using Intems.Devices.DevicePollers;
using Intems.Devices.Interfaces;

namespace Intems.SunPoint.BL
{
    class Solarium : IDeviceResponse
    {
        private const int DevNumber = 1;
        private const int ChannelNumber = 1;

        private readonly TransportLayerWorker _worker;

        private readonly ButtonPressPoller _pressPoller;
        private readonly TicksUpdater _ticksUpdater;

        public event EventHandler SunbathStarted;
        public event EventHandler SunbathStopped;
        public event EventHandler TickChanged;

        public Solarium()
        {
            _worker = new TransportLayerWorker("COM5", 19200);

            _pressPoller = new ButtonPressPoller(_worker);
            _ticksUpdater = new TicksUpdater(_worker);
        }

        private int _ticks;
        public void Start(int ticks)
        {
            _ticks = ticks;
            _pressPoller.BtnPressed += OnButtonPressed;
        }

        private void OnButtonPressed(object sender, BtnPressArgs btnPressArgs)
        {
            if (btnPressArgs.Button == PressedBtn.StartBtn)
            {
                //посылаем команду на установку занчения в канале
                var cmd = new SetChannelStateCommand(DevNumber, ChannelNumber, 0, (ushort) _ticks);
                var pkg = new Package(cmd);
                _worker.SendPackage(pkg, this);
                //запускаем диспетчер обратного отсчета
                _ticksUpdater.TicksChanged += OnTicksChanged;
                _ticksUpdater.Start();
                //оповещаем GUI о старте загара
                RaiseSunbathStarted(EventArgs.Empty);
            }
        }

        public void Stop()
        {
            var cmd = new SetChannelStateCommand(DevNumber, ChannelNumber, 0, 0);
            var pkg = new Package(cmd);
            _worker.SendPackage(pkg, this);

            _ticksUpdater.TicksChanged -= OnTicksChanged;
            RaiseSunbathStopped(EventArgs.Empty);
        }

        public void PushBytes(byte[] bytes)
        {
        }

        public void PushTimeout()
        {
        }

        public int Time { get; set; }

        private void OnTicksChanged(object sender, TicksUpdaterArgs e)
        {
            if (e.Ticks > 0)
                RaiseTickChanged(EventArgs.Empty);
            else
                RaiseSunbathStopped(EventArgs.Empty);
        }

        private void RaiseSunbathStarted(EventArgs e)
        {
            EventHandler handler = SunbathStarted;
            if (handler != null) handler(this, e);
        }

        private void RaiseSunbathStopped(EventArgs e)
        {
            EventHandler handler = SunbathStopped;
            if (handler != null) handler(this, e);
        }

        private void RaiseTickChanged(EventArgs e)
        {
            EventHandler handler = TickChanged;
            if (handler != null) handler(this, e);
        }
    }
}
