﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Intems.Devices.Commands;

namespace Intems.Devices
{
    public class TicksUpdaterArgs : EventArgs
    {
        public ushort Ticks { get; set; }
    }

    public class TicksUpdater
    {
        private const int TimeInterval = 50;
        private uint _ticks;

        private readonly Timer _timer;
        private readonly TransportLayerWorker _worker;
        private readonly PackageProcessor _packageProcessor;
        private readonly object _locker = new object();

        public event EventHandler<TicksUpdaterArgs> TicksChanged;

        public TicksUpdater(TransportLayerWorker worker)
        {
            _worker = worker;
            _worker.PackageReceived += OnPackageReceived;

            _packageProcessor = new PackageProcessor();

            _timer = new Timer {Interval = TimeInterval};
            _timer.Elapsed += OnTimerElapsed;
        }

        public void Start()
        {
            _timer.Start();
        }

//        public void Stop()
//        {
//            _timer.Stop();
//        }

        private void OnPackageReceived(object sender, PackageDataArgs args)
        {
            var result = _packageProcessor.ProcessBytes(args.Data);
            if (result == null) return;

            if (result.Type==AnswerType.Ok)
            {
                uint ticks = TicksFromBytes(result.Params);
                if (ticks < _ticks)
                {
                    RaiseTicksChanged(new TicksUpdaterArgs {Ticks = (ushort) ticks});
                    if(ticks == 0)
                        _timer.Stop(); //дотикали до конца больше не надо опрашивать
                }
                _ticks = ticks;
            }
            else
            {
                if (result.Type == AnswerType.BadPackage)
                {
                    Debugger.Break();
                }
            }
        }

        private static uint TicksFromBytes(byte[] bytes)
        {
            uint ticks = 0;
            ticks |= bytes[bytes.Length - 2]; //предпоследний
            ticks = ((ticks) << 8);
            ticks |= bytes[bytes.Length - 1]; //последний
            return ticks;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (_locker)
            {
                _timer.Stop();
                var pkg = new Package(new GetChannelStateCommand(1));
                _worker.SendPackage(pkg);
                _timer.Start();
            }
        }

        private void RaiseTicksChanged(TicksUpdaterArgs e)
        {
            EventHandler<TicksUpdaterArgs> handler = TicksChanged;
            if (handler != null) handler(this, e);
        }
    }
}
