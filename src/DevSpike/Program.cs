using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Intems.Devices;
using Intems.Devices.Helpers;
using Intems.Devices.Interfaces;

namespace DevSpike
{
    class Program
    {
        static void Main(string[] args)
        {
//            ITransportLayerWorker worker = new TransportLayerWorker("COM3", 19200);
//            worker.InitDevice();
//            var dev = new SolaryDevice(1, worker);
//
//            var btn = new Buttons(false, false);
//            while (!btn.StartButton)
//                btn = dev.GetPressedButtons();
//
//            if(btn.StartButton)
//                dev.Start(SolaryAction.Sunbath, 60);


            var helper = new PackageHelper();
            var cmd = new List<byte>(new byte[] {0x01, 0x02, 0x01, 0x00, 0x00, 0x00, 0x3c});
            var pkg = helper.PackToPackage(cmd);

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
