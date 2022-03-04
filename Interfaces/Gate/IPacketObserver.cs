using Message;
using Orleans;

namespace Interfaces.Gate;

public interface IPacketObserver : IGrainObserver
{
    void SendPacket(IMessage packet);
    void Close(IMessage? packet = null);
}