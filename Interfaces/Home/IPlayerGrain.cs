using Base;
using Interfaces.Gate;
using Message;
using Orleans;

namespace Interfaces.Home;

public interface IPlayerGrain : IGrainWithIntegerKey, IBaseGrain
{
    #region MyRegion gateway to player

    /// <summary>
    /// gateway 玩家发的消息转到对应的Home处理
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    Task TranslateTell(IRequest msg);

    Task<IResponse> TranslateAsk(IRequest msg);

    #endregion

    #region MyRegion 内部消息处理

    /// <summary>
    /// 集群内部消息
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    Task Tell(IRequest msg);

    Task<IResponse> Ask(IRequest msg);

    #endregion

    #region gateway链接

    /// <summary>
    /// gateway链接
    /// </summary>
    /// <param name="observer"></param>
    /// <returns></returns>
    Task BindPacketObserver(IPacketObserver observer);

    Task UnbindPacketObserver();

    #endregion
}