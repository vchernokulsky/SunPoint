using System;
using System.Timers;
using Intems.Devices.Commands;

namespace Intems.Devices
{
    public enum PressedBtn
    {
        None,
        StartBtn,
        StopBtn
    }
    public class BtnPressArgs : EventArgs
    {
        public PressedBtn Button { get; set; }
    }

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

        public event EventHandler<BtnPressArgs> BtnPressed;

        public void RaiseBtnPressed(BtnPressArgs e)
        {
            EventHandler<BtnPressArgs> handler = BtnPressed;
            if (handler != null) handler(this, e);
        }

        private void OnPackageReceived(object sender, PackageDataArgs packageDataArgs)
        {
            PressedBtn result = PressedBtn.None;

            var bytes = packageDataArgs.Data;
            byte stateByte = bytes[3];
            bool start = (stateByte & 0x01) == 1;
            bool stop = ((stateByte >> 1) & 0x01) == 1;

            if(start)
                result = PressedBtn.StartBtn;
            else
                if(stop)
                    result = PressedBtn.StopBtn;

            if (start || stop)
            {
                _timer.Stop();
                RaiseBtnPressed(new BtnPressArgs {Button = result});
            }
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
