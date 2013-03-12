﻿using System.Windows;
using Intems.SunPoint.ViewModels;

namespace Intems.SunPoint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SunpointViewModel _model = new SunpointViewModel();
        //TODO: переписать красиво, это ж хак
        private NumKeysControl _keysControl;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _model;
            _numKeys.DataContext = _model;
        }

        private void OnNumKeysChanged(object sender, RoutedEventArgs e)
        {
            var keysCtrl = sender as NumKeysControl;
            if (keysCtrl == null) return;

            _keysControl = keysCtrl;
            var val = ((NumKeysControl) sender).Result;
            _model.SunbathTicks = val*60;
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (SunpointViewModel) DataContext;
            viewModel.StartSolary();
        }

        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (SunpointViewModel) DataContext;
            viewModel.StopSolary();
            //TODO: переписать нафик!!!
            if (_keysControl != null)
                _keysControl.Clear();
        }
    }
}
