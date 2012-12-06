using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using Intems.Devices.Helpers;

namespace Intems.Devices {

    public enum DevResult {OK, TimoutExpired, CrcFailed, PortFailed, Unknown}

    public class DeviceManager : IDisposable {
        private readonly List<byte> _recived = new List<byte>();

        private readonly PackageHelper _packageHelper = new PackageHelper();

        private SerialPort _sp;
        private readonly string _portName;
        private readonly int _baudRate;

        private readonly int _timeout = 250; //Answer timeout

        public DeviceManager(string portName, int baudRate) {
            _portName = portName;
            _baudRate = baudRate;
        }

        private bool InitPort() {
            if (_sp == null) {
                IList<string> ports = new List<string>(SerialPort.GetPortNames());
                if (ports.Contains(_portName)) {
                    _sp = new SerialPort(_portName, _baudRate, Parity.None, 8, StopBits.One);
                    if (!_sp.IsOpen) {
                        try {
                            _sp.Open();
                        }
                        catch (IOException ioex) {
                            string msg = String.Format("Mesage: {0}\nStack: {1}", ioex.Message, ioex);
                            Debug.WriteLine(msg);
                            _sp = null;
                        }
                    }
                }
            }
            return _sp != null;
        }

        public DevResult SendCommand(IList<byte> cmd, out object answer) {
            answer = null;
            var result = DevResult.OK;

            byte[] pack = _packageHelper.PackToPackage(new List<byte>(cmd)).ToArray();
            if ( InitPort() ) {
                _recived.Clear();
                lock (_sp) {
                    _sp.DiscardInBuffer();
                    _sp.Write(pack, 0, pack.Length);
                }

                AutoResetEvent autoEvent;
                Thread thread = StartWaitingThread(out autoEvent);

                if (autoEvent.WaitOne(_timeout, false)) {
                    byte[] buffer = new byte[_recived.Count - 2];
                    if (IsPackageCorrect(buffer))
                        answer = buffer;
                    else
                        result = DevResult.CrcFailed;
                }
                else {
                    thread.Abort();
                    result = DevResult.TimoutExpired;
                }
            }
            return result;
        }

        private bool IsPackageCorrect(byte[] buffer) {
            _recived.CopyTo(0, buffer, 0, _recived.Count - 2);
            ushort crc = _packageHelper.CalcCRC(0, buffer);
            ushort packageCrc = _packageHelper.CombineToUshort(new List<byte>(_recived));
            return crc == packageCrc;
        }

        private Thread StartWaitingThread(out AutoResetEvent autoEvent) {
            autoEvent = new AutoResetEvent(false);
            var thread = new Thread(WaitAnswer) {Name = "Thread: wait solary board answer"};
            thread.Start(autoEvent);
            return thread;
        }

        private void WaitAnswer(object state) {
            while (_sp.BytesToRead == 0) {
                Debug.WriteLine("No data in COM-port");
                Thread.Sleep(50);
            }

            bool esc = false;
            byte symbol = 0x00;
            while (symbol != 0x7f) {
                symbol = (byte) _sp.ReadByte();
                if ((symbol == 0x7e) || (symbol == 0x7f)) {
                }
                else if (symbol == 0x7d) {
                    esc = true;
                }
                else {
                    if (esc) {
                        esc = false;
                        symbol ^= 0x20;
                    }
                    _recived.Add(symbol);
                }
            }
            ((AutoResetEvent) state).Set();
        }

        public void Dispose() {
            if (_sp != null) {
                _sp.Close();
                _sp = null;
            }
        }
    }
}