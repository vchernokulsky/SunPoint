using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Timers;
using Intems.Devices.Commands;
using Intems.Devices.Interfaces;
using Timer = System.Timers.Timer;

namespace Intems.Devices
{
    public class TransportLayerWorker : ITransportLayerWorker 
    {
        private readonly SerialPort _port;
        private readonly DataRetriever _retriever;

        private bool _isAnswerReceived;
        private bool _isTimeout;
        private readonly Timer _timeoutTimer;

        private readonly object _locker = new object();

        public TransportLayerWorker()
        {}

        public TransportLayerWorker(string portName, int baudRate) 
        {
            _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            _port.DataReceived += OnDataReceived;
            _port.Open();

            _retriever = new DataRetriever();
            _retriever.PackageRetrieved += OnPackageRetrieved;

            _timeoutTimer = new Timer(500);
            _timeoutTimer.Elapsed += OnTimeoutTimerElapsed;
        }

        private void OnTimeoutTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _isTimeout = !_isAnswerReceived;
        }

        public event EventHandler<PackageDataArgs> PackageReceived;

        private readonly Queue<IDeviceResponse> _responses = new Queue<IDeviceResponse>();

        public void SendPackage(Package package, IDeviceResponse deviceResponse)
        {
            if (_port != null && _port.IsOpen)
            {
                try
                {
                    lock (_locker)
                    {
                        _responses.Enqueue(deviceResponse);
                        _port.Write(package.Bytes, 0, package.Bytes.Length);
                        //говорим, что timeout не произошел и ответ не получен
                        _isAnswerReceived = false; _isTimeout = false;
                        _timeoutTimer.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }

        public void SendPackage(Package package)
        {
            if (_port != null && _port.IsOpen)
            {
                try
                {
                    lock (_locker)
                    {
                        _port.Write(package.Bytes, 0, package.Bytes.Length);
                        //говорим, что timeout не произошел и ответ не получен
                        _isAnswerReceived = false; _isTimeout = false;
                        _timeoutTimer.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!_isTimeout)
                {
                    byte[] buffer;
                    lock (_locker)
                    {
                        buffer = new byte[_port.BytesToRead];
                        _port.Read(buffer, 0, buffer.Length);
                        _isAnswerReceived = true;
                    }
                    //отпускаем системный поток
                    var th = new Thread(() => _retriever.AddBytes(buffer)) { Name = "data process thread" };
                    th.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void OnPackageRetrieved(object sender, PackageDataArgs args)
        {
            lock (_locker)
            {
                var resp = _responses.Dequeue();
                resp.PushBytes(args.Data);
            }

//            var handler = PackageReceived;
//            if (handler != null)
//                handler(this, args);
        }

//        public HVChannelSensor SenseHV(int devId) {
//            HVChannelSensor result = null;
//
//            IList<byte> package = new List<byte>();
//            package.Add((byte) devId);
//            package.Add(CmdCodes.GET_DEV_STATE);
//
//            IList<byte> answer = (IList<byte>)SendPackage(devId, package);
//
//            if (answer != null && answer[MasterId] == 0) {
//                byte channelstate = answer[4];
//                bool first = (channelstate & 0x01) == 1;
//                bool second = ((channelstate >> 1) & 0x01) == 1;
//                bool third = ((channelstate >> 2) & 0x01) == 1;
//                result = new HVChannelSensor(first, second, third);
//            }
//            return result;
//        }
    }
}