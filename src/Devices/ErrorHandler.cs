using System;
using System.Collections.Generic;
using System.IO;

namespace Intems.Devices 
{
    public enum DevResult { OK, TimoutExpired, CrcFailed, PortFailed, Unknown }

    public class ErrorEnvironment {
        private readonly DevResult _errorType;
        private readonly IList<byte> _package;
        private readonly IDictionary<DevResult, string> _errorStrings;

        public ErrorEnvironment(DevResult errorType, IList<byte> package) {
            _errorType = errorType;
            _package = package;

            _errorStrings = new Dictionary<DevResult, string>
                                {
                                    {DevResult.TimoutExpired, "Timeout expiered"},
                                    {DevResult.CrcFailed, "CRC calculation failed"},
                                    {DevResult.Unknown, "Unknown error"}
                                };
        }

        public DevResult ErrorType {
            get { return _errorType; }
        }

        public IList<byte> Package {
            get { return _package; }
        }

        public override string ToString() {
            string package = String.Empty;
            foreach (byte b in _package) {
                package += (b.ToString("X2") + " ");
            }
            return String.Format("Error type: {0}\nPackage: {1}",_errorStrings[ErrorType], package);
        }
    }

    public class ErrorHandler {
        private readonly int _errorLimit;
        private readonly IDictionary<int, int> _body = new Dictionary<int, int>();

        public ErrorHandler() {
            _errorLimit = 2;
        }

        public ErrorHandler(int errorLimit) {
            _errorLimit = errorLimit;
        }


        public void SetError(int devID, ErrorEnvironment env) {
            if (!_body.ContainsKey(devID))
                _body.Add(devID, 0);

            if (++_body[devID] > _errorLimit) {
                var ex = new IOException("Error in data transmission.");
                ex.Data.Add("DeviceID",  devID);
                ex.Data.Add("ErrorType", env.ErrorType);
                ex.Data.Add("Package", env.Package);
                throw ex;
            }
        }

        public void ClearError(int devID) {
            if (_body.ContainsKey(devID))
                _body[devID] = 0;
        }
    }
}
