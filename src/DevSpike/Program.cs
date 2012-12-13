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
            var worker = new TransportLayerWorker("COM5", 19200);

            var pkg0 = new Package(new SetChannelStateCommand(1, 1, 0, 60));
            var pkg1 = new Package(new GetChannelStateCommand(1, 1));
//            var pkg2 = new Package(new DevResetCommand(1));

            worker.SendPackage(pkg0);
            worker.PackageReceived += (sender, dataArgs) =>
                                          {
                                              var bytes = dataArgs.Data;
                                              foreach (var b in bytes) Console.Write(b + " ");
                                              Console.WriteLine();
                                          };

            var updater = new TickUpdater(worker);
            updater.Start();

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
