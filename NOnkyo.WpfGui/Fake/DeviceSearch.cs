using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOnkyo.ISCP;
using System.Net;

namespace NOnkyo.WpfGui.Fake
{
    internal class DeviceSearch : IDeviceSearch
    {
        #region Static

        private static bool mbFakeToggle;

        #endregion

        public DeviceSearch()
        {
            mbFakeToggle = !mbFakeToggle;
        }

        #region IDeviceSearch Member

        public List<Device> Discover()
        {
            return this.Discover(0);
        }

        public List<Device> Discover(int pnPort)
        {
            List<Device> loList = new List<Device>();
            if (mbFakeToggle)
            {
                loList.Add(new Device()
                {
                    Category = "1",
                    Model = "TX-NR509",
                    Port = 6821,
                    Area = "XX",
                    MacAddress = "009923233",
                    IP = IPAddress.Parse("192.168.178.24")
                });
                loList.Add(new Device()
                {
                    Category = "1",
                    Model = "TX-NR1009",
                    Port = 6821,
                    Area = "XX",
                    MacAddress = "008923233",
                    IP = IPAddress.Parse("192.168.178.55")
                });
            }
            else
                System.Threading.Thread.Sleep(2000);

            loList.ForEach(item => item.InitDeviceBehaviors());
            return loList;
        }

        #endregion
    }
}
