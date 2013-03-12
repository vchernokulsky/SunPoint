using System;
using Intems.SunPoint.BL;

namespace Intems.SunPoint.ViewModels
{
    class SunpointViewModel : BaseViewModel
    {
        private readonly Solarium _solarium;

        public SunpointViewModel()
        {
            _solarium = new Solarium();
            _solarium.TickChanged += OnTickChanged;
            _solarium.SunbathStopped += OnSunbathStopped;
        }

        public void StartSolary()
        {
            _solarium.Time = SunbathTicks;
            _solarium.Start(SunbathTicks);
            IsStarted = true;
        }

        public void StopSolary()
        {
            SunbathTicks = 0;
            _solarium.Stop();
            IsStarted = false;
        }

        private void OnSunbathStopped(object sender, EventArgs e)
        {
            SunbathTicks--;
            IsStarted = false;
        }

        private void OnTickChanged(object sender, EventArgs eventArgs)
        {
            SunbathTicks--;
        }

        #region BINDING PROPERTIES

        private int _sunbathTicks;
        public int SunbathTicks
        {
            get { return _sunbathTicks; }
            set { _sunbathTicks = value; base.RaisePropertyChanged("SunbathTicks"); }
        }

        private bool _isStarted;
        public bool IsStarted
        {
            get { return _isStarted; }
            set
            {
                _isStarted = value;
                base.RaisePropertyChanged("IsStarted");
                base.RaisePropertyChanged("IsStopped");
            }
        }

        public bool IsStopped
        {
            get { return !_isStarted; }
        }

        #endregion
    }
}
