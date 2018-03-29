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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
// This service was created based on this tutorial https://blogs.msdn.microsoft.com/cdndevs/2017/04/28/uwp-working-with-bluetooth-devices-part-1/

namespace BluetoothWatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        DeviceInformation device;
        BluetoothLEDevice leDevice;
        RingSensor ringSensor;



        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Debug is working.");

            // Initialize Device Picker
            //DevicePicker picker = new DevicePicker();
            //picker.Filter.SupportedDeviceSelectors.Add(BluetoothLEDevice.GetDeviceSelectorFromPairingState(true));
            //picker.Show(new Rect(0, 0, 200, 200));

            //picker.DeviceSelected += OnDeviceSelected;

        }

        // Method for when the user selects the UART device from the UI 
        private async void OnDeviceSelected(DevicePicker sender, DeviceSelectedEventArgs args)
        {

            // Try to just get the list of available devices
            enumerateSnapshot();

            // Debug.WriteLine("On Device Selected Called");

            // assign device to main variable
            // device = args.SelectedDevice;

            // string id = device.Id;
            // Debug.WriteLine("Selected " + id);
            // Debug.WriteLine(device.Properties.Values);

            // Assign the BluetoothLEDevice object
            leDevice = await BluetoothLEDevice.FromIdAsync("BluetoothLE#BluetoothLE9c:b6:d0:61:2a:c2-f0:f6:60:6a:fc:de");
            Debug.WriteLine(leDevice.DeviceId);

            var services = await leDevice.GetGattServicesAsync();
            GattDeviceService selectedService = null;
            foreach (var service in services.Services)
            {
                // TODO: Fix this - right now it's randomly going through and assigning services
                // You'll want to actually filter here
                Debug.WriteLine("Found a service: " + service.Uuid);
                selectedService = service;
            }

            InitializeRingSensor(selectedService);

        }

        async void enumerateSnapshot()
        {
            // select only paired bluetooth devices
            
            Debug.WriteLine("enumerateSnapshot called");
            DeviceInformationCollection collection = await DeviceInformation.FindAllAsync();
            Debug.WriteLine("number of devices in collection: " + collection.Count);
            List<DeviceInformation> filteredDevices = new List<DeviceInformation>();
            foreach(DeviceInformation d in collection)
            {
                if(d.Pairing.IsPaired == true)
                {
                    Debug.WriteLine("Pairing status is " + d.Pairing.IsPaired);
                    Debug.WriteLine("Found ID: " + d.Id);
                    foreach(var item in d.Properties)
                    {
                        Debug.WriteLine("Key: " + item.Key);
                        Debug.WriteLine("Value: " + item.Value);
                    }
                    filteredDevices.Add(d);
                }
            }
        }



        // This can be used as a utility if necessary, otherwise delete
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Button Clicked");
            OnDeviceSelected(null, null);

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            Debug.WriteLine("OnNavigatedTo Called.");
        }

        protected async void InitializeRingSensor(GattDeviceService service)
        {
            Debug.WriteLine("InitializeRingSensor Called");

            ringSensor = new RingSensor(service);
            await ringSensor.EnableNotifications();
        }

        // I think this is the dispatcher that will send messages to Unity
        // from the original GPS tutorial

        //var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        //() =>
        //{
        //    /* GPS Data Parsing / UI integration goes here */
        //}
        //);

    }
}
