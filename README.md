# BluetoothWatcher

This is the UWP application used to troubleshoot Bluetooth device communication across laptop and HoloLens devices.

## Current Limitations

Currently the HoloLens can subscribe to a 'Notify' GattCharacteristic after being provided a device MAC Address.

However, the script is ignorant and selects the last index GattService and last index GattCharacteristic of that service to subscribe to. This works for the current UART device but may need smarter filtering logic to successfully connect in future implementations.
