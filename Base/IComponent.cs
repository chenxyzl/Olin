using System.Threading.Tasks;
using Message;

namespace Base;

public interface IInnerHandlerDispatcher
{
    public Task OnTranslateTell(IBaseGrain actor, SRequest message);
    public Task<SResponse> OnTranslateAsk(IBaseGrain actor, SRequest message);
}

public interface IGateHandlerDispatcher
{
    public Task OnTell(IBaseGrain actor, GRequest message);
    public Task<GResponse> OnAsk(IBaseGrain actor, GRequest message);
}