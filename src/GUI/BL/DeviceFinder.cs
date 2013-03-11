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
        private readonly List<Receiver> _waiters = new List<Receiver>();

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
                waiter.OnDeviceTimeout += OnDeviceTimeout;
                _waiters.Add(waiter);
            }
            return String.Empty;
        }

        private void OnDeviceFound(object sender, EventArgs args)
        {
        }

        private void OnDeviceTimeout(object sender, EventArgs e)
        {
            var obj = sender as Receiver;
            _waiters.Remove(obj);
        }

        private class Receiver
        {
            private readonly Timer _timer;
            private readonly SerialPort _port;
            private readonly object _locker = new object();

            public event EventHandler DeviceFound;
            public event EventHandler OnDeviceTimeout;

            public Receiver(SerialPort port)
            {
                _port = port;
                _port.DataReceived += OnDataReceived;

                _timer = new Timer(250);
                _timer.Elapsed += OnTimerElapsed;
                _timer.Start();
            }

            public override bool Equals(object obj)
            {
                bool isEqual = false;
                var receiver = obj as Receiver;
                if(receiver != null)
                {
                    isEqual = _port.PortName == receiver._port.PortName;
                }
                return isEqual;
            }

            public override int GetHashCode()
            {
                int hash = 0;
                if (_port != null)
                    hash = _port.PortName.GetHashCode();
                return hash;
            }

            private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
            {
                lock (_locker)
                {
                    _timer.Stop();
                    if (_port.IsOpen)
                        _port.Close();

                    var handler = OnDeviceTimeout;
                    if(handler != null)
                        handler(this, EventArgs.Empty);
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
        }
    }
}
