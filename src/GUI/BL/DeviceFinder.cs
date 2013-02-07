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
        private readonly Queue<Receiver> _waiters = new Queue<Receiver>();

        public string Find()
        {
            string[] ports = SerialPort.GetPortNames();

            var cmd = new byte[] { 0x7e, 0x01, 0x00, 0x01, 0x00, 0x7f };
            foreach (var sp in ports.Select(port => new SerialPort(port, 19200, Parity.None, 8, StopBits.One)))
            {
                sp.Open();
                sp.Write(cmd, 0, cmd.Length);

                var waiter = new Receiver(sp);
                waiter.DeviceFound += OnDeviceFound;
                _waiters.Enqueue(waiter);
            }
            return String.Empty;
        }

        private void OnDeviceFound(object sender, EventArgs args)
        {}

        private class Receiver
        {
            private readonly Timer _timer;
            private readonly SerialPort _port;
            private readonly object _locker = new object();

            public Receiver(SerialPort port)
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
