using System.Collections.Generic;

namespace Intems.Devices
{
    public class ErrorProcessor
    {
        private Dictionary<object, object> _errMap;

        public void RegisterErrSrc(object obj)
        {
            _errMap.Add(obj, null);
        }
    }
}
