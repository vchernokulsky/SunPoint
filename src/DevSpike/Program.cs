using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using Intems.Devices;
using Intems.Devices.Commands;
using Intems.Devices.Interfaces;
using Intems.SunPoint.BL;

namespace DevSpike
{
    class Program
    {
        static void Main(string[] args)
        {
//            var worker = new TransportLayerWorker("COM5", 19200);
//            var pkg = new Package(new GetDevDescCommand(1));
//            var pkg0 = new Package(new SetChannelStateCommand(1, 1, 0, 60));
//            var pkg1 = new Package(new GetChannelStateCommand(1, 1));
//            var pkg2 = new Package(new DevResetCommand(1));
//            worker.SendPackage(pkg);
//            worker.PackageReceived += (sender, dataArgs) =>
//                                          {
//                                              var bytes = dataArgs.Data;
//                                              foreach (var b in bytes) Console.Write(b + " ");
//                                              Console.WriteLine();
//                                              Console.WriteLine(Encoding.ASCII.GetString(bytes).Trim(new[]{(char) 0x00, ' '}));
//                                          };

//            var updater = new TicksUpdater(worker);
//            updater.Start();

            var finder = new DeviceFinder();
            finder.Find();

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
