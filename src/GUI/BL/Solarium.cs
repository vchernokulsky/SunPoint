using System;
using System.Timers;
using Intems.Devices;
using Intems.Devices.Interfaces;

namespace Intems.SunPoint.BL
{
    class Solarium
    {
        private const int DevNumber = 1;
        private readonly SolaryDevice _solary;

        public event EventHandler SunbathStarted;
        public event EventHandler SunbathStopped;
        public event EventHandler TickChanged;

        private readonly Timer _timerStub;
        public Solarium()
        {
            ITransportLayerWorker worker = new TransportLayerWorker("COM5", 9600);
            worker.InitDevice();

            _solary = new SolaryDevice(DevNumber, worker);
            _timerStub = new Timer(1000);
            _timerStub.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Time--;
            if (Time > 0)
            {
                RaiseTickChanged(EventArgs.Empty);
            }
            else
            {
                //_solary.Stop(SolaryAction.Sunbath);
                _timerStub.Stop();
                RaiseSunbathStopped(EventArgs.Empty);
            }

        }

        public void Start()
        {
            //_solary.Start(SolaryAction.Sunbath, _time);
            _timerStub.Start();
            RaiseSunbathStarted(EventArgs.Empty);
        }

        public void Stop()
        {
            //_solary.Stop(SolaryAction.Sunbath);
            _timerStub.Stop();
            RaiseSunbathStopped(EventArgs.Empty);
        }

        public int Time { get; set; }

        public void RaiseSunbathStarted(EventArgs e)
        {
            EventHandler handler = SunbathStarted;
            if (handler != null) handler(this, e);
        }

        public void RaiseSunbathStopped(EventArgs e)
        {
            EventHandler handler = SunbathStopped;
            if (handler != null) handler(this, e);
        }

        public void RaiseTickChanged(EventArgs e)
        {
            EventHandler handler = TickChanged;
            if (handler != null) handler(this, e);
        }
    }
}
