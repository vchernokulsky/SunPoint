using System.Collections.Generic;

namespace Intems.Devices.ErrorProcessing
{
    public class ErrorDispatcher
    {
        private readonly Dictionary<object, object> _errMap = new Dictionary<object, object>();

        public void RegisterErrSrc(object obj)
        {
            if(!_errMap.ContainsKey(obj))
                _errMap.Add(obj, null);
        }

        public void PushErrorPackage(object key, PackageProcessResult result)
        {
            if (result.Type == AnswerType.Error)
            {
                ProcessError(result);
            }
        }

        private void ProcessError(PackageProcessResult result)
        {
            switch (result.ErrorType)
            {
                case ErrorType.FunctionNotExist:
                    // ответ на неверно запрошенную функцию
                    break;

                case ErrorType.DeviceFailed:
                    // устройство неисправно
                    break;

                case ErrorType.DeviceBusy:
                    // устройство занято
                    break;
            }
        }
    }
}
