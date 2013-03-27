using System;
using System.Diagnostics;
using System.Timers;
using Intems.Devices.Commands;
using Intems.Devices.Interfaces;

namespace Intems.Devices.DevicePollers
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

    public class ButtonPressPoller : IDeviceResponse
    {
        private readonly ITransportLayerWorker _worker;

        private readonly Timer _timer;
        private readonly PackageProcessor _packageProcessor;

        public ButtonPressPoller(ITransportLayerWorker worker)
        {
            _worker = worker;

            _packageProcessor = new PackageProcessor();

            _timer = new Timer(250);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        public event EventHandler<BtnPressArgs> BtnPressed;

        public void PushBytes(byte[] bytes)
        {
            var result = _packageProcessor.ProcessBytes(bytes);
            if(result.Type == AnswerType.Ok)
            {
                if(result.Function == CmdCodes.GET_DEV_STATE)
                {
                    var stateByte = result.Params[1];
                    bool start = (stateByte & 0x01) == 1;
                    bool stop =  ((stateByte >> 1) & 0x01) == 1;
                    if (start || stop)
                    {
                        var btn = start ? PressedBtn.StartBtn : PressedBtn.StopBtn;
                        RaiseBtnPressed(new BtnPressArgs {Button = btn});
                    }
                }
            }
            else
            {
                Debugger.Break();
            }
        }

        public void PushTimeout()
        {
        }

        private void RaiseBtnPressed(BtnPressArgs e)
        {
            EventHandler<BtnPressArgs> handler = BtnPressed;
            if (handler != null) handler(this, e);
#if DEBUG
            Button = e.Button;
#endif
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Stop();

            Command cmd = new GetBtnStateCommand(1);
            var package = new Package(cmd);
            _worker.SendPackage(package, this);

            _timer.Start();
        }

#if DEBUG

        public PressedBtn Button { get; private set; }

#endif
    }
}
