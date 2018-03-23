using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothWatcher
{
    public class SensorBase : IDisposable
    {
        protected GattDeviceService deviceService;
        protected string sensorDataUuid;
        protected byte[] data;
        protected bool isNotificationSupported = false;
        private GattCharacteristic dataCharacteristic;

        public SensorBase(GattDeviceService dataService, string sensorDataUuid)
        {
            Debug.WriteLine("SensorBase Initialized");
            this.deviceService = dataService;
            this.sensorDataUuid = sensorDataUuid;
        }

        public virtual async Task EnableNotifications()
        {
            Debug.WriteLine("EnableNotifications Called");
            isNotificationSupported = true;

            Debug.WriteLine("Attempting to enable notifications for " + sensorDataUuid);

            dataCharacteristic = (await deviceService.GetCharacteristicsForUuidAsync(
                new Guid(sensorDataUuid))).Characteristics[0];
            dataCharacteristic.ValueChanged += dataCharacteristic_ValueChanged;
            GattCommunicationStatus status =
                await dataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);
        }

        public virtual async Task DisableNotifications()
        {
            Debug.WriteLine("DisableNotifications Called");
            isNotificationSupported = false;
            dataCharacteristic = (await deviceService.GetCharacteristicsForUuidAsync(
                new Guid(sensorDataUuid))).Characteristics[0];
            dataCharacteristic.ValueChanged -= dataCharacteristic_ValueChanged;
            GattCommunicationStatus status =
                await dataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                GattClientCharacteristicConfigurationDescriptorValue.None);
        }

        protected async Task<byte[]> ReadValue()
        {
            Debug.WriteLine("ReadValue Called");
            if (!isNotificationSupported)
            {
                if (dataCharacteristic == null)
                    dataCharacteristic = (await deviceService.GetCharacteristicsForUuidAsync(
                        new Guid(sensorDataUuid))).Characteristics[0];
                GattReadResult readResult =
                    await dataCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                data = new byte[readResult.Value.Length];
                DataReader.FromBuffer(readResult.Value).ReadBytes(data);
            }
            return data;
        }

        private void dataCharacteristic_ValueChanged(GattCharacteristic sender,
            GattValueChangedEventArgs args)
        {
            // Debug.WriteLine("dataCharacteristic_ValueChanged Called");
            data = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);
            // string dataValue = ((string)BitConverter.ToString(data));
            // Debug.WriteLine(dataValue);
            string dataASCII = System.Text.Encoding.ASCII.GetString(data);
            Debug.WriteLine(dataASCII);



        }

        public async void Dispose()
        {
            await DisableNotifications();
        }

    }
}
