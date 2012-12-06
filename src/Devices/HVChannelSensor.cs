namespace UV.Devices {
    public class HVChannelSensor {
        public HVChannelSensor(bool firstChannel, bool secondChannel, bool thirdChannel) {
            FirstChannel = firstChannel;
            SecondChannel = secondChannel;
            ThirdChannel = thirdChannel;
        }

        public bool FirstChannel { get; set; }

        public bool SecondChannel { get; set; }

        public bool ThirdChannel { get; set; }
    }
}
