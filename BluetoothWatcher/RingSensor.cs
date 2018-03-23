﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothWatcher
{
    public class RingSensor: SensorBase
    {

        public RingSensor(GattDeviceService dataService) : base (dataService, "6e400003-b5a3-f393-e0a9-e50e24dcca9e")
        {
            Debug.WriteLine("RingSensor Called");

        }

        public async Task<string> GetValue()
        {
            Debug.WriteLine("GetValue Called from RingSensor");

            byte[] data = await ReadValue();
            string dataValue = ((string)BitConverter.ToString(data, 0));
            Debug.WriteLine(dataValue);
            return dataValue;
        }

    }
}
