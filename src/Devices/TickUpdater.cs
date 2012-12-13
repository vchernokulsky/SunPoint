using System;
using System.Timers;
using Intems.Devices.Commands;

namespace Intems.Devices
{
    public class TickUpdater
    {
        private uint _ticks;

        private readonly Timer _timer;
        private readonly TransportLayerWorker _worker;

        public TickUpdater(TransportLayerWorker worker)
        {
            _worker = worker;
            _worker.PackageReceived += OnPackageReceived;

            _timer = new Timer {Interval = 50};
            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnPackageReceived(object sender, PackageDataArgs args)
        {
            uint result = 0;

            var bytes = args.Data;
            result |= bytes[bytes.Length - 4];
            result = ((result) << 8);
            result |= bytes[bytes.Length - 3];

            Console.WriteLine(result);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Stop();
            var pkg = new Package(new GetChannelStateCommand(1));
            _worker.SendPackage(pkg);
            _timer.Start();
        }

        public void Start()
        {
            _timer.Start();
        }
    }
}
