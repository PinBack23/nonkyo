using System;
namespace NOnkyo.ISCP
{
    public interface IConnection : IDisposable
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler ConnectionClosed;

        Device CurrentDevice { get; }
        bool Connect(Device poDevice);
        void Disconnect();

        void SendCommand(Command.CommandBase poCommand);
        void SendPackage(string psMessage);
    }
}
