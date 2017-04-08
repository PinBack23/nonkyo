# Searching receiver
To search for a receiver you must know the broadcast port for you device.
Default Port is **60128**.
Example:
{code:c#}
private static Device SearchDevices()
{
    var loDeviceSearch = new DeviceSearch();
    List<Device> loDeviceList = null;
    //Try 3 times
    for (int i = 0; i < 3; i++)
    {
        loDeviceList = loDeviceSearch.Discover();
        //loDeviceList = loDeviceSearch.Discover(60127);
        if (loDeviceList.Count > 0)
        {
            return loDeviceList.First();
        }
    }
    return null;
}
{code:c#}

# Sending a command
If you have found you device you can send a command.
Example:
{code:c#}
using (Connection loConnection = new Connection())
{
    if (!loConnection.Connect(loDevice))
    {
        Console.WriteLine("Cannot connect to device!");
        Console.ReadLine();
        return;
    }
    loConnection.MessageReceived += new EventHandler<MessageReceivedEventArgs>((s, e) => Console.WriteLine(e.Message));
    loConnection.SendCommand(NOnkyo.ISCP.Command.MasterVolume.State);
    System.Threading.Thread.Sleep(300);
    loConnection.SendCommand(NOnkyo.ISCP.Command.MasterVolume.UP);
    System.Threading.Thread.Sleep(300);
    loConnection.SendCommand(NOnkyo.ISCP.Command.MasterVolume.DOWN);
}
{code:c#}
