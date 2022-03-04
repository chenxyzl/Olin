using Base.Helper;
using Common;

namespace Base;

public abstract class Game
{
    protected static Game? _ins;
    public static Game Instance => A.NotNull(_ins);

    public RoleType Role { get; }
    public ushort NodeId { get; }

    public Game(RoleType r, ushort nodeId)
    {
        Role = r;
        NodeId = nodeId;
        //初始化id生成器
        IdGenerater.GlobalInit(nodeId);
    }
}