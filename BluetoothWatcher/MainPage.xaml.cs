﻿using System;
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
        List<DeviceInformation> filteredDevices = new List<DeviceInformation>();
        HashSet<BluetoothLEDevice> possibleLeDevices = new HashSet<BluetoothLEDevice>();
        HashSet<String> possibleDeviceIds = new HashSet<String>();
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
            filteredDevices = await enumerateSnapshot();
            Boolean foundDevice = false;
            // Debug.WriteLine("On Device Selected Called");

            // assign device to main variable
            // device = args.SelectedDevice;

            // string id = device.Id;
            // Debug.WriteLine("Selected " + id);
            // Debug.WriteLine(device.Properties.Values);

            // Assign the BluetoothLEDevice object
            Debug.WriteLine("About to loop through.");
            if (filteredDevices.Count > 0)
            {
                foreach (DeviceInformation d in filteredDevices)
                {
                    try
                    {
                        Debug.WriteLine("Searching for BLE device for " + d.Id);
                        BluetoothLEDevice l = await BluetoothLEDevice.FromIdAsync(d.Id);
                        Debug.WriteLine("Found Bluetooth Device: " + l.DeviceId);
                        if (!possibleDeviceIds.Contains(l.DeviceId))
                            possibleLeDevices.Add(l);
                        possibleDeviceIds.Add(l.DeviceId);
                        String leID = "f0:f6:60:6a:fc:de";
                        if (l.DeviceId.Contains(leID) || l.DeviceId.Contains("f0f6606afcde"))
                        {
                            Debug.WriteLine("yes, we have found our one true device");
                            leDevice = l;
                            foundDevice = true;
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("No BluetoothLEDevice Found for DeviceInformation " + d.Id);
                    }
                }
            } else
            {
                Debug.WriteLine("No DeviceInformation objects meet criteria.");
            }
            Debug.WriteLine("Size of set: " + possibleLeDevices.Count);
            Debug.WriteLine("Found Device? " + foundDevice);
            if (foundDevice == false) {
                foreach (BluetoothLEDevice x in possibleLeDevices)
                {
                    Debug.WriteLine("The device is " + x.DeviceId);
                }

            }
            // Now we're going to pick THE ONE: f0:f6:60:6a:fc:de




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

        async Task<List<DeviceInformation>> enumerateSnapshot()
        {
            // select only paired bluetooth devices

            List<DeviceInformation> returnedDevices = new List<DeviceInformation>();
            
            Debug.WriteLine("enumerateSnapshot called");
            DeviceInformationCollection collection = await DeviceInformation.FindAllAsync();
            Debug.WriteLine("number of devices in collection: " + collection.Count);
            if (collection.Count > 0)
            {
                for (int i=0; i < collection.Count; i++)
                {
                    try
                    {
                        Debug.WriteLine(i + " Looping through device " + collection[i].Id);
                        if (collection[i].Pairing.IsPaired == true)
                        {
                            returnedDevices.Add(collection[i]);
                        }
                    } catch(System.Exception e)
                    {
                        Debug.WriteLine("Null reference: " + e.Message);
                    }
                }
            }
            return returnedDevices;
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
