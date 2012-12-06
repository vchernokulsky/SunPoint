using System;
using System.Collections.Generic;
using System.IO.Ports;
using Intems.Devices.Helpers;
using Intems.Devices.Interfaces;
using UV.Devices;

namespace Intems.Devices
{
    public class TransportLayerWorker : ITransportLayerWorker {
        private DeviceManager _deviceManager;
        private readonly PackageHelper _packageHelper = new PackageHelper();
        private readonly ErrorHandler _errorHandler = new ErrorHandler();

        private const int MasterId = 0;
        private int _timeout = 50; // Device answer timeout

        private readonly string _portName;
        private readonly int _baudRate;

        private readonly SerialPort _port;
        public TransportLayerWorker(string portName, int baudRate) 
        {
            _portName = portName;
            _baudRate = baudRate;

            _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            _port.DataReceived += OnDataReceived;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
        }

        public void SendPackage(Package package)
        {
            if(_port != null && _port.IsOpen)
            {
                _port.Write(package.Bytes, 0, package.Bytes.Length);
            }
        }

        private object SendPackage(int devId, IList<byte> package)
        {
            DevResult result = DevResult.Unknown;

            object reciever = null;
            while (result != DevResult.OK)
            {
                result = _deviceManager.SendCommand(package, out reciever);
                if (result != DevResult.OK)
                    _errorHandler.SetError(devId, new ErrorEnvironment(result, package));
            }
            _errorHandler.ClearError(devId);
            return reciever;
        }

        public void InitDevice() {
            if (_deviceManager == null) {
                _deviceManager = new DeviceManager(_portName, _baudRate);
            }
        }

        public string GetDeviceDescription(int devId) {
            IList<byte> package = new List<byte>();
            package.Add((byte) devId);
            package.Add(DeviceHelper.GET_DEV_DESCRIPTION);

            var answer = (IList<byte>) SendPackage(devId, package);

            string result = String.Empty;
            if (answer != null && answer[MasterId] == 0) {
                for (int i = 2; i < answer.Count; i++)
                    result += (char) answer[i];
            }
            return result;
        }

        public void ResetDevice(int devId) {
            IList<byte> package = new List<byte>();
            package.Add((byte) devId);
            package.Add(DeviceHelper.DEV_RESET);

            SendPackage(devId, package);
        }

        public int GetChannelState(int devId, int channel) {
            int result = 0;

            IList<byte> package = new List<byte>();
            package.Add((byte) devId);
            package.Add(DeviceHelper.GET_CHANNEL_STATE);
            package.Add((byte) channel);

            IList<byte> answer = (IList<byte>) SendPackage(devId, package);
            if (answer != null) {
                result |= answer[answer.Count - 2];
                result = ((result) << 8);
                result |= answer[answer.Count - 1];
            }
            return result;
        }

        public void SetChannelState(int devId, int channel, int delay, int time) {
            var package = new List<byte>
                              {
                                  (byte) devId, DeviceHelper.SET_CHANNEL_STATE, (byte) channel
                              };

            byte[] convertedDelay = _packageHelper.SplitToArray((ushort) delay);
            package.AddRange(convertedDelay);
            byte[] convertedTime = _packageHelper.SplitToArray((ushort) time);
            package.AddRange(convertedTime);

            SendPackage(devId, package);
        }

        public Buttons GetButtonsState(int devId) {
            Buttons result = null;
            IList<byte> package = new List<byte>();
            package.Add((byte) devId);
            package.Add(DeviceHelper.GET_DEV_STATE);

            IList<byte> answer = (IList<byte>)SendPackage(devId, package);

            if (answer != null && answer[MasterId] == 0) {
                byte buttons = answer[3];
                bool start = (buttons & 0x01) == 1;
                bool stop = ((buttons >> 1) & 0x01) == 1;
                result = new Buttons(start, stop);
            }
            return result;
        }

        public HVChannelSensor SenseHV(int devId) {
            HVChannelSensor result = null;

            IList<byte> package = new List<byte>();
            package.Add((byte) devId);
            package.Add(DeviceHelper.GET_DEV_STATE);

            IList<byte> answer = (IList<byte>)SendPackage(devId, package);

            if (answer != null && answer[MasterId] == 0) {
                byte channelstate = answer[4];
                bool first = (channelstate & 0x01) == 1;
                bool second = ((channelstate >> 1) & 0x01) == 1;
                bool third = ((channelstate >> 2) & 0x01) == 1;
                result = new HVChannelSensor(first, second, third);
            }
            return result;
        }

        public int TimeOut {
            get { return _timeout; }
            set {
                if ((value >= 10) && (value <= 1000)) {
                    _timeout = value;
                }
            }
        }

        public void Dispose() {
            if(_deviceManager != null) {
                _deviceManager.Dispose();
            }
        }
    }
}