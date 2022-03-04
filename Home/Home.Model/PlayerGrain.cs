using Interfaces.Gate;
using Interfaces.Home;
using Message;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Core;
using Orleans.Runtime;

namespace Home.Model;

public class PlayerGrain : Grain, IPlayerGrain
{
    private readonly ILogger _logger;
    public ulong uid { get; }
    public ulong PlayerId => uid;

    public PlayerGrain(ILogger logger)
    {
        uid = (ulong) this.GetPrimaryKeyLong();
        _logger = logger;
    }

    public Task TranslateTell(IRequest msg)
    {
        throw new NotImplementedException();
    }

    public Task<IResponse> TranslateAsk(IRequest msg)
    {
        throw new NotImplementedException();
    }

    public Task Tell(IRequest msg)
    {
        throw new NotImplementedException();
    }

    public Task<IResponse> Ask(IRequest msg)
    {
        throw new NotImplementedException();
    }

    public Task BindPacketObserver(IPacketObserver observer)
    {
        throw new NotImplementedException();
    }

    public Task UnbindPacketObserver()
    {
        throw new NotImplementedException();
    }
}