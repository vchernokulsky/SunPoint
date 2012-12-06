namespace Intems.Devices {
    public class Buttons {
        private bool _startButton;
        private bool _stopButton;


        public Buttons(bool startButton, bool stopButton) {
            _startButton = startButton;
            _stopButton = stopButton;
        }

        public bool StartButton {
            get { return _startButton; }
            set { _startButton = value; }
        }

        public bool StopButton {
            get { return _stopButton; }
            set { _stopButton = value; }
        }
    }
}
