using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
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

//            var waiter = new BtnPressWaiter(worker);
//            waiter.BtnPressed += (sender, pressArgs) => Console.WriteLine(pressArgs.Button);

            var pkg0 = new Package(new SetChannelStateCommand(1, 0, 60));
            var pkg1 = new Package(new GetChannelStateCommand(1));
            var pkg2 = new Package(new DevResetCommand(1));
            worker.SendPackage(pkg0);
            //worker.SendPackage(pkg2);
            worker.PackageReceived += (sender, dataArgs) =>
                                          {
                                              var bytes = dataArgs.Data;
                                              foreach (var b in bytes) Console.Write(b + " ");
                                              Console.WriteLine();
                                          };

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
