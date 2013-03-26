using System;
using System.Collections.Generic;
using System.Timers;
using Intems.Devices.Commands;
using Intems.Devices.ErrorProcessing;
using Intems.Devices.Interfaces;

namespace Intems.Devices
{
    public class TicksUpdaterArgs : EventArgs
    {
        public ushort Ticks { get; set; }
    }

    public class DeviceErrorArgs : EventArgs
    {}

    public class TicksUpdater : IDeviceResponse
    {
        private const int MaxBadPkgCount = 5;
        private const int MaxTimeoutCount = 5;
        private const int TimeInterval = 50;

        private uint _ticks;

        private int _badPkgCount;
        private int _timeoutCount;

        private ErrorType _devError = ErrorType.None;

        private readonly ITransportLayerWorker _worker;
        private readonly PackageProcessor _packageProcessor;
        private readonly ErrorDispatcher _errorDispatcher;

        private readonly Timer _timer;
        private readonly object _locker = new object();

        public event EventHandler<TicksUpdaterArgs> TicksChanged;
        public event EventHandler<DeviceErrorArgs> DeviceError;

        public TicksUpdater(ITransportLayerWorker worker)
        {
            _errorDispatcher = new ErrorDispatcher();
            _packageProcessor = new PackageProcessor();

            _worker = worker;
            //_worker.PackageReceived += OnPackageReceived;

            _timer = new Timer {Interval = TimeInterval};
            _timer.Elapsed += OnTimerElapsed;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void PushBytes(byte[] bytes)
        {
            var result = _packageProcessor.ProcessBytes(bytes);
            if (result == null) return;

            if (result.Type == AnswerType.Ok)
            {
                uint ticks = TicksFromBytes(result.Params);
                if (ticks < _ticks)
                {
                    RaiseTicksChanged(new TicksUpdaterArgs { Ticks = (ushort)ticks });
                    if (ticks == 0)
                        _timer.Stop(); //дотикали до конца больше не надо опрашивать
                }
                _ticks = ticks;
            }
            else
            {
                switch (result.Type)
                {
                    case AnswerType.Error:
                        _timer.Stop();
                        _devError = result.ErrorType;
                        break;

                    case AnswerType.BadPackage:
                        _badPkgCount++;
                        break;
                }
            }
        }

        public void PushTimeout()
        {
            if (++_timeoutCount >= MaxTimeoutCount)
            {
                RaiseDeviceError(new DeviceErrorArgs());
#if DEBUG
                IsTimeout = true;
#endif
            }
        }


        private static uint TicksFromBytes(IList<byte> bytes)
        {
            uint ticks = 0;
            ticks |= bytes[bytes.Count - 2]; //предпоследний
            ticks = ((ticks) << 8);
            ticks |= bytes[bytes.Count - 1]; //последний
            return ticks;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (_locker)
            {
                _timer.Stop();
                var pkg = new Package(new GetChannelStateCommand(1));
                //_worker.SendPackage(pkg);
                _worker.SendPackage(pkg, this);
                _timer.Start();
            }
        }

        private void RaiseTicksChanged(TicksUpdaterArgs e)
        {
            EventHandler<TicksUpdaterArgs> handler = TicksChanged;
            if (handler != null) handler(this, e);
        }

        private void RaiseDeviceError(DeviceErrorArgs args)
        {
            var handler = DeviceError;
            if (handler != null) handler(this, args);
        }

#region PROPERTIES FOR TEST

#if DEBUG

        public uint Ticks { get { return _ticks; } }

        public int BadPkgCount { get { return _badPkgCount; } }

        public ErrorType DevError { get { return _devError; } }

        public Timer FreqTimer { get { return _timer; } }

        public int TimeoutCount { get { return MaxTimeoutCount; } }

        public bool IsTimeout { get; private set; }
#endif

#endregion

    }
}
