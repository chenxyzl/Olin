namespace Base;

public abstract class IGlobalComponent : IComponent
{
    public LoadDelegate Load =>
        LifeHotfixManager.Instance.GetLoadDelegate(this, Game.Instance.NodeId);

    public StartDelegate Start =>
        LifeHotfixManager.Instance.GetStartDelegate(this, Game.Instance.NodeId);

    public PreStopDelegate PreStop =>
        LifeHotfixManager.Instance.GetPreStopDelegate(this, Game.Instance.NodeId);

    public StopDelegate Stop =>
        LifeHotfixManager.Instance.GetStopDelegate(this, Game.Instance.NodeId);

    public TickDelegate Tick =>
        LifeHotfixManager.Instance.GetTickDelegate(this, Game.Instance.NodeId);
}