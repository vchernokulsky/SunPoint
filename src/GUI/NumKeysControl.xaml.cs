using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Intems.SunPoint
{
    /// <summary>
    /// Interaction logic for NumKeysControl.xaml
    /// </summary>
    public partial class NumKeysControl : UserControl
    {
        private string _pressedKeys;
        private int _result;

        public NumKeysControl()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent ChangedEvent = EventManager.RegisterRoutedEvent(
                               "ChangedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumKeysControl));
        public event RoutedEventHandler Changed
        {
            add { AddHandler(ChangedEvent, value); }
            remove { RemoveHandler(ChangedEvent, value); }
        }
        public void RaiseChangedEvent()
        {
            var newEventArgs = new RoutedEventArgs(NumKeysControl.ChangedEvent);
            RaiseEvent(newEventArgs);
        }

        public int Result
        {
            get { return _result; }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            string tag = button.Tag.ToString();
            _pressedKeys += tag;

            try
            {
                var number = Int32.Parse(_pressedKeys);
                if(number <= 99)
                    _result = Int32.Parse(_pressedKeys);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            RaiseChangedEvent();
        }

        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {
            _pressedKeys = String.Empty;
            _result = 0;
            RaiseChangedEvent();
        }
    }
}
