using System;
using Intems.Devices.Helpers;
using Intems.Devices.Interfaces;

namespace Intems.Devices
{
    public enum SolaryAction
    {
        None,
        Sunbath,
        Ventilation
    }

    public class SolaryDevice : ISolaryDevice
    {
        private SolaryAction _action = SolaryAction.None;
        private int _actionTime;

        private readonly Pair<int, int>[] _actions;

        private readonly int _deviceNumber;
        private readonly ITransportLayerWorker _worker;

        public SolaryDevice(int deviceNumber, ITransportLayerWorker worker)
        {
            _deviceNumber = deviceNumber;
            _worker = worker;

            _actions = new[] {new Pair<int, int>(1, 1), new Pair<int, int>(2, 3), new Pair<int, int>(3, 3)};
        }

        #region ISolaryDevice IMPLEMENTATION 

        public bool IsAvailable()
        {
            bool result = false;
            if (_worker != null)
            {
                result = !String.IsNullOrEmpty(_worker.GetDeviceDescription(_deviceNumber));
            }
            return result;
        }

        public void Start(SolaryAction action, int time)
        {
            var channels = Array.FindAll(_actions, elem => elem.First == (int) action);
            if (_action != action)
            {
                if (_worker != null)
                {
                    foreach (var pair in channels)
                    {
                        _worker.SetChannelState(_deviceNumber, pair.Second, 0, time);
                    }
                    _action = action;
                    _actionTime = time;
                }
            }
        }

        public void Stop(SolaryAction action)
        {
            _worker.ResetDevice(_deviceNumber);

            _action = SolaryAction.None;
            _actionTime = 0;
        }

        public void ResetDevice()
        {
            if (_worker != null)
            {
                _worker.ResetDevice(_deviceNumber);
            }
        }

        #endregion

        public bool IsWorking()
        {
            return _action != SolaryAction.None;
        }

        public string GetActionName()
        {
            string result = "ÍÈ×ÅÃÎ";
            switch (_action)
            {
                case SolaryAction.Sunbath:
                    result = "ÇÀÃÀÐ";
                    break;
                case SolaryAction.Ventilation:
                    result = "ÂÅÍÒÈËßÖÈß";
                    break;
            }
            return result;
        }

        public int ActionDuration
        {
            get { return _actionTime; }
        }

        public Buttons GetPressedButtons()
        {
            return _worker.GetButtonsState(_deviceNumber);
        }

        public int GetDeviceTime()
        {
            var pair = Array.Find(_actions, elem => elem.First == (int) SolaryAction.Sunbath);
            return _worker.GetChannelState(_deviceNumber, pair.Second);
        }
    }
}
