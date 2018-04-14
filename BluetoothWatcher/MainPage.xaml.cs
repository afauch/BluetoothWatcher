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
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
// This service was created based on this tutorial https://blogs.msdn.microsoft.com/cdndevs/2017/04/28/uwp-working-with-bluetooth-devices-part-1/

// UART BLUETOOTH ADDRESS
// 264940970245342

namespace BluetoothWatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BluetoothLEDevice leDevice;
        ulong bluetoothAddress = 264940970245342;

        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Debug is working.");
            
        }

        // This can be used as a utility if necessary, otherwise delete
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Button Clicked");
            OnDeviceSelected(null, null);

        }

        // Method for when the user selects the UART device from the UI 
        private async void OnDeviceSelected(DevicePicker sender, DeviceSelectedEventArgs args)
        {

            // Next try to find the right service to connect to

            leDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress);

            Debug.WriteLine("============");
            Debug.WriteLine("============");
            Debug.WriteLine("============");

            Debug.WriteLine("Made it past BluetoothLEDevice");
            Debug.WriteLine("Name is " + leDevice.Name);
            Debug.WriteLine("ConnectionStatus is " + leDevice.ConnectionStatus);
            Debug.WriteLine("DeviceId is " + leDevice.DeviceId);
            Debug.WriteLine("Number of GattServices are " + leDevice.GattServices.Count);

            Debug.WriteLine("=GATT SERVICES=");
            GattDeviceService selectedService = null;
            foreach (GattDeviceService gds in leDevice.GattServices)
            {
                Debug.WriteLine("GattDeviceService.uuid: " + gds.Uuid);
                selectedService = gds;
            }

            Debug.WriteLine("============");
            Debug.WriteLine("============");
            Debug.WriteLine("============");

            Debug.WriteLine("=GATT CHARACTERISTICS FOR " + selectedService.Uuid + " =");

            var allCharacteristics = selectedService.GetAllCharacteristics();
            GattCharacteristic selectedCharacteristic = null;
            foreach(GattCharacteristic gc in allCharacteristics)
            {
                Debug.WriteLine("GattCharacteristic.Uuid: " + gc.Uuid);
                Debug.WriteLine("GattCharacteristic.UserDescriptions: " + gc.UserDescription);
                Debug.WriteLine("GattCharacteristic.AttributeHandle: " + gc.AttributeHandle);
                Debug.WriteLine("GattCharacteristic.CharacteristicProperties: " + gc.CharacteristicProperties);
                Debug.WriteLine("***");

                selectedCharacteristic = gc;
            }

            Debug.WriteLine("Selected Characteristic: " + selectedCharacteristic.Uuid);

            // There are two things to take care of before getting notifications:
            // 1. Write to Client Characteristic Configuration Description (CCCD)

            GattCommunicationStatus status = await selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            if(status == GattCommunicationStatus.Success)
            {
                Debug.WriteLine("Successfully wrote to CCCD");
                selectedCharacteristic.ValueChanged += Characteristic_ValueChanged;
            }
            else
            {
                Debug.WriteLine("Unsuccessful writing to CCCD");
            }

            // 2. Handle the Characteristic.valueChanged event



            // MAYBE OBSOLETE: InitializeRingSensor(selectedService);

        }

        // METHOD TO CALL ON BLUETOOTH VALUE CHANGE
        void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            string output = reader.ReadString(args.CharacteristicValue.Length);
            Debug.WriteLine(output);
        }

    }
}
