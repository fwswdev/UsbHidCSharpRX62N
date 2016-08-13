/*
https://github.com/mikeobrien/HidLibrary/wiki

- 7
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidLibrary;
using System.Threading;

namespace USBHid
{
    class Program
    {

        static HidDevice _device;

        static HidReport getHidReport()
        {
            return new HidReport(17);
        }

        static async void ReadReport()
        {
            var dev = await _device.ReadAsync(0);
        }

        static readonly object lockobj = new object();

        static void ThreadReadReport()
        {
            while(true)
            {
                lock(lockobj)
                {
                    var d = _device.ReadReport(200);
                    if(d.ReadStatus == HidDeviceData.ReadStatus.Success && _device.IsConnected)
                    {
                        //if (!_device.IsConnected) { return; }
                        Console.WriteLine("Pressed: " + (int)d.Data[1]);

                    }
                }
                Thread.Sleep(50);
            }
        }

        static void Main(string[] args)
        {
            _device = HidDevices.Enumerate(0x045B, 0x2013).FirstOrDefault();

            if (_device != null)
            {
                _device.OpenDevice(DeviceMode.NonOverlapped, DeviceMode.NonOverlapped, ShareMode.ShareRead);

                _device.Inserted += DeviceAttachedHandler;
                _device.Removed += DeviceRemovedHandler;

                _device.MonitorDeviceEvents = true;

                //_device.Read(ReadCallback, 200);
                //_device.ReadReport(OnReport);

                Thread t = new Thread(new ThreadStart(ThreadReadReport));
                t.IsBackground = true;
                //t.Start();

                Console.WriteLine("Reader found, press any key to exit.");

                while (true)
                {

                    _device.CloseDevice();
                    _device.OpenDevice();
                    var r = Console.ReadKey();
                    Console.WriteLine("");

                    lock(lockobj)
                    {
                        if (r.Key == ConsoleKey.Q)
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
                        if (r.Key == ConsoleKey.B)
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
                            for (int ctr = 0; ctr < readDev.Data.Length; ctr++)
                            {
                                Console.Write((int)readDev.Data[ctr] + ",");
                            }

                            Console.WriteLine("Read ADC");
                        }
                    }
                    Thread.Sleep(20);
                }


                _device.CloseDevice();
            }
            else
            {
                Console.WriteLine("Could not find device.");
                Console.ReadKey();
            }
        }

        //static void ReadCallback(HidDeviceData data)
        //{
        //    if (!_device.IsConnected) { return; }

        //    Console.WriteLine("Pressed: " + (int)data.Data[0]);
        //}

        private static void OnReport(HidReport report)
        {
            if (!_device.IsConnected) { return; }

            Console.WriteLine("Pressed: " + (int)report.Data[0]);

            _device.ReadReport(OnReport); // call again to wait for another report
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
