using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intems.Devices.ErrorProcessing
{
    public class CommonProcessor : IErrorProcessor
    {
        public event EventHandler<ErrorProcessArgs> Error;

        public void ProcessError(PackageProcessResult result)
        {
        }
    }
}
