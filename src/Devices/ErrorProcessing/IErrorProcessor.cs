using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intems.Devices.ErrorProcessing
{
    public class ErrorProcessArgs : EventArgs
    {
    }

    public interface IErrorProcessor
    {
        void ProcessError(PackageProcessResult result);

        event EventHandler<ErrorProcessArgs> Error;
    }
}
