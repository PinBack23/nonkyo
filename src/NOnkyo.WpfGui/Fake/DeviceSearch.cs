#region License
/*Copyright (c) 2013, Karl Sparwald
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE.*/
#endregion

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
            this.Timeout = TimeSpan.FromSeconds(1d);
        }

        #region IDeviceSearch Member

        public TimeSpan Timeout { get; set; }

        public List<Device> Discover()
        {
            return this.Discover(0);
        }

        public List<Device> Discover(int pnPort)
        {
            return this.Discover(null, pnPort);
        }

        public List<Device> Discover(IPAddress poIPAddress, int pnPort)
        {
            List<Device> loList = new List<Device>();
            if (mbFakeToggle)
            {
                if (poIPAddress == null)
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
                {
                    loList.Add(new Device()
                    {
                        Category = "1",
                        Model = "TX-NR509",
                        Port = 6821,
                        Area = "XX",
                        MacAddress = "009923233",
                        IP = poIPAddress
                    });
                }
            }
            else
                System.Threading.Thread.Sleep(2000);

            loList.ForEach(item => item.InitDeviceBehaviors());
            return loList;
        }

        #endregion
    }
}
