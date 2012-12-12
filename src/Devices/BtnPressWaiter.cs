using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Intems.Devices.Commands;

namespace Intems.Devices
{
    public class BtnPressWaiter
    {
        private readonly Timer _timer;
        private readonly TransportLayerWorker _worker;

        public BtnPressWaiter(TransportLayerWorker worker)
        {
            _worker = worker;
            _worker.PackageReceived += OnPackageReceived;

            _timer = new Timer(100);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private void OnPackageReceived(object sender, PackageDataArgs packageDataArgs)
        {
            
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Stop();

            Command cmd = new GetBtnStateCommand(1);
            var package = new Package(cmd);
            _worker.SendPackage(package);

            _timer.Start();
        }
    }
}
