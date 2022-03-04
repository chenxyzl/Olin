namespace Message;

public enum OuterOpCode
{
    Ping = 200000,
    NotifyTest = 200001,
    PushTest = 200002,
    Login = 200003,
    Offline = 200004,
    LoginElsewhere = 200005,
    SyncReward = 200106,
    GetMails = 200107,
}