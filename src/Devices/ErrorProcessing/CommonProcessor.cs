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
            if(result.Type == AnswerType.Ok) return;

            switch (result.ErrorType)
            {
                case ErrorType.DeviceFailed:
                    RaiseError(new ErrorProcessArgs());
                    break;
            }
        }

        public void RaiseError(ErrorProcessArgs e)
        {
            EventHandler<ErrorProcessArgs> handler = Error;
            if (handler != null) handler(this, e);
        }
    }
}
