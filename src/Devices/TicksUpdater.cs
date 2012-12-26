using System;
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

        public event EventHandler<TicksUpdaterArgs> TicksChanged;

        public TicksUpdater(TransportLayerWorker worker)
        {
            _worker = worker;
            _worker.PackageReceived += OnPackageReceived;

            _timer = new Timer {Interval = TimeInterval};
            _timer.Elapsed += OnTimerElapsed;
        }

        public void Start()
        {
            _timer.Start();
        }

        private void OnPackageReceived(object sender, PackageDataArgs args)
        {
            //TODO: надо решить вопрос обработки ответов
            if(args.Data[1]!=0x03) return;

            uint ticks = 0;
            ticks = TicksFromPackage(args, ticks);
            if (_ticks == 0 && ticks > 0)
            {
                _ticks = ticks;
                RaiseTicksChanged(new TicksUpdaterArgs { Ticks = (ushort)ticks });
            }
            if(ticks < _ticks)
            {
                if (ticks == 0)
                    _timer.Stop();

                _ticks = ticks;
                RaiseTicksChanged(new TicksUpdaterArgs {Ticks = (ushort) ticks});
            }
        }

        private static uint TicksFromPackage(PackageDataArgs args, uint ticks)
        {
            var bytes = args.Data;
            ticks |= bytes[bytes.Length - 4];
            ticks = ((ticks) << 8);
            ticks |= bytes[bytes.Length - 3];
            return ticks;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Stop();
            var pkg = new Package(new GetChannelStateCommand(1));
            _worker.SendPackage(pkg);
            _timer.Start();
        }

        private void RaiseTicksChanged(TicksUpdaterArgs e)
        {
            EventHandler<TicksUpdaterArgs> handler = TicksChanged;
            if (handler != null) handler(this, e);
        }
    }
}
