using System;
using System.Collections.Generic;

namespace NOnkyo.ISCP
{
    public interface IDeviceSearch
    {
        List<Device> Discover();
        List<Device> Discover(int pnPort);
    }
}
