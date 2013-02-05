using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Timers;

namespace Intems.SunPoint.BL
{
    public class DeviceFinder
    {
        private const string InfoCmd = "info";

        private readonly Queue<AnswerWaiter> _waiters = new Queue<AnswerWaiter>();

        public string Find()
        {
            string[] ports = SerialPort.GetPortNames();

            foreach (var sp in ports.Select(port => new SerialPort(port)))
            {
                sp.Open();
                sp.Write(Encoding.Default.GetBytes(InfoCmd), 0, InfoCmd.Length);

                var waiter = new AnswerWaiter(sp);
                waiter.DeviceFound += OnDeviceFound;
                _waiters.Enqueue(waiter);
            }
        }

        private void OnDeviceFound(object sender, EventArgs args)
        {}

        private class AnswerWaiter
        {
            private readonly Timer _timer;
            private readonly SerialPort _port;
            private readonly object _locker = new object();

            public AnswerWaiter(SerialPort port)
            {
                _port = port;
                _port.DataReceived += OnDataReceived;

                _timer = new Timer(250);
                _timer.Elapsed += OnTimerElapsed;
                _timer.Start();
            }

            private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
            {
                lock (_locker)
                {
                    if (_port.IsOpen)
                        _port.Close();
                }
            }

            private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                var port = (SerialPort) sender;
                if(port != null)
                {
                    var bytes = new byte[255];
                    int bytesRead = _port.Read(bytes, 0, bytes.Length);
                    var answer = Encoding.ASCII.GetString(bytes).Substring(0, bytesRead);

                    if(answer.Length > 0 && DeviceFound != null)
                        DeviceFound(this, EventArgs.Empty);
                }
            }

            public event EventHandler DeviceFound;
        }
    }
}
