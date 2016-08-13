using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidLibrary;


namespace USBHid
{
    class Program
    {

        static HidDevice _device;

        static HidReport getHidReport()
        {
            return new HidReport(17);
        }

        static void Main(string[] args)
        {
            _device = HidDevices.Enumerate(0x045B, 0x2013).FirstOrDefault();

            if (_device != null)
            {
                _device.OpenDevice();

                _device.Inserted += DeviceAttachedHandler;
                _device.Removed += DeviceRemovedHandler;

                _device.MonitorDeviceEvents = true;
                
                //_device.ReadReport(OnReport,200);

                Console.WriteLine("Reader found, press any key to exit.");

                while (true)
                {
                    var r = Console.ReadKey();
                    Console.WriteLine("");

                    if (r.Key  == ConsoleKey.Q )
                    {
                        break;
                    }
                    if (r.Key == ConsoleKey.A)
                    {
                        var h = getHidReport();
                        h.Data[0] = 0x01;
                        _device.WriteReport(h);
                        Console.WriteLine("Toggle LED");
                    }
                    if(r.Key == ConsoleKey.B)
                    {
                        var h = getHidReport();
                        h.Data[0] = 0x04;
                        h.Data[1] = 0x30;
                        h.Data[2] = 0x31;
                        _device.WriteReport(h);
                        Console.WriteLine("Update LCD");
                    }
                    if (r.Key == ConsoleKey.C)
                    {
                        var h = getHidReport();
                        h.Data[0] = 0x02;
                        _device.WriteReport(h);
                        var readDev = _device.Read();
                        for(int ctr=0;ctr< readDev.Data.Length; ctr++)
                        {
                            Console.Write((int)readDev.Data[ctr] + ",");
                        }

                        Console.WriteLine("Read ADC");
                    }
                }


                _device.CloseDevice();
            }
            else
            {
                Console.WriteLine("Could not find device.");
                Console.ReadKey();
            }
        }

        private static void OnReport(HidReport report)
        {
            if (!_device.IsConnected) { return; }

            Console.WriteLine("Pressed: " + (int)report.Data[0]);

            //_device.ReadReport(OnReport);
        }

        private static void DeviceAttachedHandler()
        {
            Console.WriteLine("Device attached.");
            //_device.ReadReport(OnReport);
        }

        private static void DeviceRemovedHandler()
        {
            Console.WriteLine("Device removed.");
        }


    }
}
