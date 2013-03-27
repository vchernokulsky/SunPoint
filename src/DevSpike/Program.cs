using System;
using System.Text;
using Intems.Devices;
using Intems.Devices.Commands;
using Intems.Devices.DevicePollers;
using Intems.SunPoint.BL;

namespace DevSpike
{
    class Program
    {
        static void Main(string[] args)
        {
            //CheckCommands();
            //FindDevices();

            var worker = new TransportLayerWorker("COM5", 19200);
            var btnPoller = new ButtonPressPoller(worker);
            btnPoller.BtnPressed += (sender, pressArgs) =>
                                        {
                                            switch (pressArgs.Button)
                                            {
                                                case PressedBtn.StartBtn:
                                                    Console.WriteLine("Start button pressed");
                                                    break;
                                                case PressedBtn.StopBtn:
                                                    Console.WriteLine("Stop button pressed");
                                                    break;
                                                default:
                                                    Console.WriteLine("Very strange situation");
                                                    break;
                                            }
                                        };

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        private static void FindDevices()
        {
            var finder = new DeviceFinder();
            finder.FindComplete += (sender, findArgs) =>
                                       {
                                           Console.WriteLine("Device finding finished!!!");
                                           foreach (string portName in findArgs.DevPortNames)
                                               Console.WriteLine(portName);
                                       };
            finder.Find();
        }

        private static void CheckCommands()
        {
            var worker = new TransportLayerWorker("COM5", 19200);
            var pkg = new Package(new GetDevDescCommand(1));
            var pkg0 = new Package(new SetChannelStateCommand(1, 1, 0, 60));
            var pkg1 = new Package(new GetChannelStateCommand(1, 1));
            var pkg2 = new Package(new DevResetCommand(1));
            worker.SendPackage(pkg);
            worker.PackageReceived += (sender, dataArgs) =>
                                          {
                                              var bytes = dataArgs.Data;
                                              foreach (var b in bytes) Console.Write(b + " ");
                                              Console.WriteLine();
                                              Console.WriteLine(Encoding.ASCII.GetString(bytes).Trim(new[] {(char) 0x00, ' '}));
                                          };

            var updater = new TicksUpdater(worker);
            updater.Start();
        }
    }
}
