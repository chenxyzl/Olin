using System.Threading;
using Base;
using Base.Helper;

namespace Share.Model.Component;

public enum ConsoleMode
{
    free, //空闲状态
    repl //交互状态
}

public class ConsoleComponent : IGlobalComponent
{
    public CancellationTokenSource CancellationTokenSource = new();
    public ConsoleMode Mode = ConsoleMode.free;
    public bool stopWatch = false;
    private long _last = 0;

    public long Last
    {
        get
        {
            if (_last == 0) _last = TimeHelper.Now();
            return _last;
        }
        set => _last = value;
    }
}