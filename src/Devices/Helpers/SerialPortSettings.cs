namespace UV.Devices.Helpers {
    public class SerialPortSettings {
        public SerialPortSettings() {
            PortName = "COM1";
            BaudRate = 19200;
            RtsUse = false;
        }

        public SerialPortSettings(string portName, int baudRate, bool rtsUse) {
            PortName = portName;
            BaudRate = baudRate;
            RtsUse = rtsUse;
        }

        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public bool RtsUse { get; set; }
    }
}
