using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothWatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Debug is working.");

            // Initialize Device Picker
            DevicePicker picker = new DevicePicker();
            picker.Filter.SupportedDeviceSelectors.Add(BluetoothLEDevice.GetDeviceSelectorFromPairingState(false));
            picker.Filter.SupportedDeviceSelectors.Add(BluetoothLEDevice.GetDeviceSelectorFromPairingState(true));
            picker.Show(new Rect(0, 0, 200, 200));

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Button Clicked");
            
        }
        

            // I think this is the dispatcher that will send it to Unity

            //var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            //() =>
            //{
            //    /* GPS Data Parsing / UI integration goes here */
            //}
            //);

    }
}
