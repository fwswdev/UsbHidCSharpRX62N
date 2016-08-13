/*
https://github.com/mikeobrien/HidLibrary/wiki - There is some issues on this.

Credits for the USB HID Lib:
http://www.codeproject.com/Tips/530836/Csharp-USB-HID-Interface - This one works

This is for HID demo for Renesas RX62N

- 7
*/


using System;
using System.Threading;
using UsbHid;
using UsbHid.USB.Classes.Messaging;

namespace USBHid
{
    class Program
    {
        static UsbHidDevice hiddevice;

        static void Main(string[] args)
        {
            hiddevice = new UsbHidDevice(0x045B, 0x2013);
            hiddevice.Connect();

            if (hiddevice != null)
            {
                hiddevice.DataReceived += Hiddevice_DataReceived;
                hiddevice.OnConnected += Hiddevice_OnConnected;
                hiddevice.OnDisConnected += Hiddevice_OnDisConnected;

                Console.WriteLine("Renesas RX62N found, press any key to exit.");

                while (true)
                {
                    var r = Console.ReadKey();
                    Console.WriteLine("");

                    if (r.Key == ConsoleKey.Q)
                    {
                        break;
                    }

                    byte[] b = new byte[17];

                    if (r.Key == ConsoleKey.A)
                    {
                        CommandMessage cmdMsg = new CommandMessage(0x01, b);
                        hiddevice.SendMessage(cmdMsg);
                        Console.WriteLine("Toggle LED");
                    }
                    if (r.Key == ConsoleKey.B)
                    {
                        b[0] = 0x30;
                        b[1] = 0x31;
                        CommandMessage cmdMsg = new CommandMessage(0x04, b);
                        hiddevice.SendMessage(cmdMsg);
                        Console.WriteLine("Update LCD");
                    }
                    if (r.Key == ConsoleKey.C)
                    {
                        CommandMessage cmdMsg = new CommandMessage(0x02, b);
                        hiddevice.SendMessage(cmdMsg);
                        Console.WriteLine("Read ADC");
                    }
                }
                hiddevice.Dispose();
            }
            else
            {
                Console.WriteLine("Could not find device.");
                Console.ReadKey();
            }
        }

        private static void Hiddevice_OnDisConnected()
        {
            Console.WriteLine("Device removed.");
        }

        private static void Hiddevice_OnConnected()
        {
            Console.WriteLine("Device attached.");
        }

        private static void Hiddevice_DataReceived(byte[] data)
        {
            for (int ctr = 0; ctr < data.Length; ctr++)
            {
                Console.Write((int)data[ctr] + ",");
            }
            Console.WriteLine("");
        }
        
    }
}
