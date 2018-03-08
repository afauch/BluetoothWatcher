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
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothWatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BluetoothLEAdvertisementWatcher watcher;
        // public static ushort BEACON_ID = 1775;

        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Debug is working.");

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Button Clicked");

            if (watcher != null)
                watcher.Stop();
            
            watcher = new BluetoothLEAdvertisementWatcher();

            // This is how we might filter the incoming Bluetooth
            // Right now, we don't want to filter

            //var manufacturerData = new BluetoothLEManufacturerData
            //{
            //    CompanyId = BEACON_ID
            //};
            // watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);

            watcher.Received += Watcher_Received;
            watcher.Start();
        }

        private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            try {

                IList<Guid> uuids = args.Advertisement.ServiceUuids;
                // ushort identifier = args.Advertisement.ManufacturerData.First().CompanyId;
                // byte[] data = args.Advertisement.ManufacturerData.First().Data.ToArray();

                int count = uuids.Count;

                Debug.WriteLine(" GOT DATA");

            } catch
            {
                Debug.WriteLine("Exception!");
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
}
