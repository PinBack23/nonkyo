using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP
{
    public class DeviceSearch : NOnkyo.ISCP.IDeviceSearch
    {
        #region Logging

        private static Lazy<NLog.Logger> moLazyLogger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        private static NLog.Logger Logger
        {
            get
            {
                return moLazyLogger.Value;
            }
        }

        #endregion

        #region Static

        private static List<IPAddress> AllBroadcastAddresses()
        {
            List<IPAddress> loBroadcastAddresses = new List<IPAddress>();

            foreach (var loNic in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var loUAddress in loNic.GetIPProperties().UnicastAddresses.Where(
                    uaddress => uaddress.Address.AddressFamily == AddressFamily.InterNetwork && uaddress.IPv4Mask != null))
                {
                    loBroadcastAddresses.Add(BroadcastAddress(loUAddress));
                }
            }

            return loBroadcastAddresses;
        }

        private static IPAddress BroadcastAddress(UnicastIPAddressInformation poUAddress)
        {
            int lnIpAddress = BitConverter.ToInt32(poUAddress.Address.GetAddressBytes(), 0);
            int lnSubnet = BitConverter.ToInt32(poUAddress.IPv4Mask.GetAddressBytes(), 0);
            int lnBroadcast = lnIpAddress | ~lnSubnet;

            return new IPAddress(BitConverter.GetBytes(lnBroadcast));
        }

        private static Device ExtractDevice(string psMessage, IPEndPoint poEndPoint)
        {
            var loMatch = Regex.Match(psMessage,
                @"!(?<category>\d)ECN(?<model>[^/]*)/(?<port>\d{5})/(?<area>\w{2})/(?<mac>.{0,12})");
            if (loMatch.Success)
            {
                return new Device()
                {
                    Category = loMatch.Groups["category"].Value,
                    Model = loMatch.Groups["model"].Value,
                    Port = Convert.ToInt32(loMatch.Groups["port"].Value),
                    Area = loMatch.Groups["area"].Value,
                    MacAddress = loMatch.Groups["mac"].Value,
                    IP = poEndPoint.Address
                };
            }
            return null;
        }

        #endregion

        public List<Device> Discover()
        {
            return this.Discover(60128);
        }

        public List<Device> Discover(int pnPort)
        {
            List<Device> loDeviceList = new List<Device>();
            UdpClient loSendUdp = new UdpClient();
            IPEndPoint loEndPoint = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                var loCommand = "!xECNQSTN".ToISCPCommandMessage(false);

                foreach (var loBroadcast in AllBroadcastAddresses())
                {
                    Logger.Debug("Find Broadcastaddress {0}", loBroadcast);
                    loSendUdp.Send(loCommand, loCommand.Length, loBroadcast.ToString(), pnPort);
                }

                System.Threading.Thread.Sleep(1000);

                while (loSendUdp.Available > 0)
                {
                    byte[] loReceiveBytes = loSendUdp.Receive(ref loEndPoint);
                    byte[] loDummyOut;
                    Logger.Debug("Receive byte[] {0}{1}", Environment.NewLine, loReceiveBytes.FormatToOutput());
                    foreach (var lsMessage in loReceiveBytes.ToISCPStatusMessage(out loDummyOut))
                    {
                        var loDevice = ExtractDevice(lsMessage, loEndPoint);
                        if (loDevice != null)
                        {
                            loDevice.InitDeviceBehaviors();
                            Logger.Debug("Find Device {0}", loDevice);
                            loDeviceList.Add(loDevice);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.LogException(NLog.LogLevel.Error, "Discover Device", exp);
            }
            finally
            {
                if (loSendUdp != null)
                    loSendUdp.Close();
            }
            return loDeviceList;
        }

    }
}