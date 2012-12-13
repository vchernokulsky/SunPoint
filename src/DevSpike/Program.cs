using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Intems.Devices;
using Intems.Devices.Commands;
using Intems.Devices.Helpers;
using Intems.Devices.Interfaces;

namespace DevSpike
{
    class Program
    {
        static void Main(string[] args)
        {
//            ITransportLayerWorker worker = new TransportLayerWorker("COM5", 19200);
//            worker.InitDevice();
//            var dev = new SolaryDevice(1, worker);
//
//            var btn = new Buttons(false, false);
//            while (!btn.StartButton)
//                btn = dev.GetPressedButtons();
//
//            if(btn.StartButton)
//                dev.Start(SolaryAction.Sunbath, 60);

            var worker = new TransportLayerWorker("COM5", 19200);

            var waiter = new BtnPressWaiter(worker);
            waiter.BtnPressed += (sender, pressArgs) => Console.WriteLine(pressArgs.Button);

//            Command cmd = new GetBtnStateCommand(1);
//            var package = new Package(cmd);
//            worker.SendPackage(package);
//            worker.PackageReceived += (sender, dataArgs) =>
//                                          {
//                                              foreach (var b in dataArgs.Data)
//                                                  Console.Write(b + " ");
//                                              Console.WriteLine();
//                                          };

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
