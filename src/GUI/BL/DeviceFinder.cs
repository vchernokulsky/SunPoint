using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Intems.SunPoint.BL
{
    public class DevFindArgs : EventArgs
    {
        public DevFindArgs(string[] devPortNames)
        {
            DevPortNames = devPortNames;
        }

        public string[] DevPortNames { get; private set; }
    }

    public class DeviceFinder
    {
        private readonly object _locker = new object();
        private readonly List<Receiver> _waiters = new List<Receiver>();
        private readonly IList<string> _portNames = new List<string>();
        private readonly byte[] _infoCmd = new byte[] { 0x7e, 0x01, 0x00, 0x01, 0x00, 0x7f };

        public event EventHandler<DevFindArgs> FindComplete;

        public void Find()
        {
            _portNames.Clear();
            _waiters.Clear();

            string[] ports = SerialPort.GetPortNames();
            foreach (var sp in ports.Select(port => new SerialPort(port, 19200, Parity.None, 8, StopBits.One)))
            {
                var waiter = new Receiver(sp);
                waiter.DeviceFound += OnDeviceFound;
                waiter.OnDeviceTimeout += OnDeviceTimeout;
                _waiters.Add(waiter);

                sp.Open();
                sp.Write(_infoCmd, 0, _infoCmd.Length);
            }
        }

        private void OnDeviceFound(object sender, EventArgs args)
        {
            lock (_locker)
            {
                var obj = sender as Receiver;
                if(obj != null && !String.IsNullOrEmpty(obj.PortName))
                {
                    _portNames.Add(obj.PortName);
                    _waiters.Remove(obj);
                    if(_waiters.Count == 0)
                        RaiseFindComplete(_portNames.ToArray());
                }
            }
        }

        private void OnDeviceTimeout(object sender, EventArgs e)
        {
            lock (_locker)
            {
                var obj = sender as Receiver;
                _waiters.Remove(obj);
                if(_waiters.Count == 0)
                    RaiseFindComplete(_portNames.ToArray());
            }
        }

        private void RaiseFindComplete(string[] portNames)
        {
            var handler = FindComplete;
            if (handler != null) handler(this, new DevFindArgs(portNames));
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

                _timer = new Timer(500);
                _timer.Elapsed += OnTimerElapsed;
                _timer.Start();
            }

            public string PortName
            {
                get
                {
                    string portName = String.Empty;
                    if (_port != null)
                        portName = _port.PortName;
                    return portName;
                }
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

                    RaiseDeviceTimeout();
                }
            }

            private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                var port = (SerialPort) sender;
                if(port != null)
                {
                    string answer = String.Empty;

                    lock (_locker)
                    {
                        var bytes = new byte[255];
                        if (_port.IsOpen)
                        {
                            int bytesRead = _port.Read(bytes, 0, bytes.Length);
                            answer = Encoding.ASCII.GetString(bytes).Substring(0, bytesRead);
                            if (!String.IsNullOrEmpty(answer))
                                _timer.Stop();
                        }
                    }
                    if (answer.Length > 0)
                        RaiseDeviceFound();
                }
            }

            private void RaiseDeviceFound()
            {
                //надо отпустить системный поток
                var th = new Thread(() =>
                                        {
                                            var handler = DeviceFound;
                                            if (handler != null)
                                                handler(this, EventArgs.Empty);
                                        });
                th.Start();
            }

            private void RaiseDeviceTimeout()
            {
                var handler = OnDeviceTimeout;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }

        }
    }
}
