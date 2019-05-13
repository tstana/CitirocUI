using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace CitirocUI
{
    class USB
    {
        [DllImport("Usb2.0.dll")]
        public static extern Int32 USB_GetNumberOfDevs();

        [DllImport("Usb2.0.dll")]
        public static extern Int32 OpenUsbDevice(string sernumstr);

        [DllImport("Usb2.0.dll")]
        public static extern Int32 USB_FindDevices(string deviceDescription);

        [DllImport("Usb2.0.dll")]
        public static extern Int32 USB_GetVersion(Int32 id);

        [DllImport("Usb2.0.dll")]
        public static extern Boolean USB_Init(Int32 id, ref Boolean verbose);

        [DllImport("Usb2.0.dll")]
        public static extern Int32 CloseUsbDevice(Int32 id);

        [DllImport("Usb2.0.dll")]
        public static extern Boolean USB_SetLatencyTimer(Int32 id, UInt32 msecs);

        [DllImport("Usb2.0.dll")]
        unsafe public static extern Boolean USB_GetLatencyTimer(Int32 id, Byte* msecs);

        [DllImport("Usb2.0.dll")]
        public static extern Boolean USB_SetXferSize(Int32 id, UInt32 txsize, UInt32 rxsize);

        [DllImport("Usb2.0.dll")]
        public static extern Boolean USB_SetTimeouts(Int32 id, UInt32 tx_timeout, UInt32 rx_timeout);

        [DllImport("Usb2.0.dll")]
        public static extern Int32 UsbWrt(Int32 id, Byte sub_addr, byte[] buffer, Int32 count);

        [DllImport("Usb2.0.dll")]
        unsafe public static extern Int32 UsbRd(Int32 id, Byte sub_addr, Byte* buffer, Int32 count);

        [DllImport("Usb2.0.dll")]
        public static extern Int32 CloseUsbDevice(UInt32 id);
    }
}
