using System;
using System.Collections.Generic;
using System.Text;

namespace UV.Devices
{
    public class DataPackage
    {
        private IList<byte> _data = new List<byte>();
        private PackageHelper _packageHelper = new PackageHelper();

        public ushort GetCRC(){
            if(_data != null && _data.Count > 0) {
                _packageHelper.CalcCRC(0, ((List<byte>) _data).ToArray());
            }
            return 0;
        }
    }
}
